using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Model;
using Service.Common;
using X.PagedList;

namespace MVC.Controllers
{
    public class StudentController : Controller
    {
        public readonly IMapper _mapper;
        public IService Service { get; set; }
        public StudentController(IService service, IMapper mapper)
        {
            Service = service;
            _mapper = mapper;
        }
        // ---------------- GET ALL ----------------        
        public async Task<ActionResult> ListStudents()
        {
            List<StudentDTO> listDTO = await Service.GetAllAsync();

            List<StudentView> listView = _mapper.Map<List<StudentView>>(listDTO);
            
            return View(listView);
        }
        // ---------------- GET ONE BY ID ----------------
        public async Task<ActionResult> GetOneStudent(Guid id)
        {
            StudentDTO studentDTO = await Service.GetOneByIdAsync(id);
            StudentView studentView = _mapper.Map<StudentView>(studentDTO);
            return View(studentView);
        }
        // ---------------- CREATE NEW ----------------
        [HttpGet]
        public async Task<ActionResult> CreateStudent()
        {
            return View();
        }
        [HttpPost]
        public async Task<ActionResult> CreateStudent(StudentView studentView)
        {
            try
            {
                StudentDTO studentDTO = _mapper.Map<StudentDTO>(studentView);

                bool created = await Service.CreateAsync(studentDTO);
                if (!created)
                {
                    return View("Failed to create");
                }
                return RedirectToAction("ListStudents");
            }
            catch (Exception)
            {
                return View("Exception");
            }
        }
        // ---------------- EDIT ----------------
        [HttpGet]
        public async Task<ActionResult> EditStudent(Guid id)
        {
            StudentDTO studentDTO = await Service.GetOneByIdAsync(id);
            StudentView studentView = _mapper.Map<StudentView>(studentDTO);
            return View(studentView);
        }
        [HttpPost]
        public async Task<ActionResult> EditStudent(StudentView studentView)
        {
            try
            {
                StudentDTO studentDTO = _mapper.Map<StudentDTO>(studentView);

                bool created = await Service.EditAsync(studentDTO, studentDTO.Id);
                if (!created)
                {
                    return View("Failed to edit");
                }
                return RedirectToAction("ListStudents");
            }
            catch (Exception)
            {
                return View("Exception");
            }
        }
        // ---------------- DELETE ----------------
        public async Task<ActionResult> DeleteStudent(Guid id)
        {
            await Service.DeleteAsync(id);
            return RedirectToAction("ListStudents");
        }
        // --------------- LIST WITH PARAMETERS -------------
        public async Task<ActionResult> ListWithParams(
            string sortBy,
            string searchBy,
            DateTime? dobMin, DateTime? dobMax,
            DateTime? regMin, DateTime? regMax,
            int? page)
        {
            List<StudentDTO> listDTO = await Service.ListWithParams(sortBy); // dohvaca sve

            // FILTERING

            List<StudentDTO> filteredList = listDTO; //prije filtriranja lista ima sve

            if (!string.IsNullOrEmpty(searchBy)) // trazi po imenu i prezimenu
            {
                filteredList = listDTO.Where(x =>
                    x.FirstName.Contains(searchBy) ||
                    x.LastName.Contains(searchBy))
                    .ToList();
            }
            ViewBag.SearchFilter = searchBy;

            if (dobMin != null && dobMax != null) // filtriranje po DateOfBirth
            {
                filteredList = filteredList.Where(x => x.DateOfBirth >= dobMin && x.DateOfBirth <= dobMax).ToList();
            }
            else if (dobMin != null)
            {
                filteredList = filteredList.Where(x => x.DateOfBirth >= dobMin).ToList();
            }
            else if (dobMax != null)
            {
                filteredList = filteredList.Where(x => x.DateOfBirth <= dobMax).ToList();
            }
            ViewBag.DobMin = dobMin;
            ViewBag.DobMax = dobMax;
            //
            if (regMin != null && regMax != null) // filtriranje po RegisteredOn
            {
                filteredList = filteredList.Where(x => x.RegisteredOn >= regMin && x.RegisteredOn <= regMax).ToList();
            }
            else if (regMin != null)
            {
                filteredList = filteredList.Where(x => x.RegisteredOn >= regMin).ToList();
            }
            else if (regMax != null)
            {
                filteredList = filteredList.Where(x => x.RegisteredOn <= regMax).ToList();
            }
            ViewBag.RegMin = regMin;
            ViewBag.RegMax = regMax;

            // SORTING
            ViewBag.SortBySignUp = string.IsNullOrEmpty(sortBy) ? "signup_asc" : "";
            ViewBag.SortByDob = sortBy == "dob_asc" ? "dob_desc" : "dob_asc";
            ViewBag.SortByFirstName = sortBy == "name_asc" ? "name_desc" : "name_asc";
            ViewBag.SortByLastName = sortBy == "surname_asc" ? "surname_desc" : "surname_asc";
            ViewBag.CurrentSort = sortBy;

            // PAGING part 1
            int count = filteredList.Count; // mislim da PagedList usporedjuje count sa pageSize i zato nam treba

            int pageNumber = page ?? 1;
            int pageSize = 5;

            List<StudentDTO> filteredDTO = filteredList.Skip((pageNumber - 1) * pageSize)
                                                        .Take(pageSize)
                                                        .ToList();

            // mapiranje
            List<StudentView> listView = _mapper.Map<List<StudentView>>(filteredDTO); // zamjenio listDTO sa filteredList
            //

            // PAGING part 2
            StaticPagedList<StudentView> pagedList = new StaticPagedList<StudentView>(listView, pageNumber, pageSize, count);
            ViewBag.CurrentPage = page;

            return View(pagedList);
        }
    }
}
