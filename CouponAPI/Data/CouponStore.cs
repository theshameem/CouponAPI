using CouponAPI.Models;

namespace CouponAPI.Data
{
	public static class CouponStore
	{
		public static List<Coupon> CouponList =
		[
			new Coupon { Id = 1, Name = "33FF", Percent = 10, IsActive = true },
			new Coupon { Id = 2, Name = "21FF", Percent = 5, IsActive = true }
		];
	}
}
