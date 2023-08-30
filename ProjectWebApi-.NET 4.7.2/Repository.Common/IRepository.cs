using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Common
{
    public interface IRepository
    {
        Task<List<StudentDTO>> GetAllAsync();
        Task<StudentDTO> GetOneByIdAsync(Guid id);
        Task<bool> CreateAsync(StudentDTO student);
        Task<bool> EditAsync(StudentDTO student, Guid id);
        Task<bool> DeleteAsync(Guid id);
        Task<List<StudentDTO>> ParamsAsync(
            string sortBy, 
            string firstName, string lastName,
            string dobBefore, string dobAfter,
            string regBefore, string regAfter,
            string pageNumber, string studentsPerPage);
    }
}
