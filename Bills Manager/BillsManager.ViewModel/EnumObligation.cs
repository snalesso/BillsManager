using BillsManager.Localization;

namespace BillsManager.ViewModels
{
    public enum Obligation
    {
        [LocalizedDisplayName("Creditor_toSupplier")]
        Creditor = -1,
        [LocalizedDisplayName("None_toSupplier")]
        None = 0,
        [LocalizedDisplayName("Debtor_toSupplier")]
        Debtor = 1
    }
}