using System.Text.Json;

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

app.UseHttpsRedirection();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
{
    var forecast =  Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast")
.WithOpenApi();

app.MapPost("/CustomerEcho", (Customer cust) =>
{
    return Results.Json(new
    {
        Message = $"Customer {cust.name} is on address {cust.address}."
    });
});

app.MapPost("/Customer", async (HttpRequest request) =>
{
    try
    {
        using var reader = new StreamReader(request.Body);
        var body = await reader.ReadToEndAsync();

        // Optional: parse JSON manually
        var jsonDoc = JsonDocument.Parse(body);
        if(jsonDoc.RootElement.TryGetProperty("id", out JsonElement idElement))
        {
            var id = idElement.GetString();
            var cust = new Customer(id);
            if(jsonDoc.RootElement.TryGetProperty("name", out JsonElement nameElement))
                cust.name = nameElement.GetString();
            if(jsonDoc.RootElement.TryGetProperty("address", out JsonElement addressElement))
                cust.address = addressElement.GetString();
            if(jsonDoc.RootElement.TryGetProperty("headquarters", out JsonElement headquartersElement))
                cust.headquarters = headquartersElement.GetString();
            if(jsonDoc.RootElement.TryGetProperty("country", out JsonElement countryElement))
                cust.country = countryElement.GetString();
            if(jsonDoc.RootElement.TryGetProperty("telephone_1", out JsonElement telephone_1Element))
                cust.telephone_1 = telephone_1Element.GetString();
            if(jsonDoc.RootElement.TryGetProperty("email_1", out JsonElement email_1Element))
                cust.email_1 = email_1Element.GetString();
            if(jsonDoc.RootElement.TryGetProperty("cre_date", out JsonElement cre_dateElement))
                cust.cre_date = cre_dateElement.GetString();
            if(jsonDoc.RootElement.TryGetProperty("cha_date", out JsonElement cha_dateElement))
                cust.cha_date = cha_dateElement.GetString();

            return Results.Ok(new {UpdatedId = cust.id});
        }
        else
        {
            return Results.BadRequest(new {Msg = "Request has to have ID"});            
        }
    }
    catch (Exception ex)
    {
        return Results.BadRequest(new { Error = ex.Message });
    }
});

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
