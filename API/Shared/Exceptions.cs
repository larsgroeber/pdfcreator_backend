using System;

namespace API.Shared
{
    public class Exceptions
    {
        public class WrongLoginException : UnauthorizedAccessException
        {
            public WrongLoginException()
            {
            }

            public WrongLoginException(string message)
                : base(message)
            {
            }

            public WrongLoginException(string message, Exception inner)
                : base(message, inner)
            {
            }
        }
        
        public class LoginAlreadyExistsException : Exception
        {
            public LoginAlreadyExistsException()
            {
            }

            public LoginAlreadyExistsException(string message)
                : base(message)
            {
            }

            public LoginAlreadyExistsException(string message, Exception inner)
                : base(message, inner)
            {
            }
        }
        
        public class LatexException : Exception
        {
            public LatexException()
            {
            }

            public LatexException(string message)
                : base(message)
            {
            }

            public LatexException(string message, Exception inner)
                : base(message, inner)
            {
            }
        }
    }
}