namespace BO;
//
internal class Exceptions
{
    [Serializable]
    public class BlDoesNotExistException : Exception
    {
        public BlDoesNotExistException(string? message) : base(message) { }
        public BlDoesNotExistException(string message, Exception innerException)
        : base(message, innerException) { }
    }
    [Serializable]
    public class BlAlreadyExistsException : Exception
    {
        public BlAlreadyExistsException(string? message) : base(message) { }
        public BlAlreadyExistsException(string message, Exception innerException)
        : base(message, innerException) { }
    }
    [Serializable]
    public class BlNullPropertyException : Exception
    {
        public BlNullPropertyException(string? message) : base(message) { }
    }


    //

    [Serializable]
    public class BlDeletionImpossible : Exception
    {
        public BlDeletionImpossible(string? message) : base(message) { }
    }
    public class BlXMLFileLoadCreateException : Exception
    {
        public BlXMLFileLoadCreateException(string? message) : base(message) { }
    }
    [Serializable]
    public class BLInvalidDataException : Exception
    {
        public BLInvalidDataException(string? message) : base(message) { }
        public BLInvalidDataException(string? message, Exception innerException) : base(message, innerException) { }
    }
    [Serializable]
    public class BLGeneralException : Exception
    {
        public BLGeneralException(string? message) : base(message) { }
        public BLGeneralException(string? message, Exception innerException) : base(message, innerException) { }
    }
    [Serializable]
    public class BLUnauthorizedException : Exception
    {
        public BLUnauthorizedException(string? message) : base(message) { }
        public BLUnauthorizedException(string? message, Exception innerException) : base(message, innerException) { }
    }
}