namespace BillsManager.ViewModels
{
    public interface IDialogViewModelFactory
    {
        DialogViewModel Ok(string okText = null);
        DialogViewModel OkCancel(string okText = null, string cancelText = null);
        DialogViewModel YesNo(string yesText = null, string noText = null);
        DialogViewModel YesNoCancel(string yesText = null, string noText = null, string cancelText = null);
        DialogViewModel RetryCancel(string retryText = null, string cancelText = null);
    }
}