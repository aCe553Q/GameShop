﻿using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using System.Linq.Expressions;
using System.Security.Claims;

namespace GameShop.Repositories
{
    public class CartRepository : ICartRepository
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CartRepository(ApplicationDbContext db, IHttpContextAccessor httpContextAccessor,
            UserManager<IdentityUser> userManager)
        {
            _db = db;
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task<int> AddItem(int gameId, int quantity)
        {
            string userId = GetUserId();
            using var transaction = _db.Database.BeginTransaction();
            try
            {               
                if (string.IsNullOrEmpty(userId))

                    throw new Exception("user is not logged");

                var cart = await GetCart(userId);
                if (cart is null)
                {
                    cart = new ShoppingCart //added to dataase
                    {
                        UserId = userId
                    };
                    _db.ShoppingCarts.Add(cart);
                }
                _db.SaveChanges();
                var cartItem = _db.CartDetails // Find cart item
                                .FirstOrDefault(a => a.ShoppingCartId == cart.Id && a.GameId == gameId);
                if (cartItem is not null)
                {
                    cartItem.Quantity += quantity; //increase quantity
                }
                else
                {
                    var game = _db.Games.Find(gameId);
                    cartItem = new CartDetail
                    {

                        GameId = gameId,
                        ShoppingCartId = cart.Id,
                        Quantity = quantity,
                        UnitPrice = game.Price
                    };
                    _db.CartDetails.Add(cartItem);
                }
                _db.SaveChanges();
                transaction.Commit(); //save to database
                
            }
            catch (Exception ex)
            {
                
            }
            var cartItemCount = await GetCartItemCount(userId); // total number of items
            return cartItemCount;
        }

        public async Task<int> RemoveItem(int gameId)
        {
            string userId = GetUserId();
            //using var transaction = _db.Database.BeginTransaction();
            try
            {
                if (string.IsNullOrEmpty(userId))

                    throw new Exception("user is not logged");

                var cart = await GetCart(userId);
                if (cart is null)
                {
                    throw new Exception("Invalid cart");
                }
                _db.SaveChanges();
                var cartItem = _db.CartDetails
                                .FirstOrDefault(a => a.ShoppingCartId == cart.Id && a.GameId == gameId);
                if (cartItem is null)
                {
                    throw new Exception("No items in cart");
                }

                else if (cartItem.Quantity == 1)
                {
                    _db.CartDetails.Remove(cartItem);
                }
                else
                {
                    cartItem.Quantity = cartItem.Quantity - 1;
                }
                _db.SaveChanges();
                // transaction.Commit(); not needed because it's a same cart
                
            }
            catch (Exception ex)
            {
                
            }
            var cartItemCount = await GetCartItemCount(userId);
            return cartItemCount;
        }

        public async Task<ShoppingCart> GetUserCart()
        {
            //query joining tables
            var userId = GetUserId();
            if(userId== null)
            {
                throw new Exception("Invalid userId");
            }
            var shoppingCart = await _db.ShoppingCarts.Include(a => a.CartDetails)
                                                .ThenInclude(a => a.Game)
                                                .ThenInclude(a => a.Genre)
                                                .Where(a => a.UserId == userId).FirstOrDefaultAsync();
            return shoppingCart;

        }


        public  async Task<ShoppingCart> GetCart(string userId)
        {
            var cart = await _db.ShoppingCarts.FirstOrDefaultAsync(x => x.UserId == userId);
            return cart;

        }

        public async Task<int> GetCartItemCount(string userId="")
        {
            //no userId
            if(string.IsNullOrEmpty(userId))
            {
                userId = GetUserId();
            }
            var data = await (from cart in _db.ShoppingCarts
                              join cartDetail in _db.CartDetails
                              on cart.Id equals cartDetail.ShoppingCartId // changed
                              where cart.UserId == userId
                              select new { cartDetail.Id }
                              ).ToListAsync();
            return data.Count;
        }

        public async Task<bool> DoCheckout(CheckoutModel model)
        {
            using var transaction = _db.Database.BeginTransaction();
            try
            {
                //order orderdetail
                //remove data cartdetail
                var userId = GetUserId();
                if(string.IsNullOrEmpty(userId) )               
                    throw new Exception("User is not logged");
                    var cart = await GetCart(userId);
                    if(cart is null)
                    throw new Exception("Invalid Cart");
                    var cartDetail = _db.CartDetails
                                    .Where(a=> a.ShoppingCartId == cart.Id).ToList();
                if(cartDetail.Count == 0)
                    throw new Exception("Cart is empty");
                var pendingRecord = _db.OrderStatuses.FirstOrDefault
                                    (s => s.StatusName == "Pending");
                if (pendingRecord is null)
                    throw new Exception("Order status does not have Pending status");

                var order = new Order
                {
                    UserId = userId,
                    CreateDate = DateTime.Now,
                    Name = model.Name,
                    Email = model.EmailAddress,
                    Number = model.Number,
                    PaymentMethod = model.PaymentMethod,
                    Address = model.Address,
                    IsPaid = false,
                    OrderStatusId = pendingRecord.Id, //pending order
                };
                _db.Orders.Add(order);
                _db.SaveChanges();
                foreach(var item in cartDetail)
                {
                    var orderDetail = new OrderDetail
                    {
                        GameId = item.GameId,
                        OrderId = order.Id,
                        Quantity = item.Quantity,
                        UnitPrice = item.UnitPrice
                    };
                    _db.OrderDetails.Add(orderDetail);
                }
                _db.SaveChanges();
                //remove cartdetails
                _db.CartDetails.RemoveRange(cartDetail);
                transaction.Commit();
                return true;
                
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        private string GetUserId()
        {
            var principal = _httpContextAccessor.HttpContext.User;
            string userId = _userManager.GetUserId(principal);
            return userId;
        }



    }
}
