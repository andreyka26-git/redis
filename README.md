# redis

It's a simple, light-weight and basic implementation of distributed cache like Redis.
Currently it allows only deploy-time partitioning, so functionality is poor.

You can run by docker-compose or manually using .net core CLI. Both master and child allow swagger endpoing

TODO:

add authentication (at least some token in header)

add error handling

add replication (for availability)

add rebalancing of the partitions

add load test
