using MediatR;

namespace IRRRL.Web.Features.Common;

/// <summary>
/// Marker interface for commands (write operations that change state)
/// Commands can have side effects and should be designed for their specific use case
/// </summary>
/// <typeparam name="TResponse">The type of result returned by the command</typeparam>
public interface ICommand<out TResponse> : IRequest<TResponse>
{
}

/// <summary>
/// Command that doesn't return a value
/// </summary>
public interface ICommand : IRequest
{
}

