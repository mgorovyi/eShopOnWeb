using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Ardalis.GuardClauses;

using Microsoft.eShopWeb.ApplicationCore.Entities.OrderAggregate;
using Microsoft.eShopWeb.ApplicationCore.Exceptions;
using Microsoft.eShopWeb.ApplicationCore.Interfaces;
using Microsoft.eShopWeb.ApplicationCore.Specifications;

namespace Microsoft.eShopWeb.ApplicationCore.Services;

public class OrderQueryService : IOrderQueryService
{

    private readonly IRepository<Order> _orderRepository;
    private readonly IAppLogger<BasketService> _logger;
    public OrderQueryService(IRepository<Order> orderRepository,
        IAppLogger<BasketService> logger
        )
    {
        _orderRepository = orderRepository ?? throw new ArgumentNullException(nameof(Order));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }
    public async Task<Order> GetAsync(int orderId)
    {
        var orderSpec = new OrderWithItemsByIdSpec(orderId);
        var order = await _orderRepository.GetBySpecAsync(orderSpec);

        _ = order ?? throw new OrderNotFoundException(orderId);

        return order;
    }
}
