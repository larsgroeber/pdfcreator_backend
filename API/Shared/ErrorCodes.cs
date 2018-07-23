using System;

namespace API.Shared
{
    public class ErrorCodes
    {
        public static string GetErrorCode(Exception exception)
        {
            if (exception is Exceptions.LoginAlreadyExistsException)
            {
                return "1000";
            }

            if (exception is Exceptions.WrongLoginException)
            {
                return "1010";
            }
            
            if (exception is UnauthorizedAccessException)
            {
                return "1020";
            }

            if (exception is Exceptions.LatexException)
            {
                return "2000";
            }
            
            if (exception is Exceptions.NotAZipFileException)
            {
                return "2010";
            }

            return "0000";
        }
    }
}