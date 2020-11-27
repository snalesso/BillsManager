using Billy.Domain.CQRS.Command;

namespace Billy.Billing.Persistence.CQRS.Write
{
    public record CreateSupplierCommand : CreateEntityCommand, ICommand
    {
        //public string Name { get; init; }
        //public string Notes { get; init; }
        //public string Website{ get; init; }
        //public string Email{ get; init; }
        //public string Phone{ get; init; }
        //public string AgentName { get; init; }
        //public string AgentSurname { get; init; }
        //public string AgentPhone{ get; init; }
    }

    public record DeleteSupplierCommand : DeleteEntityCommand<long>, ICommand { }
    public record UpdateteSupplierCommand : UpdateEntityCommand<long>, ICommand { }
}
