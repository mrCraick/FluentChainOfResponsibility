namespace FluentChainOfResponsibility
{
    /// <summary>
    /// Defines a request
    /// </summary>
    /// <typeparam name="TResponse">The type of response from the handler</typeparam>
    public interface IRequest<out TResponse>
        where TResponse : class
    {
    }
}