using CouponAPI.Data;
using CouponAPI.DTO.RequestModel;
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
app.MapPost("/api/coupons", ([FromBody] CouponCreateDto request) =>
{
	if (string.IsNullOrEmpty(request.Name))
	{
		Results.BadRequest("Invalid Id or Coupon Name");
	}

	if (CouponStore.CouponList.FirstOrDefault(u =>
		    string.Equals(u.Name, request.Name, StringComparison.CurrentCultureIgnoreCase)) != null)
	{
		return Results.BadRequest("Coupon name already exists");
	}

	var coupon = new Coupon
	{
		Id = CouponStore.CouponList.Max(u => u.Id) + 1,
		Name = request.Name,
		Percent = request.Percent,
		IsActive = request.IsActive,
		CreatedOn = DateTime.Now,
		LastUpdated = DateTime.Now
	};

	CouponStore.CouponList.Add(coupon);

	return Results.Created($"/api/coupon/{coupon.Id}", coupon);
}).WithName("CreateCoupon").Accepts<CouponCreateDto>("application/json").Produces<Coupon>(201).Produces(400);

// GET the list of coupons
app.MapGet("/api/coupons", (ILogger<Program> logger) =>
{
	var result = CouponStore.CouponList;

	logger.LogInformation("List of coupons are ready for the client");

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

