using Billy.Domain.CQRS.Command;

namespace Billy.Billing.Persistence.CQRS.Write
{
    public record CreateBillCommand : CreateEntityCommand, ICommand { }
    public record DeleteBillCommand : DeleteEntityCommand<long>, ICommand { }
    public record UpdateteBillCommand : UpdateEntityCommand<long>, ICommand { }
}
