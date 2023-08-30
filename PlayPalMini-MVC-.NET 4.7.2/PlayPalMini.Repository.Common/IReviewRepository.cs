using PlayPalMini.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlayPalMini.Repository.Common
{
    public interface IReviewRepository
    {
        Task<List<ReviewDTO>> GetAllReviewsAsync(string sorting);
        Task<bool> CreateReviewAsync(ReviewDTO reviewDTO);
        Task<bool> EditReviewAsync(ReviewDTO reviewDTO, Guid id);
        Task<ReviewDTO> GetOneReviewAsync(Guid id);
        Task<bool> DeleteReviewAsync(Guid id);
    }
}
