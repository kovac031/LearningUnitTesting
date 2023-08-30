using System;
using System.Collections.Generic;

namespace WebAPI;

public partial class Student
{
    public Guid Id { get; set; }

    public string? FirstName { get; set; } //;

    public string? LastName { get; set; } // = null!;

    public DateTime DateOfBirth { get; set; }

    public string? EmailAddress { get; set; } // = null!;

    public DateTime RegisteredOn { get; set; }
}
