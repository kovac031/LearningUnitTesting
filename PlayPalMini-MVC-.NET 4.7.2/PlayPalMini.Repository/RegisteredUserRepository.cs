using PlayPalMini.DAL;
using PlayPalMini.Model;
using PlayPalMini.Repository.Common;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlayPalMini.Repository
{
    public class RegisteredUserRepository : IRegisteredUserRepository
    {
        public EFContext Context { get; set; }
        public RegisteredUserRepository(EFContext context)
        {
            Context = context;
        }

        //------------CREATE NEW USER / SIGN UP--------------------
        public async Task<bool> SignUpAsync(RegisteredUserDTO userDTO)
        {
            RegisteredUser user = new RegisteredUser
            {
                Id = userDTO.Id,
                Username = userDTO.Username,
                Pass = userDTO.Pass,
                UserRole = userDTO.UserRole,
                CreatedBy = userDTO.CreatedBy,
                UpdatedBy = userDTO.UpdatedBy,
                DateCreated = userDTO.DateCreated,
                DateUpdated = userDTO.DateUpdated
            };
            Context.RegisteredUsers.Add(user);

            await Context.SaveChangesAsync();

            return true;
        }

        //--------------FIND USER FOR LOGIN-------------------
        public async Task<RegisteredUserDTO> FindOneUserAsync(string username, string password)
        {
            RegisteredUserDTO userDTO = await Context.RegisteredUsers
                .Where(x => x.Username.Equals(username, StringComparison.OrdinalIgnoreCase) 
                        && x.Pass.Equals(password, StringComparison.OrdinalIgnoreCase))
                .Select(x => new RegisteredUserDTO()
            {
                Id = x.Id,
                Username = x.Username,
                Pass = x.Pass,
                UserRole = x.UserRole
            }).FirstOrDefaultAsync<RegisteredUserDTO>();

            return userDTO;
        }

        //--------------------EDIT USER---------------------
        public async Task<bool> EditUserAsync(Guid id, RegisteredUserDTO userDTO)
        {
            RegisteredUser user = await Context.RegisteredUsers.FirstOrDefaultAsync(x => x.Id == id);

            user.Id = userDTO.Id;
            user.Username = userDTO.Username;
            user.Pass = userDTO.Pass;
            user.UserRole = userDTO.UserRole;
            user.UpdatedBy = userDTO.UpdatedBy;
            user.DateUpdated = userDTO.DateUpdated;

            await Context.SaveChangesAsync();
            return true;
        }

        //--------------GET ONE BY ID-------------------
        public async Task<RegisteredUserDTO> GetUserByIdAsync(Guid id)
        {
            RegisteredUserDTO user = await Context.RegisteredUsers.Where(x => x.Id == id).Select(x => new RegisteredUserDTO()
            {
                Id = x.Id,
                Username = x.Username,
                Pass = x.Pass,
                UserRole = x.UserRole,
                UpdatedBy = x.UpdatedBy,
                DateUpdated = x.DateUpdated
            }).FirstOrDefaultAsync<RegisteredUserDTO>();

            return user;
        }

        //-------------------GET ALL----------------------
        public async Task<List<RegisteredUserDTO>> GetAllUsersAsync()
        {
            IQueryable<RegisteredUser> users = Context.RegisteredUsers;

            List<RegisteredUserDTO> list = await users.Select(x => new RegisteredUserDTO()
            {
                Id = x.Id,
                Username = x.Username,
                UserRole = x.UserRole,
                CreatedBy = x.CreatedBy,
                DateCreated = x.DateCreated,
                UpdatedBy = x.UpdatedBy,                
                DateUpdated = x.DateUpdated
            }).ToListAsync<RegisteredUserDTO>();

            return list;
        }
    }
}
