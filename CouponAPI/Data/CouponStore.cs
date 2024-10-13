using CouponAPI.Models;

namespace CouponAPI.Data
{
	public static class CouponStore
	{
		public static List<Coupon> CouponList =
		[
			new Coupon { Name = "33FF", Percent = 10, IsActive = true },
			new Coupon { Name = "21FF", Percent = 5, IsActive = true }
		];
	}
}
