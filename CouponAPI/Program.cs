using CouponAPI.Data;
using CouponAPI.DTO.RequestModel;
using CouponAPI.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
	options.UseSqlServer(builder.Configuration.GetConnectionString("CouponAPIConnectionString"))
);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

#region Coupon APIs

// CREATE a coupon
app.MapPost("/api/coupons", async ([FromBody] CouponCreateDto request, ApplicationDbContext context) =>
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
		Name = request.Name,
		Percent = request.Percent,
		IsActive = request.IsActive,
		CreatedOn = DateTime.Now,
		LastUpdated = DateTime.Now
	};

	//CouponStore.CouponList.Add(coupon);
	context.Coupons.Add(coupon);
	await context.SaveChangesAsync();

	return Results.Created($"/api/coupon/{coupon.Id}", coupon);
}).WithName("CreateCoupon").Accepts<CouponCreateDto>("application/json").Produces<Coupon>(201).Produces(400);

// GET the list of coupons
app.MapGet("/api/coupons", async (ILogger<Program> logger, ApplicationDbContext context) =>
{
	//var result = CouponStore.CouponList;
	var result = await context.Coupons.ToListAsync();

	logger.LogInformation("List of coupons are ready for the client");

	return Results.Ok(result);
}).WithName("GetCoupons").Produces(200);

// GET coupon by ID
app.MapGet("/api/coupons/{id:Guid}", (Guid id) =>
{
	var coupon = CouponStore.CouponList.FirstOrDefault(u => u.Id == id);

	return coupon == null ? Results.NotFound("No coupon found with the provided id " + id) : Results.Ok(coupon);
});

// Update coupon by id
app.MapPut("/api/coupons", (CouponUpdateDto request) =>
{
	var index = CouponStore.CouponList.FindIndex(u => u.Id == request.Id);

	Console.WriteLine("Index: {0}", index);

	if (index == -1)
	{
		return Results.BadRequest("No coupon found with the provided id");
	}

	var coupon = new Coupon
	{
		Name = request.Name,
		IsActive = request.IsActive,
		Percent = request.Percent,
		LastUpdated = DateTime.Now
	};

	CouponStore.CouponList[index] = coupon;

	return Results.Ok("Coupon updated successfully");
}).Produces<BadRequest>(400).Produces<Ok>();

// Delete coupon by id
app.MapDelete("/api/coupons/{id:Guid}", (Guid id) =>
{
	var coupon = CouponStore.CouponList.FirstOrDefault(u => u.Id == id);

	if (coupon == null) return Results.BadRequest("No coupon found with the provided Id");
	
	CouponStore.CouponList.Remove(coupon);

	return Results.Ok("Coupon deleted.");

});

#endregion

app.UseHttpsRedirection();

app.Run();

