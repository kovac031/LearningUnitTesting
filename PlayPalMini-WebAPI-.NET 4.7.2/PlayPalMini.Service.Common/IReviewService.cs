using PlayPalMini.Common;
using PlayPalMini.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlayPalMini.Service.Common
{
    public interface IReviewService
    {
        Task<(List<Review>, string)> GetAllAsync();
        Task<(Review, string)> GetOneByIdAsync(Guid id);
        Task<(bool, string)> CreateReviewAsync(Review review);
        Task<(Review, string)> EditReviewAsync(Review review, Guid id);
        Task<(bool, string)> DeleteReviewAsync(Guid id);
        Task<(List<Review>, string)> GetAllWithParamsAsync(SearchParam search, SortParam sort, PageParam page);
        Task<(List<Review>, string)> GetAllReviewsForOneGame(Guid id, SearchParam search, SortParam sort, PageParam page);
    }
}
