using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Get API key from configuration
var apiKey = app.Configuration["AppSettings:ApiKey"];

if (string.IsNullOrWhiteSpace(apiKey))
{
    throw new InvalidOperationException("API key is not configured.");
}

var connectionString = app.Configuration["AppSettings:ConnectionString"];

if (string.IsNullOrWhiteSpace(connectionString))
{
    throw new InvalidOperationException("Connection string is not configured.");
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// API key middleware
app.Use(async (context, next) =>
{
    // Allow Swagger without API key in Development
    if (app.Environment.IsDevelopment() &&
        context.Request.Path.StartsWithSegments("/swagger"))
    {
        await next();
        return;
    }

    // Read API key from ApiKey header
    if (!context.Request.Headers.TryGetValue("ApiKey", out var providedApiKey))
    {
        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
        await context.Response.WriteAsJsonAsync(new
        {
            Error = "API key is required."
        });
        return;
    }

    // Compare API keys
    if (providedApiKey != apiKey)
    {
        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
        await context.Response.WriteAsJsonAsync(new
        {
            Error = "Invalid API key."
        });
        return;
    }

    await next();
});


app.MapPost("/Customer", async (HttpRequest request) =>
{
    try
    {
        using var reader = new StreamReader(request.Body);
        var body = await reader.ReadToEndAsync();

        // Optional: parse JSON manually
        var jsonDoc = JsonDocument.Parse(body);

        if(jsonDoc.RootElement.ValueKind == JsonValueKind.Array)
        {
            foreach (JsonElement el in jsonDoc.RootElement.EnumerateArray())
            {
                var cust = new Customer();
                cust.FillFromJsonElement(el);
                try
                {
                    var result = await cust.SendToDb(connectionString);
                }
                catch (Exception)
                {
                    return Results.Problem(
                        "An error occurred while updating the database.", statusCode:StatusCodes.Status500InternalServerError);
                }                
            }
            return Results.Ok(new
            {
                Id = "0",
                Result = "Success"
            });
        }
        else if(jsonDoc.RootElement.TryGetProperty("id", out JsonElement idElement))
        {
            var cust = new Customer();
            cust.FillFromJsonElement(jsonDoc.RootElement);

            try
            {
                var result = await cust.SendToDb(connectionString);

                return Results.Ok(new
                {
                    Id = result.Id,
                    Result = result.Result
                });
            }
            catch (Exception)
            {
                return Results.Problem(
                    "An error occurred while updating the database.", statusCode:StatusCodes.Status500InternalServerError);
            }

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
