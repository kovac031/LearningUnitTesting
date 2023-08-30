using PlayPalMini.Common;
using PlayPalMini.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlayPalMini.Repository.Common
{
    public interface IGameRepository
    {
        Task<(List<BoardGame>, string)> GetAllAsync();
        Task<(BoardGame, string)> GetOneByIdAsync(Guid id);
        Task<(bool, string)> CreateGameAsync(BoardGame game);
        Task<(BoardGame, string)> EditGameAsync(BoardGame game, Guid id);
        Task<(bool, string)> DeleteGameAsync(Guid id);
        Task<(List<BoardGame>, string)> GetAllWithParamsAsync(SearchParam search, SortParam sort, PageParam page);
    }
}
