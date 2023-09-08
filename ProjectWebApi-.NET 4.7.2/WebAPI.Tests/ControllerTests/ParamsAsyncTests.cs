using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Project.WebAPI.Controllers;
using Service.Common;
using System;
using System.Net.Http;
using System.Web.Http.Hosting;
using System.Web.Http;
using Model;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using System.Linq;

namespace WebAPI.Tests.ControllerTests
{
    [TestClass]
    public class ParamsAsyncTests
    {
        private DefaultController _controller;
        private Mock<IService> _service;

        [TestInitialize]
        public void Setup()
        {
            _service = new Mock<IService>();
            _controller = new DefaultController(_service.Object);
            _controller.Request = new HttpRequestMessage();
            _controller.Request.Properties.Add(HttpPropertyKeys.HttpConfigurationKey, new HttpConfiguration());
        }
        //-----------------------------------------

        [TestMethod]
        public async Task NoParamsProvided_OnSuccess_ReturnFullList()
        {
            // Arrange
            List<StudentDTO> fakeList = HelperClass.GetFakeStudents();
            _service.Setup(service => service.ParamsAsync(null, null, null, null, null, null, null, null, null)).ReturnsAsync(fakeList);

            // Act
            HttpResponseMessage result = await _controller.ParamsAsync(); // No params

            List<StudentDTO> students = await result.Content.ReadAsAsync<List<StudentDTO>>();

            // meni za dusu -----------
            Console.WriteLine("Passed through service:");
            for (int i = 0; i < students.Count; i++) // i=0 mora biti jer jer to prvi, ali zato redni broj mijenjam sa 0 u 1 sa i+1
            {
                Console.WriteLine($"{i + 1}. {students[i].FirstName} {students[i].LastName}");
            }
            Console.WriteLine("Moq-faked list:");
            for (int i = 0; i < fakeList.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {fakeList[i].FirstName} {fakeList[i].LastName}");
            }
            // -----------------------------------

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(HttpStatusCode.OK, result.StatusCode);
        }

        [TestMethod]
        public async Task SomeParamsProvided_OnSuccess_ReturnModifiedList() // shvatio da nema smila ovo sto radim, jer u kontroleru nema sorting i sl logika pa se to ni ne moze testirati, ja cim izvorno reduciram listu, dalje je svejedno koje parametre saljem, dob_asc ili null ce isto vratiti
        {
            // Arrange
            List<StudentDTO> fakeList = HelperClass.GetFakeStudents().OrderBy(s => s.DateOfBirth).Skip(2).Take(1).ToList(); // modified lista koja je kao dosla iz repositorija gdje je logika za sortiranje i ostalo
            _service.Setup(service => service.ParamsAsync("dob_asc", null, null, null, null, null, null, "2", "1")).ReturnsAsync(fakeList);

            // Act
            HttpResponseMessage result = await _controller.ParamsAsync("dob_asc", null, null, null, null, null, null, "2", "1"); // na zadnjoj stranici ce biti najstariji

            List<StudentDTO> students = await result.Content.ReadAsAsync<List<StudentDTO>>(); // dohvaca listu koja je prosla kroz kontroler, ako mi treba za nesto, ovdje npr je suvisna linija koda
                        
            Console.WriteLine("Students from fakeList:");
            foreach (StudentDTO student in fakeList)
            {
                Console.WriteLine($"FirstName: {student.FirstName}, DateOfBirth: {student.DateOfBirth}");
            }

            Console.WriteLine("Students from students list:");
            foreach (StudentDTO student in students)
            {
                Console.WriteLine($"FirstName: {student.FirstName}, DateOfBirth: {student.DateOfBirth}");
            }

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(HttpStatusCode.OK, result.StatusCode);
            Assert.AreEqual(fakeList.Count, students.Count);
        }

        [TestMethod]
        public async Task SomeParamsProvided_OnFail_ReturnInternalServerError() // exception
        {
            // Arrange
            _service.Setup(service => service.ParamsAsync("dob_asc", null, null, null, null, null, null, "2", "1")).Throws(new Exception("MY TEST EXCEPTION"));

            // Act
            HttpResponseMessage result = await _controller.ParamsAsync("dob_asc", null, null, null, null, null, null, "2", "1");

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(HttpStatusCode.InternalServerError, result.StatusCode);

            HttpError error = await result.Content.ReadAsAsync<HttpError>();
            Assert.IsTrue(error.Message.Contains("Error for ParamsAsync: MY TEST EXCEPTION"));
        }

        // dovoljno, mogu se pojedinacni parametri testirati ali je sve ponavljanje
    }
}
