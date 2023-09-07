using DataAccessLayer;
using Model;
using Moq;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebAPI.Tests
{
    public static class HelperClass
    {
        public static List<StudentDTO> GetFakeStudents()
        {
            return new List<StudentDTO>
            {
            new StudentDTO {    Id = new Guid("3f2504e0-4f89-11d3-9a0c-0305e82c3301"),
                                FirstName = "René",
                                LastName = "D'Artagnan",
                                DateOfBirth = new DateTime(1991, 1, 2),
                                EmailAddress = "renerene@reneemail.com",
                                RegisteredOn = new DateTime(2021, 1, 2) },

            new StudentDTO {    Id = new Guid("6dcd4ce0-4f89-11d3-9a0c-0305e82c8811"),
                                FirstName = "Zoë",
                                LastName = "Müller",
                                DateOfBirth = new DateTime(1995, 11, 22),
                                EmailAddress = "zoezoe@zoeemail.com",
                                RegisteredOn = new DateTime(2022, 11, 22) },

            new StudentDTO {    Id = new Guid("8b3e8170-4f89-11d3-9a0c-0305e82c9902"),
                                FirstName = "José",
                                LastName = "Castañeda",
                                DateOfBirth = new DateTime(2000, 3, 9),
                                EmailAddress = "josejose@joseemail.com",
                                RegisteredOn = new DateTime(2023, 8, 31) }
            };
        }

        //---------------------------------------

        public static Mock<DbSet<T>> CreateMockDbSet<T>(List<T> sourceList) where T : class
        {
            var mockSet = new Mock<DbSet<T>>();
            mockSet.As<IQueryable<T>>().Setup(m => m.Provider).Returns(sourceList.AsQueryable().Provider);
            mockSet.As<IQueryable<T>>().Setup(m => m.Expression).Returns(sourceList.AsQueryable().Expression);
            mockSet.As<IQueryable<T>>().Setup(m => m.ElementType).Returns(sourceList.AsQueryable().ElementType);
            mockSet.As<IQueryable<T>>().Setup(m => m.GetEnumerator()).Returns(sourceList.AsQueryable().GetEnumerator());
            return mockSet;
        }

        public static List<Student> GetFakeDBStudents()
        {
            return new List<Student>
            {
            new Student {   Id = new Guid("3f2504e0-4f89-11d3-9a0c-0305e82c3301"),
                            FirstName = "René",
                            LastName = "D'Artagnan",
                            DateOfBirth = new DateTime(1991, 1, 2),
                            EmailAddress = "renerene@reneemail.com",
                            RegisteredOn = new DateTime(2021, 1, 2) },

            new Student {   Id = new Guid("6dcd4ce0-4f89-11d3-9a0c-0305e82c8811"),
                            FirstName = "Zoë",
                            LastName = "Müller",
                            DateOfBirth = new DateTime(1995, 11, 22),
                            EmailAddress = "zoezoe@zoeemail.com",
                            RegisteredOn = new DateTime(2022, 11, 22) },

            new Student {   Id = new Guid("8b3e8170-4f89-11d3-9a0c-0305e82c9902"),
                            FirstName = "José",
                            LastName = "Castañeda",
                            DateOfBirth = new DateTime(2000, 3, 9),
                            EmailAddress = "josejose@joseemail.com",
                            RegisteredOn = new DateTime(2023, 8, 31) }
            };
        }
    }
}
