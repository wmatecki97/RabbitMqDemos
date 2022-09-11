using RabbitMQ.Client;
using System.Text;
using System.Threading.Channels;

Console.WriteLine("Hello, World!");
var conn = new ConnectionFactory()
{
    HostName = "localhost",
    Port=5672,
};
using var channel = conn.CreateConnection();
using var model = channel.CreateModel();

model.ExchangeDeclare("ex.headers", "headers", true, false);
model.QueueDeclare("q.info", true, false, false);
model.QueueDeclare("q.error", true, false, false);

model.QueueBind("q.info", "ex.headers", "", new Dictionary<string, object>
{
    ["x-match"] = "any",
    ["test1"] = "1",
    ["test2"] = "2"
});
model.QueueBind("q.error", "ex.headers","",  new Dictionary<string, object>
{
    ["x-match"] = "all",
    ["test1"] = "1",
    ["test2"] = "2"
});

var props = model.CreateBasicProperties();
props.Headers = new Dictionary<string, object>
{
    ["test1"] = "1"
};

model.BasicPublish("ex.headers", "", props, Encoding.UTF8.GetBytes("test message"));

props.Headers.Add("test2", "2");
model.BasicPublish("ex.headers", "", props, Encoding.UTF8.GetBytes("test message to both queues"));


model.QueueDelete("q.info");
model.QueueDelete("q.error");
model.ExchangeDelete("ex.direct");
