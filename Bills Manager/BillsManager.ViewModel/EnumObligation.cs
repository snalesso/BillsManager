using BillsManager.Localization.Attributes;

namespace BillsManager.ViewModels
{
    public enum Obligation
    {
        [LocalizedDisplayName("Creditor_toSupplier")]
        Creditor = -1,
        [LocalizedDisplayName("Unbound_toSupplier")]
        Unbound = 0,
        [LocalizedDisplayName("Debtor_toSupplier")]
        Debtor = 1
    }
}