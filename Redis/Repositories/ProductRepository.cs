using System.Data;
using Dapper;
using Oracle.ManagedDataAccess.Client;
using StackExchange.Redis;
using Redis.Models;
using System.Text.Json;

namespace Redis.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly string _oracleConnectionString;
        private readonly IDatabase _redisDb;
        private const string CacheKey = "products_cache";

        public ProductRepository(string oracleConnectionString, string redisConnectionString)
        {
            _oracleConnectionString = oracleConnectionString;
            var redis = ConnectionMultiplexer.Connect(redisConnectionString);
            _redisDb = redis.GetDatabase();
        }

        // GET
        public async Task<IEnumerable<Product>> GetProductsAsync()
        {
            var cachedData = await _redisDb.StringGetAsync(CacheKey);

            if (!cachedData.IsNullOrEmpty)
            {
                // Lấy từ Redis
                return JsonSerializer.Deserialize<List<Product>>(cachedData);
            }

            // Redis trống → query DB
            using var connection = new OracleConnection(_oracleConnectionString);
            var sql = "SELECT Id, Name FROM Products";
            var products = (await connection.QueryAsync<Product>(sql)).ToList();

            // Lưu vào Redis với TTL 60s
            var jsonData = JsonSerializer.Serialize(products);
            await _redisDb.StringSetAsync(CacheKey, jsonData, TimeSpan.FromSeconds(60));

            return products;
        }

        // POST
        public async Task AddProductAsync(Product product)
        {
            using var connection = new OracleConnection(_oracleConnectionString);
            var sql = "INSERT INTO Products (Name) VALUES (:Name)";
            await connection.ExecuteAsync(sql, new { product.Name });

            // Lấy danh sách hiện tại từ Redis
            var cacheData = await _redisDb.StringGetAsync(CacheKey);
            List<Product> products;

            if (!cacheData.IsNullOrEmpty)
            {
                products = JsonSerializer.Deserialize<List<Product>>(cacheData) ?? new List<Product>();
            }
            else
            {
                products = new List<Product>();
            }

            // Thêm product mới
            products.Add(product);

            // Cache lại danh sách mới
            var jsonData = JsonSerializer.Serialize(products);
            await _redisDb.StringSetAsync(CacheKey, jsonData, TimeSpan.FromSeconds(60));
        }
    }

}
