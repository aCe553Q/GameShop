namespace GameShop.Repositories
{
    public interface ICartRepository
    {
        Task<int> AddItem(int gameId, int quantity);
        Task<int> RemoveItem(int gameId);

        Task<ShoppingCart> GetUserCart();

        Task<int> GetCartItemCount(string userId = "");

        Task<ShoppingCart> GetCart(string userId);

    }
}
