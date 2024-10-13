namespace CouponAPI.Models
{
	public class Coupon
	{
		public Guid Id { get; set; }
		public string Name { get; set; }
		public int  Percent { get; set; }
		public bool IsActive { get; set; }
		public DateTime? CreatedOn { get; set; }
		public DateTime? LastUpdated { get; set; }
	}
}
