using System;
using System.Collections.Generic;

namespace Billy
{
    public static class IDisposableMixins
    {
        public static TDisposable DisposeWith<TDisposable>(this TDisposable disposable, ICollection<IDisposable> disposables)
            where TDisposable : IDisposable
        {
            if (disposables == null)
                throw new ArgumentNullException(nameof(disposables));

            disposables.Add(disposable);

            return disposable;
        }
    }
}