using Microsoft.AspNetCore.Mvc;

namespace MediHz.Application.Handlers;

public record EmptyRequest();

public record Result<T>(int StatusCode, T? Value, ProblemDetails? Problem)
{
    public static Result<T> Success(T value, int statusCode = 200) => new(statusCode, value, null);

    public static Result<T> Failure(string Title, string Detail, int statusCode = 400) =>
        new(
            statusCode,
            default,
            new ProblemDetails
            {
                Title = Title,
                Detail = Detail,
                Status = statusCode,
            }
        );

    public object? Body => (object?)Value ?? Problem;
}

public interface IRequestHandler<TRequest, TResult>
{
    Task<Result<TResult>> Handle(TRequest request, CancellationToken cancellationToken);
}
