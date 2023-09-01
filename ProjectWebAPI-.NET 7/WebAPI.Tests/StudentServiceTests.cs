using FakeItEasy;
using Model;
using Repository.Common;
using Service;
using Service.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebAPI.Controllers;

namespace WebAPI.Tests
{
    public class StudentServiceTests
    {
        private readonly StudentService _service;
        private readonly IRepository _repository;
        public StudentServiceTests()
        {
            _repository = A.Fake<IRepository>(); //FakeItEasy
            _service = new StudentService(_repository); // SUT, system under test
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
        //---------------------------------------------------------------------------
        [Fact]
        public async Task GetAllAsync_ReturnList()
        {
            // Arrange
            List<StudentDTO> fakeStudents = GetFakeStudents();
            A.CallTo(() => _repository.GetAllAsync()).Returns(fakeStudents);

            // Act
            List<StudentDTO> result = await _service.GetAllAsync();

            // Assert
            Assert.Equal(fakeStudents, result);
        }
        [Fact]
        public async Task GetOneByIdAsync_ReturnOneStudent()
        {
            // Arrange
            Guid id = new Guid("3f2504e0-4f89-11d3-9a0c-0305e82c3301");
            StudentDTO student = GetFakeStudents().FirstOrDefault(s => s.Id == id);

            A.CallTo(() => _repository.GetOneByIdAsync(id)).Returns(student);

            // Act
            var result = await _service.GetOneByIdAsync(student.Id);

            // Assert
            Assert.Equal(student, result);
        }
        [Fact]
        public async Task CreateAsync_ReturnsTrue()
        {
            // Arrange
            var newStudent = new StudentDTO
            {
                Id = new Guid("7f2504e0-4f89-11d3-9a0c-0405e82c4402"),
                FirstName = "Pierre",
                LastName = "Dupont",
                DateOfBirth = new DateTime(1989, 4, 5),
                EmailAddress = "pierre@pierreemail.com",
                RegisteredOn = DateTime.Now
            };

            A.CallTo(() => _repository.CreateAsync(newStudent)).Returns(true); // repository vraca true ako uspije kreirati novog

            // Act
            var result = await _service.CreateAsync(newStudent);

            // Assert
            Assert.True(result); // test prolazi ako true
        }
        [Fact]
        public async Task EditAsync_ReturnsTrue()
        {
            // Arrange
            Guid id = new Guid("3f2504e0-4f89-11d3-9a0c-0305e82c3301");
            StudentDTO studentToUpdate = GetFakeStudents().FirstOrDefault(s => s.Id == id);

            studentToUpdate.FirstName = "UpdatedName";

            A.CallTo(() => _repository.EditAsync(studentToUpdate, id)).Returns(true);

            // Act
            var result = await _service.EditAsync(studentToUpdate, id);

            // Assert
            Assert.True(result);
        }
        [Fact]
        public async Task DeleteAsync_ReturnsTrue()
        {
            // Arrange
            Guid id = new Guid("3f2504e0-4f89-11d3-9a0c-0305e82c3301");

            A.CallTo(() => _repository.DeleteAsync(id)).Returns(true); 

            // Act
            var result = await _service.DeleteAsync(id);

            // Assert
            Assert.True(result);
        }
        [Fact]
        public async Task ParamsAsync_ReturnsFilteredList()
        {
            // Arrange
            List<StudentDTO> fakeStudents = GetFakeStudents(); 
            string sortBy = "name_desc";
            string firstName = "o";
            string pageNumber = "2";
            string studentsPerPage = "1";

            A.CallTo(() => _repository.ParamsAsync(
                sortBy,
                firstName, null,
                null, null,
                null, null,
                pageNumber, studentsPerPage))
                .Returns(fakeStudents.Skip((int.Parse(pageNumber) - 1) * int.Parse(studentsPerPage))
                                    .Take(int.Parse(studentsPerPage)) 
                                    .ToList());
            // Act
            List<StudentDTO> result = await _service.ParamsAsync(sortBy, firstName, null, null, null, null, null, pageNumber, studentsPerPage);

            // Assert
            Assert.True(result.Count == 1); // znam da ocekujem jedan rezultat jer je studentsPerPage 1
            Assert.Contains(result, s => s.FirstName.Contains("o", StringComparison.OrdinalIgnoreCase));
        }
    }
}
