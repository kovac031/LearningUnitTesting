using PlayPalMini.Common;
using PlayPalMini.Model;
using PlayPalMini.Service;
using PlayPalMini.Service.Common;
using PlayPalMini.WebAPI.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.UI;

namespace PlayPalMini.WebAPI.Controllers
{
    public class UserController : ApiController
    {
        // dependency injection
        public IUserService Service { get; set; }
        public UserController(IUserService service)
        {
            Service = service;
        }

        //---------------------------GET ALL-----------------------------
        [JwtAuthentication]
        [AuthorizeRole("Administrator")]
        [HttpGet]
        [Route("user/getall/")]
        public async Task<HttpResponseMessage> GetAllAsync()
        {
            try
            {
                (List<RegisteredUser> list, string message) = await Service.GetAllAsync();
                return Request.CreateResponse(HttpStatusCode.OK, new { Message = message, List = list });
            }
            catch (Exception x)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, $"Error for GetAllAsync: {x.Message}");
            }
        }
        //---------------------------GET ONE BY ID-----------------------------
        [JwtAuthentication]
        [AuthorizeRole("Administrator")]
        [HttpGet]
        [Route("user/getonebyid/{id}")]
        public async Task<HttpResponseMessage> GetOneByIdAsync(Guid id)
        {
            try
            {
                (RegisteredUser user, string message) = await Service.GetOneByIdAsync(id);
                return Request.CreateResponse(HttpStatusCode.OK, new { Message = message, User = user });
            }
            catch (Exception x)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, $"Error for GetOneByIdAsync: {x.Message}");
            }
        }
        //---------------------------CREATE NEW-----------------------------
        // bez autentikacije jer kao registracija treba biti omogućena kao, ta logika        
        [HttpPost]
        [Route("user/create")]
        public async Task<HttpResponseMessage> CreateUserAsync(RegisteredUser user)
        {
            try
            {
                (bool result, string message) = await Service.CreateUserAsync(user);

                if (result)
                {
                    return Request.CreateResponse(HttpStatusCode.Created, message);
                }
                else
                {
                    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, message);
                }
            }
            catch (Exception x)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, $"Error for CreateUserAsync: {x.Message}");
            }
        }
        //---------------------------EDIT USER-----------------------------
        [JwtAuthentication]
        [AuthorizeRole("Administrator", "User")]
        [HttpPut]
        [Route("user/edit/{id}")]
        public async Task<HttpResponseMessage> EditUserAsync(RegisteredUser user, Guid id) // parametri za upit na Service
        {
            try
            {
                (RegisteredUser editedUser, string message) = await Service.EditUserAsync(user, id);
                return Request.CreateResponse(HttpStatusCode.OK, new { Message = message, User = editedUser });

            }
            catch (Exception x)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, $"Error for EditUserAsync: {x.Message}");
            }
        }
        //---------------------------DELETE USER-----------------------------
        [JwtAuthentication]
        [AuthorizeRole("Administrator")]
        [HttpDelete]
        [Route("user/delete/{id}")]
        public async Task<HttpResponseMessage> DeleteUserAsync(Guid id)
        {
            try
            {
                (bool result, string message) = await Service.DeleteUserAsync(id);

                if (result)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, message);
                }
                else
                {
                    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, message);
                }
            }
            catch (Exception x)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, $"Error for DeleteUserAsync: {x.Message}");
            }
        }
        //--------------------GET ALL WITH FILTERING, PAGING, SORTING-----------------------
        [JwtAuthentication]
        [AuthorizeRole("Administrator")]
        [HttpGet]
        [Route("user/params/")]
        public async Task<HttpResponseMessage> GetAllWithParamsAsync([FromUri]SearchParam search, [FromUri]SortParam sort, [FromUri]PageParam page) // ovo [FromUri] je bitno, tako zna da ce traziti parametre u URL
        {
            try
            {
                (List<RegisteredUser> list, string message) = await Service.GetAllWithParamsAsync(search, sort, page);
                return Request.CreateResponse(HttpStatusCode.OK, new { Message = message, List = list });
            }
            catch (Exception x)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, $"Error for GetAllWithParamsAsync: {x.Message}");
            }
        }
    }
}
