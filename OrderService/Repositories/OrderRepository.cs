using Microsoft.EntityFrameworkCore;
using OrderService.Infrastructure.Database;
using OrderService.Models.v1.DomainModels;
using OrderService.Models.v1.DTOs;

namespace OrderService.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly DatabaseContext _databaseContext;

        public OrderRepository(DatabaseContext databaseContext)
        {
            _databaseContext = databaseContext;
        }

        public async Task<Order?> GetAsync(Guid orderId)
        {
            return await _databaseContext.Orders.FindAsync(orderId);
        }

        public async Task<Order?> GetAsync(Guid userId, Guid orderId)
        {
            return await _databaseContext.Orders
                .Include(order => order.OrderDetails)
                .ThenInclude(detail => detail.Product)
                .FirstOrDefaultAsync(order => order.UserId == userId && order.Id == orderId);
        }

        public async Task<(List<OrderDto> Orders, int TotalRecords)> GetOrdersWithPaginationAsync(Guid userId, string? orderName, int pageNumber, int pageSize)
        {
            var query = _databaseContext.Orders
                .AsNoTracking()
                .Include(order => order.OrderDetails)
                .ThenInclude(detail => detail.Product)
                .Where(order => order.UserId == userId && (string.IsNullOrEmpty(orderName) || order.Name.StartsWith(orderName)));

            var totalRecords = await query.CountAsync();

            var orders = await query
                .OrderBy(x => x.Name)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(order => new OrderDto()
                {
                    Id = order.Id,
                    UserId = order.UserId,
                    Name = order.Name,
                    Status = order.Status,
                    TotalAmount = order.TotalAmount,
                    CreatedAtUtc = order.CreatedAtUtc,
                    UpdatedAtUtc = order.UpdatedAtUtc,
                    OrderDetails = order.OrderDetails.Select(d => new OrderDetailDto()
                    {
                        ProductId = d.ProductId,
                        ProductName = d.Product.Name,
                        Quantity = d.Quantity,
                        UnitPrice = d.UnitPrice
                    }).ToList()
                })
                .ToListAsync();

            return (orders, totalRecords);
        }

        public async Task<OrderDto?> GetOrderDtoAsync(Guid userId, Guid orderId)
        {
            return await _databaseContext.Orders
                .Include(order => order.OrderDetails)
                .ThenInclude(detail => detail.Product)
                .Where(order => order.UserId == userId && order.Id == orderId)
                .Select(order => new OrderDto
                {
                    Id = order.Id,
                    UserId = order.UserId,
                    Name = order.Name,
                    Status = order.Status,
                    TotalAmount = order.TotalAmount,
                    CreatedAtUtc = order.CreatedAtUtc,
                    UpdatedAtUtc = order.UpdatedAtUtc,
                    OrderDetails = order.OrderDetails.Select(d => new OrderDetailDto()
                    {
                        ProductId = d.ProductId,
                        ProductName = d.Product.Name,
                        Quantity = d.Quantity,
                        UnitPrice = d.UnitPrice
                    }).ToList()
                })
                .FirstOrDefaultAsync();
        }

        public async Task<List<OrderDto>> GetOrdersAsync(Guid userId, string? orderName, int pageNumber, int pageSize)
        {
            var query = _databaseContext.Orders
                .AsNoTracking()
                .Include(order => order.OrderDetails)
                .ThenInclude(detail => detail.Product)
                .Where(order => order.UserId == userId && (string.IsNullOrEmpty(orderName) || order.Name.StartsWith(orderName)))
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(order => new OrderDto()
                {
                    Id = order.Id,
                    UserId = order.UserId,
                    Name = order.Name,
                    Status = order.Status,
                    TotalAmount = order.TotalAmount,
                    CreatedAtUtc = order.CreatedAtUtc,
                    UpdatedAtUtc = order.UpdatedAtUtc,
                    OrderDetails = order.OrderDetails.Select(d => new OrderDetailDto()
                    {
                        ProductId = d.ProductId,
                        ProductName = d.Product.Name,
                        Quantity = d.Quantity,
                        UnitPrice = d.UnitPrice
                    }).ToList()
                });

            return await query.ToListAsync();
        }

        public void Insert(Order order)
        {
            _databaseContext.Orders.Add(order);
        }

        public void Update(Order order)
        {
            _databaseContext.Orders.Update(order);
        }

        public async Task SaveChangeAsync()
        {
            await _databaseContext.SaveChangesAsync();
        }
    }
}
