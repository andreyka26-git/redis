namespace Redis.Common.Abstractions
{
    public interface IPrimeNumberService
    {
        int GetPrime(int min);
    }
}