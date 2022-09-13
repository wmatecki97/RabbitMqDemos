using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

Console.WriteLine("Hello, World!");

var factory = new ConnectionFactory()
{
    HostName = "localhost",
    Port = 5672
};

using var connection = factory.CreateConnection();
using var model = connection.CreateModel();

model.ExchangeDeclare("ex1", "direct", true, false, new Dictionary<string, object>
{
    ["alternate-exchange"] = "ex.alt"
});
model.ExchangeDeclare("ex.alt", "fanout", true, false);

model.QueueDeclare("q1", true, false, false);
model.QueueDeclare("alt", true, false, false);

model.QueueBind("q1", "ex1", "test1");
model.QueueBind("alt", "ex.alt", "");


var consumer = new EventingBasicConsumer(model);
consumer.Received += HandleMessage;

void HandleMessage(object? sender, BasicDeliverEventArgs e)
{
    Console.WriteLine(Encoding.UTF8.GetString(e.Body.ToArray()));
}

model.BasicPublish("ex1", "", null, Encoding.UTF8.GetBytes("test message"));
model.BasicConsume("alt", true, consumer);

Console.ReadLine();

model.ExchangeDelete("ex1");
model.ExchangeDelete("ex.alt");
model.QueueDelete("q1");
model.QueueDelete("alt");




