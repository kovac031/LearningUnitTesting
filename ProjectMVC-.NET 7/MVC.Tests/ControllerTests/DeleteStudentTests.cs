using AutoMapper;
using FakeItEasy;
using Microsoft.AspNetCore.Mvc;
using MVC.Controllers;
using Service.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVC.Tests.ControllerTests
{
    public class DeleteStudentTests
    {
        private readonly StudentController _controller;
        private readonly IService _service;
        private readonly IMapper _mapper;
        public DeleteStudentTests()
        {
            _service = A.Fake<IService>(); //FakeItEasy
            _mapper = A.Fake<IMapper>();
            _controller = new StudentController(_service, _mapper); // SUT, system under test
        }
        [Fact]
        public async Task SuccessfulDelete_RedirectsToListStudents()
        {
            // Arrange
            Guid fakeId = new Guid("3f2504e0-4f89-11d3-9a0c-0305e82c3301");

            A.CallTo(() => _service.DeleteAsync(fakeId)).Returns(Task.FromResult(true));

            // Act
            ActionResult result = await _controller.DeleteStudent(fakeId);

            // Assert
            RedirectToActionResult redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("ListStudents", redirectResult.ActionName);
        }
    }
}
