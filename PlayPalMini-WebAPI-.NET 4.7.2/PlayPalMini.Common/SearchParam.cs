using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlayPalMini.Common
{
    public class SearchParam
    {
        public string Username { get; set; }
        public string Role { get; set; }
        public string CreatedBy { get; set; }
        public string UpdatedBy { get; set; }
        public string CreatedBefore { get; set; }
        public string CreatedAfter { get; set; }
        public string UpdatedBefore { get; set; }
        public string UpdatedAfter { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Comment { get; set; }
        public int Rating { get; set; } // u Review
        public string AverageGreaterThan { get; set; } // nije ni Rating morao int, jer sve ionako ide u SQL komandu koja dalje tumači tip podatka
        public string AverageLessThan { get; set;}        
    }
}
