# Transactional Outbox-Pattern
**Creating a loosely coupled system using transactional outbox pattern**

**.NET 6 Restful API - OutboxPattern:**

 Creates an order and outbox notification record simultaneously.  

**Minimal API - OutboxPublisher:**

Acting as the background worker process intended to ,
read the unprocessed message and send to service bus. Then update record as processed.

![Untitled](https://github.com/HqRhn/MovieCatalog/assets/141786593/49a935e0-aa10-4a47-8d04-b8f2c1fb3d38)
