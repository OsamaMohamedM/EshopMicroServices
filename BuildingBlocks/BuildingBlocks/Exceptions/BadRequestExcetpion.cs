namespace BuildingBlocks.Exceptions
{
    public class BadRequestException : Exception
    {
        public BadRequestException(string message) : base(message)
        {
        }

        public BadRequestException(string message, string innerException) : base(message)
        {
            Details = innerException;
        }

        public string? Details { get; }
    }
}