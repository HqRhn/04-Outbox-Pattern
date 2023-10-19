# Transactional Outbox-Pattern
**Creating a loosely coupled system using transactional outbox pattern : **

 **Create an entity and outbox record simultaneously :**
  ```
        public async Task<ActionResult<Guid>> AddNewOrder([FromBody] OrderRequest request)
        {

            using var transaction = _repository.Database.BeginTransaction();
            Order neworder = new Order()
            {
                OrderId = new Guid(),
                OrderDate = request.OrderDate,
                OrderReference = request.OrderReference
            };
            _repository.Orders.Add(neworder);
            await _repository.SaveChangesAsync();

            OrderEvent newOrderEvent = new OrderEvent()
            {
                OrderId = neworder.OrderId,
                OrderDate = neworder.OrderDate,
                OrderReference = neworder.OrderReference
            };
            OutboxMessage newOutboxMessage = new OutboxMessage()
            {
                EventName = "Order Created",
                MessageContent = JsonConvert.SerializeObject(newOrderEvent)
            };
            _repository.OutboxMessages.Add(newOutboxMessage);
            await _repository.SaveChangesAsync(); 
            
            transaction.Commit(); 
            return Ok(neworder.OrderId);
        }
```


**Read the unprocessed message and send to service bus. Then update record as processed.**

    protected async override Task ExecuteAsync(CancellationToken cancellationToken)
        {
            while(!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    IEnumerable<OutboxMessage> messages = _repository.OutboxMessages.Where(e=>e.Processed!=true).OrderBy(o=>o.Id).ToList();

                    foreach(var message in messages)
                    {
                        SendMessageAsync(message);
                        SetMessageAsProcessed(message);
                    }
                }
                catch(Exception ex)
                {
                    _logger.LogError($"Error occurred in sending message: {ex.Message}");
                }
                finally
                {
                    await Task.Delay(5000, cancellationToken);
                }
            }
        }

![Untitled](https://github.com/HqRhn/MovieCatalog/assets/141786593/49a935e0-aa10-4a47-8d04-b8f2c1fb3d38)
