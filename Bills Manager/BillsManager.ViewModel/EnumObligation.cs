using BillsManager.Localization;

namespace BillsManager.ViewModels
{
    public enum Obligation
    {
        [Localize("Creditor_toSupplier")]
        Creditor = -1,
        [Localize("None_toSupplier")]
        None = 0,
        [Localize("Debtor_toSupplier")]
        Debtor = 1
    }
}