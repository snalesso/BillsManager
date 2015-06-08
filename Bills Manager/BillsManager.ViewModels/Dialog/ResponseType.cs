using BillsManager.Localization.Attributes;

namespace BillsManager.ViewModels
{
    public enum ResponseType
    {
        [LocalizedDisplayName("Unset")]
        Unset

        ,
        [LocalizedDisplayName("Yes")]
        Yes
        ,
        [LocalizedDisplayName("No")]
        No
        ,
        [LocalizedDisplayName("Ok")]
        Ok
        ,
        [LocalizedDisplayName("Cancel")]
        Cancel
        ,
        [LocalizedDisplayName("Retry")]
        Retry
        //,
        //[LocalizedDisplayName("Abort")]
        //Abort
    }
}