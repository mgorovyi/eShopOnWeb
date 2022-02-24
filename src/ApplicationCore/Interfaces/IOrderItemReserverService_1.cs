using System.Collections.Generic;
using System.Threading.Tasks;

namespace Microsoft.eShopWeb.ApplicationCore.Interfaces;

public interface IOrderItemReserverService
{
    Task SendItemRequestAsync(Dictionary<string,int> items);
}
