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

namespace WebAPI.Tests.ControllerTests
{
    [TestClass]
    public class CreateAsyncTests
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
        public async Task OnSuccess_ReturnHttpStatusCode_Created()
        {
            // Arrange
            StudentDTO newStudent = new StudentDTO 
            {
                Id = Guid.NewGuid(),
                FirstName = "Marcus",
                LastName = "Aurelius",
                DateOfBirth = new DateTime(2001, 1, 1),
                EmailAddress = "markimark@roma.com",
                RegisteredOn = DateTime.Now
            };
            _service.Setup(service => service.CreateAsync(newStudent)).ReturnsAsync(true);

            // Act
            HttpResponseMessage result = await _controller.CreateAsync(newStudent);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(HttpStatusCode.Created, result.StatusCode);
            Assert.AreEqual("\"Created!\"", await result.Content.ReadAsStringAsync()); // navodnici pravili problem, cito ih je bez
        }

        [TestMethod]
        public async Task OnFail_ReturnHttpStatusCode_BadRequest()
        {
            // Arrange
            StudentDTO newStudent = new StudentDTO
            {
                Id = Guid.NewGuid(),
                FirstName = "Marcus",
                LastName = "Aurelius",
                DateOfBirth = new DateTime(2001, 1, 1),
                EmailAddress = "markimark@roma.com",
                RegisteredOn = DateTime.Now
            };
            _service.Setup(service => service.CreateAsync(newStudent)).ReturnsAsync(false);

            // Act
            HttpResponseMessage result = await _controller.CreateAsync(newStudent);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(HttpStatusCode.BadRequest, result.StatusCode);
            Assert.IsTrue((await result.Content.ReadAsStringAsync()).Contains("Failed to create"));
        }

        [TestMethod]
        public async Task OnException_ReturnHttpStatusCode_InternalServerError()
        {
            // Arrange
            StudentDTO newStudent = new StudentDTO
            {
                Id = Guid.NewGuid(),
                FirstName = "Marcus",
                LastName = "Aurelius",
                DateOfBirth = new DateTime(2001, 1, 1),
                EmailAddress = "markimark@roma.com",
                RegisteredOn = DateTime.Now
            };
            _service.Setup(service => service.CreateAsync(newStudent)).Throws(new Exception("MY TEST EXCEPTION"));

            // Act
            HttpResponseMessage result = await _controller.CreateAsync(newStudent);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(HttpStatusCode.InternalServerError, result.StatusCode);
            Assert.IsTrue((await result.Content.ReadAsStringAsync()).Contains("Error for CreateAsync: MY TEST EXCEPTION"));
        }
    }
}
