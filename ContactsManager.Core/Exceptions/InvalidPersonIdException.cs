namespace ContactsManager.Exceptions
{
    public class InvalidPersonIdException : Exception
    {
        public InvalidPersonIdException() : base() { }

        public InvalidPersonIdException(string? message) : base(message) { }

        public InvalidPersonIdException(string? message, Exception? innerException) : base(message, innerException) { }
    }
}
