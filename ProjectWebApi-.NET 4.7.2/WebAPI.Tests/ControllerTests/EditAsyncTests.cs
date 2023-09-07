using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Project.WebAPI.Controllers;
using Service.Common;
using System;
using System.Net.Http;
using System.Web.Http.Hosting;
using System.Web.Http;
using Model;
using System.Net;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;

namespace WebAPI.Tests.ControllerTests
{
    [TestClass]
    public class EditAsyncTests
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
        public async Task OnSuccess_ReturnHttpStatusCode_Ok()
        {
            // Arrange
            List<StudentDTO> fakeList = HelperClass.GetFakeStudents();
            StudentDTO fakeStudent = fakeList.First(s => s.Id == new Guid("6dcd4ce0-4f89-11d3-9a0c-0305e82c8811"));
            _service.Setup(service => service.GetOneByIdAsync(fakeStudent.Id)).ReturnsAsync(fakeStudent);

            _service.Setup(service => service.EditAsync(fakeStudent, fakeStudent.Id)).ReturnsAsync(true);
            
            Console.WriteLine($"email prije edita: {fakeStudent.EmailAddress}");

            // editing the student info
            fakeStudent.EmailAddress = "newmail@lolmail.com";

            // Act
            HttpResponseMessage result = await _controller.EditAsync(fakeStudent, fakeStudent.Id);

            Console.WriteLine($"email poslije edita: {fakeStudent.EmailAddress}");

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(HttpStatusCode.OK, result.StatusCode);
            Assert.AreEqual("\"Edited!\"", await result.Content.ReadAsStringAsync()); 
        }

        [TestMethod]
        public async Task OnFail_ReturnHttpStatusCode_BadRequest()
        {
            // Arrange
            List<StudentDTO> fakeList = HelperClass.GetFakeStudents();
            StudentDTO fakeStudent = fakeList.First(s => s.Id == new Guid("6dcd4ce0-4f89-11d3-9a0c-0305e82c8811"));
            _service.Setup(service => service.GetOneByIdAsync(fakeStudent.Id)).ReturnsAsync(fakeStudent);

            _service.Setup(service => service.EditAsync(fakeStudent, fakeStudent.Id)).ReturnsAsync(false);

            // editing the student info
            fakeStudent.EmailAddress = "newmail@lolmail.com";

            // Act
            HttpResponseMessage result = await _controller.EditAsync(fakeStudent, fakeStudent.Id);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(HttpStatusCode.BadRequest, result.StatusCode);
            Assert.IsTrue((await result.Content.ReadAsStringAsync()).Contains("Failed to edit"));
        }

        [TestMethod]
        public async Task OnException_ReturnHttpStatusCode_InternalServerError()
        {
            // Arrange
            List<StudentDTO> fakeList = HelperClass.GetFakeStudents();
            StudentDTO fakeStudent = fakeList.First(s => s.Id == new Guid("6dcd4ce0-4f89-11d3-9a0c-0305e82c8811"));
            _service.Setup(service => service.GetOneByIdAsync(fakeStudent.Id)).ReturnsAsync(fakeStudent);

            _service.Setup(service => service.EditAsync(fakeStudent, fakeStudent.Id)).Throws(new Exception("MY TEST EXCEPTION"));

            // editing the student info
            fakeStudent.EmailAddress = "newmail@lolmail.com";

            // Act
            HttpResponseMessage result = await _controller.EditAsync(fakeStudent, fakeStudent.Id);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(HttpStatusCode.InternalServerError, result.StatusCode);
            Assert.IsTrue((await result.Content.ReadAsStringAsync()).Contains("Error for EditAsync: MY TEST EXCEPTION"));
        }
    }
}
