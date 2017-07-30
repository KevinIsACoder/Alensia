﻿using System;
using Zenject;

namespace Alensia.Core.Input
{
    public interface IInput : IInitializable, ITickable, IDisposable
    {
    }

    namespace Generic
    {
        public interface IInput<T> : IInput
        {
            T Value { get; }

            UniRx.IObservable<T> OnChange { get; }
        }
    }
}
