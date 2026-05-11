using Carter;
using Mapster;
using MediatR;

namespace Basket.API.Basket.DeleteBasket
{
    //public record DeleteBasketRequest(string UserName);
    public record DeleteBasketResponse(string UserName);

    public class DeleteBasketApi : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapDelete("/api/basket/{userName}", async (string userName, ISender sender) =>
             {
                 var result = await sender.Send(new DeleteBasketCommand(userName));
                 var response = result.Adapt<DeleteBasketResponse>();
                 if (result.Success)
                 {
                     return Results.Ok(response.UserName);
                 }
                 else
                 {
                     return Results.BadRequest("Failed to Delete the basket.");
                 }
             }).WithDescription("EndPoint For Delete Basket Items with User Name")
               .WithName("DeleteBasket")
               .Produces<DeleteBasketResult>(StatusCodes.Status200OK)
               .ProducesProblem(StatusCodes.Status400BadRequest);
        }
    }
}