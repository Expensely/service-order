using Expensely.Swagger.Extensions;
using Microsoft.AspNetCore.Mvc.ApiExplorer;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddSwagger();

var app = builder.Build();
var apiVersionDescriptionProvider = app.Services.GetRequiredService<IApiVersionDescriptionProvider>();

app.UseSwagger(apiVersionDescriptionProvider);

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();