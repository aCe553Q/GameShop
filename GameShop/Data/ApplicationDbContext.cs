﻿using GameShop.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace GameShop.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<Game>  Games { get; set; }
        public DbSet<Genre>  Genres { get; set; }
        public DbSet<ShoppingCart>  ShoppingCarts { get; set; }
        public DbSet<CartDetail>  CartDetails { get; set; }
        public DbSet<Order>  Orders { get; set; }
        public DbSet<OrderDetail>  OrderDetails { get; set; }
        public DbSet<OrderStatus>  orderStatuses { get; set; }
    }
}
