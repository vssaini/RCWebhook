using Microsoft.Extensions.Primitives;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.MapMethods("/webhook", ["GET", "POST"], async (HttpContext context) =>
{
    context.Request.Headers.TryGetValue("Validation-Token", out var validationToken);
    if (!StringValues.IsNullOrEmpty(validationToken))
    {
        context.Response.Headers.Append("Validation-Token", validationToken);
        return Results.Ok();
    }

    // Handle normal POST event payloads
    if (context.Request.Method == "POST")
    {
        using var reader = new StreamReader(context.Request.Body, Encoding.UTF8);
        var payload = await reader.ReadToEndAsync();
        Console.WriteLine(payload);
    }

    return Results.Ok();
});

app.Run();