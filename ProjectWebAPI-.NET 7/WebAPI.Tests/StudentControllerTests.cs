using FakeItEasy;
using Microsoft.AspNetCore.Mvc;
using Model;
using Newtonsoft.Json;
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
        //----------------- GET ALL / LIST -------------- // moj GetAllAsync ima dva returna, listu ako uspije i exception ako ne, testam to dvoje i testiram konkretan popis
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
        [Fact]
        public async Task StudentController_GetAllAsync_ReturnFakeList() // fake lista
        {
            // Arrange
            List<StudentDTO> fakeStudents = GetFakeStudents();
            A.CallTo(() => _service.GetAllAsync()).Returns(fakeStudents); // service dohvaca fake studente umjesto podatke iz stvarnog servisa

            // Act
            var result = await _controller.GetAllAsync(); // sprema listu u result

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result); // provjerava i prebacuje u okResult

            List<StudentDTO> list = Assert.IsType<List<StudentDTO>>(okResult.Value); // provjerava i prebacuje u list, tu su list i fakeStudents sada isti

            Assert.Equal(fakeStudents.Count, list.Count);

            for (int i = 0; i < fakeStudents.Count; i++)
            {
                Assert.Equal(fakeStudents[i].Id, list[i].Id);
                Assert.Equal(fakeStudents[i].FirstName, list[i].FirstName);
                Assert.Equal(fakeStudents[i].LastName, list[i].LastName);
                Assert.Equal(fakeStudents[i].DateOfBirth, list[i].DateOfBirth);
                Assert.Equal(fakeStudents[i].EmailAddress, list[i].EmailAddress);
                Assert.Equal(fakeStudents[i].RegisteredOn, list[i].RegisteredOn);
            }
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
        [Fact]
        public async Task StudentController_GetOneByIdAsync_ReturnSpecificFakeStudent()
        {
            // Arrange
            List<StudentDTO> fakeStudents = GetFakeStudents();

            StudentDTO specificStudent = fakeStudents.First(s => s.Id == new Guid("6dcd4ce0-4f89-11d3-9a0c-0305e82c8811"));

            A.CallTo(() => _service.GetOneByIdAsync(specificStudent.Id)).Returns(specificStudent);

            // Act
            var result = await _controller.GetOneByIdAsync(specificStudent.Id);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var student = Assert.IsType<StudentDTO>(okResult.Value);

            Assert.Equal(specificStudent.Id, student.Id);
            Assert.Equal(specificStudent.FirstName, student.FirstName);
            Assert.Equal(specificStudent.LastName, student.LastName);
            Assert.Equal(specificStudent.DateOfBirth, student.DateOfBirth);
            Assert.Equal(specificStudent.EmailAddress, student.EmailAddress);
            Assert.Equal(specificStudent.RegisteredOn, student.RegisteredOn);
        }
        // ------------------- CREATE ---------------
        [Fact]
        public async Task StudentController_CreateAsync_CreationSuccess()
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

            A.CallTo(() => _service.CreateAsync(newStudent)).Returns(true);

            // Act
            var result = await _controller.CreateAsync(newStudent);

            // Assert
            var createdAtRouteResult = Assert.IsType<CreatedAtRouteResult>(result);

            var serializedValue = JsonConvert.SerializeObject(createdAtRouteResult.Value);
            var response = JsonConvert.DeserializeAnonymousType(serializedValue, new
            {
                message = default(string),
                data = default(StudentDTO)
            });

            Assert.NotNull(response);
            Assert.Equal("Student created. Ignore the NULL values, it's fine!", response.message);
            Assert.Equal(newStudent.Id, response.data.Id);
        }
        [Fact]
        public async Task StudentController_CreateAsync_CreationFail()
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

            A.CallTo(() => _service.CreateAsync(newStudent)).Returns(false);

            // Act
            var result = await _controller.CreateAsync(newStudent);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Failed to create", badRequestResult.Value);
        }
        [Fact]
        public async Task StudentController_CreateAsync_ExceptionThrown()
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

            var exceptionMessage = "Database connection error";
            A.CallTo(() => _service.CreateAsync(newStudent)).Throws(new Exception(exceptionMessage));

            // Act
            var result = await _controller.CreateAsync(newStudent);

            // Assert
            var objectResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal((int)HttpStatusCode.InternalServerError, objectResult.StatusCode);
            Assert.Equal($"Error for CreateAsync: {exceptionMessage}", objectResult.Value);
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
