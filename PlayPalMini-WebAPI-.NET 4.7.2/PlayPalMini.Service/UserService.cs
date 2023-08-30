using PlayPalMini.Common;
using PlayPalMini.Model;
using PlayPalMini.Repository;
using PlayPalMini.Repository.Common;
using PlayPalMini.Service.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlayPalMini.Service
{
    public class UserService : IUserService
    {
        // dependency injection
        public IUserRepository Repository { get; set; }
        public UserService(IUserRepository repository)
        {
            Repository = repository;
        }
        public async Task<(List<RegisteredUser>, string)> GetAllAsync()
        {
            (List<RegisteredUser> list, string message) = await Repository.GetAllAsync();
            if (list == null)
            {
                throw new Exception(message);
            }
            return (list, message);
        }
        //----------------
        public async Task<(RegisteredUser, string)> GetOneByIdAsync(Guid id)
        {
            (RegisteredUser user, string message) = await Repository.GetOneByIdAsync(id);
            if (user == null)
            {
                throw new Exception(message);
            }
            return (user, message);
        }
        //----------------
        public async Task<(bool, string)> CreateUserAsync(RegisteredUser user)
        {
            (bool result, string message) = await Repository.CreateUserAsync(user);
            if (!result) 
            {
                throw new Exception(message);
            }
            return (result, message);
        }
        //-----------------
        public async Task<(RegisteredUser, string)> EditUserAsync(RegisteredUser user, Guid id)
        {
            (RegisteredUser userService, string message) = await Repository.EditUserAsync(user, id);
            if (userService == null)
            {
                throw new Exception(message);
            }
            return (userService, message);
        }
        //-----------------
        public async Task<(bool, string)> DeleteUserAsync(Guid id)
        {
            (bool result, string message) = await Repository.DeleteUserAsync(id);
            if (!result)
            {
                throw new Exception(message);
            }
            return (result, message);
        }
        //------------------
        public async Task<(List<RegisteredUser>, string)> GetAllWithParamsAsync(SearchParam search, SortParam sort, PageParam page)
        {
            (List<RegisteredUser> list, string message) = await Repository.GetAllWithParamsAsync(search, sort, page);
            if (list == null)
            {
                throw new Exception(message);
            }
            return (list, message);
        }
        //----------------
        public async Task<(RegisteredUser, string)> FindUserAsync(string username, string password)
        {
            (RegisteredUser user, string message) = await Repository.FindUserAsync(username, password);
            if (user == null)
            {
                throw new Exception(message);
            }
            return (user, message);
        }
    }
}
