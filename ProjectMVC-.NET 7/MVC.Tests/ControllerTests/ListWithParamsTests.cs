using AutoMapper;
using FakeItEasy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestPlatform.Utilities;
using Model;
using MVC.Controllers;
using Service.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using X.PagedList;
using Xunit.Abstractions;

namespace MVC.Tests.ControllerTests
{
    public class ListWithParamsTests
    {
        private readonly StudentController _controller;
        private readonly IService _service;
        private readonly IMapper _mapper;

        private readonly ITestOutputHelper _output;

        public ListWithParamsTests(ITestOutputHelper output)
        {
            _service = A.Fake<IService>(); //FakeItEasy
            _mapper = A.Fake<IMapper>();
            _controller = new StudentController(_service, _mapper); // SUT, system under test
            _output = output;
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
        //--------------------------------------------------------------------------
        [Fact]
        public async Task NoParametersGiven_ReturnsFullList()
        {
            // Arrange
            List<StudentDTO> fakeStudents = GetFakeStudents();
            A.CallTo(() => _service.ListWithParams(A<string>.Ignored)).Returns(fakeStudents);

            // Act
            ActionResult result = await _controller.ListWithParams(null, null, null, null, null, null, null);

            // Assert
            ViewResult viewResult = Assert.IsType<ViewResult>(result);
            StaticPagedList<StudentView> list = Assert.IsAssignableFrom<StaticPagedList<StudentView>>(viewResult.Model);
            Assert.Equal(fakeStudents.Count, list.TotalItemCount);
        }
        [Fact]
        public async Task SortBy_ReturnsSortedList()
        {
            // Arrange
            List<StudentDTO> fakeStudents = GetFakeStudents();
            A.CallTo(() => _service.ListWithParams(A<string>.Ignored)).Returns(fakeStudents);
            Assert.NotEmpty(fakeStudents);

            // Act
            _mapper.ConfigurationProvider.AssertConfigurationIsValid();

            ActionResult result = await _controller.ListWithParams( sortBy: "dob_asc",
                                                                    searchBy: null,
                                                                    dobMin: null,
                                                                    dobMax: null,
                                                                    regMin: null,
                                                                    regMax: null,
                                                                    page: null);

            // Assert
            ViewResult viewResult = Assert.IsType<ViewResult>(result);
            StaticPagedList<StudentView> sortedList = Assert.IsAssignableFrom<StaticPagedList<StudentView>>(viewResult.Model);
            
            foreach (StudentView student in sortedList)
            {
                _output.WriteLine($"Passed from controller - FirstName: {student.FirstName}, LastName: {student.LastName}");
            }
            
            Assert.Equal(sortedList.Count, fakeStudents.Count);

            List<StudentDTO> meSorting = fakeStudents.OrderBy(s => s.DateOfBirth).ToList();

            for (int i = 0; i < meSorting.Count; i++)
            {
                Assert.Equal(meSorting[i].DateOfBirth, sortedList[i].DateOfBirth); // usporedjuje sa liste, jedan po jedan unos, ako se poklope jednako su sortirani
            }
        }
        [Fact]
        public async Task SearchBy_ReturnsFilteredList()
        {
            // Arrange
            List<StudentDTO> fakeStudents = GetFakeStudents();
            A.CallTo(() => _service.ListWithParams(A<string>.Ignored)).Returns(fakeStudents);

            // Act
            ActionResult result = await _controller.ListWithParams( sortBy: null,
                                                                    searchBy: "re",  
                                                                    dobMin: null,
                                                                    dobMax: null,
                                                                    regMin: null,
                                                                    regMax: null,
                                                                    page: null);


            // Assert
            ViewResult viewResult = Assert.IsType<ViewResult>(result);
            StaticPagedList<StudentView> list = Assert.IsAssignableFrom<StaticPagedList<StudentView>>(viewResult.Model);

            foreach (StudentView student in list)
            {
                _output.WriteLine($"Passed from controller - FirstName: {student.FirstName}, LastName: {student.LastName}");
            }

            List<StudentDTO> filteredStudents = fakeStudents.Where(s => s.FirstName.IndexOf("re", StringComparison.OrdinalIgnoreCase) >= 0 ||
                                                                        s.LastName.IndexOf("re", StringComparison.OrdinalIgnoreCase) >= 0).ToList();
            Assert.Equal(filteredStudents.Count, list.TotalItemCount);
        }
        [Fact]
        public async Task BornAfter_ReturnsFilteredList()
        {
            DateTime dobMin = DateTime.Parse("1995-01-01");

            // Arrange
            List<StudentDTO> fakeStudents = GetFakeStudents();
            A.CallTo(() => _service.ListWithParams(A<string>.Ignored)).Returns(fakeStudents);

            // Act
            ActionResult result = await _controller.ListWithParams(sortBy: null,
                                                                    searchBy: null,
                                                                    dobMin,
                                                                    dobMax: null,
                                                                    regMin: null,
                                                                    regMax: null,
                                                                    page: null);


            // Assert
            ViewResult viewResult = Assert.IsType<ViewResult>(result);
            StaticPagedList<StudentView> list = Assert.IsAssignableFrom<StaticPagedList<StudentView>>(viewResult.Model);

            foreach (StudentView student in list)
            {
                _output.WriteLine($"Passed from controller - FirstName: {student.FirstName}, DOB: {student.DateOfBirth}");
            }

            List<StudentDTO> filteredStudents = fakeStudents.Where(s => s.DateOfBirth >= dobMin).ToList();
            Assert.Equal(filteredStudents.Count, list.TotalItemCount);
        }
        [Fact]
        public async Task BornBefore_ReturnsFilteredList()
        {
            DateTime dobMax = DateTime.Parse("1995-01-01");

            // Arrange
            List<StudentDTO> fakeStudents = GetFakeStudents();
            A.CallTo(() => _service.ListWithParams(A<string>.Ignored)).Returns(fakeStudents);

            // Act
            ActionResult result = await _controller.ListWithParams(sortBy: null,
                                                                    searchBy: null,
                                                                    dobMin: null,
                                                                    dobMax,
                                                                    regMin: null,
                                                                    regMax: null,
                                                                    page: null);


            // Assert
            ViewResult viewResult = Assert.IsType<ViewResult>(result);
            StaticPagedList<StudentView> list = Assert.IsAssignableFrom<StaticPagedList<StudentView>>(viewResult.Model);

            foreach (StudentView student in list)
            {
                _output.WriteLine($"Passed from controller - FirstName: {student.FirstName}, DOB: {student.DateOfBirth}");
            }

            List<StudentDTO> filteredStudents = fakeStudents.Where(s => s.DateOfBirth <= dobMax).ToList();
            Assert.Equal(filteredStudents.Count, list.TotalItemCount);
        }
        //
        [Fact]
        public async Task RegisteredAfter_ReturnsFilteredList()
        {
            DateTime regMin = DateTime.Parse("2022-01-01");

            // Arrange
            List<StudentDTO> fakeStudents = GetFakeStudents();
            A.CallTo(() => _service.ListWithParams(A<string>.Ignored)).Returns(fakeStudents);

            // Act
            ActionResult result = await _controller.ListWithParams(sortBy: null,
                                                                    searchBy: null,
                                                                    dobMin: null,
                                                                    dobMax: null,
                                                                    regMin,
                                                                    regMax: null,
                                                                    page: null);


            // Assert
            ViewResult viewResult = Assert.IsType<ViewResult>(result);
            StaticPagedList<StudentView> list = Assert.IsAssignableFrom<StaticPagedList<StudentView>>(viewResult.Model);

            foreach (StudentView student in list)
            {
                _output.WriteLine($"Passed from controller - FirstName: {student.FirstName}, RegisteredOn: {student.RegisteredOn}");
            }

            List<StudentDTO> filteredStudents = fakeStudents.Where(s => s.RegisteredOn >= regMin).ToList();
            Assert.Equal(filteredStudents.Count, list.TotalItemCount);
        }
        [Fact]
        public async Task RegisteredBefore_ReturnsFilteredList()
        {
            DateTime regMax = DateTime.Parse("2022-01-01");

            // Arrange
            List<StudentDTO> fakeStudents = GetFakeStudents();
            A.CallTo(() => _service.ListWithParams(A<string>.Ignored)).Returns(fakeStudents);

            // Act
            ActionResult result = await _controller.ListWithParams(sortBy: null,
                                                                    searchBy: null,
                                                                    dobMin: null,
                                                                    dobMax: null,
                                                                    regMin: null,
                                                                    regMax,
                                                                    page: null);


            // Assert
            ViewResult viewResult = Assert.IsType<ViewResult>(result);
            StaticPagedList<StudentView> list = Assert.IsAssignableFrom<StaticPagedList<StudentView>>(viewResult.Model);

            foreach (StudentView student in list)
            {
                _output.WriteLine($"Passed from controller - FirstName: {student.FirstName}, DOB: {student.RegisteredOn}");
            }

            List<StudentDTO> filteredStudents = fakeStudents.Where(s => s.RegisteredOn <= regMax).ToList();
            Assert.Equal(filteredStudents.Count, list.TotalItemCount);
        }
        [Fact]
        public async Task SpecificPageSet_ReturnsFilteredList()
        {
            // Arrange
            List<StudentDTO> fakeStudents = GetFakeStudents();
            A.CallTo(() => _service.ListWithParams(A<string>.Ignored)).Returns(fakeStudents);

            int pageNumber = 1;
            int pageSize = 5; // kontroler ce uvijek returnat koliko je u kontroleru zadano, s obzirom da ih je fejkano 3, a u kontroleru pagesize 5, uvijek ce sva 3 vratiti
                                // ako zadam ovdje manje od 3, test faila, jer ocekuje koliko mu zadam a kontroler returna svo troje jer ih moze sve kako je setano u kontroleru

            // Act
            ActionResult result = await _controller.ListWithParams(sortBy: null,
                                                                    searchBy: null,
                                                                    dobMin: null,
                                                                    dobMax: null,
                                                                    regMin: null,
                                                                    regMax: null,
                                                                    page: pageNumber);

            // Assert
            ViewResult viewResult = Assert.IsType<ViewResult>(result);
            StaticPagedList<StudentView> list = Assert.IsAssignableFrom<StaticPagedList<StudentView>>(viewResult.Model);

            foreach (StudentView student in list)
            {
                _output.WriteLine($"Passed from controller - FirstName: {student.FirstName}, LastName: {student.LastName}");
            }

            // Simulating your method's logic to get what should have been filtered
            List<StudentDTO> filteredStudents = fakeStudents.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();

            // Ensuring the count matches
            Assert.Equal(filteredStudents.Count, list.TotalItemCount);
        }
    }
}
