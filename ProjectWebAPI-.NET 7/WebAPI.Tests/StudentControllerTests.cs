using FakeItEasy;
using Microsoft.AspNetCore.Mvc;
using Model;
using Service.Common;
using System;
using System.Collections.Generic;
using System.Linq;
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
        [Fact]
        public async Task StudentController_GetAllAsync_ReturnList()
        {
            // Arrange
            A.CallTo(() => _service.GetAllAsync()).Returns(new List<StudentDTO>());

            // Act
            var result = await _controller.GetAllAsync();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);

            // Ensure that the service method was called
            A.CallTo(() => _service.GetAllAsync()).MustHaveHappenedOnceExactly();
        }
        public async Task StudentController_GetOneByIdAsync_ReturnObject()
        {

        }
        public async Task StudentController_CreateAsync_ReturnBool()
        {
            // one automatske se moze provjeriti npr registered on, je li prije ili poslije nekog datuma
        }
        public async Task StudentController_EditAsync_ReturnBool()
        {

        }
        public async Task StudentController_DeleteAsync_ReturnBool()
        {

        }
        public async Task StudentController_ParamsAsync_ReturnList()
        {

        }
    }
}
