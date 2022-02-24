using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace DeliveryCore.Models;


public record Product
{
    [JsonPropertyName("productId")]
    public string ProductId { get; set; } = string.Empty;

    [JsonPropertyName("quantity")]
    public int Quantity { get; set; } = 0;
}


public record DeliveryOrder
{

    [JsonPropertyName("orderId")]
    public int OrderId { get; init; } = 0;

   
    [JsonPropertyName("street")]
    public string Street { get; init; } = string.Empty;
    
    [JsonPropertyName("city")]
    public string City { get; init; } = string.Empty;
    
    [JsonPropertyName("state")]
    public string State { get; init; } = string.Empty;
   
    [JsonPropertyName("country")]
    public string Country { get; init; } = string.Empty;
    
    [JsonPropertyName("zipCode")]
    public string ZipCode { get; init; } = string.Empty;
    
    [JsonPropertyName("total")]
    public decimal Total { get; init; } = 0;

    public List<Product> Products { get; init; } = new List<Product>();

}

