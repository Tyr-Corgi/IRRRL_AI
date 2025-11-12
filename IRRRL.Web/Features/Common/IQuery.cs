using MediatR;

namespace IRRRL.Web.Features.Common;

/// <summary>
/// Marker interface for queries (read operations that return data)
/// Queries should be side-effect free and idempotent
/// </summary>
/// <typeparam name="TResponse">The type of data returned by the query</typeparam>
public interface IQuery<out TResponse> : MediatR.IRequest<TResponse>
{
}

