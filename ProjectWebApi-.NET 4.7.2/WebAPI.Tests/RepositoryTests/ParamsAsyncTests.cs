﻿using AutoMapper;
using Common;
using DataAccessLayer;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Model;
using Moq;
using Repository;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core;
using System.Linq;
using System.Threading.Tasks;

namespace WebAPI.Tests.RepositoryTests
{
    [TestClass]
    public class ParamsAsyncTests
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
        public async Task NoParams_ReturnsDefaultList()
        {
            // Arrange
            List<Student> fakeList = HelperClass.GetFakeDBStudents();
            Mock<DbSet<Student>> mockSet = HelperClass.CreateMockDbSet(fakeList);

            Mock<EFContext> mockContext = new Mock<EFContext>();
            mockContext.Setup(c => c.Students).Returns(mockSet.Object);

            StudentRepository repository = new StudentRepository(mockContext.Object, _mapper);

            // My params
            string sortBy = null;
            string firstName = null;
            string lastName = null;
            string dobBefore = null;
            string dobAfter = null;
            string regBefore = null;
            string regAfter = null;
            string pageNumber = null;
            string studentsPerPage = null;

            // Act
            List<StudentDTO> result = await repository.ParamsAsync(
                sortBy,
                firstName, lastName,
                dobBefore, dobAfter,
                regBefore, regAfter,
                pageNumber, studentsPerPage
            );

            // Assert
            Assert.AreEqual(fakeList.Count, result.Count);
        }

        [TestMethod]
        public async Task SomeParams_ReturnsExpectedList()
        {
            // Arrange
            List<Student> fakeList = HelperClass.GetFakeDBStudents();
            Mock<DbSet<Student>> mockSet = HelperClass.CreateMockDbSet(fakeList);

            Mock<EFContext> mockContext = new Mock<EFContext>();
            mockContext.Setup(c => c.Students).Returns(mockSet.Object);

            StudentRepository repository = new StudentRepository(mockContext.Object, _mapper);

            string sortBy = "dob_asc";
            string firstName = null;
            string lastName = null;
            string dobBefore = null;
            string dobAfter = null;
            string regBefore = null;
            string regAfter = null;
            string pageNumber = "3";
            string studentsPerPage = "1";

            List<StudentDTO> expectedList = HelperClass.GetFakeStudents().OrderBy(s => s.DateOfBirth).Skip(2).Take(1).ToList(); // skip 2 je treca jer je 0 prva

            // Act
            List<StudentDTO> result = await repository.ParamsAsync(
                sortBy,
                firstName, lastName,
                dobBefore, dobAfter,
                regBefore, regAfter,
                pageNumber, studentsPerPage
            );

            // Assert
            Assert.AreEqual(expectedList.Count, result.Count);

            for (int i = 0; i < expectedList.Count; i++)
            {
                Assert.AreEqual(expectedList[i].Id, result[i].Id);
                Assert.AreEqual(expectedList[i].FirstName, result[i].FirstName);
                Assert.AreEqual(expectedList[i].LastName, result[i].LastName);                
            }
        }

        [TestMethod]
        [ExpectedException(typeof(EntityException))]
        public async Task SomeParams_ThrowsException()
        {
            // Arrange
            List<Student> fakeList = HelperClass.GetFakeDBStudents();
            Mock<DbSet<Student>> mockSet = HelperClass.CreateMockDbSet(fakeList);

            Mock<EFContext> mockContext = new Mock<EFContext>();
            mockContext.Setup(c => c.Students).Throws(new EntityException("Database Error"));

            StudentRepository repository = new StudentRepository(mockContext.Object, _mapper);

            string sortBy = "dob_asc";
            string firstName = null;
            string lastName = null;
            string dobBefore = null;
            string dobAfter = null;
            string regBefore = null;
            string regAfter = null;
            string pageNumber = "3";
            string studentsPerPage = "1";

            List<StudentDTO> expectedList = HelperClass.GetFakeStudents().OrderBy(s => s.DateOfBirth).Skip(2).Take(1).ToList(); // skip 2 je treca jer je 0 prva

            // Act
            List<StudentDTO> result = await repository.ParamsAsync(
                sortBy,
                firstName, lastName,
                dobBefore, dobAfter,
                regBefore, regAfter,
                pageNumber, studentsPerPage
            );

            // Assert
        }
    }
}
