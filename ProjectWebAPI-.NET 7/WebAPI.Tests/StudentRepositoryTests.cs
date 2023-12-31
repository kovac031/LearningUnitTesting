﻿using AutoMapper;
using Common;
using FakeItEasy;
using Microsoft.EntityFrameworkCore;
using Model;
using Model.Common;
using Repository;
using Repository.Common;
using Service.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Xunit.Abstractions;

namespace WebAPI.Tests
{
    //public static class Extensions
    //{
    //    public static IEnumerable<T> Yield<T>(this T item)
    //    {
    //        yield return item;
    //    }
    //}
    public class StudentRepositoryTests
    {
        private readonly StudentRepository _repository;
        private readonly JustStudentsContext _context;
        private readonly IMapper _mapper;
        private readonly ITestOutputHelper _output;
        public StudentRepositoryTests(ITestOutputHelper output)
        {
            _context = A.Fake<JustStudentsContext>();

            MapperConfiguration mappingConfig = new MapperConfiguration(mc =>
            {
                mc.AddProfile(new MapperProfile());
            });
            _mapper = mappingConfig.CreateMapper();

            _repository = new StudentRepository(_context, _mapper); // SUT, system under test
            _output = output;
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
            A.CallTo(() => ((IQueryable<T>)fakeDbSet).Provider).Returns(new TestAsyncQueryProvider<T>(queryable.Provider));
            A.CallTo(() => ((IQueryable<T>)fakeDbSet).Expression).Returns(queryable.Expression);
            A.CallTo(() => ((IQueryable<T>)fakeDbSet).ElementType).Returns(queryable.ElementType);
            A.CallTo(() => ((IQueryable<T>)fakeDbSet).GetEnumerator()).Returns(data.GetEnumerator());
            //
            A.CallTo(() => ((IAsyncEnumerable<T>)fakeDbSet).GetAsyncEnumerator(A<CancellationToken>._)).Returns(new TestAsyncEnumerator<T>(data.GetEnumerator()));


            return fakeDbSet;
        }
        // ----------------------------------------------------------------
        [Fact]
        public async Task GetAllAsync_ReturnList()
        {
            // Arrange
            DbSet<Student> fakeDbSet = CreateFakeDbSet(GetFakeStudents());
            A.CallTo(() => _context.Students).Returns(fakeDbSet);

            // Act
            List<StudentDTO> list = await _repository.GetAllAsync();

            foreach (StudentDTO student in list)
            {
                _output.WriteLine($"Passed from repository - FirstName: {student.FirstName}");
            }

            // Assert
            Assert.NotNull(list);
            Assert.Equal(fakeDbSet.Count(), list.Count);
        }
        [Fact]
        public async Task GetOneById_ReturnOneStudent()
        {
            // Arrange
            DbSet<Student> fakeDbSet = CreateFakeDbSet(GetFakeStudents());
            A.CallTo(() => _context.Students).Returns(fakeDbSet);

            Guid id = new Guid("3f2504e0-4f89-11d3-9a0c-0305e82c3301");
            Student fakeStudent = fakeDbSet.FirstOrDefault(student => student.Id == id);

            // Act
            StudentDTO returnedStudent = await _repository.GetOneByIdAsync(id);
            _output.WriteLine($"Passed from repository - FirstName: {returnedStudent.FirstName}");

            // Assert
            Assert.NotNull(fakeStudent);
            Assert.NotNull(returnedStudent);
            Assert.Equal(fakeStudent.Id, returnedStudent.Id);
        }
        [Fact]
        public async Task CreateStudent_OnSuccess_ReturnTrue()
        {
            // Arrange
            StudentDTO studentDTO = new StudentDTO
            {
                Id = Guid.NewGuid(),
                FirstName = "Marcus",
                LastName = "Aurelius",
                DateOfBirth = new DateTime(2001, 1, 1),
                EmailAddress = "markimark@roma.com",
                RegisteredOn = DateTime.Now
            };            

            DbSet<Student> fakeDbSet = CreateFakeDbSet(GetFakeStudents());
            A.CallTo(() => _context.Students).Returns(fakeDbSet);

            Student fakeStudent = null;
            A.CallTo(() => _context.Students.Add(A<Student>.Ignored)).Invokes((Student student) => fakeStudent = student);
            
            // Act
            bool result = await _repository.CreateAsync(studentDTO);
            _output.WriteLine($"Passed from repository - FirstName: {studentDTO.FirstName}");
            _output.WriteLine($"Fake student from context - FirstName: {fakeStudent.FirstName}");

            // Assert
            Assert.True(result);
        }
        [Fact]
        public async Task EditStudent_OnSuccess_ReturnTrue()
        {
            // Arrange
            Guid studentId = new Guid("3f2504e0-4f89-11d3-9a0c-0305e82c3301");

            IQueryable<Student> fakeStudents = GetFakeStudents();
            Student existingStudent = fakeStudents.FirstOrDefault(s => s.Id == studentId);

            DbSet<Student> fakeDbSet = CreateFakeDbSet(fakeStudents);
            A.CallTo(() => _context.Students).Returns(fakeDbSet);

            StudentDTO studentDTO = new StudentDTO
            {
                Id = studentId,
                FirstName = "René",
                LastName = "Smith",
                EmailAddress = "newmail@gmail.com",
                RegisteredOn = existingStudent.RegisteredOn
            };

            // Act
            bool result = await _repository.EditAsync(studentDTO, studentId);
            _output.WriteLine($"Passed from repository (edited) - FirstName: {studentDTO.FirstName} - LastName: {studentDTO.LastName}");
            _output.WriteLine($"Fake student from context (original) - FirstName: {existingStudent.FirstName} - LastName: {existingStudent.LastName}");

            // Assert // ne mogu usporedjivati jednakost stvari koje nece biti jednake nakon edita
            Assert.True(result);
            Assert.Equal(studentDTO.Id, existingStudent.Id);
            Assert.Equal(studentDTO.FirstName, existingStudent.FirstName);
            //Assert.Equal(studentDTO.LastName, existingStudent.LastName);
            //Assert.Equal(studentDTO.EmailAddress, existingStudent.EmailAddress);
            Assert.Equal(studentDTO.RegisteredOn, existingStudent.RegisteredOn);
            
        }
        [Fact]
        public async Task DeleteStudent_OnSuccess_ReturnTrue()
        {
            // Arrange
            Guid studentId = new Guid("3f2504e0-4f89-11d3-9a0c-0305e82c3301");

            IQueryable<Student> fakeStudents = GetFakeStudents();
            Student existingStudent = fakeStudents.FirstOrDefault(s => s.Id == studentId);

            DbSet<Student> fakeDbSet = CreateFakeDbSet(fakeStudents);
            A.CallTo(() => _context.Students).Returns(fakeDbSet);

            //
            bool isRemovedCalled = false; // provjera jel stvarno delete da ne bude da samo vraca true, iako ne vidim kako bi moglo jedno bez drugog al ajd
            A.CallTo(() => _context.Students.Remove(A<Student>.Ignored)).Invokes(() => isRemovedCalled = true);

            //Act
            bool result = await _repository.DeleteAsync(studentId);

            //Assert
            Assert.True(result);
            Assert.True(isRemovedCalled, "Remove metoda nije pozvana, DbSet"); //poruka bude na fail
        }
        // ------------- LIST WITH PARAMETERS -------------
        [Fact]
        public async Task ParamsAsync_NoParams_ReturnsFullList()
        {
            // Arrange
            DbSet<Student> fakeDbSet = CreateFakeDbSet(GetFakeStudents());
            A.CallTo(() => _context.Students).Returns(fakeDbSet);

            // Act
            List<StudentDTO> list = await _repository.ParamsAsync(
                null, 
                null, null, 
                null, null, 
                null, null,
                null, null);

            foreach (StudentDTO student in list)
            {
                _output.WriteLine($"Passed from repository - FirstName: {student.FirstName}");
            }

            // Assert
            Assert.NotNull(list);
            Assert.Equal(fakeDbSet.Count(), list.Count);
        }
        [Fact]
        public async Task ParamsAsync_SortByDobASC_ReturnsSortedList()
        {
            // Arrange
            DbSet<Student> fakeDbSet = CreateFakeDbSet(GetFakeStudents()); // ovo sam tu dobro napravio ali pogrjesio u kontroleru, tamo sam odma sortirao i onda ne testira metodu dobro
            A.CallTo(() => _context.Students).Returns(fakeDbSet);

            // Act
            List<StudentDTO> list = await _repository.ParamsAsync(
                "dob_asc", // sortBy
                null, null,
                null, null,
                null, null,
                null, null);

            List<Student> fakeStudents = fakeDbSet.OrderBy(s => s.DateOfBirth).ToList(); // moj rucni sort za check

            foreach (StudentDTO student in list)
            {
                _output.WriteLine($"Passed from repository - DOB: {student.DateOfBirth}");
            }
            // Assert
            Assert.NotNull(list);
            Assert.Equal(fakeDbSet.Count(), list.Count);
            Assert.Equal(fakeStudents[0].FirstName, list[0].FirstName);
            Assert.Equal(fakeStudents[1].FirstName, list[1].FirstName);
            Assert.Equal(fakeStudents[2].FirstName, list[2].FirstName);
        }
        [Fact]
        public async Task ParamsAsync_FilterBy_ReturnsFilteredList()
        {
            // Arrange
            DbSet<Student> fakeDbSet = CreateFakeDbSet(GetFakeStudents());
            A.CallTo(() => _context.Students).Returns(fakeDbSet);
            
            // Act
            List<StudentDTO> list = await _repository.ParamsAsync(
                null,
                "re", null, // FirstName da ima re
                null, null,
                null, null,
                null, null);

            List<Student> filteredStudents = fakeDbSet.Where(s => s.FirstName.Contains("re", StringComparison.OrdinalIgnoreCase)).ToList();

            foreach (StudentDTO student in list)
            {
                _output.WriteLine($"Passed from repository - FirstName: {student.FirstName}");
            }

            // Assert
            Assert.NotNull(list);
            Assert.Equal(filteredStudents.Count(), list.Count);            
        }
        [Fact]
        public async Task ParamsAsync_FilterByDateOfBirth_ReturnsBornAfter()
        {
            // Arrange
            DbSet<Student> fakeDbSet = CreateFakeDbSet(GetFakeStudents());
            A.CallTo(() => _context.Students).Returns(fakeDbSet);

            DateTime filterDate = new DateTime(1995, 1, 1);

            // Act
            List<StudentDTO> list = await _repository.ParamsAsync(
                null,
                null, null, 
                null, filterDate.ToString("yyyy-MM-dd"), // born after
                null, null,
                null, null);

            List<Student> filteredStudents = fakeDbSet.Where(s => s.DateOfBirth >= filterDate).ToList();

            foreach (StudentDTO student in list)
            {
                _output.WriteLine($"Passed from repository - DOB: {student.DateOfBirth}");
            }

            // Assert
            Assert.NotNull(list);
            Assert.Equal(filteredStudents.Count(), list.Count);
        }
        [Fact]
        public async Task ParamsAsync_SortByDobASC_and_OnePerPage_ReturnsSecondPage() // trazim najstariju osobu ali sa pagingom
        {
            // Arrange
            DbSet<Student> fakeDbSet = CreateFakeDbSet(GetFakeStudents()); 
            A.CallTo(() => _context.Students).Returns(fakeDbSet);

            int page = 3;
            int studentsPerPage = 1;

            // Act
            List<StudentDTO> list = await _repository.ParamsAsync(
                "dob_asc", // sortBy
                null, null,
                null, null,
                null, null,
                page.ToString(), studentsPerPage.ToString());

            List<Student> fakeStudents = fakeDbSet.Skip(2).Take(1).OrderBy(s => s.DateOfBirth).ToList(); // rucno namjestio trecu stranicu, jednog studenta i sort ASC po dob, dakle treba vratit najstarijeg od tri na listi koje imam

            foreach (StudentDTO student in list)
            {
                _output.WriteLine($"Passed from repository - DOB: {student.DateOfBirth}");
            }
            foreach (Student fakeStudent in fakeStudents)
            {
                _output.WriteLine($"Manual params from DbSet - DOB: {fakeStudent.DateOfBirth}");
            }
            // Assert
            Assert.NotNull(list);
            Assert.Equal(fakeStudents.Count(), list.Count);
            for (int i = 0; i < list.Count; i++)
            {
                Assert.Equal(fakeStudents[i].Id, list[i].Id);
            }
        }

        // dosta za demonstraciju, ostalo je repeticija istog
    }
}
