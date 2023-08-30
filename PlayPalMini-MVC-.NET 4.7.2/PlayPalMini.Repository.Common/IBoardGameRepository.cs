using PagedList;
using PlayPalMini.DAL;
using PlayPalMini.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlayPalMini.Repository.Common
{
    public interface IBoardGameRepository
    {
        Task<List<BoardGameDTO>> GetAllBoardGamesAsync(string sorting);
        Task<BoardGameDTO> GetSpecificBoardGameAsync(Guid id);
        Task<bool> CreateBoardGameAsync(BoardGameDTO boardGameDTO);
        Task<bool> EditBoardGameAsync(BoardGameDTO boardGameDTO, Guid id);
        Task<bool> DeleteBoardGameAsync(Guid id);

    }
}
