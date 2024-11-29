using DynamicFilterinApi.Database;
using DynamicFilterinApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddDbContext<ProductDbContext>(x => x.UseInMemoryDatabase("ProductDb"));

var app = builder.Build();

// Configure the HTTP request pipeline.
app.MapGet("/products", async ([FromBody]ProductSearchCriteria productSearch,ProductDbContext dbContext) =>
{
    await dbContext.Database.EnsureCreatedAsync();

    // Product para = new Product();
    ParameterExpression parameterExp = Expression.Parameter(typeof(Product), "para");

    // var predicate = true;
    Expression predicate = Expression.Constant(true);

    if(productSearch.IsActive == true)
    {   
        // memberExp = para.IsActive
        MemberExpression memberExp = Expression.Property(parameterExp, nameof(Product.IsActive));
        // bool constantExp = para.IsActive
        ConstantExpression constantExp = Expression.Constant(productSearch.IsActive.Value, typeof(bool));
        // memberExp == constantExp
        BinaryExpression binaryExp = Expression.Equal(memberExp, constantExp);
        // para => true && para.IsActive=true
        predicate = Expression.AndAlso(predicate, binaryExp);

    }

    var lambdaExp = Expression.Lambda<Func<Product, bool>>(predicate, parameterExp);

    var data = await dbContext.Products.Where(lambdaExp).ToListAsync();

    return Results.Ok(data);

});

app.UseHttpsRedirection();


app.Run();

