namespace CouponAPI.DTO.RequestModel
{
	public class CouponUpdateDto
	{
		public Guid Id { get; set; }
		public string Name { get; set; }
		public int Percent { get; set; }
		public bool IsActive { get; set; }
	}
}
