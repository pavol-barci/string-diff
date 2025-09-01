# Stringdiff

This repository contains code for interview. The solution is a simple web api with a few endpoints: 

- PUT diff/{version}/{id}/left -> create/update resource with left string to compare
- PUT diff/{version}/{id}/right -> create/update resource with right string to compare
- GET diff/{version}/{id} -> get the current result

The solution will accept left and right strings in format 

```json
{"input":"some value to be compared"}
```

and then calculates a simple difference between those two. 

The GET endpoint the retrieve results:

- NotFinished => only one side provided
- DifferentSize => if strings are not same size
- Equals => if both strings are equal
- NotEqual => when there is a difference between strings. 
    - Offset => at which point the difference occurs
    - Length => lenght of the different string

The architecture is example of onion archiecture with 
- Domain/Contracts => defining models and exceptions, DTOs
- Infrastructure => database layer, repositories
- Application and Application.Abstraction -> Services and helpers, like mapper of objects. Abstraction contains only interfaces
- StringDiff => basically presentation layer, with API controllers, middlewares etc

### How to run

The solution is writen in .NET 8.0 using rider. There is a single build configuration ```StringDiff``` which will run the solution locally. In development mode the persistance layer will be in memory database.

When running in another modes some database could be added but it is not part of this solution.

### Tests

There are 2 test projects
- ```StringDiff.Tests``` - unit tests
- ```StringDiff.IntegrationTests``` - integration tests 

To test the solution also sample ```StringDiff.http``` file can be used to call endpoints.

#### StringDiff.Tests

Unit tests can be run inside IDE 

#### StringDiff.IntegrationTests

Integration tests can be run also inside IDE but the API needs to be running locally usin the ```StringDiff``` build configuration


## Limitations

Current solution have a few limitations. 

- There is no authentication or authorization
    - I went with a simple solution but at least authentication would be nice to have
- Persistance is only in memory
    - again instead of integrating to real database for this example I used only in memory
- Concurrency, when left/right at the same time may cause issues and overwrites
    - IMPROVEMENT -> maybe some locking mechanism would be nice to have
- Currently when left is called same multiple times it always update databse
    - IMPROVEMENT -> when the input is same, do not update database
- Diff is calculated during left/right request, so long strings might block and take a long time
    - IMPROVEMENT -> just store the string and fire an event, or do a background job to periodically calculates the diffs.
- No request size validation, large strings might cause high data and cpu usage or even fail
    - IMPROVEMENT -> validate the size of the input string
- No caching
    - IMPROVEMENT -> cache the result so it will not always request database once calculated
