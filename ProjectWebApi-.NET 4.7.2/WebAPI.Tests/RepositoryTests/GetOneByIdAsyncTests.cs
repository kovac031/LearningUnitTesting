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
using System.Data.Entity.Core;
using System.Linq;
using System.Threading.Tasks;

namespace WebAPI.Tests.RepositoryTests
{
    [TestClass]
    public class GetOneByIdAsyncTests
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
        public async Task OnSuccess_ReturnExpectedStudent() // proradilo kad sam u kontroleru mako return await async
        {
            // Arrange
            Guid id = new Guid("6dcd4ce0-4f89-11d3-9a0c-0305e82c8811");
            List<Student> fakeList = HelperClass.GetFakeDBStudents(); // dohvacas listu
            Mock<DbSet<Student>> mockSet = HelperClass.CreateMockDbSet(fakeList); // listu stavljas u fake set

            Student fakeStudent = fakeList.First(s => s.Id == id); // samostalno nadjes studenta po id

            Mock<EFContext> mockContext = new Mock<EFContext>(); // iniciras fake context
            mockContext.Setup(x => x.Students).Returns(mockSet.Object); // fake set stavljas u fake context

            StudentRepository repository = new StudentRepository(mockContext.Object, _mapper); // fake context pozivas umjesto stvarnog kad testiras repositorij

            // Act
            StudentDTO result = await repository.GetOneByIdAsync(id); // tu ti repositorij logika nadje studenta po id

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(id, result.Id);
            Assert.AreEqual(fakeStudent.FirstName, result.FirstName);
            Assert.AreEqual(fakeStudent.LastName, result.LastName);
        }

        [TestMethod]
        [ExpectedException(typeof(EntityException))]
        public async Task OnDatabaseError_ThrowsException()
        {
            // Arrange
            Guid id = Guid.NewGuid();
            Mock<EFContext> mockContext = new Mock<EFContext>();
            mockContext.Setup(x => x.Students).Throws(new EntityException("Database Error"));

            StudentRepository repository = new StudentRepository(mockContext.Object, _mapper);

            // Act
            await repository.GetOneByIdAsync(id);

            // Assert
        }
    }
}
