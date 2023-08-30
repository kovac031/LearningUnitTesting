using Model.Common;

namespace Model
{
    public class StudentDTO : IStudent
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string EmailAddress { get; set; }
        public DateTime RegisteredOn { get; set; }
    }
}