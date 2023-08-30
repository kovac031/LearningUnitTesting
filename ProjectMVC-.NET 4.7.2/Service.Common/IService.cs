using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Common
{
    public interface IService
    {
        Task<List<StudentDTO>> GetAllAsync(string sortBy);
        Task<StudentDTO> GetOneByIdAsync(Guid id);
        Task<bool> CreateAsync(StudentDTO student);
        Task<bool> EditAsync(StudentDTO student, Guid id);
        Task<bool> DeleteAsync(Guid id);
    }
}
