using Model;
using Repository.Common;
using Service.Common;

namespace Service
{
    public class StudentService : IService
    {
        public IRepository Repository { get; set; }
        public StudentService(IRepository repository)
        {
            Repository = repository;
        }
        public async Task<List<StudentDTO>> GetAllAsync()
        {
            List<StudentDTO> list = await Repository.GetAllAsync();
            return list;
        }
        public async Task<StudentDTO> GetOneByIdAsync(Guid id)
        {
            StudentDTO student = await Repository.GetOneByIdAsync(id);
            return student;
        }
        public async Task<bool> CreateAsync(StudentDTO student)
        {
            bool result = await Repository.CreateAsync(student);
            return result;
        }
        public async Task<bool> EditAsync(StudentDTO student, Guid id)
        {
            bool result = await Repository.EditAsync(student, id);
            return result;
        }
        public async Task<bool> DeleteAsync(Guid id)
        {
            bool result = await Repository.DeleteAsync(id);
            return result;
        }
        public async Task<List<StudentDTO>> ListWithParams(string sortBy)
        {
            List<StudentDTO> list = await Repository.ListWithParams(sortBy);
            return list;
        }
    }
}