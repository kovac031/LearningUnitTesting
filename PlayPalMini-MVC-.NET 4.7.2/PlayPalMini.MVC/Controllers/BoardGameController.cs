using PagedList;
using PlayPalMini.DAL;
using PlayPalMini.Model;
using PlayPalMini.MVC.Models;
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

namespace PlayPalMini.MVC.Controllers
{
    public class BoardGameController : Controller
    {
        public IBoardGameService BoardGameService { get; set; }
        public IReviewService ReviewService { get; set; }
        public BoardGameController(IBoardGameService boardGameService, IReviewService reviewService)
        {
            BoardGameService = boardGameService;
            ReviewService = reviewService;
        }

        //----------GET ALL-------------
        public async Task<ActionResult> GetAllBoardGamesAsync(int? page, string filterBy, string sorting) // parametri koji ce ici u URL, moraju se navesti na kraju u cshtmlu i za njih mora biti ViewBag svaki
        {
            List<BoardGameDTO> allGames = await BoardGameService.GetAllBoardGamesAsync(sorting); // dohvati sve // dodao sorting kasnije

            List<BoardGameDTO> filteredList = allGames; //prije filtriranja lista ima sve igre

            if (!string.IsNullOrEmpty(filterBy)) //filtrira, pretrazuje u title i description stupcima
            {
                filteredList = allGames.Where(x =>
                    x.Title.IndexOf(filterBy, StringComparison.OrdinalIgnoreCase) >= 0 ||
                    x.Description.IndexOf(filterBy, StringComparison.OrdinalIgnoreCase) >= 0)
                    .ToList();
            }

            int count = filteredList.Count; // mislim da PagedList usporedjuje count sa pageSize i zato nam treba

            int pageNumber = page ?? 1;
            int pageSize = 10;

            List<BoardGameView> listView = filteredList
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(x => new BoardGameView()
                {
                    Id = x.Id,
                    Title = x.Title,
                    Description = x.Description,
                    AvgRating = Math.Round(x.AvgRating, 2),
                    CreatedBy = x.CreatedBy,
                    UpdatedBy = x.UpdatedBy,
                    DateCreated = x.DateCreated,
                    DateUpdated = x.DateUpdated
                })
                .ToList();

            StaticPagedList<BoardGameView> pagedList = new StaticPagedList<BoardGameView>(listView, pageNumber, pageSize, count);

            ViewBag.CurrentPage = page;
            ViewBag.CurrentFilter = filterBy;

            ViewBag.SortByTitle = string.IsNullOrEmpty(sorting) ? "title_desc" : ""; //prvi klik prije sortanja je true pa postavi title_desc, a svaki klik kad vec ima sorting u biti je notnull, tj true pa ga obrise
            ViewBag.SortByRating = sorting == "rating_asc" ? "rating_desc" : "rating_asc"; //prvi klik je false pa postavi rating_asc, drugi klik je true pa postavi rating_desc
            ViewBag.CurrentSort = sorting;

            return View(pagedList);
        }

        //------- GET ONE BY ID ----------
        //[HttpGet]
        //[Route("api/BoardGame/GetSpecificBoardGameAsync/{id}")]
        public async Task<ActionResult> GetSpecificBoardGameAsync(Guid id)
        {
            BoardGameDTO boardGameDTO = await BoardGameService.GetSpecificBoardGameAsync(id);
            BoardGameView boardGameView = new BoardGameView();

            boardGameView.Id = boardGameDTO.Id; // ne treba za GET, ali treba za usporedbu id i boardGameId
            boardGameView.Title = boardGameDTO.Title;
            boardGameView.Description = boardGameDTO.Description;
            boardGameView.AvgRating = Math.Round(boardGameDTO.AvgRating, 2);
            //------------------------------------------------
            boardGameView.CreatedBy = boardGameDTO.CreatedBy;
            boardGameView.UpdatedBy = boardGameDTO.UpdatedBy;
            boardGameView.DateCreated = boardGameDTO.DateCreated;
            boardGameView.DateUpdated = boardGameDTO.DateUpdated;

            return View(boardGameView);
        }

        //--------------CREATE BOARD GAME------------------
        [HttpGet]
        public async Task<ActionResult> CreateBoardGameAsync()
        {
            BoardGameDTO boardGameDTO = new BoardGameDTO();
            BoardGameView boardGameView = new BoardGameView();
            boardGameView.Id = boardGameDTO.Id;
            boardGameView.Title = boardGameDTO.Title;
            boardGameView.Description = boardGameDTO.Description;
            //--------------------------------------------------
            boardGameView.CreatedBy = boardGameDTO.CreatedBy;
            boardGameView.UpdatedBy = boardGameDTO.UpdatedBy;
            boardGameView.DateCreated = boardGameDTO.DateCreated;
            boardGameView.DateUpdated = boardGameDTO.DateUpdated;

            return View();
        }
        [HttpPost]
        public async Task<ActionResult> CreateBoardGameAsync(BoardGameView boardGameView, string sorting) //sorting samo zbog potrebe get all metode
        {
            //----------------------dohvacanje usera nakon autentikacije------
            string userName = "";
            if (User.Identity is FormsIdentity identity)
            {
                userName = identity.Name;
            }
            //-----------------------------------------------------------------
            List<BoardGameDTO> check = await BoardGameService.GetAllBoardGamesAsync(sorting);
            if (check.Any(u => u.Title == boardGameView.Title))
            {
                ModelState.AddModelError("Title", "A board game with that name is already in the database.");
                return View(boardGameView);
            }

            BoardGameDTO boardGameDTO = new BoardGameDTO()
            {
                Id = Guid.NewGuid(),
                Title = boardGameView.Title,
                Description = boardGameView.Description,
                //-------------------------------------------
                CreatedBy = userName,
                UpdatedBy = "n/a",
                DateCreated = DateTime.Now,
                DateUpdated = DateTime.Now
            };
            await BoardGameService.CreateBoardGameAsync(boardGameDTO);

            TempData["ConfirmationMessage"] = "Saved!";
            return View();
            //return RedirectToAction("GetAllBoardGamesAsync");
        }

        //----------------EDIT BOARD GAME-------------------
        [HttpGet]
        public async Task<ActionResult> EditBoardGameAsync(Guid id)
        {
            BoardGameDTO boardGameDTO = await BoardGameService.GetSpecificBoardGameAsync(id);
            BoardGameView boardGameView = new BoardGameView();

            boardGameView.Id = boardGameDTO.Id;
            boardGameView.Title = boardGameDTO.Title;
            boardGameView.Description = boardGameDTO.Description;
            //--------------------------------------------------
            boardGameView.UpdatedBy = boardGameDTO.UpdatedBy;
            boardGameView.DateUpdated = boardGameDTO.DateUpdated;

            return View(boardGameView);
        }
        [HttpPost]
        public async Task<ActionResult> EditBoardGameAsync(BoardGameView boardGameView)
        {
            //----------------------dohvacanje usera nakon autentikacije------
            string userName = "";
            if (User.Identity is FormsIdentity identity)
            {
                userName = identity.Name;
            }
            //-----------------------------------------------------------------

            BoardGameDTO boardGameDTO = new BoardGameDTO();
            boardGameDTO.Id = boardGameView.Id;
            boardGameDTO.Title = boardGameView.Title;
            boardGameDTO.Description = boardGameView.Description;
            //----------------------------------
            boardGameDTO.UpdatedBy = userName;
            boardGameDTO.DateUpdated = DateTime.Now;

            await BoardGameService.EditBoardGameAsync(boardGameDTO, boardGameDTO.Id); //bitan redoslijed parametara kao u servisu, inace se buni

            TempData["ConfirmationMessage"] = "Saved!";
            return View(boardGameView);
            //return RedirectToAction("GetAllBoardGamesAsync");
        }

        //--------------DELETE BOARD GAME------------
        public async Task<ActionResult> DeleteBoardGameAsync(Guid id)
        {
            await BoardGameService.DeleteBoardGameAsync(id);
            return RedirectToAction("GetAllBoardGamesAsync");
        }
    }
}