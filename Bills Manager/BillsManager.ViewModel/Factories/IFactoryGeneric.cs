namespace BillsManager.ViewModel.Factories
{
    public interface IFactory<T>
    {
        T Create();
    }
}
