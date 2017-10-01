using System;

namespace Alensia.Core.Common
{
    public interface IOpenable : IOpenableOnce, ICloseableOnce
    {
        IObservable<bool> OnOpenStateChange { get; }
    }
}