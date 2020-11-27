using Billy.Domain.CQRS.Command;
using System.Collections.Generic;

namespace Billy.Billing.Persistence.CQRS.Write
{
    public record CreateEntityCommand : ICommand
    {
        public IReadOnlyDictionary<string, object> Data { get; init; }
    }

    public record UpdateEntityCommand<TIdentity> : ICommand
    {
        public TIdentity Id { get; init; }
        public IReadOnlyDictionary<string, object> Changes { get; init; }
    }

    public record DeleteEntityCommand<TIdentity> : ICommand
    {
        public TIdentity Id { get; init; }
    }
}
