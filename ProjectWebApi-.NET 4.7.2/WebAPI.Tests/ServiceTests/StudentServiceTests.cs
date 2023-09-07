using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Project.WebAPI.Controllers;
using Service.Common;
using System;
using System.Net.Http;
using System.Web.Http.Hosting;
using System.Web.Http;
using Repository.Common;
using Service;
using Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace WebAPI.Tests.ServiceTests
{
    [TestClass]
    public class StudentServiceTests
    {
        private StudentService _service;
        private Mock<IRepository> _repository;

        [TestInitialize]
        public void Setup()
        {
            _repository = new Mock<IRepository>();
            _service = new StudentService(_repository.Object);
        }
        //-------------------------------------------------------------------

        [TestMethod]
        public async Task GetAllAsync_OnSuccess_ReturnList()
        {
            // Arrange
            List<StudentDTO> fakeList = HelperClass.GetFakeStudents();
            _repository.Setup(x => x.GetAllAsync()).ReturnsAsync(fakeList);

            // Act
            List<StudentDTO> result = await _service.GetAllAsync();

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(fakeList.Count, result.Count);
        }

        [TestMethod]
        public async Task GetAllAsync_OnFail_ThrowsException()
        {
            // Arrange
            _repository.Setup(x => x.GetAllAsync()).ThrowsAsync(new Exception("MY TEST EXCEPTION"));

            // Act & Assert
            await Assert.ThrowsExceptionAsync<Exception>(() => _service.GetAllAsync());
        }

        //-----------------------------------------------------------------

    }
}
