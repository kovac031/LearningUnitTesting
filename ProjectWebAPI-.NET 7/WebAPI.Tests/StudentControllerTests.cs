using FakeItEasy;
using Microsoft.AspNetCore.Mvc;
using Model;
using Service.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using WebAPI.Controllers;

namespace WebAPI.Tests
{
    public class StudentControllerTests
    {
        private readonly StudentController _controller;
        private readonly IService _service;
        public StudentControllerTests()
        {
            _service = A.Fake<IService>(); //FakeItEasy
            _controller = new StudentController(_service); // SUT, system under test
        }

        //----------------- GET ALL / LIST -------------- // moj GetAllAsync ima dva returna, listu ako uspije i exception ako ne, testam to dvoje
        [Fact]
        public async Task StudentController_GetAllAsync_ReturnList() // lista
        {
            // Arrange
            A.CallTo(() => _service.GetAllAsync()).Returns(new List<StudentDTO>());

            // Act
            var result = await _controller.GetAllAsync();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.IsType<List<StudentDTO>>(okResult.Value);
        }
        [Fact]
        public async Task StudentController_GetAllAsync_ThrowException() // exception
        {
            // Arrange
            var exception = "Database connection error";
            A.CallTo(() => _service.GetAllAsync()).Throws(new Exception(exception));

            // Act
            var result = await _controller.GetAllAsync();

            // Assert
            var objectResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal((int)HttpStatusCode.InternalServerError, objectResult.StatusCode);
            Assert.Equal($"Error for GetAllAsync: {exception}", objectResult.Value);
        }
        // ---------------- GET ONE BY ID ---------------
        [Fact]
        public async Task StudentController_GetOneByIdAsync_ReturnObject()
        {
            // Arrange
            Guid id = Guid.NewGuid(); // moze i konkretan guid tu
            A.CallTo(() => _service.GetOneByIdAsync(id)).Returns(new StudentDTO());

            // Act
            var result = await _controller.GetOneByIdAsync(id);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.IsType<StudentDTO>(okResult.Value);
        }
        [Fact]
        public async Task StudentController_GetOneByIdAsync_ThrowException() 
        {
            // Arrange
            Guid id = Guid.NewGuid();
            var exception = "Database connection error";
            A.CallTo(() => _service.GetOneByIdAsync(id)).Throws(new Exception(exception));

            // Act
            var result = await _controller.GetOneByIdAsync(id);

            // Assert
            var objectResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal((int)HttpStatusCode.InternalServerError, objectResult.StatusCode);
            Assert.Equal($"Error for GetOneByIdAsync: {exception}", objectResult.Value);
        }
        // ------------------- CREATE ---------------
        public async Task StudentController_CreateAsync_ReturnBool()
        {
            // one automatske se moze provjeriti npr registered on, je li prije ili poslije nekog datuma
        }
        // -------------------- EDIT ---------------
        public async Task StudentController_EditAsync_ReturnBool()
        {

        }
        // ------------------- DELETE ---------------
        public async Task StudentController_DeleteAsync_ReturnBool()
        {

        }
        // ------------ GET LIST WITH PARAMETERS ---------------
        public async Task StudentController_ParamsAsync_ReturnList()
        {

        }
    }
}
