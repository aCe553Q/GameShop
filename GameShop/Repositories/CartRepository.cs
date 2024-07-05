using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using System.Linq.Expressions;
using System.Security.Claims;

namespace GameShop.Repositories
{
    public class CartRepository
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CartRepository(ApplicationDbContext db, IHttpContextAccessor httpContextAccessor,
            UserManager<IdentityUser> userManager)
        {
            db = db;
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task<bool> AddItem(int gameId, int quantity)
        {
            using var transaction = _db.Database.BeginTransaction();
            try
            {



                string userId = GetUserId();
                if (string.IsNullOrEmpty(userId))

                    return false;

                var cart = await GetCart(userId);
                if (cart is null)
                {
                    cart = new ShoppingCart
                    {
                        UserId = userId
                    };
                    _db.ShoppingCarts.Add(cart);
                }
                _db.SaveChanges();
                var cartItem = _db.CartDetails.FirstOrDefault(a=>a.ShoppingCart_Id== cart.Id && a.GameId==gameId);
                if(cart is not null)
                {
                    cartItem.Quantity += quantity;
                }
                else
                {
                    cartItem = new CartDetail
                    {
                        GameId = gameId,
                        ShoppingCart_Id = cart.Id,
                        Quantity = quantity
                    };
                    _db.CartDetails.Add(cartItem);
                }
                _db.SaveChanges();
                transaction.Commit();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
         }
        
        
        private  async Task<ShoppingCart> GetCart(string userId)
        {
            var cart = await _db.ShoppingCarts.FirstOrDefaultAsync(x => x.UserId == userId);
            return cart;

        }

        private string GetUserId()
        {
            var principal = _httpContextAccessor.HttpContext.User;
            string userId = _userManager.GetUserId(principal);
            return userId;
        }



    }
}
