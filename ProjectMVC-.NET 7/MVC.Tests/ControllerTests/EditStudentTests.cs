using AutoMapper;
using FakeItEasy;
using Microsoft.AspNetCore.Mvc;
using Model;
using MVC.Controllers;
using Service.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVC.Tests.ControllerTests
{
    public class EditStudentTests
    {
        private readonly StudentController _controller;
        private readonly IService _service;
        private readonly IMapper _mapper;
        public EditStudentTests()
        {
            _service = A.Fake<IService>(); //FakeItEasy
            _mapper = A.Fake<IMapper>();
            _controller = new StudentController(_service, _mapper); // SUT, system under test
        }
        //----------------------------------------------
        [Fact]
        public async Task ReturnsViewWithStudentData()
        {
            // Arrange
            Guid fakeId = new Guid("3f2504e0-4f89-11d3-9a0c-0305e82c3301");
            StudentDTO fakeStudentDTO = new StudentDTO
            {
                Id = fakeId,
                FirstName = "John",
                LastName = "Doe"
            };
            StudentView fakeStudentView = new StudentView
            {
                Id = fakeId,
                FirstName = "John",
                LastName = "Doe"
            };
            A.CallTo(() => _service.GetOneByIdAsync(fakeId)).Returns(Task.FromResult(fakeStudentDTO));
            A.CallTo(() => _mapper.Map<StudentView>(fakeStudentDTO)).Returns(fakeStudentView);

            // Act
            ActionResult result = await _controller.EditStudent(fakeId);

            // Assert
            ViewResult viewResult = Assert.IsType<ViewResult>(result);
            StudentView returnedStudent = viewResult.Model as StudentView;

            Assert.NotNull(returnedStudent);
            Assert.Equal(fakeId, returnedStudent.Id);
        }
        [Fact]
        public async Task SuccessfulEdit_RedirectsToListStudents()
        {
            // Arrange
            Guid fakeId = new Guid("3f2504e0-4f89-11d3-9a0c-0305e82c3301");
            StudentDTO fakeStudentDTO = new StudentDTO
            {
                Id = fakeId,
                FirstName = "John",
                LastName = "Doe"
            };
            StudentView fakeStudentView = new StudentView
            {
                Id = fakeId,
                FirstName = "John",
                LastName = "Doe"
            };
            A.CallTo(() => _mapper.Map<StudentDTO>(fakeStudentView)).Returns(fakeStudentDTO);
            A.CallTo(() => _service.EditAsync(fakeStudentDTO, fakeId)).Returns(Task.FromResult(true));

            // Act
            ActionResult result = await _controller.EditStudent(fakeStudentView);

            // Assert
            RedirectToActionResult redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("ListStudents", redirectResult.ActionName);
        }
        [Fact]
        public async Task FailedEdit_ReturnsFailedToCreateView()
        {
            // Arrange
            Guid fakeId = new Guid("3f2504e0-4f89-11d3-9a0c-0305e82c3301");
            StudentDTO fakeStudentDTO = new StudentDTO
            {
                Id = fakeId,
                FirstName = "John",
                LastName = "Doe"
            };
            StudentView fakeStudentView = new StudentView
            {
                Id = fakeId,
                FirstName = "John",
                LastName = "Doe"
            };
            A.CallTo(() => _mapper.Map<StudentDTO>(fakeStudentView)).Returns(fakeStudentDTO);
            A.CallTo(() => _service.EditAsync(fakeStudentDTO, fakeId)).Returns(Task.FromResult(false));

            // Act
            ActionResult result = await _controller.EditStudent(fakeStudentView);

            // Assert
            ViewResult viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("Failed to edit", viewResult.ViewName);
        }
        [Fact]
        public async Task ExceptionThrown_ReturnsExceptionView()
        {
            // Arrange
            Guid fakeId = new Guid("3f2504e0-4f89-11d3-9a0c-0305e82c3301");
            StudentDTO fakeStudentDTO = new StudentDTO
            {
                Id = fakeId,
                FirstName = "John",
                LastName = "Doe"
            };
            StudentView fakeStudentView = new StudentView
            {
                Id = fakeId,
                FirstName = "John",
                LastName = "Doe"
            };
            A.CallTo(() => _mapper.Map<StudentDTO>(fakeStudentView)).Returns(fakeStudentDTO);
            A.CallTo(() => _service.EditAsync(fakeStudentDTO, fakeId)).Throws<Exception>();

            // Act
            ActionResult result = await _controller.EditStudent(fakeStudentView);

            // Assert
            ViewResult viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("Exception", viewResult.ViewName);
        }
    }
}
