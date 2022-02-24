#Module 7

1. Create order
2. Send order data message to ServiceBus Queue 
    2.1 If sending message failed send order details by email using Logic App
3. Upload order details as file into Storage Blob while processing queue
	3.1 If uploading failed send order details by email using Logic App

# Retry
https://docs.microsoft.com/en-us/azure/architecture/best-practices/retry-service-specific
https://docs.microsoft.com/en-us/azure/service-bus-messaging/service-bus-to-event-grid-integration-example
https://docs.microsoft.com/en-us/dotnet/api/overview/azure/service-bus?view=azure-dotnet