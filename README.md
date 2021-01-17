# redis

It's a simple, light-weight and basic implementation of distributed cache like Redis with replication and partitioning.

You can run by docker-compose or manually using .net core CLI. Both master and child allow swagger endpoing

To run application go to root directory and use
```
docker-compose up
```

To run functional tests go to root directory and use

```
docker-compose -f docker-compose-test.yml up -d
```
In order to check test results from functional tests you can use 
```
docker logs
```
for functional-test container.
