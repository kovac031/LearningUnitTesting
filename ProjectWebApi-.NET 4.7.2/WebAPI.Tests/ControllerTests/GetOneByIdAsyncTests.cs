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
    public class GetOneByIdAsyncTests
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
        //-------------------------------------------------------------------
        [TestMethod]
        public async Task OnSuccess_ReturnStudentDTO()
        {
            // Arrange
            List<StudentDTO> fakeList = HelperClass.GetFakeStudents();

            StudentDTO fakeStudent = fakeList.First(s => s.Id == new Guid("6dcd4ce0-4f89-11d3-9a0c-0305e82c8811"));
            _service.Setup(service => service.GetOneByIdAsync(fakeStudent.Id)).ReturnsAsync(fakeStudent);

            // Act
            HttpResponseMessage result = await _controller.GetOneByIdAsync(fakeStudent.Id);
            StudentDTO foundStudent = await result.Content.ReadAsAsync<StudentDTO>();

            // meni za dusu -----------
            Console.WriteLine($"fakeStudent {fakeStudent.FirstName}");
            Console.WriteLine($"foundStudent {foundStudent.FirstName}");
            // -----------------------------------

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(HttpStatusCode.OK, result.StatusCode);
            Assert.AreEqual(fakeStudent.Id, foundStudent.Id);
        }

        [TestMethod]
        public async Task OnFail_ReturnInternalServerError() // exception
        {
            // Arrange
            Guid id = Guid.NewGuid(); // id nebitan kad radim fail
            _service.Setup(service => service.GetOneByIdAsync(id)).Throws(new Exception("MY TEST EXCEPTION"));

            // Act
            HttpResponseMessage result = await _controller.GetOneByIdAsync(id);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(HttpStatusCode.InternalServerError, result.StatusCode);

            HttpError error = await result.Content.ReadAsAsync<HttpError>();
            Assert.IsTrue(error.Message.Contains("Error for GetOneByIdAsync: MY TEST EXCEPTION"));
        }
    }
}
