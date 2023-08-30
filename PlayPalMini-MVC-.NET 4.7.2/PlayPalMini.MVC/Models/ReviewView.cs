using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PlayPalMini.MVC.Models
{
    public class ReviewView
    {
        public Guid Id { get; set; }
        public string Title { get; set; }

        [DataType(DataType.MultilineText)]
        public string Comment { get; set; }

        public int Rating { get; set; }
        public Guid BoardGameId { get; set; }

        //---------------------------------------
        public string CreatedBy { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateUpdated { get; set; }

        //-------------------------------------------
        public Guid RegisteredUserId { get; set; }

        public string BoardGameName { get; set; }

    }
}