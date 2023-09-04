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
        public ListWithParamsTests()
        {
            _service = A.Fake<IService>(); //FakeItEasy
            _mapper = A.Fake<IMapper>();
            _controller = new StudentController(_service, _mapper); // SUT, system under test
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
            //Assert.NotEmpty(sortedList);
            //Assert.NotEmpty(fakeStudents);
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
            List<StudentDTO> filteredStudents = fakeStudents.Where(s => s.FirstName.Contains("re") || s.LastName.Contains("re")).ToList(); // dakle ovdje dok je bila provjera veliko malo slovo nije prolazilo, cim sam mako prolazi test

            Assert.Equal(filteredStudents.Count, list.TotalItemCount);
        }
    }
}
