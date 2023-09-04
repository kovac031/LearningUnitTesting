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
    public class CreateStudentTests
    {
        private readonly StudentController _controller;
        private readonly IService _service;
        private readonly IMapper _mapper;
        public CreateStudentTests()
        {
            _service = A.Fake<IService>(); //FakeItEasy
            _mapper = A.Fake<IMapper>();
            _controller = new StudentController(_service, _mapper); // SUT, system under test
        }
        //----------------------------------------------
        [Fact]
        public async Task ReturnsView()
        {
            // Act
            ActionResult result = await _controller.CreateStudent();

            // Assert
            Assert.IsType<ViewResult>(result);
        }
        [Fact]
        public async Task SuccessfulCreation_RedirectsToListStudents()
        {
            // Arrange
            StudentView fakeStudentView = new StudentView // kako u kontroleru ne provjeravam mapiranje, ovdje prolazi bilo kakvi parametri
            {                
                FirstName = "John",
                LastName = "Doe",
                DateOfBirth = new DateTime(1991, 1, 1),
                EmailAddress = "johndoe@jemail.com"
            };
            StudentDTO fakeStudentDTO = _mapper.Map<StudentDTO>(fakeStudentView);

            A.CallTo(() => _mapper.Map<StudentDTO>(fakeStudentView)).Returns(fakeStudentDTO);
            A.CallTo(() => _service.CreateAsync(fakeStudentDTO)).Returns(Task.FromResult(true)); // uvijek salje true, tu mu kaze da je unos (mapiranje) prihvaceno

            // Act
            ActionResult result = await _controller.CreateStudent(fakeStudentView);

            // Assert
            RedirectToActionResult redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("ListStudents", redirectResult.ActionName);
        }
        [Fact]
        public async Task FailedCreation_ReturnsFailedToCreateView()
        {
            // Arrange
            StudentView fakeStudentView = new StudentView 
            {
                FirstName = "John",
                LastName = "Doe",
                DateOfBirth = new DateTime(1991, 1, 1),
                EmailAddress = "johndoe@jemail.com"
            };
            StudentDTO fakeStudentDTO = _mapper.Map<StudentDTO>(fakeStudentView);

            A.CallTo(() => _mapper.Map<StudentDTO>(fakeStudentView)).Returns(fakeStudentDTO);
            A.CallTo(() => _service.CreateAsync(fakeStudentDTO)).Returns(Task.FromResult(false)); // salje false kao da unos novog studenta nije prihvacen, nije kreiran

            // Act
            ActionResult result = await _controller.CreateStudent(fakeStudentView);

            // Assert
            ViewResult viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("Failed to create", viewResult.ViewName);
        }
        [Fact]
        public async Task ExceptionThrown_ReturnsExceptionView()
        {
            // Arrange
            StudentView fakeStudentView = new StudentView
            {
                FirstName = "John",
                LastName = "Doe",
                DateOfBirth = new DateTime(1991, 1, 1),
                EmailAddress = "johndoe@jemail.com"
            };
            StudentDTO fakeStudentDTO = _mapper.Map<StudentDTO>(fakeStudentView);

            A.CallTo(() => _mapper.Map<StudentDTO>(fakeStudentView)).Returns(fakeStudentDTO);
            A.CallTo(() => _service.CreateAsync(fakeStudentDTO)).Throws<Exception>();

            // Act
            ActionResult result = await _controller.CreateStudent(fakeStudentView);

            // Assert
            ViewResult viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("Exception", viewResult.ViewName);
        }
    }
}
