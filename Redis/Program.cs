using Redis.Repositories;

namespace Redis
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Cấu hình DI cho ProductRepository
            builder.Services.AddSingleton<IProductRepository>(sp =>
            {
                return new ProductRepository(
                    // Oracle connection string
                    "User Id=algodatafeed;Password=UATUser2022;Data Source=(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST=172.16.255.11)(PORT=1521))(CONNECT_DATA=(SERVICE_NAME=ALGOUATDB)))",
                    // Redis connection
                    "localhost:6379"
                );
            });

            builder.Services.AddControllers();

            var app = builder.Build();

            app.MapControllers();

            app.Run();
        }
    }
}