﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlayPalMini.Model.Common
{
    public interface IBoardGame
    {
        Guid Id { get; set; }
        string Title { get; set; }
        string Description { get; set; }
        string CreatedBy { get; set; }
        string UpdatedBy { get; set; }
        DateTime DateCreated { get; set; }
        DateTime DateUpdated { get; set; }
    }
}
