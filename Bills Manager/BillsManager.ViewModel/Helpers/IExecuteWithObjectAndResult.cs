namespace BillsManager.ViewModels.Helpers
{

    /// <summary>
    /// This interface is meant for the <see cref="WeakFunc{T}" /> class and can be 
    /// useful if you store multiple WeakFunc{T} instances but don't know in advance
    /// what type T represents.
    /// </summary>
    ////[ClassInfo(typeof(WeakAction))]
    public interface IExecuteWithObjectAndResult
    {

        /// <summary>
        /// Executes a func and returns the result.
        /// </summary>
        /// <param name="parameter">A parameter passed as an object,
        /// to be casted to the appropriate type.</param>
        object ExecuteWithObject(object parameter);
    }
}