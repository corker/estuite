Estuite [![Build status](https://ci.appveyor.com/api/projects/status/et4nsy7i9x6eg7y5?svg=true)](https://ci.appveyor.com/project/corker/estuite)
==

An event store for Microsoft Azure.

This is an early alpha version. Feel free to join.

Why?
====

There are a number of implementations for event store that perfectly work with .NET environment:
- EventStore - https://geteventstore.com/
- NEventStore - http://neventstore.org/
- Cirqus - https://github.com/d60/Cirqus

Here you could find a lot of resources related to DDD and ES:
- https://github.com/heynickc/awesome-ddd

There is even one that implements event store on top of Azure table store:
- https://github.com/yevhen/Streamstone

Some of them are really good but was designed for on premise environments. 
Thus they can't utilize all benefits of Azure platform such as partitioning. 
Or have to run on dedicated virtual machines which is not the way to go for cloud native solutions. 
Some have been designed to run on Azure although they lack useful features.
Like idempotency check when saving events to a store which is available in NEventStore in combination with NES.

There is one more interesting source that I really look forward to borrow some nice patterns related to Azure table store:
- http://cqrsjourney.github.io/

The last topic. 
Azure table store has been designed to be simple. 
There is no support for secondary indexes. 
You can only have a partition key and a row key.
Automatic scaling based on partition key is a huge advantage of Azure Table Store.
Microsoft says that Azure Table Store is extremely cheap. This statement will be validated and results posted here.
Although a typical event store has to query streams when dehydrate aggregates as well as events when denormalizing projections.
This is a challenge that can't be implemented within a single Azure Table.
Estuite uses stream table as a single source of truth and dispatch all events from stream table to event table asynchronously.
The challenge would be to keep these two tables in sync.

Who am I?
--
My name is Michael Borisov. I'm interested in CQRS, DDD, event sourcing and micro services architecture.

If you have any questions or comments regarding to the project please feel free to contact me on [Twitter](https://twitter.com/fkem) or [LinkedIn](https://www.linkedin.com/in/michaelborisov)

Happy coding!