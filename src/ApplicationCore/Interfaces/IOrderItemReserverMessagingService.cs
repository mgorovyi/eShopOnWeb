﻿using System.Collections.Generic;
using System.Threading.Tasks;

using DeliveryCore.Models;

namespace Microsoft.eShopWeb.ApplicationCore.Interfaces;

public interface IOrderItemReserverMessagingService
{
    Task SendOrderCompleteMessageAsync(IList<Product> products);
}
