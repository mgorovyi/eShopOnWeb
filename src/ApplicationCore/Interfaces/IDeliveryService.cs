using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DeliveryCore.Models;

namespace Microsoft.eShopWeb.ApplicationCore.Interfaces;

public interface IDeliveryService
{
    Task SetOrderAsync(int orderId);
}
