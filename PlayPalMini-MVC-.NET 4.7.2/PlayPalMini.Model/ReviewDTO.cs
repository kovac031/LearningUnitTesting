using PlayPalMini.Model.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlayPalMini.Model
{
    public class ReviewDTO : IReview
    { 
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Comment { get; set; }
        public int Rating { get; set; }
        public Guid BoardGameId { get; set; }

        //---------------------------------------
        public string CreatedBy { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateUpdated { get; set; }

        //-----------------------------------------
        public Guid RegisteredUserId { get; set; }

    }

}
