namespace BillsManager.ViewModels.Factories
{
    public interface IFactory<T>
    {
        T Create();
    }
}
