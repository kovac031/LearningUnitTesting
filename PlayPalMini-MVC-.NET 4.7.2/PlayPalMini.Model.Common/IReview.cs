using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlayPalMini.Model.Common
{
    public interface IReview
    {
        Guid Id { get; set; }
        string Title { get; set; }
        string Comment { get; set; }
        int Rating { get; set; }
        Guid BoardGameId { get; set; }
    }
}
