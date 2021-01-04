# redis

It's a simple, light-weight and basic implementation of distributed cache like Redis.
Currently it allows only deploy-time partitioning, so functionality is poor.

You can run by docker-compose or manually using .net core CLI. Both master and child allow swagger endpoing

TODO:
    //TODO add authentication (at least some token in header)
    //TODO add prime factor to partition counter in order to get uniform distribution of hash
    //TODO add error handling
    //TODO add replication (for availability)
    //TODO add rebalancing of the partitions
    //TODO add load test
    //TODO change hash generation to platform and deploy-version independent
