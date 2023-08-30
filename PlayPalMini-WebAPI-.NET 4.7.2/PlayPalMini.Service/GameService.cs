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
    public class GameService : IGameService
    {
        public IGameRepository Repository { get; set; }
        public GameService(IGameRepository repository)
        {
            Repository = repository;
        }
        public async Task<(List<BoardGame>, string)> GetAllAsync()
        {
            (List<BoardGame> list, string message) = await Repository.GetAllAsync();
            if (list == null)
            {
                throw new Exception(message);
            }
            return (list, message);
        }
        //----------------
        public async Task<(BoardGame, string)> GetOneByIdAsync(Guid id)
        {
            (BoardGame game, string message) = await Repository.GetOneByIdAsync(id);
            if (game == null)
            {
                throw new Exception(message);
            }
            return (game, message);
        }
        //----------------
        public async Task<(bool, string)> CreateGameAsync(BoardGame game)
        {
            (bool result, string message) = await Repository.CreateGameAsync(game);
            if (!result)
            {
                throw new Exception(message);
            }
            return (result, message);
        }
        //-----------------
        public async Task<(BoardGame, string)> EditGameAsync(BoardGame game, Guid id)
        {
            (BoardGame gameService, string message) = await Repository.EditGameAsync(game, id);
            if (gameService == null)
            {
                throw new Exception(message);
            }
            return (gameService, message);
        }
        //-----------------
        public async Task<(bool, string)> DeleteGameAsync(Guid id)
        {
            (bool result, string message) = await Repository.DeleteGameAsync(id);
            if (!result)
            {
                throw new Exception(message);
            }
            return (result, message);
        }
        //------------------
        public async Task<(List<BoardGame>, string)> GetAllWithParamsAsync(SearchParam search, SortParam sort, PageParam page)
        {
            (List<BoardGame> list, string message) = await Repository.GetAllWithParamsAsync(search, sort, page);
            if (list == null)
            {
                throw new Exception(message);
            }
            return (list, message);
        }
    }
}
