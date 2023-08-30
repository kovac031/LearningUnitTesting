using PagedList;
using PlayPalMini.DAL;
using PlayPalMini.Model;
using PlayPalMini.Repository.Common;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;

namespace PlayPalMini.Repository
{
    public class ReviewRepository : IReviewRepository
    {
        public EFContext Context { get; set; }
        public ReviewRepository(EFContext context)
        {
            Context = context;
        }
        //-------------------GET ALL----------------------
        public async Task<List<ReviewDTO>> GetAllReviewsAsync(string sorting)
        {
            IQueryable<Review> reviews = Context.Reviews;

            switch (sorting)
            {
                case "rating_desc":
                    reviews = reviews.OrderByDescending(x => x.Rating);
                    break;
                case "rating_asc":
                    reviews = reviews.OrderBy(x => x.Rating);
                    break;
                case "user_desc":
                    reviews = reviews.OrderByDescending(x => x.CreatedBy);
                    break;
                default:
                    reviews = reviews.OrderBy(x => x.CreatedBy);
                    break;
            }

            List<ReviewDTO> list = await reviews.Select(x => new ReviewDTO()
            {
                Id = x.Id,
                Title = x.Title,
                Rating = x.Rating,
                Comment = x.Comment,
                BoardGameId = x.BoardGameId,
                CreatedBy = x.CreatedBy,
                UpdatedBy = x.UpdatedBy,
                DateCreated = x.DateCreated,
                DateUpdated = x.DateUpdated,
                RegisteredUserId = x.RegisteredUserId

            }).ToListAsync<ReviewDTO>();

            return list;
        }

        //--------------GET ONE BY ID-------------------
        public async Task<ReviewDTO> GetOneReviewAsync(Guid id)
        {
            ReviewDTO review = await Context.Reviews.Where(x => x.Id == id).Select(x => new ReviewDTO()
            {
                Id = x.Id,
                Title = x.Title,
                Comment = x.Comment,
                Rating = x.Rating,
                BoardGameId = x.BoardGameId,
                CreatedBy = x.CreatedBy,
                UpdatedBy = x.UpdatedBy,
                DateCreated = x.DateCreated,
                DateUpdated = x.DateUpdated,
                RegisteredUserId = x.RegisteredUserId
            }).FirstOrDefaultAsync<ReviewDTO>();

            return review;
        }

        //----------- ADD NEW REVIEW -------------------------------
        public async Task<bool> CreateReviewAsync(ReviewDTO reviewDTO) // "ReviewDTO reviewDTO" ide tu jer metoda inace ne vidi "reviewDTO"
        {
            try
            {
                Review review = new Review
                {
                    Id = reviewDTO.Id,
                    Title = reviewDTO.Title,
                    Comment = reviewDTO.Comment,
                    Rating = reviewDTO.Rating,
                    BoardGameId = reviewDTO.BoardGameId,
                    CreatedBy = reviewDTO.CreatedBy,
                    UpdatedBy = reviewDTO.UpdatedBy,
                    DateCreated = reviewDTO.DateCreated,
                    DateUpdated = reviewDTO.DateUpdated,
                    RegisteredUserId = reviewDTO.RegisteredUserId
                };
                Context.Reviews.Add(review);

                await Context.SaveChangesAsync();

                return true;
            }
            catch (Exception) { return false; }            
        }

        //------------------ EDIT REVIEW ------------
        public async Task<bool> EditReviewAsync(ReviewDTO reviewDTO, Guid id)
        {
            Review review = await Context.Reviews.FirstOrDefaultAsync(r => r.Id == id);

            review.Id = reviewDTO.Id;
            review.Title = reviewDTO.Title;
            review.Comment = reviewDTO.Comment;
            review.Rating = reviewDTO.Rating;
            review.UpdatedBy = reviewDTO.UpdatedBy;
            review.DateUpdated = reviewDTO.DateUpdated;

            await Context.SaveChangesAsync();
            return true;
        }

        //-------------------DELETE REVIEW--------------------
        public async Task<bool> DeleteReviewAsync(Guid id)
        {
            Review review = await Context.Reviews.FirstOrDefaultAsync(r => r.Id == id);
            Context.Reviews.Remove(review);
            await Context.SaveChangesAsync();

            return true;
        }
    }
}
