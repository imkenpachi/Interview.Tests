﻿namespace OrderService.Models.v1.DTOs
{
    public class ProductDto
    {
        public Guid Id { get; set; }
        public required string Name { get; set; }
        public string? Description { get; set; }
        public decimal? Price { get; set; }
    }
}
