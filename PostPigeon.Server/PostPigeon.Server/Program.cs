using Mapster;
using MapsterMapper;
using PostPigeon.DAL;
using PostPigeon.DAL.DbRepositories;
using PostPigeon.DAL.DbRepositories.Interfaces;
using PostPigeon.Server.Mappings;
using PostPigeon.Server.Services;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddGrpc();

//Add MongoDb
builder.Services.Configure<ArsysChatDatabaseSettings>(builder.Configuration.GetSection("PostPigeonDatabase"));

//Add mapster
builder.Services.AddSingleton(GetConfiguredMappingConfig());
builder.Services.AddScoped<IMapper, ServiceMapper>();

//Add services
builder.Services.AddSingleton<IMessagesRepository, MessagesRepository>();
builder.Services.AddSingleton<IUsersRepository, UsersRepository>();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.MapGrpcService<ChatroomService>();
app.MapGet("/", () => "Test");

app.Run();



TypeAdapterConfig GetConfiguredMappingConfig()
{
    var config = new TypeAdapterConfig();
    new RegisterMapper().Register(config);
    return config;
}
