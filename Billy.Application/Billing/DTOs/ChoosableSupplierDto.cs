using Billy.Billing.Models;

namespace Billy.Billing.Application.DTOs
{
    public sealed class ChoosableSupplierDto
    {
        public ChoosableSupplierDto(Supplier supplier)
        {
            this.Id = supplier.Id;
            this.Name = supplier.Name;
        }

        public long Id { get; }
        public string Name { get; }
    }
}
