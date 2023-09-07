using AutoMapper;
using Common;
using DataAccessLayer;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Model;
using Moq;
using Repository;
using Repository.Common;
using Service;
using Service.Common;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core;
using System.Threading.Tasks;

namespace WebAPI.Tests.RepositoryTests
{
    [TestClass]
    public class GetAllAsyncTests
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
        public async Task OnSuccess_ReturnList() // proradilo kad sam u kontroleru mako return await async
        {
            // Arrange
            List<Student> fakeList = HelperClass.GetFakeDBStudents();
            Mock<DbSet<Student>> mockSet = HelperClass.CreateMockDbSet(fakeList);

            Mock<EFContext> mockContext = new Mock<EFContext>();
            mockContext.Setup(c => c.Students).Returns(mockSet.Object);

            StudentRepository repository = new StudentRepository(mockContext.Object, _mapper);

            // Act
            List<StudentDTO> result = await repository.GetAllAsync();

            // Assert
            Assert.AreEqual(fakeList.Count, result.Count);
        }

        [TestMethod]
        [ExpectedException(typeof(EntityException))]
        public async Task OnDatabaseError_ThrowsException()
        {
            // Arrange            
            Mock<EFContext> mockContext = new Mock<EFContext>();
            mockContext.Setup(c => c.Students).Throws(new EntityException("Database Error"));

            StudentRepository repository = new StudentRepository(mockContext.Object, _mapper);

            // Act
            await repository.GetAllAsync();

            // Assert
        }
    }
}
