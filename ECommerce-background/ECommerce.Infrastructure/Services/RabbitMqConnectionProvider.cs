using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;

public class RabbitMqConnectionProvider : IRabbitMqConnectionProvider
{
    private readonly string _rabbitHost;
    private readonly string _rabbitUser;
    private readonly string _rabbitPass;
    private IConnection? _connection;
    private IModel? _channel;

    private const string DeadLetterExchange = "order.dlx.exchange";
    private const string ExpiredQueueName = "order.expired.queue";
    private const string DelayQueueName = "order.delay.queue";

    public RabbitMqConnectionProvider(IConfiguration configuration)
    {
        _rabbitHost = Environment.GetEnvironmentVariable("RABBITMQ_HOST") ?? "localhost";
        _rabbitUser = Environment.GetEnvironmentVariable("RABBITMQ_USER") ?? "guest";
        _rabbitPass = Environment.GetEnvironmentVariable("RABBITMQ_PASS") ?? "guest";
    }

    public IConnection GetConnection()
    {
        if (_connection == null || !_connection.IsOpen)
        {
            var factory = new ConnectionFactory
            {
                HostName = _rabbitHost,
                UserName = _rabbitUser,
                Password = _rabbitPass,
                DispatchConsumersAsync = true
            };
            _connection = factory.CreateConnection();
        }
        return _connection;
    }

    public IModel GetChannel()
    {
        if (_channel == null || _channel.IsClosed)
        {
            _channel = GetConnection().CreateModel();
        }
        return _channel;
    }

    public void EnsureQueuesAndExchanges()
    {
        var channel = GetChannel();
        channel.ExchangeDeclare(exchange: DeadLetterExchange, type: ExchangeType.Direct, durable: true, autoDelete: false, arguments: null);
        channel.QueueDeclare(queue: ExpiredQueueName, durable: true, exclusive: false, autoDelete: false, arguments: null);
        channel.QueueBind(queue: ExpiredQueueName, exchange: DeadLetterExchange, routingKey: ExpiredQueueName);

        var delayQueueArgs = new Dictionary<string, object>
        {
            { "x-dead-letter-exchange", DeadLetterExchange },
            { "x-dead-letter-routing-key", ExpiredQueueName }
        };
        channel.QueueDeclare(queue: DelayQueueName, durable: true, exclusive: false, autoDelete: false, arguments: delayQueueArgs);
    }
}