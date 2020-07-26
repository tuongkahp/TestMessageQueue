namespace TestMessageQueue.Services.Interface
{
    public interface IServiceWrapper
    {
        IRabbitMQService RabbitMQService { get; }
    }
}
