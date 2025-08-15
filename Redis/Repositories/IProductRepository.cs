using Redis.Models;


// lớp interface khai báo cho ProductRepository
namespace Redis.Repositories
{
    public interface IProductRepository
    {
        Task<IEnumerable<Product>> GetProductsAsync();
        Task AddProductAsync(Product product);
    }
}
