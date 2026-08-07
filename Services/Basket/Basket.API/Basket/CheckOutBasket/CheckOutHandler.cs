using Basket.API.Data.Repositories;
using Basket.API.Dto;
using BuildingBlocks.CQRS;
using BuildingBlocksMessageing.Events;
using FluentValidation;
using Mapster;
using MassTransit;

namespace Basket.API.Basket.CheckOutBasket;

public record CheckOutBasketCommand(BasketDto BasketDto) : ICommand<CheckOutBasketResult>;

public class CheckOutBasketCommandValidator : AbstractValidator<CheckOutBasketCommand>
{
    public CheckOutBasketCommandValidator()
    {
        RuleFor(x => x.BasketDto).NotNull().WithMessage("BasketDto is required");
        RuleFor(x => x.BasketDto.UserName).NotEmpty().WithMessage("UserName is required");
    }
}

public record CheckOutBasketResult(bool Success);

public class CheckOutHandler(IBasketRepository basketRepository, IPublishEndpoint publishEndpoint) : ICommandHandler<CheckOutBasketCommand, CheckOutBasketResult>
{
    public async Task<CheckOutBasketResult> Handle(CheckOutBasketCommand request, CancellationToken cancellationToken)
    {
        var basket = await basketRepository.GetShoppingCartAsync(request.BasketDto.UserName);
        if (basket == null)
            return new CheckOutBasketResult(false);
        var eventMessage = request.BasketDto.Adapt<BasketCheckOutEvent>();
        eventMessage.TotalPrice = basket.TotalPrice;
        await publishEndpoint.Publish(eventMessage, cancellationToken);
        await basketRepository.DeleteShoppingCartAsync(request.BasketDto.UserName, cancellationToken);
        return new CheckOutBasketResult(true);
    }
}