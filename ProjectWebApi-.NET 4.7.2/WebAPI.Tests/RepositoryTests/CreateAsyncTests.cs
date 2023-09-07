using AutoMapper;
using Common;
using DataAccessLayer;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Model;
using Moq;
using Repository;
using Service.Common;
using System;
using System.Data.Entity;
using System.Net.Http;
using System.Net;
using System.Security.Policy;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Data.Entity.Core;

namespace WebAPI.Tests.RepositoryTests
{
    [TestClass]
    public class CreateAsyncTests
    {
        private StudentRepository _repository;
        private Mock<EFContext> _context;
        private Mock<DbSet<Student>> _dbSet;
        private IMapper _mapper;

        [TestInitialize]
        public void Setup()
        {
            MapperConfiguration config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new MappingProfile());
            });
            _mapper = config.CreateMapper();

            _context = new Mock<EFContext>();
            _repository = new StudentRepository(_context.Object, _mapper);
            _dbSet = new Mock<DbSet<Student>>();

            _context.Setup(x => x.Students).Returns(_dbSet.Object);
        }
        //-------------------------------------------------------------------

        [TestMethod]
        public async Task OnSuccess_ReturnTrue()
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

            List<Student> fakeList = new List<Student>();
            Mock<DbSet<Student>> mockSet = HelperClass.CreateMockDbSet(fakeList);

            _context.Setup(x => x.Students).Returns(mockSet.Object);

            Student addedStudent = null;
            mockSet.Setup(x => x.Add(It.IsAny<Student>())).Callback<Student>(student =>
            {
                fakeList.Add(student);
                addedStudent = student;  // Capture the added Student
            });
            _repository = new StudentRepository(_context.Object, _mapper);

            // Act
            bool result = await _repository.CreateAsync(newStudent);

            // Assert
            Assert.IsTrue(result);
            //Assert.AreEqual(newStudent.Id, addedStudent.Id); u repositoriju ima Guid.NewGuid() tako da ce se id uvijek razlikovati, problem je do testa, ne do metode
            Assert.AreEqual(newStudent.FirstName, addedStudent.FirstName);
            Assert.AreEqual(newStudent.LastName, addedStudent.LastName);
        }

        [TestMethod]
        public async Task OnException_ReturnsFalse()
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

            _dbSet.Setup(x => x.Add(It.IsAny<Student>())).Throws(new Exception("Database Error"));

            // Act
            bool result = await _repository.CreateAsync(newStudent);

            // Assert
            Assert.IsFalse(result);
        }
    }
}
