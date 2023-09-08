using AutoMapper;
using Common;
using DataAccessLayer;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Model;
using Moq;
using Repository;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http.Results;

namespace WebAPI.Tests.RepositoryTests
{
    [TestClass]
    public class DeleteAsyncTests
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
        public async Task OnSuccess_ReturnsTrue()
        {
            // Arrange
            Guid id = new Guid("6dcd4ce0-4f89-11d3-9a0c-0305e82c8811");
            List<Student> fakeList = HelperClass.GetFakeDBStudents();
            Mock<DbSet<Student>> mockSet = HelperClass.CreateMockDbSet(fakeList);

            Mock<EFContext> mockContext = new Mock<EFContext>();
            mockContext.Setup(x => x.Students).Returns(mockSet.Object);

            // This setup is for handling FindAsync
            mockContext.Setup(x => x.Students.FindAsync(It.IsAny<object[]>()))
                       .ReturnsAsync((object[] keyValues) => fakeList.FirstOrDefault(s => s.Id == new Guid(keyValues[0].ToString())));

            StudentRepository repository = new StudentRepository(mockContext.Object, _mapper);

            // Act
            bool result = await repository.DeleteAsync(id);

            // Assert
            Assert.IsTrue(result);
            mockSet.Verify(m => m.Remove(It.IsAny<Student>()), Times.Once()); // Verify that the Remove method was called once
            mockContext.Verify(m => m.SaveChangesAsync(), Times.Once()); // Verify that SaveChangesAsync was called once
        }

        [TestMethod]
        public async Task OnFail_ReturnFalse() // exception
        {
            // Arrange
            Guid id = new Guid("6dcd4ce0-4f89-11d3-9a0c-0305e82c8811");
            List<Student> fakeList = HelperClass.GetFakeDBStudents();
            Mock<DbSet<Student>> mockSet = HelperClass.CreateMockDbSet(fakeList);

            Mock<EFContext> mockContext = new Mock<EFContext>();
            mockContext.Setup(x => x.Students).Returns(mockSet.Object);

            mockContext.Setup(x => x.Students.FindAsync(It.IsAny<object[]>()))
                       .ReturnsAsync((object[] keyValues) => fakeList.FirstOrDefault(s => s.Id == new Guid(keyValues[0].ToString())));

            StudentRepository repository = new StudentRepository(mockContext.Object, _mapper);

            mockContext.Setup(x => x.SaveChangesAsync()).ThrowsAsync(new Exception());

            // Act
            bool result = await repository.DeleteAsync(id);

            // Assert
            Assert.IsFalse(result);
            mockSet.Verify(m => m.Remove(It.IsAny<Student>()), Times.Once()); // Verify that the Remove method was called once
            mockContext.Verify(m => m.SaveChangesAsync(), Times.Once()); // Verify that SaveChangesAsync was called once
        }

    }
}
