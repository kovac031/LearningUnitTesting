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
    public class ListStudentsTests
    {
        private readonly StudentController _controller;
        private readonly IService _service;
        private readonly IMapper _mapper;
        public ListStudentsTests()
        {
            _service = A.Fake<IService>(); //FakeItEasy
            _mapper = A.Fake<IMapper>();
            _controller = new StudentController(_service, _mapper); // SUT, system under test
        }
        //--------------- FAKE LIST ----------------
        private List<StudentDTO> GetFakeStudents()
        {
            return new List<StudentDTO>
            {
            new StudentDTO {    Id = new Guid("3f2504e0-4f89-11d3-9a0c-0305e82c3301"),
                                FirstName = "René",
                                LastName = "D'Artagnan",
                                DateOfBirth = new DateTime(1991, 1, 2),
                                EmailAddress = "renerene@reneemail.com",
                                RegisteredOn = new DateTime(2021, 1, 2) },

            new StudentDTO {    Id = new Guid("6dcd4ce0-4f89-11d3-9a0c-0305e82c8811"),
                                FirstName = "Zoë",
                                LastName = "Müller",
                                DateOfBirth = new DateTime(1995, 11, 22),
                                EmailAddress = "zoezoe@zoeemail.com",
                                RegisteredOn = new DateTime(2022, 11, 22) },

            new StudentDTO {    Id = new Guid("8b3e8170-4f89-11d3-9a0c-0305e82c9902"),
                                FirstName = "José",
                                LastName = "Castañeda",
                                DateOfBirth = new DateTime(2000, 3, 9),
                                EmailAddress = "josejose@joseemail.com",
                                RegisteredOn = new DateTime(2023, 8, 31) }
            };
        }
        //--------------------------------------------------------------------------
        [Fact]
        public async Task CallsServiceAndMapper_ReturnsCorrectView()
        {
            // Arrange
            List<StudentDTO> fakeStudents = GetFakeStudents();
            A.CallTo(() => _service.GetAllAsync()).Returns(Task.FromResult(fakeStudents));

            List<StudentView> fakeStudentViews = fakeStudents.Select(dto => new StudentView
            {
                Id = dto.Id,
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                DateOfBirth = dto.DateOfBirth,
                EmailAddress = dto.EmailAddress,
                RegisteredOn = dto.RegisteredOn
            }).ToList();

            A.CallTo(() => _mapper.Map<List<StudentView>>(fakeStudents)).Returns(fakeStudentViews);

            // Act
            ActionResult result = await _controller.ListStudents();

            // Assert
            ViewResult viewResult = Assert.IsType<ViewResult>(result);
            List<StudentView> model = Assert.IsType<List<StudentView>>(viewResult.Model);
            Assert.Equal(fakeStudents.Count, model.Count);

            A.CallTo(() => _service.GetAllAsync()).MustHaveHappenedOnceExactly();
            A.CallTo(() => _mapper.Map<List<StudentView>>(fakeStudents)).MustHaveHappenedOnceExactly();
        }
    }
}
