using System;
using System.Collections.Generic;

namespace MVC;

public partial class Student
{
    public Guid Id { get; set; }

    public string FirstName { get; set; }

    public string LastName { get; set; }

    public DateTime DateOfBirth { get; set; }

    public string EmailAddress { get; set; }

    public DateTime RegisteredOn { get; set; }
}
