using Model;
using Service.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace Project.WebAPI.Controllers
{
    public class DefaultController : ApiController
    {
        public IService Service { get; set; }
        public DefaultController(IService service)
        {
            Service = service;
        }
        // ---------------- GET ALL ----------------
        [HttpGet]
        [Route("getall")]
        public async Task<HttpResponseMessage> GetAllAsync()
        {
            try
            {
                List<StudentDTO> list = await Service.GetAllAsync();
                return Request.CreateResponse(HttpStatusCode.OK, list);
            }
            catch (Exception x)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, $"Error for GetAllAsync: {x.Message}");
            }
        }
        // ---------------- GET ONE BY ID ----------------
        [HttpGet]
        [Route("getone/{id}")]
        public async Task<HttpResponseMessage> GetOneByIdAsync(Guid id)
        {
            try
            {
                StudentDTO student = await Service.GetOneByIdAsync(id);
                return Request.CreateResponse(HttpStatusCode.OK, student);
            }
            catch (Exception x)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, $"Error for GetOneByIdAsync: {x.Message}");
            }
        }
        //--------------- CREATE NEW ---------------------
        [HttpPost]
        [Route("create")]
        public async Task<HttpResponseMessage> CreateAsync(StudentDTO student)
        {
            try
            {
                bool result = await Service.CreateAsync(student);

                if (result)
                {
                    return Request.CreateResponse(HttpStatusCode.Created, "Created!");
                }
                else
                {
                    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Failed to create");
                }
            }
            catch (Exception x)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, $"Error for CreateAsync: {x.Message}");
            }
        }
        //--------------- EDIT ---------------------
        [HttpPut]
        [Route("edit/{id}")]
        public async Task<HttpResponseMessage> EditAsync(StudentDTO student, Guid id)
        {
            try
            {
                bool result = await Service.EditAsync(student, id);

                if (result)
                {
                    return Request.CreateResponse(HttpStatusCode.Created, "Edited!");
                }
                else
                {
                    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Failed to edit");
                }
            }
            catch (Exception x)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, $"Error for EditAsync: {x.Message}");
            }
        }
        //-------------- DELETE ---------------
        [HttpDelete]
        [Route("delete/{id}")]
        public async Task<HttpResponseMessage> DeleteAsync(Guid id)
        {
            try
            {
                bool result = await Service.DeleteAsync(id);

                if (result)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, "Deleted!");
                }
                else
                {
                    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Failed to delete");
                }
            }
            catch (Exception x)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, $"Error for DeleteAsync: {x.Message}");
            }
        }
        // ---------------- GET ALL WITH SORTING, PAGING, FILTERING ----------------
        [HttpGet]
        [Route("params")]
        public async Task<HttpResponseMessage> ParamsAsync(
            [FromUri]string sortBy = null, 
            [FromUri] string firstName = null, [FromUri] string lastName = null,
            [FromUri] string dobBefore = null, [FromUri] string dobAfter = null,
            [FromUri] string regBefore = null, [FromUri] string regAfter = null,
            [FromUri] string pageNumber = null, [FromUri] string studentsPerPage = null) // bez null nedaje listu ako ne saljem parametar
        {
            try
            {
                List<StudentDTO> list = await Service.ParamsAsync(
                    sortBy, 
                    firstName, lastName, 
                    dobBefore, dobAfter,
                    regBefore, regAfter,
                    pageNumber, studentsPerPage);

                return Request.CreateResponse(HttpStatusCode.OK, list);
            }
            catch (Exception x)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, $"Error for ParamsAsync: {x.Message}");
            }
        }
    }
}
