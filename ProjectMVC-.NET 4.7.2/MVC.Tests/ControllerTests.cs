using AutoMapper;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Project.Controllers;
using Service.Common;
using System;
using Common;
using System.Collections.Generic;
using System.Threading.Tasks;
using Model;
using System.Web.Mvc;
using PagedList;
using System.Diagnostics;
using System.Linq;
using System.Data;

namespace MVC.Tests
{
    [TestClass]
    public class ControllerTests
    {
        private DefaultController _controller;
        private Mock<IService> _service;
        private IMapper _mapper;

        [TestInitialize] 
        public void Setup()
        {
            _service = new Mock<IService>();

            MapperConfiguration config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new MappingProfile());
            });
            _mapper = config.CreateMapper();

            _controller = new DefaultController(_service.Object, _mapper); // redoslijed bitan, ovo mora biti ispod mapper konfiguracije
        }
        //-------------------------------------------------------------------
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

        // ------------ GET ALL WITH PARAMS ----------------

        [TestMethod]
        public async Task GetAllAsync_ParamsNull_ReturnsDefaultView()
        {
            // Arrange
            List<StudentDTO> fakeList = GetFakeStudents();

            _service.Setup(x => x.GetAllAsync(null)).ReturnsAsync(fakeList);

            // Act
            ActionResult result = await _controller.GetAllAsync(null, null, null, null, null, null, null);

            // Assert
            Assert.IsInstanceOfType(result, typeof(ViewResult));
            ViewResult viewResult = result as ViewResult;

            Assert.IsInstanceOfType(viewResult.Model, typeof(StaticPagedList<StudentView>));
            StaticPagedList<StudentView> model = viewResult.Model as StaticPagedList<StudentView>;
        }

        [TestMethod]
        public async Task GetAllAsync_SomeParams_ReturnsFilteredSortedListView()
        {
            // Arrange
            string sortBy = "dob_asc";
            DateTime? dobMin = new DateTime(1995, 1, 1); // born after
            int? page = 1;

            List<StudentDTO> fakeList = GetFakeStudents();

            _service.Setup(x => x.GetAllAsync(sortBy)).ReturnsAsync(fakeList); // repository hendla sortBy, salje servisu, a servis kontroleru, zato ga stavljam tu a ostale parammetre ne

            // Act   
            ActionResult result = await _controller.GetAllAsync(sortBy, null, dobMin, null, null, null, page); // ali metoda u kontroleru nema samo sortBy, nego i ostale

            List<StudentDTO> myList = fakeList.Where(x => x.DateOfBirth >= dobMin)
                                      .OrderBy(x => x.DateOfBirth)
                                      .ToList();

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(ViewResult));
            ViewResult viewResult = result as ViewResult;
            Assert.IsNotNull(viewResult.Model);
            Assert.IsInstanceOfType(viewResult.Model, typeof(StaticPagedList<StudentView>));
            StaticPagedList<StudentView> pagedList = viewResult.Model as StaticPagedList<StudentView>;

            Assert.AreEqual(myList.Count, pagedList.TotalItemCount);
            Assert.AreEqual(myList[0].DateOfBirth, pagedList[0].DateOfBirth);
            Console.WriteLine($"myList Dob {myList[0].DateOfBirth}, model Dob {pagedList[0].DateOfBirth}");

            for (int i = 0; i < myList.Count; i++)
            {
                Assert.AreEqual(myList[i].Id, pagedList[i].Id);
                Assert.AreEqual(myList[i].DateOfBirth, pagedList[i].DateOfBirth);
                Console.WriteLine($"{i+1}. in myList: {myList[i].FirstName} {myList[i].DateOfBirth}, in pagedList: {pagedList[i].FirstName} {pagedList[i].DateOfBirth}");
            }
        }

        [TestMethod]
        [ExpectedException(typeof(EntityException))]
        public async Task GetAllAsync_SomeParams_ThrowsException()
        {
            // Arrange
            string sortBy = "dob_asc";
            DateTime? dobMin = new DateTime(1995, 1, 1);
            int? page = 1;

            _service.Setup(x => x.GetAllAsync(sortBy)).ThrowsAsync(new EntityException("MY TEST EXCEPTION"));

            // Act
            ActionResult result = await _controller.GetAllAsync(sortBy, null, dobMin, null, null, null, page);

            // Assert
        }
    }
}
