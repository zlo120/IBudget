namespace IBudget.Core.Exceptions
{
    public class MongoCRUDException : Exception
    {
        public MongoCRUDException(string message) : base(message)
        {
        }

        public MongoCRUDException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public MongoCRUDException()
        {
        }
    }
}
