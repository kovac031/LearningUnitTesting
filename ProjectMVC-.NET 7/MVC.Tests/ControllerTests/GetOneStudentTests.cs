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
    public class GetOneStudentTests
    {
        private readonly StudentController _controller;
        private readonly IService _service;
        private readonly IMapper _mapper;
        public GetOneStudentTests()
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
        private StudentDTO GetFakeStudentById(Guid id)
        {
            List<StudentDTO> fakeStudents = GetFakeStudents();
            return fakeStudents.FirstOrDefault(student => student.Id == id);
        }
        //--------------------------------------------------------------------------
        [Fact]
        public async Task GivenSpecificId_ReturnsCorrectStudentView()
        {
            // Arrange
            Guid specificId = new Guid("6dcd4ce0-4f89-11d3-9a0c-0305e82c8811");

            StudentDTO specificFakeStudentDTO = GetFakeStudentById(specificId);

            StudentView specificFakeStudentView = new StudentView
            {
                Id = specificFakeStudentDTO.Id,
                FirstName = specificFakeStudentDTO.FirstName,
                LastName = specificFakeStudentDTO.LastName,
                DateOfBirth = specificFakeStudentDTO.DateOfBirth,
                EmailAddress = specificFakeStudentDTO.EmailAddress,
                RegisteredOn = specificFakeStudentDTO.RegisteredOn
            };

            A.CallTo(() => _service.GetOneByIdAsync(specificId)).Returns(Task.FromResult(specificFakeStudentDTO));
            A.CallTo(() => _mapper.Map<StudentView>(specificFakeStudentDTO)).Returns(specificFakeStudentView);

            // Act
            ActionResult result = await _controller.GetOneStudent(specificId);

            // Assert
            ViewResult viewResult = Assert.IsType<ViewResult>(result);
            StudentView model = Assert.IsType<StudentView>(viewResult.Model);
            Assert.Equal(specificId, model.Id);

            A.CallTo(() => _service.GetOneByIdAsync(specificId)).MustHaveHappenedOnceExactly();
            A.CallTo(() => _mapper.Map<StudentView>(specificFakeStudentDTO)).MustHaveHappenedOnceExactly();
        }
    }
}
