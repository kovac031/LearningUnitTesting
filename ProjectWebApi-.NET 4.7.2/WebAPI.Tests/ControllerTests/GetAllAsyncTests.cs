using Microsoft.VisualStudio.TestTools.UnitTesting;
using Model;
using Moq;
using Project.WebAPI.Controllers;
using Service.Common;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http.Hosting;
using System.Web.Http;

namespace WebAPI.Tests.ControllerTests
{
    [TestClass]
    public class GetAllAsyncTests
    {
        private DefaultController _controller;
        private Mock<IService> _service;

        [TestInitialize]  // drugacije nego s xUnit dependency injection
        public void Setup()
        {
            _service = new Mock<IService>();
            _controller = new DefaultController(_service.Object);

            // http fore za webapi controller, toga nema u .net 7 verziji
            _controller.Request = new HttpRequestMessage();
            _controller.Request.Properties.Add(HttpPropertyKeys.HttpConfigurationKey, new HttpConfiguration());
        }
        //-------------------------------------------------------------------

        [TestMethod]
        public async Task OnSuccess_ReturnList()
        {
            // Arrange
            List<StudentDTO> fakeList = HelperClass.GetFakeStudents();
            _service.Setup(service => service.GetAllAsync()).ReturnsAsync(fakeList);

            // Act
            HttpResponseMessage result = await _controller.GetAllAsync();

            List<StudentDTO> students = await result.Content.ReadAsAsync<List<StudentDTO>>();

            // meni za dusu -----------
            Console.WriteLine("Passed through service:");
            for (int i = 0; i < students.Count; i++) // i=0 mora biti jer jer to prvi, ali zato redni broj mijenjam sa 0 u 1 sa i+1
            {
                Console.WriteLine($"{i+1}. {students[i].FirstName} {students[i].LastName}");
            }
            Console.WriteLine("Moq-faked list:");
            for (int i = 0; i < students.Count; i++)
            {
                Console.WriteLine($"{i+1}. {fakeList[i].FirstName} {fakeList[i].LastName}");
            }
            // -----------------------------------

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(HttpStatusCode.OK, result.StatusCode);            
            Assert.AreEqual(fakeList.Count, students.Count);
        }

        [TestMethod]
        public async Task OnFail_ReturnInternalServerError() // exception
        {
            // Arrange
            _service.Setup(service => service.GetAllAsync()).Throws(new Exception("MY TEST EXCEPTION"));

            // Act
            HttpResponseMessage result = await _controller.GetAllAsync();

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(HttpStatusCode.InternalServerError, result.StatusCode);

            HttpError error = await result.Content.ReadAsAsync<HttpError>();
            Assert.IsTrue(error.Message.Contains("Error for GetAllAsync: MY TEST EXCEPTION"));
        }

    }
}
