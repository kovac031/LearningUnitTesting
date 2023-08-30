namespace PlayPalMini.DAL
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Review")]
    public partial class Review
    {
        public Guid Id { get; set; }

        [Column(TypeName = "text")]
        [Required]
        public string Title { get; set; }

        [Column(TypeName = "text")]
        public string Comment { get; set; }

        public int Rating { get; set; }

        public Guid BoardGameId { get; set; }

        [StringLength(255)]
        public string CreatedBy { get; set; }

        [StringLength(255)]
        public string UpdatedBy { get; set; }

        public DateTime DateCreated { get; set; }

        public DateTime DateUpdated { get; set; }

        public Guid RegisteredUserId { get; set; }

        public virtual BoardGame BoardGame { get; set; }

        public virtual RegisteredUser RegisteredUser { get; set; }
    }
}
