using PlayPalMini.Common;
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
        public IReviewRepository Repository { get; set; }
        public ReviewService(IReviewRepository repository)
        {
            Repository = repository;
        }
        public async Task<(List<Review>, string)> GetAllAsync()
        {
            (List<Review> list, string message) = await Repository.GetAllAsync();
            if (list == null)
            {
                throw new Exception(message);
            }
            return (list, message);
        }
        //----------------
        public async Task<(Review, string)> GetOneByIdAsync(Guid id)
        {
            (Review review, string message) = await Repository.GetOneByIdAsync(id);
            if (review == null)
            {
                throw new Exception(message);
            }
            return (review, message);
        }
        //----------------
        public async Task<(bool, string)> CreateReviewAsync(Review review)
        {
            (bool result, string message) = await Repository.CreateReviewAsync(review);
            if (!result)
            {
                throw new Exception(message);
            }
            return (result, message);
        }
        //-----------------
        public async Task<(Review, string)> EditReviewAsync(Review review, Guid id)
        {
            (Review reviewService, string message) = await Repository.EditReviewAsync(review, id);
            if (reviewService == null)
            {
                throw new Exception(message);
            }
            return (reviewService, message);
        }
        //-----------------
        public async Task<(bool, string)> DeleteReviewAsync(Guid id)
        {
            (bool result, string message) = await Repository.DeleteReviewAsync(id);
            if (!result)
            {
                throw new Exception(message);
            }
            return (result, message);
        }
        //------------------
        public async Task<(List<Review>, string)> GetAllWithParamsAsync(SearchParam search, SortParam sort, PageParam page)
        {
            (List<Review> list, string message) = await Repository.GetAllWithParamsAsync(search, sort, page);
            if (list == null)
            {
                throw new Exception(message);
            }
            return (list, message);
        }
        //------------------
        public async Task<(List<Review>, string)> GetAllReviewsForOneGame(Guid id, SearchParam search, SortParam sort, PageParam page)
        {
            (List<Review> list, string message) = await Repository.GetAllReviewsForOneGame(id, search, sort, page);
            if (list == null)
            {
                throw new Exception(message);
            }
            return (list, message);
        }
    }
}
