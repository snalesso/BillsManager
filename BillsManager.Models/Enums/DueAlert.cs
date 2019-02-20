using BillsManager.Localization.Attributes;

namespace BillsManager.Models
{
    public enum DueAlert
    {
        [LocalizedDisplayName("None_toDueLevel")]
        None = 0,
        [LocalizedDisplayName("Low_toDueLevel")]
        Low = 1,
        [LocalizedDisplayName("Medium_toDueLevel")]
        Medium = 2,
        [LocalizedDisplayName("High_toDueLevel")]
        High = 3,
        [LocalizedDisplayName("Critical_toDueLevel")]
        Critical = 4
    }
}