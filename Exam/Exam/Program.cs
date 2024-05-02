using Exam.Application.Database;
using Exam.Application.Repositories;
using Exam.Application.Services;
using Exam.BackgroundTasks;
using Exam.Repositories;
using Exam.Setting;
using Manonero.MessageBus.Kafka.Extensions;

var builder = WebApplication.CreateBuilder(args);
var appSetting = AppSetting.MapValue(builder.Configuration);
var consumerId = builder.Configuration.GetSection("ConsumerSettings:0:Id").Value;
// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddOracle<OrderDbContext>(builder.Configuration.GetConnectionString("OracleConn"));
builder.Services.AddScoped<IOrderRepositories, OrderRepositories>();
builder.Services.AddScoped<IOrderServices, OrderServices>();
builder.Services.AddHttpClient();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddKafkaProducers(producerBuilder =>
{
    producerBuilder.AddProducer(appSetting.GetProducerSetting(builder.Configuration.GetSection("ProducerSettings:0:Id").Value));
});
builder.Services.AddKafkaConsumers(ConsumerBuilder =>
{
    ConsumerBuilder.AddConsumer<ConsumerBackgroundTask>(appSetting.GetConsumerSetting(consumerId));
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();
app.UseKafkaMessageBus(messageBus =>
{
    messageBus.RunConsumer(consumerId);
}
);
app.Run();
