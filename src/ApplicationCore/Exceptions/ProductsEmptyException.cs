using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.eShopWeb.ApplicationCore.Exceptions;

public class ProductsEmptyException : Exception
{
    public ProductsEmptyException() : base("Products can not be empty.")
    {

    }
}
