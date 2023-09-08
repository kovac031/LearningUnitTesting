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

        // ------------ GET ONE BY ID ----------------

        [TestMethod]
        public async Task GetOneById_OnSuccess_ReturnStudent()
        {
            // Arrange
            Guid id = new Guid("6dcd4ce0-4f89-11d3-9a0c-0305e82c8811");
            StudentDTO fakeStudentDTO = GetFakeStudents().FirstOrDefault(x => x.Id == id);

            _service.Setup(x => x.GetOneByIdAsync(id)).ReturnsAsync(fakeStudentDTO);

            // Act
            ActionResult result = await _controller.GetOneByIdAsync(id);

            // Assert
            Assert.IsNotNull(result); // Ensuring that we actually get a result
            Assert.IsInstanceOfType(result, typeof(ViewResult)); // Ensuring that the result is a ViewResult
            ViewResult viewResult = result as ViewResult; // Cast to ViewResult so we can inspect the Model

            Assert.IsNotNull(viewResult.Model); // Ensuring that the model is not null
            Assert.IsInstanceOfType(viewResult.Model, typeof(StudentView)); // Ensuring that the model is of type StudentView
            StudentView foundStudentView = viewResult.Model as StudentView; // Cast to StudentView to check the data
                        
            Assert.AreEqual(fakeStudentDTO.Id, foundStudentView.Id);
            Assert.AreEqual(fakeStudentDTO.FirstName, foundStudentView.FirstName);
            Assert.AreEqual(fakeStudentDTO.DateOfBirth, foundStudentView.DateOfBirth);
        }

        [TestMethod]
        [ExpectedException(typeof(EntityException))]
        public async Task GetOneById_OnFail_ThrowException()
        {
            // Arrange            
            Guid id = new Guid("6dcd4ce0-4f89-11d3-9a0c-0305e82c8811");
            StudentDTO fakeStudentDTO = GetFakeStudents().FirstOrDefault(x => x.Id == id);

            _service.Setup(x => x.GetOneByIdAsync(id)).ThrowsAsync(new EntityException("MY TEST EXCEPTION"));

            // Act
            ActionResult result = await _controller.GetOneByIdAsync(id);

            // Assert
        }

        // ------------ CREATE NEW ----------------

        [TestMethod]
        public async Task CreateAsync_HttpGet_OnSuccess_ReturnsView()
        {
            // Act
            ActionResult result = await _controller.CreateAsync();

            // Assert
            Assert.IsInstanceOfType(result, typeof(ViewResult));
        }

        [TestMethod]
        public async Task CreateAsync_HttpPost_OnSuccess_ReturnsRedirect()
        {
            // Arrange
            StudentView fakeStudentView = new StudentView // kako u kontroleru ne provjeravam mapiranje, ovdje prolazi bilo kakvi parametri, nema veze sto je id null
            {
                FirstName = "John",
                LastName = "Doe",
                DateOfBirth = new DateTime(1991, 1, 1),
                EmailAddress = "johndoe@jemail.com"
            };
            _service.Setup(x => x.CreateAsync(It.IsAny<StudentDTO>())).ReturnsAsync(true);

            // Act
            ActionResult result = await _controller.CreateAsync(fakeStudentView);

            // Assert
            Assert.IsInstanceOfType(result, typeof(RedirectToRouteResult));
        }

        [TestMethod]
        public async Task CreateAsync_HttpPost_OnFail_ReturnsFailedMessageView()
        {
            // Arrange
            StudentView fakeStudentView = new StudentView 
            {
                FirstName = "John",
                LastName = "Doe",
                DateOfBirth = new DateTime(1991, 1, 1),
                EmailAddress = "johndoe@jemail.com"
            };
            _service.Setup(x => x.CreateAsync(It.IsAny<StudentDTO>())).ReturnsAsync(false);

            // Act
            ActionResult result = await _controller.CreateAsync(fakeStudentView);

            // Assert
            Assert.IsInstanceOfType(result, typeof(ViewResult));
            ViewResult viewResult = result as ViewResult;
            Assert.AreEqual("Failed to create", viewResult.ViewName);
        }

        [TestMethod]
        public async Task CreateAsync_HttpPost_OnException_ReturnsExceptionMessageView()
        {
            // Arrange
            StudentView fakeStudentView = new StudentView
            {
                FirstName = "John",
                LastName = "Doe",
                DateOfBirth = new DateTime(1991, 1, 1),
                EmailAddress = "johndoe@jemail.com"
            };
            _service.Setup(x => x.CreateAsync(It.IsAny<StudentDTO>())).ThrowsAsync(new Exception());

            // Act
            ActionResult result = await _controller.CreateAsync(fakeStudentView);

            // Assert
            Assert.IsInstanceOfType(result, typeof(ViewResult));
            ViewResult viewResult = result as ViewResult;
            Assert.AreEqual("Exception", viewResult.ViewName);
        }

        // ------------ EDIT ----------------

        [TestMethod]
        public async Task EditAsync_HttpGet_ReturnsViewWithModel()
        {
            // Arrange
            Guid id = new Guid("6dcd4ce0-4f89-11d3-9a0c-0305e82c8811");
            StudentDTO fakeStudentDTO = GetFakeStudents().FirstOrDefault(x => x.Id == id);
            _service.Setup(x => x.GetOneByIdAsync(id)).ReturnsAsync(fakeStudentDTO);

            // Act
            ActionResult result = await _controller.EditAsync(id);

            // Assert
            Assert.IsInstanceOfType(result, typeof(ViewResult));
            ViewResult viewResult = result as ViewResult;
            Assert.IsInstanceOfType(viewResult.Model, typeof(StudentView));

            StudentView foundStudentView = viewResult.Model as StudentView;
            Console.WriteLine($"fakeStudentDTO: {fakeStudentDTO.FirstName} {fakeStudentDTO.LastName},\nfoundStudentView: {foundStudentView.FirstName} {foundStudentView.LastName}");
        }

        [TestMethod]
        public async Task EditAsync_HttpPost_OnSuccess_ReturnsRedirect()
        {
            // Arrange
            Guid id = new Guid("6dcd4ce0-4f89-11d3-9a0c-0305e82c8811");
            StudentDTO fakeStudentDTO = GetFakeStudents().FirstOrDefault(x => x.Id == id);

            Console.WriteLine($"student prije edita: {fakeStudentDTO.FirstName} {fakeStudentDTO.EmailAddress}");

            StudentView fakeStudentView = new StudentView 
            {
                Id = id,
                FirstName = fakeStudentDTO.FirstName,
                LastName = fakeStudentDTO.LastName,
                DateOfBirth = fakeStudentDTO.DateOfBirth,
                EmailAddress = "newmail@email.com", // ovo je kao novo
                RegisteredOn = fakeStudentDTO.RegisteredOn
            };

            StudentDTO editedStudentDTO = null; // studentView je samo ono sta ja unosim, bazi na edit se salje studentDTO model, tako da ce ovo biti taj nakon kontrolera

            _service.Setup(x => x.EditAsync(It.IsAny<StudentDTO>(), It.IsAny<Guid>())).ReturnsAsync(true) // ovo je klasika set up
                   .Callback<StudentDTO, Guid>((student, studentId) => { editedStudentDTO = student; }); // ovo je bitno, da nakon "call" kontrolera "back"-a DTO kojeg je obradio kontroler, ili sam tako bar shvatio, tako da se tu vidi promjena na DTO
                                                                                                        // doslovno se mapirao sa studentDTO u kontroleru, a koji se opet mapirao automaperom sa studentView u kontroleru
            // Act
            ActionResult result = await _controller.EditAsync(fakeStudentView); // saljemo fakeStudentView, dakle ne EditedStudentDTO

            Console.WriteLine($"student nakon edita: {editedStudentDTO.FirstName} {editedStudentDTO.EmailAddress}"); // tek se tu pojavljuje EditedStudentDTO, koji je od negdje pokupio updejtane podatke

            // redirect to action nije view pa nema u sebi model za prikazati, imati na umu

            // Assert
            Assert.IsInstanceOfType(result, typeof(RedirectToRouteResult));
        }

        [TestMethod]
        public async Task EditAsync_HttpPost_OnFail_ReturnsFailedMessageView()
        {
            // Arrange
            Guid id = new Guid("6dcd4ce0-4f89-11d3-9a0c-0305e82c8811");
            StudentDTO fakeStudentDTO = GetFakeStudents().FirstOrDefault(x => x.Id == id);

            Console.WriteLine($"student before edit: {fakeStudentDTO.FirstName} {fakeStudentDTO.EmailAddress}");

            StudentView fakeStudentView = new StudentView
            {
                Id = id,
                FirstName = fakeStudentDTO.FirstName,
                LastName = fakeStudentDTO.LastName,
                DateOfBirth = fakeStudentDTO.DateOfBirth,
                EmailAddress = "newmail@email.com", // ovo je kao novo
                RegisteredOn = fakeStudentDTO.RegisteredOn
            };
            _service.Setup(x => x.EditAsync(It.Is<StudentDTO>(s => s.Id == fakeStudentDTO.Id), id)).ReturnsAsync(false);

            // Act
            ActionResult result = await _controller.EditAsync(fakeStudentView);

            // Assert
            Assert.IsInstanceOfType(result, typeof(ViewResult));
            ViewResult viewResult = result as ViewResult;
            Assert.AreEqual("Failed to edit", viewResult.ViewName);

            Console.WriteLine($"student after edit: {fakeStudentDTO.FirstName} {fakeStudentDTO.EmailAddress}"); // mislim da ovo ipak nema smisla, mislim da ispisuje izvorni DTO a ne DTO koji je izasao iz kontrolera neizmjenjen zbog fejla
        }

        [TestMethod]
        public async Task EditAsync_HttpPost_OnException_ReturnsExceptionMessageView()
        {
            // Arrange
            Guid id = new Guid("6dcd4ce0-4f89-11d3-9a0c-0305e82c8811");
            StudentDTO fakeStudentDTO = GetFakeStudents().FirstOrDefault(x => x.Id == id);

            StudentView fakeStudentView = new StudentView
            {
                Id = id,
                FirstName = fakeStudentDTO.FirstName,
                LastName = fakeStudentDTO.LastName,
                DateOfBirth = fakeStudentDTO.DateOfBirth,
                EmailAddress = "newmail@email.com", // ovo je kao novo
                RegisteredOn = fakeStudentDTO.RegisteredOn
            };
            _service.Setup(x => x.EditAsync(It.Is<StudentDTO>(s => s.Id == fakeStudentDTO.Id), id)).ThrowsAsync(new Exception());

            // Act
            ActionResult result = await _controller.EditAsync(fakeStudentView);

            // Assert
            Assert.IsInstanceOfType(result, typeof(ViewResult));
            ViewResult viewResult = result as ViewResult;
            Assert.AreEqual("Exception", viewResult.ViewName);
        }
    }
}
