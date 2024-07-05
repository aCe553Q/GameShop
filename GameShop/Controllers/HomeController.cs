using GameShop.Models;
using GameShop.Models.DTOs;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace GameShop.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IHomeRepository _homeRepository;

        public HomeController(ILogger<HomeController> logger, IHomeRepository homeRepository)
        {
            _logger = logger;
            _homeRepository = homeRepository;
        }

        public async Task<IActionResult> Index(string sterm = "", int genreId = 0)
        {         
            IEnumerable<Game> games = await _homeRepository.GetGames(sterm, genreId);
            IEnumerable<Genre> genres = await _homeRepository.Genres();
            GameDisplayModel gameModel = new GameDisplayModel
            {
                Games = games,
                Genres = genres,
                STerm = sterm,
                GenreId = genreId
            };
            return View(gameModel);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
