public interface IRequestHandler<TRequest, TResult>
{
    Task<TResult> Handle(TRequest request);
}
