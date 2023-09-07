using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Project.WebAPI.Controllers;
using Service.Common;
using System;
using System.Net.Http;
using System.Web.Http.Hosting;
using System.Web.Http;
using System.Net;
using System.Threading.Tasks;

namespace WebAPI.Tests.ControllerTests
{
    [TestClass]
    public class DeleteAsyncTests
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
        public async Task OnSuccess_ReturnHttpStatusCode_Ok()
        {
            // Arrange // mogu kao s edit testom prvo dovuc studenta sa pravim id, ali ne moram
            Guid id = Guid.NewGuid();
            _service.Setup(service => service.DeleteAsync(id)).ReturnsAsync(true);

            // Act
            HttpResponseMessage result = await _controller.DeleteAsync(id);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(HttpStatusCode.OK, result.StatusCode);
            Assert.AreEqual("\"Deleted!\"", await result.Content.ReadAsStringAsync());
        }

        [TestMethod]
        public async Task OnFail_ReturnHttpStatusCode_BadRequest()
        {
            // Arrange // mogu kao s edit testom prvo dovuc studenta sa pravim id, ali ne moram
            Guid id = Guid.NewGuid();
            _service.Setup(service => service.DeleteAsync(id)).ReturnsAsync(false);

            // Act
            HttpResponseMessage result = await _controller.DeleteAsync(id);

            // Assert            
            Assert.IsNotNull(result);
            Assert.AreEqual(HttpStatusCode.BadRequest, result.StatusCode);
            Assert.IsTrue((await result.Content.ReadAsStringAsync()).Contains("Failed to delete"));
        }

        [TestMethod]
        public async Task OnException_ReturnHttpStatusCode_InternalServerError()
        {
            // Arrange // mogu kao s edit testom prvo dovuc studenta sa pravim id, ali ne moram
            Guid id = Guid.NewGuid();
            _service.Setup(service => service.DeleteAsync(id)).Throws(new Exception("MY TEST EXCEPTION"));


            // Act
            HttpResponseMessage result = await _controller.DeleteAsync(id);

            // Assert            
            Assert.IsNotNull(result);
            Assert.AreEqual(HttpStatusCode.InternalServerError, result.StatusCode);
            Assert.IsTrue((await result.Content.ReadAsStringAsync()).Contains("Error for DeleteAsync: MY TEST EXCEPTION"));
        }
    }
}
