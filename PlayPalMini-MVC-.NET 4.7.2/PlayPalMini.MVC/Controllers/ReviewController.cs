using PagedList;
using PlayPalMini.Model;
using PlayPalMini.MVC.Models;
using PlayPalMini.Repository;
using PlayPalMini.Service;
using PlayPalMini.Service.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;


namespace PlayPalMini.MVC.Controllers
{
    public class ReviewController : Controller
    {
        public IReviewService ReviewService { get; set; }
        public IBoardGameService BoardGameService { get; set; }
        public ReviewController(IReviewService reviewService, IBoardGameService boardGameService)
        {
            ReviewService = reviewService;
            BoardGameService = boardGameService;
        }
        //----------------GET ALL---------------------
        public async Task<ActionResult> GetAllReviewsAsync(string sorting, int? page) // parametri za URL u zagradi
        {
            List<ReviewDTO> listDTO = await ReviewService.GetAllReviewsAsync(sorting);

            //-------------MAPIRANJE BEZ PAGINGA---------------

            //List<ReviewView> listView = new List<ReviewView>();
            //foreach (ReviewDTO reviewDTO in listDTO)
            //{
            //    ReviewView reviewView = new ReviewView();
            //    reviewView.Id = reviewDTO.Id;
            //    reviewView.Title = reviewDTO.Title;
            //    reviewView.Rating = reviewDTO.Rating;
            //    reviewView.Comment = reviewDTO.Comment;
            //    reviewView.BoardGameId = reviewDTO.BoardGameId;
            //    //--------------------------------------------
            //    reviewView.CreatedBy = reviewDTO.CreatedBy;
            //    reviewView.UpdatedBy = reviewDTO.UpdatedBy;
            //    reviewView.DateCreated = reviewDTO.DateCreated;
            //    reviewView.DateUpdated = reviewDTO.DateUpdated;
            //    reviewView.RegisteredUserId = reviewDTO.RegisteredUserId;
            //    listView.Add(reviewView);
            //}

            //---------------PAGING----------------------------------

            int count = listDTO.Count;
            int pageNumber = page ?? 1;
            int pageSize = 10;

            List<ReviewView> listView = listDTO
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(x => new ReviewView()
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
                })
                .ToList();

            StaticPagedList<ReviewView> pagedList = new StaticPagedList<ReviewView>(listView, pageNumber, pageSize, count);
            ViewBag.CurrentPage = page;
            //--------------------------------------------

            ViewBag.SortByUser = string.IsNullOrEmpty(sorting) ? "user_desc" : "";
            ViewBag.SortByRating = sorting == "rating_asc" ? "rating_desc" : "rating_asc";
            ViewBag.CurrentSort = sorting;

            return View(pagedList);
        }

        //------PARTIAL---- Get all reviews for specific game -----
        public async Task<ActionResult> GetReviewsForSpecificGameAsync(Guid id, string sorting, int? page)
        {
            List<ReviewDTO> listDTO = await ReviewService.GetAllReviewsAsync(sorting);

            //int count = listDTO.Count; //broji sve reviewse, zato ne valja

            List<ReviewDTO> filteredList = listDTO
                .Where(reviewDTO => reviewDTO.BoardGameId == id)
                .ToList();

            int count = filteredList.Count;

            int pageNumber = page ?? 1;
            int pageSize = 10;

            List<ReviewView> listView = filteredList
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(x => new ReviewView()
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
                })
                .ToList();

            StaticPagedList<ReviewView> pagedList = new StaticPagedList<ReviewView>(listView, pageNumber, pageSize, count);
            ViewBag.CurrentPage = page;
            ViewBag.SortByUser = string.IsNullOrEmpty(sorting) ? "user_desc" : "";
            ViewBag.SortByRating = sorting == "rating_asc" ? "rating_desc" : "rating_asc";
            ViewBag.CurrentSort = sorting;
            ViewBag.Id = id;

            return PartialView("_GetReviewsForSpecificGameAsync", pagedList);
        }

        //------------------GET ONE BY ID------------------
        public async Task<ActionResult> GetOneReviewAsync(Guid id)
        {
            ReviewDTO reviewDTO = await ReviewService.GetOneReviewAsync(id);
            ReviewView reviewView = new ReviewView();

            reviewView.Id = reviewDTO.Id;
            reviewView.Title = reviewDTO.Title;
            reviewView.Comment = reviewDTO.Comment;
            reviewView.Rating = reviewDTO.Rating;
            reviewView.BoardGameId = reviewDTO.BoardGameId;
            //------------------------------------------------
            reviewView.CreatedBy = reviewDTO.CreatedBy;
            reviewView.UpdatedBy = reviewDTO.UpdatedBy;
            reviewView.DateCreated = reviewDTO.DateCreated;
            reviewView.DateUpdated = reviewDTO.DateUpdated;
            reviewView.RegisteredUserId = reviewDTO.RegisteredUserId;

            return View(reviewView);
        }

        //--------------- CREATE REVIEW ---------------
        [HttpGet]
        public async Task<ActionResult> CreateReviewAsync()
        {
            ReviewView reviewView = new ReviewView();
            ReviewDTO reviewDTO = new ReviewDTO();
            reviewView.Id = reviewDTO.Id;
            reviewView.Title = reviewDTO.Title;
            reviewView.Comment = reviewDTO.Comment;
            reviewView.Rating = reviewDTO.Rating;
            reviewView.BoardGameId = reviewDTO.BoardGameId;
            //------------------------------------------------
            reviewView.CreatedBy = reviewDTO.CreatedBy;
            reviewView.UpdatedBy = reviewDTO.UpdatedBy;
            reviewView.DateCreated = reviewDTO.DateCreated;
            reviewView.DateUpdated = reviewDTO.DateUpdated;
            reviewView.RegisteredUserId = reviewDTO.RegisteredUserId;
            //return View();
            return PartialView("_CreateReviewAsync");
        }
        [HttpPost]
        public async Task<ActionResult> CreateReviewAsync(ReviewView reviewView, Guid id)
        {
            //Guid temporary = Guid.Parse("6042bcd9-9938-40ed-a102-0d9a98313310"); //privremeni boardGameId jer je FK, pa nemoze novi nego mora postojeci

            //----------------------dohvacanje usera nakon autentikacije------
            string userId = "";
            string userName = "";

            if (User.Identity is FormsIdentity identity)
            {
                userId = identity.Ticket.UserData;
                userName = identity.Name;
            }
            //-----------------------------------------------------------------

            ReviewDTO reviewDTO = new ReviewDTO
            {
                Id = Guid.NewGuid(),
                Title = reviewView.Title,
                Comment = reviewView.Comment,
                Rating = reviewView.Rating,
                //BoardGameId = temporary,
                BoardGameId = id,
                //--------------------------------
                CreatedBy = userName,
                UpdatedBy = "n/a",
                DateCreated = DateTime.Now,
                DateUpdated = DateTime.Now,
                RegisteredUserId = new Guid(userId)
            };

            await ReviewService.CreateReviewAsync(reviewDTO);

            TempData["ConfirmationMessage"] = "Your review is added!";
            return RedirectToAction("GetSpecificBoardGameAsync", "BoardGame", new { id });
        }

        //-----------EDIT REVIEW----------------
        [HttpGet]
        public async Task<ActionResult> EditReviewAsync(Guid id)
        {
            ReviewDTO reviewDTO = await ReviewService.GetOneReviewAsync(id);
            ReviewView reviewView = new ReviewView();

            reviewView.Id = reviewDTO.Id;
            reviewView.Title = reviewDTO.Title;
            reviewView.Comment = reviewDTO.Comment;
            reviewView.Rating = reviewDTO.Rating;
            //-----------------------------------------------
            reviewView.UpdatedBy = reviewDTO.UpdatedBy;
            reviewView.DateUpdated = reviewDTO.DateUpdated;

            return View(reviewView);
        }
        [HttpPost]
        public async Task<ActionResult> EditReviewAsync(ReviewView reviewView)
        {
            //----------------------dohvacanje usera nakon autentikacije------
            string userName = "";
            if (User.Identity is FormsIdentity identity)
            {
                userName = identity.Name;
            }
            //-----------------------------------------------------------------

            ReviewDTO reviewDTO = new ReviewDTO();
            reviewDTO.Id = reviewView.Id;
            reviewDTO.Title = reviewView.Title;
            reviewDTO.Comment = reviewView.Comment;
            reviewDTO.Rating = reviewView.Rating;
            //--------------------------------------
            reviewDTO.UpdatedBy = userName;
            reviewDTO.DateUpdated = DateTime.Now;

            await ReviewService.EditReviewAsync(reviewDTO, reviewDTO.Id); //bitan redoslijed parametara kao u servisu, inace se buni

            TempData["ConfirmationMessage"] = "Saved!";
            return View();
            //return RedirectToAction("GetAllReviewsAsync");
        }

        //----------------------DELETE REVIEW---------------------
        public async Task<ActionResult> DeleteReviewAsync(Guid id)
        {
            ReviewDTO reviewDTO = await ReviewService.GetOneReviewAsync(id); // ovaj dio samo treba za boardGameId da mogu refreshat page
              
            await ReviewService.DeleteReviewAsync(id);

            return RedirectToAction("GetSpecificBoardGameAsync", "BoardGame", new { id = reviewDTO.BoardGameId }); 
        }

        //---------------GET MY REVIEWS----------------------
        public async Task<ActionResult> GetMyReviewsAsync(string sorting)
        {
            string userName = "";
            if (User.Identity is FormsIdentity identity)
            {
                userName = identity.Name;
            }
            List<ReviewDTO> listDTO = await ReviewService.GetAllReviewsAsync(sorting);
            List<ReviewDTO> shortListDTO = listDTO.Where(x => x.CreatedBy == userName).ToList();
            List<ReviewView> listView = new List<ReviewView>();

            foreach (ReviewDTO reviewDTO in shortListDTO)
            {
                ReviewView reviewView = new ReviewView();
                reviewView.Id = reviewDTO.Id;
                reviewView.Title = reviewDTO.Title;
                reviewView.Rating = reviewDTO.Rating;
                reviewView.Comment = reviewDTO.Comment;
                reviewView.BoardGameId = reviewDTO.BoardGameId;

                BoardGameDTO gameDTO = await BoardGameService.GetSpecificBoardGameAsync(reviewDTO.BoardGameId);
                reviewView.BoardGameName = gameDTO.Title;
                
                listView.Add(reviewView);
            }
            return View(listView); // mora (listView), ne moze ()
        }
    }
}