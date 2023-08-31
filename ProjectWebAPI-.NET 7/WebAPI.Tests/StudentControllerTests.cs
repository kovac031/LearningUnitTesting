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
        public async Task GetAllAsync_ReturnList() // lista
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
        public async Task GetAllAsync_ThrowException() // exception
        {
            // Arrange
            var exception = "Exception returned from service layer";
            A.CallTo(() => _service.GetAllAsync()).Throws(new Exception(exception));

            // Act
            var result = await _controller.GetAllAsync();

            // Assert
            var objectResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal((int)HttpStatusCode.InternalServerError, objectResult.StatusCode);
            Assert.Equal($"Error for GetAllAsync: {exception}", objectResult.Value);
        }
        [Fact]
        public async Task GetAllAsync_ReturnFakeList() // fake lista
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
        public async Task GetOneByIdAsync_ReturnObject()
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
        public async Task GetOneByIdAsync_ThrowException() 
        {
            // Arrange
            Guid id = Guid.NewGuid();
            var exception = "Exception returned from service layer";
            A.CallTo(() => _service.GetOneByIdAsync(id)).Throws(new Exception(exception));

            // Act
            var result = await _controller.GetOneByIdAsync(id);

            // Assert
            var objectResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal((int)HttpStatusCode.InternalServerError, objectResult.StatusCode);
            Assert.Equal($"Error for GetOneByIdAsync: {exception}", objectResult.Value);
        }
        [Fact]
        public async Task GetOneByIdAsync_ReturnSpecificFakeStudent()
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
        public async Task CreateAsync_CreationSuccess()
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

            A.CallTo(() => _service.CreateAsync(newStudent)).Returns(true); // CreateAsync(StudentDTO student), mocka true

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
        public async Task CreateAsync_CreationFail()
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
        public async Task CreateAsync_ExceptionThrown()
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

            var exceptionMessage = "Exception returned from service layer";
            A.CallTo(() => _service.CreateAsync(newStudent)).Throws(new Exception(exceptionMessage));

            // Act
            var result = await _controller.CreateAsync(newStudent);

            // Assert
            var objectResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal((int)HttpStatusCode.InternalServerError, objectResult.StatusCode);
            Assert.Equal($"Error for CreateAsync: {exceptionMessage}", objectResult.Value);
        }
        // -------------------- EDIT ---------------
        [Fact]
        public async Task EditAsync_EditSuccess()
        {
            // Arrange
            StudentDTO studentToEdit = new StudentDTO
            {
                Id = Guid.NewGuid(),
                FirstName = "Marcus",
                LastName = "Aurelius",
                DateOfBirth = new DateTime(2001, 1, 1),
                EmailAddress = "markimark@roma.com"
            };

            A.CallTo(() => _service.EditAsync(studentToEdit, studentToEdit.Id)).Returns(true); // u kontroleru je EditAsync(StudentDTO student, Guid id), dakle te parametre trazi jel, a mocka true

            // Act
            var result = await _controller.EditAsync(studentToEdit, studentToEdit.Id);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("Edited!", okResult.Value);
        }
        [Fact]
        public async Task EditAsync_EditEmailSuccess()
        {
            // Arrange
            Guid studentId = Guid.NewGuid();
            StudentDTO studentToEdit = new StudentDTO
            {
                Id = studentId,
                EmailAddress = "new.email@domain.com"
            };

            A.CallTo(() => _service.EditAsync(studentToEdit, studentId)).Returns(true);

            // Act
            var result = await _controller.EditAsync(studentToEdit, studentId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("Edited!", okResult.Value);
        }
        [Fact]
        public async Task EditAsync_EditFail()
        {
            // Arrange
            StudentDTO studentToEdit = new StudentDTO
            {
                Id = Guid.NewGuid(),
                FirstName = "Marcus",
                LastName = "Aurelius",
                DateOfBirth = new DateTime(2001, 1, 1),
                EmailAddress = "markimark@roma.com"
            };

            A.CallTo(() => _service.EditAsync(studentToEdit, studentToEdit.Id)).Returns(false);

            // Act
            var result = await _controller.EditAsync(studentToEdit, studentToEdit.Id);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Failed to edit", badRequestResult.Value);
        }
        [Fact]
        public async Task EditAsync_ExceptionThrown()
        {
            // Arrange
            StudentDTO studentToEdit = new StudentDTO
            {
                Id = Guid.NewGuid(),
                FirstName = "Marcus",
                LastName = "Aurelius",
                DateOfBirth = new DateTime(2001, 1, 1),
                EmailAddress = "markimark@roma.com"
            };

            var exceptionMessage = "Exception returned from service layer";
            A.CallTo(() => _service.EditAsync(studentToEdit, studentToEdit.Id)).Throws(new Exception(exceptionMessage));

            // Act
            var result = await _controller.EditAsync(studentToEdit, studentToEdit.Id);

            // Assert
            var objectResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal((int)HttpStatusCode.InternalServerError, objectResult.StatusCode);
            Assert.Equal($"Error for EditAsync: {exceptionMessage}", objectResult.Value);
        }
        // ------------------- DELETE ---------------
        [Fact]
        public async Task DeleteAsync_DeleteSuccess()
        {
            // Arrange
            Guid studentId = Guid.NewGuid(); // ne treba nam cijeli student jer se ne pojavljuje u metodi u kontroleru
            A.CallTo(() => _service.DeleteAsync(studentId)).Returns(true); 

            // Act
            var result = await _controller.DeleteAsync(studentId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("Deleted!", okResult.Value);
        }
        [Fact]
        public async Task DeleteAsync_DeleteFail()
        {
            // Arrange
            Guid studentId = Guid.NewGuid();

            A.CallTo(() => _service.DeleteAsync(studentId)).Returns(false);

            // Act
            var result = await _controller.DeleteAsync(studentId);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Failed to delete", badRequestResult.Value);
        }
        [Fact]
        public async Task DeleteAsync_ExceptionThrown()
        {
            // Arrange
            Guid studentId = Guid.NewGuid();
            var exceptionMessage = "Exception returned from service layer";
            A.CallTo(() => _service.DeleteAsync(studentId)).Throws(new Exception(exceptionMessage));

            // Act
            var result = await _controller.DeleteAsync(studentId);

            // Assert
            var objectResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal((int)HttpStatusCode.InternalServerError, objectResult.StatusCode);
            Assert.Equal($"Error for DeleteAsync: {exceptionMessage}", objectResult.Value);
        }
        // ------------ GET LIST WITH PARAMETERS ---------------
        [Fact]
        public async Task ParamsAsync_ReturnList_NoParams()
        {
            // Arrange
            List<StudentDTO> fakeStudents = GetFakeStudents();
            A.CallTo(() => _service.ParamsAsync(null,           // sortBy
                                                null, null,     // firstName, lastName
                                                null, null,     // dobBefore, dobAfter
                                                null, null,     // regBefore, regAfter
                                                null, null))    // pageNumber, studentsPerPage
                .Returns(fakeStudents);

            // Act
            var result = await _controller.ParamsAsync();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            List<StudentDTO> list = Assert.IsType<List<StudentDTO>>(okResult.Value);
            Assert.Equal(fakeStudents.Count, list.Count);
        }
        // ----------------------------------------------------- FILTERING
        [Fact]
        public async Task ParamsAsync_FilterByFirstName()
        {
            // Arrange
            List<StudentDTO> fakeStudents = GetFakeStudents().Where(s => s.FirstName.Contains("re", StringComparison.OrdinalIgnoreCase)).ToList();
            A.CallTo(() => _service.ParamsAsync(null, "re", null, null, null, null, null, null, null)).Returns(fakeStudents);

            // Act
            var result = await _controller.ParamsAsync(firstName: "re"); // tu vidi taj firstName iz kontrolera, ali iznad isto taj format ne prolazi

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            List<StudentDTO> list = Assert.IsType<List<StudentDTO>>(okResult.Value);
            Assert.Equal(fakeStudents.Count, list.Count);
        }
        //
        [Fact]
        public async Task ParamsAsync_FilterByLastName()
        {
            // Arrange
            List<StudentDTO> fakeStudents = GetFakeStudents().Where(s => s.LastName.Contains("a", StringComparison.OrdinalIgnoreCase)).ToList();
            A.CallTo(() => _service.ParamsAsync(null, null, "a", null, null, null, null, null, null)).Returns(fakeStudents);

            // Act
            var result = await _controller.ParamsAsync(lastName: "a"); 

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            List<StudentDTO> list = Assert.IsType<List<StudentDTO>>(okResult.Value);
            Assert.Equal(fakeStudents.Count, list.Count);
        }
        //
        [Fact]
        public async Task ParamsAsync_FilterByDateOfBirth_BornBefore()
        {
            DateTime filterDate = new DateTime(1995, 1, 1); // parametar za usporedbu

            // Arrange
            List<StudentDTO> fakeStudents = GetFakeStudents().Where(s => s.DateOfBirth <= filterDate).ToList();
            A.CallTo(() => _service.ParamsAsync(null, null, null, filterDate.ToString("yyyy-MM-dd"), null, null, null, null, null)).Returns(fakeStudents);

            // Act
            var result = await _controller.ParamsAsync(dobBefore: filterDate.ToString("yyyy-MM-dd"));

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            List<StudentDTO> list = Assert.IsType<List<StudentDTO>>(okResult.Value);
            Assert.Equal(fakeStudents.Count, list.Count);
        }
        [Fact]
        public async Task ParamsAsync_FilterByDateOfBirth_BornAfter()
        {
            DateTime filterDate = new DateTime(1995, 1, 1); // parametar za usporedbu

            // Arrange
            List<StudentDTO> fakeStudents = GetFakeStudents().Where(s => s.DateOfBirth >= filterDate).ToList();
            A.CallTo(() => _service.ParamsAsync(null, null, null, null, filterDate.ToString("yyyy-MM-dd"), null, null, null, null)).Returns(fakeStudents);

            // Act
            var result = await _controller.ParamsAsync(dobAfter: filterDate.ToString("yyyy-MM-dd"));

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            List<StudentDTO> list = Assert.IsType<List<StudentDTO>>(okResult.Value);
            Assert.Equal(fakeStudents.Count, list.Count);
        }
        //
        [Fact]
        public async Task ParamsAsync_FilterByRegisteredOn_RegisteredBefore()
        {
            DateTime filterDate = new DateTime(2022, 1, 1); // parametar za usporedbu

            // Arrange
            List<StudentDTO> fakeStudents = GetFakeStudents().Where(s => s.DateOfBirth <= filterDate).ToList();
            A.CallTo(() => _service.ParamsAsync(null, null, null, null, null, filterDate.ToString("yyyy-MM-dd"), null, null, null)).Returns(fakeStudents);

            // Act
            var result = await _controller.ParamsAsync(regBefore: filterDate.ToString("yyyy-MM-dd"));

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            List<StudentDTO> list = Assert.IsType<List<StudentDTO>>(okResult.Value);
            Assert.Equal(fakeStudents.Count, list.Count);
        }
        [Fact]
        public async Task ParamsAsync_FilterByRegisteredOn_RegisteredAfter()
        {
            DateTime filterDate = new DateTime(2022, 1, 1); // parametar za usporedbu

            // Arrange
            List<StudentDTO> fakeStudents = GetFakeStudents().Where(s => s.DateOfBirth >= filterDate).ToList();
            A.CallTo(() => _service.ParamsAsync(null, null, null, null, null, null, filterDate.ToString("yyyy-MM-dd"), null, null)).Returns(fakeStudents);

            // Act
            var result = await _controller.ParamsAsync(regAfter: filterDate.ToString("yyyy-MM-dd"));

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            List<StudentDTO> list = Assert.IsType<List<StudentDTO>>(okResult.Value);
            Assert.Equal(fakeStudents.Count, list.Count);
        }
        // ----------------------------------------------------- SORTING
        [Fact]
        public async Task ParamsAsync_SortByFirstName_Desc()
        {
            // Arrange
            List<StudentDTO> fakeStudents = GetFakeStudents().OrderByDescending(s => s.FirstName).ToList();
            A.CallTo(() => _service.ParamsAsync("name_desc", null, null, null, null, null, null, null, null)).Returns(fakeStudents);

            // Act
            var result = await _controller.ParamsAsync(sortBy: "name_desc");

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            List<StudentDTO> list = Assert.IsType<List<StudentDTO>>(okResult.Value);
            Assert.Equal(fakeStudents[0].FirstName, list[0].FirstName); // provjerava jesu li se poklopili pri sortiranju, ja znam da imam tri pa sam ih stavio tri
            Assert.Equal(fakeStudents[1].FirstName, list[1].FirstName);
            Assert.Equal(fakeStudents[2].FirstName, list[2].FirstName);
        }
        [Fact]
        public async Task ParamsAsync_SortByFirstName_Asc()
        {
            // Arrange
            List<StudentDTO> fakeStudents = GetFakeStudents().OrderByDescending(s => s.FirstName).ToList();
            A.CallTo(() => _service.ParamsAsync("name_asc", null, null, null, null, null, null, null, null)).Returns(fakeStudents);

            // Act
            var result = await _controller.ParamsAsync(sortBy: "name_asc");

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            List<StudentDTO> list = Assert.IsType<List<StudentDTO>>(okResult.Value);
            Assert.Equal(fakeStudents[0].FirstName, list[0].FirstName); 
            Assert.Equal(fakeStudents[1].FirstName, list[1].FirstName);
            Assert.Equal(fakeStudents[2].FirstName, list[2].FirstName);
        }
        //
        [Fact]
        public async Task ParamsAsync_SortByLastName_Desc()
        {
            // Arrange
            List<StudentDTO> fakeStudents = GetFakeStudents().OrderByDescending(s => s.LastName).ToList();
            A.CallTo(() => _service.ParamsAsync("surname_desc", null, null, null, null, null, null, null, null)).Returns(fakeStudents);

            // Act
            var result = await _controller.ParamsAsync(sortBy: "surname_desc");

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            List<StudentDTO> list = Assert.IsType<List<StudentDTO>>(okResult.Value);
            Assert.Equal(fakeStudents[0].FirstName, list[0].FirstName);
            Assert.Equal(fakeStudents[1].FirstName, list[1].FirstName);
            Assert.Equal(fakeStudents[2].FirstName, list[2].FirstName);
        }
        [Fact]
        public async Task ParamsAsync_SortByLastName_Asc()
        {
            // Arrange
            List<StudentDTO> fakeStudents = GetFakeStudents().OrderByDescending(s => s.LastName).ToList();
            A.CallTo(() => _service.ParamsAsync("surname_asc", null, null, null, null, null, null, null, null)).Returns(fakeStudents);

            // Act
            var result = await _controller.ParamsAsync(sortBy: "surname_asc");

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            List<StudentDTO> list = Assert.IsType<List<StudentDTO>>(okResult.Value);
            Assert.Equal(fakeStudents[0].FirstName, list[0].FirstName);
            Assert.Equal(fakeStudents[1].FirstName, list[1].FirstName);
            Assert.Equal(fakeStudents[2].FirstName, list[2].FirstName);
        }
        //
        [Fact]
        public async Task ParamsAsync_SortByDateOfBirth_Desc()
        {
            // Arrange
            List<StudentDTO> fakeStudents = GetFakeStudents().OrderByDescending(s => s.DateOfBirth).ToList();
            A.CallTo(() => _service.ParamsAsync("dob_desc", null, null, null, null, null, null, null, null)).Returns(fakeStudents);

            // Act
            var result = await _controller.ParamsAsync(sortBy: "dob_desc");

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            List<StudentDTO> list = Assert.IsType<List<StudentDTO>>(okResult.Value);
            Assert.Equal(fakeStudents[0].FirstName, list[0].FirstName);
            Assert.Equal(fakeStudents[1].FirstName, list[1].FirstName);
            Assert.Equal(fakeStudents[2].FirstName, list[2].FirstName);
        }
        [Fact]
        public async Task ParamsAsync_SortByDateOfBirth_Asc()
        {
            // Arrange
            List<StudentDTO> fakeStudents = GetFakeStudents().OrderByDescending(s => s.DateOfBirth).ToList();
            A.CallTo(() => _service.ParamsAsync("dob_asc", null, null, null, null, null, null, null, null)).Returns(fakeStudents);

            // Act
            var result = await _controller.ParamsAsync(sortBy: "dob_asc");

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            List<StudentDTO> list = Assert.IsType<List<StudentDTO>>(okResult.Value);
            Assert.Equal(fakeStudents[0].FirstName, list[0].FirstName);
            Assert.Equal(fakeStudents[1].FirstName, list[1].FirstName);
            Assert.Equal(fakeStudents[2].FirstName, list[2].FirstName);
        }
        [Fact]
        public async Task ParamsAsync_SortByRegisteredOn_Desc()
        {
            // Arrange
            List<StudentDTO> fakeStudents = GetFakeStudents().OrderByDescending(s => s.RegisteredOn).ToList();
            A.CallTo(() => _service.ParamsAsync("signup_desc", null, null, null, null, null, null, null, null)).Returns(fakeStudents);

            // Act
            var result = await _controller.ParamsAsync(sortBy: "signup_desc");

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            List<StudentDTO> list = Assert.IsType<List<StudentDTO>>(okResult.Value);
            Assert.Equal(fakeStudents[0].FirstName, list[0].FirstName);
            Assert.Equal(fakeStudents[1].FirstName, list[1].FirstName);
            Assert.Equal(fakeStudents[2].FirstName, list[2].FirstName);
        }
        [Fact]
        public async Task ParamsAsync_SortByRegisteredOn_Asc()
        {
            // Arrange
            List<StudentDTO> fakeStudents = GetFakeStudents().OrderByDescending(s => s.RegisteredOn).ToList();
            A.CallTo(() => _service.ParamsAsync("signup_asc", null, null, null, null, null, null, null, null)).Returns(fakeStudents);

            // Act
            var result = await _controller.ParamsAsync(sortBy: "signup_asc");

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            List<StudentDTO> list = Assert.IsType<List<StudentDTO>>(okResult.Value);
            Assert.Equal(fakeStudents[0].FirstName, list[0].FirstName);
            Assert.Equal(fakeStudents[1].FirstName, list[1].FirstName);
            Assert.Equal(fakeStudents[2].FirstName, list[2].FirstName);
        }
        // ------------------------------------------------------------ PAGING
        [Fact]
        public async Task ParamsAsync_Paging()
        {
            // Arrange
            List<StudentDTO> fakeStudents = GetFakeStudents().Skip(1).Take(1).ToList(); // Imam 3 studenta na listi, tako da je ovo 1 po stranici, druga stranica (prekoci 1)
            A.CallTo(() => _service.ParamsAsync(null, null, null, null, null, null, null, "2", "1")).Returns(fakeStudents);

            // Act
            var result = await _controller.ParamsAsync(pageNumber: "2", studentsPerPage: "1");

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            List<StudentDTO> list = Assert.IsType<List<StudentDTO>>(okResult.Value);
            Assert.Equal(fakeStudents.Count, list.Count);
        }
        // ------------------------------------------------------ EXCEPTION
        [Fact]
        public async Task ParamsAsync_ThrowException()
        {
            // Arrange
            var exception = "Exception returned from service layer";
            A.CallTo(() => _service.ParamsAsync(null, null, null, null, null, null, null, null, null)).Throws(new Exception(exception));

            // Act
            var result = await _controller.ParamsAsync();

            // Assert
            var objectResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal((int)HttpStatusCode.InternalServerError, objectResult.StatusCode);
            Assert.Equal($"Error for ParamsAsync: {exception}", objectResult.Value);
        }
    }
}
