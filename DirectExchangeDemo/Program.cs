using RabbitMQ.Client;

Console.WriteLine("Hello, World!");
var conn = new ConnectionFactory()
{
    HostName = "localhost",
    Port=5672,
};
using var channel = conn.CreateConnection();
using var model = channel.CreateModel();

model.ExchangeDeclare("ex.direct", "direct", true, false);
model.QueueDeclare("q.info", true, false, false);
model.QueueDeclare("q.error", true, false, false);

model.QueueBind("q.info", "ex.direct", "info");
model.QueueBind("q.error", "ex.direct", "error");

model.QueueDelete("q.info");
model.QueueDelete("queue.error");
model.ExchangeDelete("ex.direct");
