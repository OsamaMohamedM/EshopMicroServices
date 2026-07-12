namespace OrderingDomain.Exceptions
{
    public class OrderDomainException : Exception
    {
        public OrderDomainException(string message) : base($"Domain Exception message : {message}")
        {
        }
    }
}