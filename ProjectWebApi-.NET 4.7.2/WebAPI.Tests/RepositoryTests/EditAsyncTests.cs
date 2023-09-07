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

namespace WebAPI.Tests.RepositoryTests
{
    [TestClass]
    public class EditAsyncTests
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
            Guid id = new Guid("6dcd4ce0-4f89-11d3-9a0c-0305e82c8811");
            List<Student> fakeList = HelperClass.GetFakeDBStudents(); // dohvacas listu
            Mock<DbSet<Student>> mockSet = HelperClass.CreateMockDbSet(fakeList); // listu stavljas u fake set

            Student fakeStudent = fakeList.First(s => s.Id == id); // samostalno nadjes studenta po id
            StudentDTO fakeStudentDTO = _mapper.Map<StudentDTO>(fakeStudent);

            Mock<EFContext> mockContext = new Mock<EFContext>(); // iniciras fake context
            mockContext.Setup(x => x.Students).Returns(mockSet.Object); // fake set stavljas u fake context

            // linija koja je spasila stvar // entity framework i Moq imaju svoje neke forice sta im treba da se slazu
            mockContext.Setup(x => x.Students.FindAsync(It.IsAny<object[]>())).ReturnsAsync((object[] keyValues) => fakeList.FirstOrDefault(s => s.Id == new Guid(keyValues[0].ToString())));
            //

            StudentRepository repository = new StudentRepository(mockContext.Object, _mapper); // fake context pozivas umjesto stvarnog kad testiras repositorij
            
            Console.WriteLine($"fakestudent email prije edita: {fakeStudent.EmailAddress}");
            Console.WriteLine($"fakestudentDTO email prije edita: {fakeStudentDTO.EmailAddress}");

            // Act
            fakeStudentDTO.EmailAddress = "newmail@lolmail.com"; // samo ovo editiram
            bool result = await repository.EditAsync(fakeStudentDTO, id); // tu ti repositorij logika nadje studenta po id
            
            Console.WriteLine($"fakestudent email nakon edita: {fakeStudent.EmailAddress}");
            Console.WriteLine($"fakestudentDTO email nakon edita: {fakeStudentDTO.EmailAddress}");

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(fakeStudentDTO.EmailAddress, fakeStudent.EmailAddress);
        }

        [TestMethod]
        public async Task OnFailure_ReturnFalse() // exception
        {
            // Arrange
            Guid id = new Guid("6dcd4ce0-4f89-11d3-9a0c-0305e82c8811");
            List<Student> fakeList = HelperClass.GetFakeDBStudents(); 
            Mock<DbSet<Student>> mockSet = HelperClass.CreateMockDbSet(fakeList); 

            Student fakeStudent = fakeList.First(s => s.Id == id); 
            StudentDTO fakeStudentDTO = _mapper.Map<StudentDTO>(fakeStudent); 

            Mock<EFContext> mockContext = new Mock<EFContext>(); 
            mockContext.Setup(x => x.Students).Returns(mockSet.Object); 

            mockContext.Setup(x => x.Students.FindAsync(It.IsAny<object[]>())).ReturnsAsync((object[] keyValues) => fakeList.FirstOrDefault(s => s.Id == new Guid(keyValues[0].ToString())));

            StudentRepository repository = new StudentRepository(mockContext.Object, _mapper); 

            // ovo je linija koja zada exception, iznad je sve isto
            mockContext.Setup(x => x.SaveChangesAsync()).ThrowsAsync(new Exception());

            // Act
            bool exceptionResult = await repository.EditAsync(fakeStudentDTO, id); 

            // Assert
            Assert.IsFalse(exceptionResult); 
        }
    }
}
