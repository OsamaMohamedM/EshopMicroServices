using BuildingBlocks.Exceptions;

namespace Basket.API.Exceptions
{
    public class BasketNotFound : NotFoundException
    {
        public BasketNotFound(string userName) : base($"Basket Not found for {userName}")
        {
        }
    }
}