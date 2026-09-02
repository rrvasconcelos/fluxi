using Fluxi.Api.Abstractions.Endpoints;
using Fluxi.Api.Extensions;
using Fluxi.Api.Http;
using Fluxi.Application.Abstractions.Messaging;
using Fluxi.Application.Features.Accounts.CreateAccount;

namespace Fluxi.Api.Features.Accounts.CreateAccount;

public sealed class CreateAccountEndpoint : IEndpoint
{
    #region Methods

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/accounts", async (
            CreateAccountCommand command,
            ICommandHandler<CreateAccountCommand> handler,
            CancellationToken cancellationToken) =>
        {
            var result = await handler.Handle(command, cancellationToken);

            return result.Match(
                () => Results.Ok(),
                CustomResults.Problem);
        });
    }

    #endregion
}
