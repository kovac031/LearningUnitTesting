using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Project.WebAPI.Controllers;
using Service.Common;
using System;
using System.Net.Http;
using System.Web.Http.Hosting;
using System.Web.Http;
using Repository.Common;
using Service;
using Model;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace WebAPI.Tests.ServiceTests
{
    [TestClass]
    public class StudentServiceTests
    {
        private StudentService _service;
        private Mock<IRepository> _repository;

        [TestInitialize]
        public void Setup()
        {
            _repository = new Mock<IRepository>();
            _service = new StudentService(_repository.Object);
        }
        //-------------------------------------------------------------------

        [TestMethod]
        public async Task GetAllAsync_OnSuccess_ReturnList()
        {
            // Arrange
            List<StudentDTO> fakeList = HelperClass.GetFakeStudents();
            _repository.Setup(x => x.GetAllAsync()).ReturnsAsync(fakeList);

            // Act
            List<StudentDTO> result = await _service.GetAllAsync();

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(fakeList.Count, result.Count);
        }

        [TestMethod]
        public async Task GetAllAsync_OnFail_ThrowsException()
        {
            // Arrange
            _repository.Setup(x => x.GetAllAsync()).ThrowsAsync(new Exception("MY TEST EXCEPTION"));

            // Act & Assert
            await Assert.ThrowsExceptionAsync<Exception>(() => _service.GetAllAsync());
        }

        //-----------------------------------------------------------------

        [TestMethod]
        public async Task GetOneByIdAsync_OnSuccess_ReturnStudent()
        {
            // Arrange
            Guid id = new Guid("3f2504e0-4f89-11d3-9a0c-0305e82c3301");
            StudentDTO fakeStudent = HelperClass.GetFakeStudents().FirstOrDefault(s => s.Id == id);
            _repository.Setup(x => x.GetOneByIdAsync(id)).ReturnsAsync(fakeStudent);

            // Act
            StudentDTO result = await _service.GetOneByIdAsync(id);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(fakeStudent.Id, result.Id);
            Assert.AreEqual(fakeStudent.FirstName, result.FirstName);
        }

        [TestMethod]
        public async Task GetOneByIdAsync_OnFail_ThrowsException()
        {
            // Arrange
            Guid id = Guid.NewGuid();
            _repository.Setup(x => x.GetOneByIdAsync(id)).ThrowsAsync(new Exception("MY TEST EXCEPTION"));

            // Act & Assert
            await Assert.ThrowsExceptionAsync<Exception>(() => _service.GetOneByIdAsync(id));
        }

        //-----------------------------------------------------------------

        [TestMethod]
        public async Task CreateAsync_OnSuccess_ReturnTrue()
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
            _repository.Setup(x => x.CreateAsync(newStudent)).ReturnsAsync(true);

            // Act
            bool result = await _service.CreateAsync(newStudent);

            // Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public async Task CreateAsync_OnFail_ReturnFalse()
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
            _repository.Setup(x => x.CreateAsync(newStudent)).ReturnsAsync(false);

            // Act
            bool result = await _service.CreateAsync(newStudent);

            // Assert
            Assert.IsFalse(result);
        }

        //-----------------------------------------------------------------

        [TestMethod]
        public async Task EditAsync_OnSuccess_ReturnTrue()
        {
            // Arrange
            StudentDTO updatedInfo = new StudentDTO // mogao sam stvarnoga dohvatiti ali rezultat s testom ce biti isti, tako da svejedno
            {
                EmailAddress = "newmail@email.com"
            };
            _repository.Setup(x => x.EditAsync(updatedInfo, updatedInfo.Id)).ReturnsAsync(true);

            // Act
            bool result = await _service.EditAsync(updatedInfo, updatedInfo.Id);

            // Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public async Task EditAsync_OnFail_ReturnFalse()
        {
            // Arrange
            StudentDTO updatedInfo = new StudentDTO 
            {
                EmailAddress = "newmail@email.com"
            };
            _repository.Setup(x => x.EditAsync(updatedInfo, updatedInfo.Id)).ReturnsAsync(false);

            // Act
            bool result = await _service.EditAsync(updatedInfo, updatedInfo.Id);

            // Assert
            Assert.IsFalse(result);
        }

        //-----------------------------------------------------------------

        [TestMethod]
        public async Task DeleteAsync_OnSuccess_ReturnTrue()
        {
            // Arrange
            Guid id = Guid.NewGuid();
            _repository.Setup(x => x.DeleteAsync(id)).ReturnsAsync(true);

            // Act
            bool result = await _service.DeleteAsync(id);

            // Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public async Task DeleteAsync_OnFail_ReturnFalse()
        {
            // Arrange
            Guid id = Guid.NewGuid();
            _repository.Setup(x => x.DeleteAsync(id)).ReturnsAsync(false);

            // Act
            bool result = await _service.DeleteAsync(id);

            // Assert
            Assert.IsFalse(result);
        }

        //-------------------------------------------------------------------

        [TestMethod]
        public async Task ParamsAsync_OnSuccess_ReturnModifiedList()
        {
            // Arrange
            List<StudentDTO> fakeList = HelperClass.GetFakeStudents().OrderBy(s => s.DateOfBirth).Skip(2).Take(1).ToList();
            _repository.Setup(x => x.ParamsAsync("dob_asc", null, null, null, null, null, null, "2", "1")).ReturnsAsync(fakeList);

            // Act
            List<StudentDTO> result = await _service.ParamsAsync("dob_asc", null, null, null, null, null, null, "2", "1");

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(fakeList.Count, result.Count);
            for (int i = 0; i < fakeList.Count; i++) //provjera jesu li jednako sortirani
            {
                Assert.AreEqual(fakeList[i].DateOfBirth, result[i].DateOfBirth);
            }
        }

        [TestMethod]
        public async Task ParamsAsync_OnFail_ThrowsException()
        {
            // Arrange
            _repository.Setup(x => x.ParamsAsync("dob_asc", null, null, null, null, null, null, "2", "1")).ThrowsAsync(new Exception("MY TEST EXCEPTION"));

            // Act & Assert
            await Assert.ThrowsExceptionAsync<Exception>(() => _service.ParamsAsync("dob_asc", null, null, null, null, null, null, "2", "1"));
        }
    }
}
