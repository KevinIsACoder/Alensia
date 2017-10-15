﻿using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Alensia.Core.Input;
using Alensia.Core.Input.Generic;
using UniRx;
using UnityEngine;

namespace Alensia.Core.Camera
{
    public class RotatableCameraControl : CameraControl
    {
        public virtual IBindingKey<IAxisInput> Yaw => Keys.Yaw;

        public virtual IBindingKey<IAxisInput> Pitch => Keys.Pitch;

        protected IAxisInput X { get; private set; }

        protected IAxisInput Y { get; private set; }

        public override bool Valid => base.Valid && X != null && Y != null;

        protected override bool Supports(ICameraMode mode) => mode is IRotatableCamera;

        protected override IEnumerable<IBindingKey> PrepareBindings() => new List<IBindingKey> {Yaw, Pitch};

        protected override void RegisterDefaultBindings()
        {
            base.RegisterDefaultBindings();

            InputManager.Register(Yaw, new AxisInput("Mouse X"));
            InputManager.Register(Pitch, new AxisInput("Mouse Y"));
        }

        protected override void OnBindingChange(IBindingKey key)
        {
            base.OnBindingChange(key);

            if (Equals(key, Yaw))
            {
                X = InputManager.Get(Yaw);
            }

            if (Equals(key, Pitch))
            {
                Y = InputManager.Get(Pitch);
            }
        }

        protected override void Subscribe(ICollection<IDisposable> disposables)
        {
            Observable
                .Zip(X.OnChange, Y.OnChange)
                .Where(_ => Valid)
                .Select(xs => new Vector2(xs[0], xs[1]))
                .Subscribe(OnRotate)
                .AddTo(disposables);
        }

        protected void OnRotate(Vector2 input) => OnRotate(input, (IRotatableCamera) CameraManager.Mode);

        protected virtual void OnRotate(Vector2 input, IRotatableCamera mode)
        {
            mode.Heading += input.x * Sensitivity.Horizontal;
            mode.Elevation += input.y * Sensitivity.Vertical;
        }

        [SuppressMessage("ReSharper", "FieldCanBeMadeReadOnly.Global")]
        public class Keys
        {
            public static IBindingKey<IAxisInput> Yaw = new BindingKey<IAxisInput>(Category + ".Yaw");

            public static IBindingKey<IAxisInput> Pitch = new BindingKey<IAxisInput>(Category + ".Pitch");
        }
    }
}