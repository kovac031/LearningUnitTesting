using PagedList;
using PlayPalMini.DAL;
using PlayPalMini.Model;
using PlayPalMini.Repository.Common;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlayPalMini.Repository
{
    public class BoardGameRepository : IBoardGameRepository
    {
        //--------------dependency injection----------
        public EFContext Context { get; set; }
        public BoardGameRepository(EFContext context)
        {
            Context = context;
        }

        //---------GET ALL-------------------
        public async Task<List<BoardGameDTO>> GetAllBoardGamesAsync(string sorting)
        {
            IQueryable<BoardGame> boardGame = Context.BoardGames;

            switch (sorting)
            {
                case "rating_desc":
                    boardGame = boardGame.OrderByDescending(x => x.Reviews.Any() ? x.Reviews.Average(r => r.Rating) : 0);
                    break;
                case "rating_asc":
                    boardGame = boardGame.OrderBy(x => x.Reviews.Any() ? x.Reviews.Average(r => r.Rating) : 0);
                    break;
                case "title_desc":
                    boardGame = boardGame.OrderByDescending(x => x.Title);
                    break;
                //case "title_asc":
                //    boardGame = boardGame.OrderBy(x => x.Title);
                //    break;
                default:
                    boardGame = boardGame.OrderBy(x => x.Title);
                    break;
            }

            List<BoardGameDTO> list = await boardGame.Select(x => new BoardGameDTO()
            {
                Id = x.Id,
                Title = x.Title,
                Description = x.Description,
                AvgRating = x.Reviews.Any() ? x.Reviews.Average(r => r.Rating) : 0,
                CreatedBy = x.CreatedBy,
                UpdatedBy = x.UpdatedBy,
                DateCreated = x.DateCreated,
                DateUpdated = x.DateUpdated
            })
                .ToListAsync();

            return list;
        }

        //---------GET ONE BY ID----------------
        public async Task<BoardGameDTO> GetSpecificBoardGameAsync(Guid id)
        {
            BoardGameDTO game = await Context.BoardGames.Where(x => x.Id == id).Select(x => new BoardGameDTO()
            {
                Id = x.Id, // ne treba za GET, ali treba za usporedbu id i boardGameId
                Title = x.Title,
                Description = x.Description,
                AvgRating = x.Reviews.Any() ? x.Reviews.Average(r => r.Rating) : 0,
                CreatedBy = x.CreatedBy,
                UpdatedBy = x.UpdatedBy,
                DateCreated = x.DateCreated,
                DateUpdated = x.DateUpdated
            }).FirstOrDefaultAsync<BoardGameDTO>();

            return game;
        }

        //-----------------CREATE BOARD GAME----------------------
        public async Task<bool> CreateBoardGameAsync(BoardGameDTO boardGameDTO)
        {
            BoardGame game = new BoardGame
            {
                Id = boardGameDTO.Id,
                Title = boardGameDTO.Title,
                Description = boardGameDTO.Description,
                CreatedBy = boardGameDTO.CreatedBy,
                UpdatedBy = boardGameDTO.UpdatedBy,
                DateCreated = boardGameDTO.DateCreated,
                DateUpdated = boardGameDTO.DateUpdated
            };
            Context.BoardGames.Add(game);

            await Context.SaveChangesAsync();

            return true;
        }

        //--------------EDIT BOARD GAME------------------
        public async Task<bool> EditBoardGameAsync(BoardGameDTO boardGameDTO, Guid id)
        {
            BoardGame game = await Context.BoardGames.FirstOrDefaultAsync(x => x.Id == id);

            game.Id = boardGameDTO.Id;
            game.Title = boardGameDTO.Title;
            game.Description = boardGameDTO.Description;
            game.UpdatedBy = boardGameDTO.UpdatedBy;
            game.DateUpdated = boardGameDTO.DateUpdated;

            await Context.SaveChangesAsync();
            return true;
        }

        //------------DELETE BOARD GAME--------------
        public async Task<bool> DeleteBoardGameAsync(Guid id)
        {
            BoardGame game = await Context.BoardGames.FirstOrDefaultAsync(x => x.Id == id);
            Context.BoardGames.Remove(game);
            await Context.SaveChangesAsync();

            return true;
        }
    }
}
