using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using MinimalShoppingListAPI;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<ApiDbContext>(options =>
{
    options.UseInMemoryDatabase("MinShoppingListApi");
});

var app = builder.Build();
// method to create an endpoint to create HTTP type GET
app.MapGet("/shoppinglist", async (ApiDbContext db) =>
    await db.Groceries.ToListAsync());


app.MapPost("/shoppinglist", async (ApiDbContext db, Grocery grocery) =>
{
    db.Groceries.Add(grocery);
    await db.SaveChangesAsync();

    return Results.Created($"/shoppinglist/{grocery.Id}", grocery);
});


app.MapGet("/shoppinglist/{id}", async (ApiDbContext db, int id) =>
{
    var grocery = await db.Groceries.FindAsync(id);

    if (grocery == null)
    {
        return Results.NotFound($"Grocery with the Id: {id} not found");
    }

    return Results.Ok(grocery);
});


app.MapPut("/shoppinglist/{id}", async (ApiDbContext db, int id, Grocery grocery) =>
{
    if(id != grocery.Id)
    {
        return Results.BadRequest($"Given id:{id} doesn't match with the grocery Id {grocery.Id}");
    }

    var updateGrocery = await db.Groceries.FindAsync(grocery.Id);

    if(updateGrocery == null)
    {
        return Results.NotFound($"Grocery with the Id:{id} not found");
    }

    updateGrocery.Name = grocery.Name;
    updateGrocery.Purchased = grocery.Purchased;

    db.Update(updateGrocery);
    //db.Entry(updateGrocery).State = EntityState.Modified;
    await db.SaveChangesAsync();

    return Results.Ok($"Updated Successfully!");
});


app.MapDelete("/shoppinglist/{id}", async (ApiDbContext db, int id) =>
{
    var grocery = await db.Groceries.FindAsync(id);
    
    if(grocery is null)
    {
        return Results.NotFound($"Grocery with the Id:{id} not found");
    }

    db.Groceries.Remove(grocery);
    await db.SaveChangesAsync();

    return Results.NoContent();
});


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.Run();