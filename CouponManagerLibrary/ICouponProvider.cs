using System;
using System.Threading.Tasks;

namespace CouponManagerLibrary
{
    public interface ICouponProvider
    {
        Task<Coupon> Retrieve(Guid couponId);
    }
}
