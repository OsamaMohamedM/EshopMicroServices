using Discount.Grpc.Data;
using Discount.Grpc.Models;
using Discount.Grpc.Protos;
using Grpc.Core;
using Microsoft.EntityFrameworkCore;
using Mapster;

namespace Discount.Grpc.Services
{
    public class DiscountService(DiscountContext db, ILogger<DiscountService> logger) :
        DiscountProtoService.DiscountProtoServiceBase
    {
        public override async Task<GetDiscountResponse> GetDiscount(GetDiscountRequest request, ServerCallContext context)
        {
            var coupon = await db.Coupons
                                 .AsQueryable()
                                 .FirstOrDefaultAsync(c => c.ProductName == request.ProductName);

            if (coupon == null)
            {
                logger.LogInformation("Discount for {ProductName} not found, returning default entity.", request.ProductName);
                coupon = new Coupon
                {
                    ProductName = request.ProductName,
                    Amount = 0,
                    Description = "No discount available"
                };
            }

            var response = new GetDiscountResponse
            {
                Coupon = coupon.Adapt<CouponModel>()
            };

            return response;
        }

        public override async Task<CreateDiscountResponse> CreateDiscount(CreateDiscountRequest request, ServerCallContext context)
        {
            var coupon = request.Coupon.Adapt<Coupon>();

            db.Coupons.Add(coupon);
            await db.SaveChangesAsync();

            logger.LogInformation("Discount is successfully created. ProductName: {ProductName}", coupon.ProductName);

            var response = new CreateDiscountResponse
            {
                Coupon = coupon.Adapt<CouponModel>()
            };
            return response;
        }

        public override async Task<UpdateDiscountResponse> UpdateDiscount(UpdateDiscountRequest request, ServerCallContext context)
        {
            var coupon = await db.Coupons
                                 .AsQueryable()
                                 .FirstOrDefaultAsync(c => c.Id == request.Coupon.Id);

            if (coupon == null)
            {
                throw new RpcException(new Status(StatusCode.NotFound, $"Discount with ID={request.Coupon.Id} not found."));
            }
            request.Coupon.Adapt(coupon);

            db.Coupons.Update(coupon);
            await db.SaveChangesAsync();

            logger.LogInformation("Discount is successfully updated. ProductName: {ProductName}", coupon.ProductName);

            var response = new UpdateDiscountResponse
            {
                Coupon = coupon.Adapt<CouponModel>()
            };
            return response;
        }

        public override async Task<DeleteDiscountResponse> DeleteDiscount(DeleteDiscountRequest request, ServerCallContext context)
        {
            var coupon = await db.Coupons
                                 .AsQueryable()
                                 .FirstOrDefaultAsync(c => c.Id == request.Id);

            if (coupon == null)
            {
                throw new RpcException(new Status(StatusCode.NotFound, $"Discount with Id={request.Id} not found."));
            }

            db.Coupons.Remove(coupon);
            await db.SaveChangesAsync();

            logger.LogInformation("Discount is successfully deleted. Id: {Id}", request.Id);

            var response = new DeleteDiscountResponse
            {
                Success = true
            };
            return response;
        }
    }
}