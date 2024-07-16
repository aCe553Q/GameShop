using Microsoft.EntityFrameworkCore;

namespace GameShop.Repositories
{
    public interface IGameRepository
    {
        Task AddGame(Game game);
        Task DeleteGame(Game game);
        Task<Game?> GetGameById(int id);
        Task<IEnumerable<Game>> GetGames();
        Task UpdateGame(Game game);
    }
    public class GameRepository : IGameRepository
    {
        private readonly ApplicationDbContext _context;
        public GameRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddGame(Game game)
        {
            _context.Games.Add(game);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateGame(Game game)
        {
            _context.Games.Update(game);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteGame(Game game)
        {
            _context.Games.Remove(game);
            await _context.SaveChangesAsync();
        }

        public async Task<Game?> GetGameById(int id) => await _context.Games.FindAsync(id);

        public async Task<IEnumerable<Game>> GetGames() => await _context.Games.Include(a => a.Genre).ToListAsync();
    }
    }
