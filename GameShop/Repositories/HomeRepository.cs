using GameShop.Data;
using GameShop.Models;
using Microsoft.EntityFrameworkCore;

namespace GameShop.Repositories
{
    public class HomeRepository : IHomeRepository
    {
        private readonly ApplicationDbContext _db;

        public HomeRepository(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<IEnumerable<Genre>> Genres()
        {
            return await _db.Genres.ToListAsync(); 
        }
        

        // sTerm searchTerm
        public async Task<IEnumerable<Game>> GetGames(string sTerm="", int genreId = 0)
        {
            sTerm = sTerm.ToLower();
            IEnumerable<Game> games = await (from game in _db.Games
                         join genre in _db.Genres
                         on game.GenreId equals genre.Id
                         where string.IsNullOrWhiteSpace(sTerm) ||(game!=null && game.GameName.ToLower().StartsWith(sTerm))
                         
                         select new Game
                         {
                             Id = game.Id,
                             Image = game.Image,
                             AuthorName = game.AuthorName,
                             GameName = game.GameName,
                             GenreId = game.GenreId,
                             Price = game.Price,
                             GenreName = genre.GenreName
                         }


                         ).ToListAsync();
            if(genreId > 0)
            {
                games = games.Where(a => a.GenreId == genreId).ToList();
            }
            return games;
        }
    }
}
