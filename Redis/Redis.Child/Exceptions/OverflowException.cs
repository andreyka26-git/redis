using System;

namespace Redis.Child.Exceptions
{
    public class ChildOverflowException : Exception
    {
        public ChildOverflowException() : base("Child is overflown")
        {
            
        }
        public ChildOverflowException(string message) : base(message)
        {

        }

        public ChildOverflowException(string message, Exception inner) : base(message, inner)
        {

        }
    }
}
