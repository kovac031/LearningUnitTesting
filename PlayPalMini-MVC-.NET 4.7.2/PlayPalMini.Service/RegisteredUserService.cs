using PlayPalMini.Model;
using PlayPalMini.Repository.Common;
using PlayPalMini.Service.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlayPalMini.Service
{
    public class RegisteredUserService : IRegisteredUserService
    {
        public IRegisteredUserRepository RegisteredUserRepository { get; set; }
        public RegisteredUserService(IRegisteredUserRepository repository)
        {
            RegisteredUserRepository = repository;
        }
        public async Task<bool> SignUpAsync(RegisteredUserDTO userDTO)
        {
            bool result = await RegisteredUserRepository.SignUpAsync(userDTO);
            return result;
        }
        public async Task<RegisteredUserDTO> FindOneUserAsync(string username, string password)
        {
            RegisteredUserDTO user = await RegisteredUserRepository.FindOneUserAsync(username, password);
            return user;
        }
        public async Task<bool> EditUserAsync(Guid id, RegisteredUserDTO userDTO)
        {
            bool result = await RegisteredUserRepository.EditUserAsync(id, userDTO);
            return result;
        }
        public async Task<RegisteredUserDTO> GetUserByIdAsync(Guid id)
        {
            RegisteredUserDTO user = await RegisteredUserRepository.GetUserByIdAsync(id);
            return user;
        }
        public async Task<List<RegisteredUserDTO>> GetAllUsersAsync()
        {
            List<RegisteredUserDTO> list = await RegisteredUserRepository.GetAllUsersAsync();
            return list;
        }
    }
}
