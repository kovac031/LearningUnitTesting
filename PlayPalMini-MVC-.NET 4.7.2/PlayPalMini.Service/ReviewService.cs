using PagedList;
using PlayPalMini.Model;
using PlayPalMini.Repository.Common;
using PlayPalMini.Service.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlayPalMini.Service
{
    public class ReviewService : IReviewService
    {
        public IReviewRepository ReviewRepository { get; set; }
        public ReviewService(IReviewRepository reviewRepository)
        {
            ReviewRepository = reviewRepository;
        }

        public async Task<List<ReviewDTO>> GetAllReviewsAsync(string sorting)
        {
            List<ReviewDTO> list = await ReviewRepository.GetAllReviewsAsync(sorting);
            return list;
        }

        public async Task<bool> CreateReviewAsync(ReviewDTO reviewDTO)
        {
            bool result = await ReviewRepository.CreateReviewAsync(reviewDTO);
            return result;
        }

        public async Task<bool> EditReviewAsync(ReviewDTO reviewDTO, Guid id)
        {
            bool result = await ReviewRepository.EditReviewAsync(reviewDTO, id);
            return result;
        }
        public async Task<ReviewDTO> GetOneReviewAsync(Guid id)
        {
            ReviewDTO review = await ReviewRepository.GetOneReviewAsync(id);
            return review;
        }
        public async Task<bool> DeleteReviewAsync(Guid id)
        {
            bool result = await ReviewRepository.DeleteReviewAsync(id);
            return result;
        }
    }
}
