## Endpoints
Each endpoint is located in its own module folder this allows for good separation of concerns as well as employing the [Mediator Pattern](https://en.wikipedia.org/wiki/Mediator_pattern) provided by the [Wolverine Message Bus](https://wolverine.netlify.app/tutorials/mediator.html).

### Handlers
A handler class is used to process an incoming command for example the Login endpoint in the Authentication folder is expecting a LoginRequest command and this is done using Wolverine's built in automatic handler discovery to find the candidate message handler methods and correlate them by message type.  Wolverine then builds some connective code at runtime using runtime message processing to relay the messages passed into `IMessageBus` to the right message handler methods.
