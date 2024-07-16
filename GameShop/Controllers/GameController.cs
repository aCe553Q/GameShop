using GameShop.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GameShop.Controllers
{

    [Authorize(Roles = nameof(Roles.Admin))]
    public class GameController : Controller
    {
        private readonly IGameRepository _gameRepo;
        private readonly IGenreRepository _genreRepo;
        private readonly IFileService _fileService;

        public GameController(IGameRepository gameRepo, IGenreRepository genreRepo, IFileService fileService)
        {
            _gameRepo = gameRepo;
            _genreRepo = genreRepo;
            _fileService = fileService;
        }

        public async Task<IActionResult> Index()
        {
            var games = await _gameRepo.GetGames();
            return View(games);
        }

        public async Task<IActionResult> AddGame()
        {
            var genreSelectList = (await _genreRepo.GetGenres()).Select(genre => new SelectListItem
            {
                Text = genre.GenreName,
                Value = genre.Id.ToString(),
            });
            GameDTO gameToAdd = new() { GenreList = genreSelectList };
            return View(gameToAdd);
        }

        [HttpPost]
        public async Task<IActionResult> AddGame(GameDTO gameToAdd)
        {
            var genreSelectList = (await _genreRepo.GetGenres()).Select(genre => new SelectListItem
            {
                Text = genre.GenreName,
                Value = genre.Id.ToString(),
            });
            gameToAdd.GenreList = genreSelectList;

            if (!ModelState.IsValid)
                return View(gameToAdd);

            try
            {
                if (gameToAdd.ImageFile != null)
                {
                    if (gameToAdd.ImageFile.Length > 1 * 1024 * 1024)
                    {
                        throw new InvalidOperationException("Image file can not exceed 1 MB");
                    }
                    string[] allowedExtensions = [".jpeg", ".jpg", ".png"];
                    string imageName = await _fileService.SaveFile(gameToAdd.ImageFile, allowedExtensions);
                    gameToAdd.Image = imageName;
                }

                Game game = new()
                {
                    Id = gameToAdd.Id,
                    GameName = gameToAdd.GameName,
                    AuthorName = gameToAdd.AuthorName,
                    Image = gameToAdd.Image,
                    GenreId = gameToAdd.GenreId,
                    Price = gameToAdd.Price
                };
                await _gameRepo.AddGame(game);
                TempData["successMessage"] = "Game is added successfully";
                return RedirectToAction(nameof(AddGame));
            }
            catch (InvalidOperationException ex)
            {
                TempData["errorMessage"] = ex.Message;
                return View(gameToAdd);
            }
            catch (FileNotFoundException ex)
            {
                TempData["errorMessage"] = ex.Message;
                return View(gameToAdd);
            }
            catch (Exception ex)
            {
                TempData["errorMessage"] = "Error on saving data";
                return View(gameToAdd);
            }
        }

        public async Task<IActionResult> UpdateGame(int id)
        {
            var game = await _gameRepo.GetGameById(id);
            if (game == null)
            {
                TempData["errorMessage"] = $"Game with the id: {id} does not found";
                return RedirectToAction(nameof(Index));
            }
            var genreSelectList = (await _genreRepo.GetGenres()).Select(genre => new SelectListItem
            {
                Text = genre.GenreName,
                Value = genre.Id.ToString(),
                Selected = genre.Id == game.GenreId
            });
            GameDTO gameToUpdate = new()
            {
                GenreList = genreSelectList,
                GameName = game.GameName,
                AuthorName = game.AuthorName,
                GenreId = game.GenreId,
                Price = game.Price,
                Image = game.Image
            };
            return View(gameToUpdate);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateGame(GameDTO gameToUpdate)
        {
            var genreSelectList = (await _genreRepo.GetGenres()).Select(genre => new SelectListItem
            {
                Text = genre.GenreName,
                Value = genre.Id.ToString(),
                Selected = genre.Id == gameToUpdate.GenreId
            });
            gameToUpdate.GenreList = genreSelectList;

            if (!ModelState.IsValid)
                return View(gameToUpdate);

            try
            {
                string oldImage = "";
                if (gameToUpdate.ImageFile != null)
                {
                    if (gameToUpdate.ImageFile.Length > 1 * 1024 * 1024)
                    {
                        throw new InvalidOperationException("Image file can not exceed 1 MB");
                    }
                    string[] allowedExtensions = [".jpeg", ".jpg", ".png"];
                    string imageName = await _fileService.SaveFile(gameToUpdate.ImageFile, allowedExtensions);

                    oldImage = gameToUpdate.Image;
                    gameToUpdate.Image = imageName;
                }

                Game game = new()
                {
                    Id = gameToUpdate.Id,
                    GameName = gameToUpdate.GameName,
                    AuthorName = gameToUpdate.AuthorName,
                    GenreId = gameToUpdate.GenreId,
                    Price = gameToUpdate.Price,
                    Image = gameToUpdate.Image
                };
                await _gameRepo.UpdateGame(game);
                // if image is updated, then delete it from the folder too
                if (!string.IsNullOrWhiteSpace(oldImage))
                {
                    _fileService.DeleteFile(oldImage);
                }
                TempData["successMessage"] = "Game is updated successfully";
                return RedirectToAction(nameof(Index));
            }
            catch (InvalidOperationException ex)
            {
                TempData["errorMessage"] = ex.Message;
                return View(gameToUpdate);
            }
            catch (FileNotFoundException ex)
            {
                TempData["errorMessage"] = ex.Message;
                return View(gameToUpdate);
            }
            catch (Exception ex)
            {
                TempData["errorMessage"] = "Error on saving data";
                return View(gameToUpdate);
            }
        }

        public async Task<IActionResult> DeleteGame(int id)
        {
            try
            {
                var game = await _gameRepo.GetGameById(id);
                if (game == null)
                {
                    TempData["errorMessage"] = $"Game with the id: {id} does not found";
                }
                else
                {
                    await _gameRepo.DeleteGame(game);
                    if (!string.IsNullOrWhiteSpace(game.Image))
                    {
                        _fileService.DeleteFile(game.Image);
                    }
                }
            }
            catch (InvalidOperationException ex)
            {
                TempData["errorMessage"] = ex.Message;
            }
            catch (FileNotFoundException ex)
            {
                TempData["errorMessage"] = ex.Message;
            }
            catch (Exception ex)
            {
                TempData["errorMessage"] = "Error on deleting the data";
            }
            return RedirectToAction(nameof(Index));
        }
    }
}

