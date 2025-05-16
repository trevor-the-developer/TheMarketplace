# Repository structure
Repositories generally align with endpoints and/or routes and not all data store operations will be encapsulated within the repository this should be handled 
in the service layer away from the business and view domains.  Everything is a repository to assist with Mediator pattern coverd by 
the [WolverineFx](https://github.com/JasperFx/wolverine) library.

## Roles:
- **GenericRepository** - Base class for all other repositories encapsulating common data store operations.
- **ListingRepository** - Listing related data operations.
- **CardRepository** - Card related data operations.
- **ProductRepository** - Product related data operations.
- **ProductDetailRepository** - ProductDetails related data operations.
- **DocumentRepository** - Document related data operations.
- **MediaRepository** - Media related data operations.
- **AuthenticationRepository** - Auth-Authz related data operations.
