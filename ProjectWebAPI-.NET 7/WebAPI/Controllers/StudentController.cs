using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Model;
using Service.Common;
using System.Net;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentController : ControllerBase
    {
        public IService Service { get; set; }
        public StudentController(IService service)
        {
            Service = service;
        }
        // ---------------- GET ALL ----------------
        [HttpGet]
        [Route("getall")]
        public async Task<IActionResult> GetAllAsync()
        {
            try
            {
                List<StudentDTO> list = await Service.GetAllAsync();
                return Ok(list); 
            }
            catch (Exception x)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error for GetAllAsync: {x.Message}");
            }
        }
        // ---------------- GET ONE BY ID ----------------
        [HttpGet]
        [Route("getone/{id}", Name = "GetOneByIdAsync")]
        public async Task<IActionResult> GetOneByIdAsync(Guid id)
        {
            try
            {
                StudentDTO student = await Service.GetOneByIdAsync(id);
                return Ok(student);
            }
            catch (Exception x)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error for GetOneByIdAsync: {x.Message}");
            }
        }
        //--------------- CREATE NEW ---------------------
        [HttpPost]
        [Route("create")]
        public async Task<IActionResult> CreateAsync(StudentDTO student)
        {
            try
            {
                bool result = await Service.CreateAsync(student);

                if (result)
                {
                    var response = new
                    {
                        message = "Student created. Ignore the NULL values, it's fine!",
                        data = student
                    };
                    return CreatedAtRoute(nameof(GetOneByIdAsync), new { id = student.Id }, response);
                }
                else
                {
                    return BadRequest("Failed to create");
                }
            }
            catch (Exception x)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, $"Error for CreateAsync: {x.Message}"); // int je error broj - 500, 200, 404 i sl
            }
        }
        //--------------- EDIT ---------------------
        [HttpPut]
        [Route("edit/{id}")]
        public async Task<IActionResult> EditAsync(StudentDTO student, Guid id)
        {
            try
            {
                bool result = await Service.EditAsync(student, id);

                if (result)
                {
                    return Ok("Edited!");
                }
                else
                {
                    return BadRequest("Failed to edit");
                }
            }
            catch (Exception x)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, $"Error for EditAsync: {x.Message}");
            }
        }
        //-------------- DELETE ---------------
        [HttpDelete]
        [Route("delete/{id}")]
        public async Task<IActionResult> DeleteAsync(Guid id)
        {
            try
            {
                bool result = await Service.DeleteAsync(id);

                if (result)
                {
                    return Ok("Deleted!");
                }
                else
                {
                    return BadRequest("Failed to delete");
                }
            }
            catch (Exception x)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, $"Error for DeleteAsync: {x.Message}");
            }
        }
        // ---------------- GET ALL WITH SORTING, PAGING, FILTERING ----------------
        [HttpGet]
        [Route("params")]
        public async Task<IActionResult> ParamsAsync(
            [FromQuery] string sortBy = null,
            [FromQuery] string firstName = null, [FromQuery] string lastName = null,
            [FromQuery] string dobBefore = null, [FromQuery] string dobAfter = null,
            [FromQuery] string regBefore = null, [FromQuery] string regAfter = null,
            [FromQuery] string pageNumber = null, [FromQuery] string studentsPerPage = null) // bez null nedaje listu ako ne saljem parametar
        {
            try
            {
                List<StudentDTO> list = await Service.ParamsAsync(
                    sortBy,
                    firstName, lastName,
                    dobBefore, dobAfter,
                    regBefore, regAfter,
                    pageNumber, studentsPerPage);

                return Ok(list);
            }
            catch (Exception x)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, $"Error for ParamsAsync: {x.Message}");
            }
        }
    }
}
