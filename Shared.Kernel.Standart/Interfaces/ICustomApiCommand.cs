namespace Shared.Kernel.Interfaces
{
    public interface ICustomApiCommand<TRequest, TResponse>
        where TRequest : ICustomApiRequest
        where TResponse : class
    {
    }
}
