using Microsoft.AspNetCore.Mvc;
using Redis.Models;
using Redis.Repositories;

namespace Redis.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly IProductRepository _productRepository;

        public ProductsController(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var products = await _productRepository.GetProductsAsync();
            return Ok(products);
        }

        [HttpPost]
        public async Task<IActionResult> Create(Product product)
        {
            await _productRepository.AddProductAsync(product);
            return Ok(new { message = "Product created successfully" });
        }
    }
}
