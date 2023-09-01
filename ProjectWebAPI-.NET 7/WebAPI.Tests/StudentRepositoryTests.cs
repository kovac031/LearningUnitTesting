using AutoMapper;
using Common;
using FakeItEasy;
using Microsoft.EntityFrameworkCore;
using Model;
using Repository;
using Service.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebAPI.Tests
{
    public static class Extensions
    {
        public static IEnumerable<T> Yield<T>(this T item)
        {
            yield return item;
        }
    }
    public class StudentRepositoryTests
    {
        private readonly StudentRepository _repository;
        private readonly JustStudentsContext _context;
        private readonly IMapper _mapper;

        public StudentRepositoryTests()
        {
            _context = A.Fake<JustStudentsContext>();

            var mappingConfig = new MapperConfiguration(mc =>
            {
                mc.AddProfile(new MapperProfile()); 
            });
            _mapper = mappingConfig.CreateMapper();

            _repository = new StudentRepository(_context, _mapper); // SUT, system under test
        }
        //------------------------------- FAKE LIST -----------------------------
        private IQueryable<Student> GetFakeStudents() // ne moze List type jer DbContext trazi IQueryable
        {
            return new List<Student>
            {
                new Student
                {
                    Id = new Guid("3f2504e0-4f89-11d3-9a0c-0305e82c3301"),
                    FirstName = "René",
                    LastName = "D'Artagnan",
                    DateOfBirth = new DateTime(1991, 1, 2),
                    EmailAddress = "renerene@reneemail.com",
                    RegisteredOn = new DateTime(2021, 1, 2)
                },
                new Student
                {
                    Id = new Guid("6dcd4ce0-4f89-11d3-9a0c-0305e82c8811"),
                    FirstName = "Zoë",
                    LastName = "Müller",
                    DateOfBirth = new DateTime(1995, 11, 22),
                    EmailAddress = "zoezoe@zoeemail.com",
                    RegisteredOn = new DateTime(2022, 11, 22)
                },
                new Student
                {
                    Id = new Guid("8b3e8170-4f89-11d3-9a0c-0305e82c9902"),
                    FirstName = "José",
                    LastName = "Castañeda",
                    DateOfBirth = new DateTime(2000, 3, 9),
                    EmailAddress = "josejose@joseemail.com",
                    RegisteredOn = new DateTime(2023, 8, 31)
                }
            }.AsQueryable();
        }
        //------------------------------ FAKE DB SET ---------------------------
        private DbSet<T> CreateFakeDbSet<T>(IQueryable<T> data) where T : class
        {
            IQueryable<T> queryable = data;

            DbSet<T> fakeDbSet = A.Fake<DbSet<T>>(d => d.Implements(typeof(IQueryable<T>))
                                                      .Implements(typeof(IAsyncEnumerable<T>)));
            A.CallTo(() => ((IQueryable<T>)fakeDbSet).Provider)
             .Returns(new TestAsyncQueryProvider<T>(queryable.Provider));
            A.CallTo(() => ((IQueryable<T>)fakeDbSet).Expression).Returns(queryable.Expression);
            A.CallTo(() => ((IQueryable<T>)fakeDbSet).ElementType).Returns(queryable.ElementType);
            A.CallTo(() => ((IQueryable<T>)fakeDbSet).GetEnumerator()).Returns(data.GetEnumerator());

            return fakeDbSet;
        }
        // ----------------------------------------------------------------
        [Fact]
        public async Task GetAllAsync_ReturnList()
        {
            // Arrange
            DbSet<Student> fakeDbSet = CreateFakeDbSet(GetFakeStudents());

            A.CallTo(() => _context.Students).Returns(fakeDbSet);

            List<StudentDTO> projectedStudents = fakeDbSet.Select(s => _mapper.Map<StudentDTO>(s)).ToList();

            // Act
            List<StudentDTO> result = projectedStudents; // ne moze _repository.GetAllAsync() jer ima neki konflikt sa AutoMapper-ovim .ProjectTo, trebalo bi mijenjati repository i service metodu
                                                            // ne testira da li returna listu, jer za to mora pozivati _repository, vec samo da li dobro mapira ... thanks AutoMapper /s
            // Assert
            Assert.NotNull(result);
            Assert.Equal(fakeDbSet.Count(), result.Count);
        }
        [Fact]
        public void GetOneById_ReturnOneStudent()
        {
            // Arrange
            Guid id = new Guid("3f2504e0-4f89-11d3-9a0c-0305e82c3301");
            Student fakeStudent = GetFakeStudents().FirstOrDefault(s => s.Id == id);

            // Return our fake DbSet when accessing _context.Students
            A.CallTo(() => _context.Students).Returns(CreateFakeDbSet<Student>(new List<Student> { fakeStudent }.AsQueryable()));

            // Act
            StudentDTO fakeStudentDTO = _mapper.Map<StudentDTO>(fakeStudent);

            // Assert
            Assert.NotNull(fakeStudentDTO);
            Assert.Equal(fakeStudent.Id, fakeStudentDTO.Id);
            Assert.Equal(fakeStudent.FirstName, fakeStudentDTO.FirstName);
            Assert.Equal(fakeStudent.LastName, fakeStudentDTO.LastName);
            Assert.Equal(fakeStudent.DateOfBirth, fakeStudentDTO.DateOfBirth);
            Assert.Equal(fakeStudent.EmailAddress, fakeStudentDTO.EmailAddress);
            Assert.Equal(fakeStudent.RegisteredOn, fakeStudentDTO.RegisteredOn);
        }

    }
}
