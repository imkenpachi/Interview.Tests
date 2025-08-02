using OrderService.Infrastructure.Database;
using OrderService.Models.v1.DomainModels;

namespace OrderService.Infrastructure
{
    public static class SeedData
    {
        private static readonly Guid DEFAULT_USER_ID = Guid.Parse("5a2fb4b2-f281-4c64-8bbc-ec3500b40c76");

        public static void Seed(DatabaseContext context)
        {
            var productScenario1 = GetPreConfiguredProductScenario1();
            var productScenario2 = GetPreConfiguredProductScenario2();

            if (!context.Products.Any())
            {
                context.Products.Add(productScenario1);
                context.Products.Add(productScenario2);
                context.SaveChanges();
            }

            if (!context.Orders.Any())
            {
                var orders = Enumerable.Range(0, 100).SelectMany(_ => new[]
                {
                    GetPreConfiguredOrder(productScenario1),
                    GetPreConfiguredOrder(productScenario2)
                }).ToList();

                context.Orders.AddRange(orders);
                context.SaveChanges();
            }
        }

        private static Product GetPreConfiguredProductScenario1()
        {
            return new()
            {
                Id = Guid.NewGuid(),
                Name = "Product1",
                Description = "Product for producing Payment Successful scenario",
                UnitPrice = 100.99m,
            };
        }
        private static Product GetPreConfiguredProductScenario2()
        {
            return new()
            {
                Id = Guid.NewGuid(),
                Name = "Product2",
                Description = "Product for producing Payment Failed scenario",
                UnitPrice = 50.11m,
            };
        }

        private static Order GetPreConfiguredOrder(Product product)
        {
            decimal quantity = 1m;
            return new()
            {
                Id = Guid.NewGuid(),
                
                Status = Models.v1.Enums.OrderStatus.Created,
                TotalAmount = product.UnitPrice * quantity,
                UserId = DEFAULT_USER_ID,
                CreatedBy = DEFAULT_USER_ID,
                OrderDetails = new List<OrderDetail>() {
                        new()
                        {
                            ProductId = product.Id,
                            Quantity = 1m,
                            UnitPrice = product.UnitPrice,
                            CreatedBy = DEFAULT_USER_ID,
                        }
                    }
            };
        }
    }
}
