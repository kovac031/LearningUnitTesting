using PagedList;
using PlayPalMini.Model;
using PlayPalMini.Repository.Common;
using PlayPalMini.Service.Common;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlayPalMini.Service
{
    public class BoardGameService : IBoardGameService
    {
        public IBoardGameRepository BoardGameRepository { get; set; }
        public BoardGameService(IBoardGameRepository boardGameRepository)
        {
            BoardGameRepository = boardGameRepository;
        }

        public async Task<List<BoardGameDTO>> GetAllBoardGamesAsync(string sorting)
        {
            List<BoardGameDTO> list = await BoardGameRepository.GetAllBoardGamesAsync(sorting);
            return list;
        }

        public async Task<BoardGameDTO> GetSpecificBoardGameAsync(Guid id)
        {
            BoardGameDTO boardGameDTO = await BoardGameRepository.GetSpecificBoardGameAsync(id);
            return boardGameDTO;
        }

        public async Task<bool> CreateBoardGameAsync(BoardGameDTO boardGameDTO)
        {
            bool result = await BoardGameRepository.CreateBoardGameAsync(boardGameDTO);
            return result;
        }

        public async Task<bool> EditBoardGameAsync(BoardGameDTO boardGameDTO, Guid id)
        {
            bool result = await BoardGameRepository.EditBoardGameAsync(boardGameDTO, id);
            return result;
        }

        public async Task<bool> DeleteBoardGameAsync(Guid id)
        {
            bool result = await BoardGameRepository.DeleteBoardGameAsync(id);
            return result;
        }
    }
}
