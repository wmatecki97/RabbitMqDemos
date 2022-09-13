// See https://aka.ms/new-console-template for more information
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

model.ExchangeDeclare("ex1", "direct", true, false);
model.ExchangeDeclare("ex2", "direct", true, false);


model.ExchangeBind("ex1", "ex2", "ex1");

model.QueueDeclare("q",true,false,false);
model.QueueDeclare("q2",true,false,false);
model.QueueBind("q", "ex1", "ex1");
model.QueueBind("q2", "ex2", "");



var consumer = new EventingBasicConsumer(model);
consumer.Received += HandleRecieved;

void HandleRecieved(object? sender, BasicDeliverEventArgs e)
{
    Console.WriteLine(Encoding.UTF8.GetString(e.Body.ToArray()));
}

model.BasicPublish("ex2", "ex1", null, Encoding.UTF8.GetBytes("test message"));


model.BasicConsume("q", true, consumer);


model.ExchangeDelete("ex1");
model.ExchangeDelete("ex2");
model.QueueDelete("q");
model.QueueDelete("q2");

Console.ReadLine();