using JokesApi.Helpers;
using JokesApi.Options;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

// could use this instead of the attributes in models but i prefer attributes
//    .AddJsonOptions(options =>
//{
//    options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
//});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddOptions<JokeOptions>()
    .BindConfiguration("Joke")
    .ValidateDataAnnotations();

builder.Services.AddDadJokeServices();

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseSwagger();
app.UseSwaggerUI();


//not needed for containerized deploy.  In this case digital ocean will handle the https redirect
//app.UseHttpsRedirection();

app.MapControllers();

app.Run();
