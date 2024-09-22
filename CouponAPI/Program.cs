using CouponAPI.Data;
using CouponAPI.Models;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

#region Coupon APIs

// CREATE a coupon
app.MapPost("/api/coupons", ([FromBody] Coupon coupon) =>
{
	if (coupon.Id != 0 || string.IsNullOrEmpty(coupon.Name))
	{
		Results.BadRequest("Invalid Id or Coupon Name");
	}

	if (CouponStore.CouponList.FirstOrDefault(u =>
		    string.Equals(u.Name, coupon.Name, StringComparison.CurrentCultureIgnoreCase)) != null)
	{
		return Results.BadRequest("Coupon name already exists");
	}

	coupon.Id = CouponStore.CouponList.Max(u => u.Id) + 1;

	CouponStore.CouponList.Add(coupon);

	return Results.Created($"/api/coupon/{coupon.Id}", coupon);
}).WithName("CreateCoupon").Produces<Coupon>(201).Produces(400);

// GET the list of coupons
app.MapGet("/api/coupons", () =>
{
	var result = CouponStore.CouponList;

	return Results.Ok(result);
}).WithName("GetCoupons");

// GET coupon by ID
app.MapGet("/api/coupons/{id:int}", (int id) =>
{
	var coupon = CouponStore.CouponList.FirstOrDefault(u => u.Id == id);

	return coupon == null ? Results.NotFound("No coupon found with the provided id " + id) : Results.Ok(coupon);
}).WithName("GetCoupon");

#endregion

app.UseHttpsRedirection();

app.Run();

