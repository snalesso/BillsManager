using BillsManager.Localization;

namespace BillsManager.ViewModels
{
    public enum DueAlert
    {
        [Localize("None_toDueLevel")]
        None = 0,
        [Localize("Low_toDueLevel")]
        Low = 1,
        [Localize("Medium_toDueLevel")]
        Medium = 2,
        [Localize("High_toDueLevel")]
        High = 3
    }
}