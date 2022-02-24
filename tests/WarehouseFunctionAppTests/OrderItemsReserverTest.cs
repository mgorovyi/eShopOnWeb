using Xunit;
using Moq;
using Azure.Storage.Blobs;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.AspNetCore.Http;
using System.Text.Json.Serialization;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.IO;
using System;
using Azure.Storage.Blobs.Models;
using System.Collections.Generic;
using Azure;
using Azure.Storage;
using System.Threading;


namespace OrderItemReserverMykola;


public class OrderItemsReserverTest
{

    Mock<BlobClient> mockBlobClient = new Mock<BlobClient>();
    Mock<BlobContainerClient> mockBlobContainerClient = new Mock<BlobContainerClient>();

    public OrderItemsReserverTest()
    {

    }

    [Fact]
    public void SuccessTest()
    {
        var logger = NullLoggerFactory.Instance.CreateLogger("Null logger");
        var request = new DefaultHttpRequest(new DefaultHttpContext());
        dynamic data = new { id = 1, quantity = 100 };
        var json = System.Text.Json.JsonSerializer.Serialize(data);
        byte[] buffer = Encoding.UTF8.GetBytes(json);

        using (var ms = new MemoryStream(buffer))
        {
            request.Body = ms;
            request.ContentLength = ms.Length;

            var response = OrderItemReserverMykola.OrderItemsReserver.Run(request, GetBlobContainerClientMock(), logger);
            response.Wait();

            Assert.IsAssignableFrom<OkObjectResult>(response.Result);
            mockBlobContainerClient.Verify(c => c.GetBlobClient(It.IsAny<string>()), Times.Once);
            mockBlobClient.Verify(c => c.UploadAsync(It.IsAny<Stream>()), Times.Once);
        }
    }

    public BlobContainerClient GetBlobContainerClientMock()
    {
        // Create a mock response
        var mockResponse = new Mock<Response>();
        // Create a mock value
        var mockValue = BlobsModelFactory.BlobContentInfo(
            It.IsAny<ETag>(),
            It.IsAny<DateTimeOffset>(), 
            It.IsAny<byte[]>(),
            It.IsAny<string>(), 
            It.IsAny<string>(), 
            It.IsAny<string>(), 
            It.IsAny<long>());


        mockBlobClient
            .Setup(c => c.UploadAsync(It.IsAny<Stream>()))
           .Returns(
                Task.FromResult(
                    Response.FromValue(mockValue, mockResponse.Object)
               ));

        mockBlobContainerClient
            .Setup(c => c.GetBlobClient(It.IsAny<string>()))
            .Returns(mockBlobClient.Object)
            .Verifiable();

        return mockBlobContainerClient.Object;
    }
}
