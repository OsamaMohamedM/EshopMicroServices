using BuildingBlocks.Exceptions;

namespace Catalog.Api.Exception
{
    public class ProductNotFountException : NotFoundException
    {
        public ProductNotFountException(Guid Id) : base($"Product not found and its id is {Id}")
        {
        }
    }
}