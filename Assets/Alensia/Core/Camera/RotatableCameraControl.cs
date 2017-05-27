﻿using System.Collections.Generic;
using Alensia.Core.Input;
using Alensia.Core.Input.Generic;
using UniRx;
using UnityEngine;

namespace Alensia.Core.Camera
{
    public class RotatableCameraControl : CameraControl
    {
        public const string Id = "Camera";

        public IBindingKey<IAxisInput> Yaw => Keys.Yaw;

        public IBindingKey<IAxisInput> Pitch => Keys.Pitch;

        protected IAxisInput X { get; private set; }

        protected IAxisInput Y { get; private set; }

        public override bool Valid => base.Valid && X != null && Y != null;

        public RotatableCameraControl(
            ICameraManager cameraManager,
            IInputManager inputManager) : base(cameraManager, inputManager)
        {
        }

        protected override ICollection<IBindingKey> PrepareBindings()
        {
            return new List<IBindingKey> {Yaw, Pitch};
        }

        protected override void OnBindingChange(IBindingKey key)
        {
            base.OnBindingChange(key);

            if (Equals(key, Keys.Yaw))
            {
                X = InputManager.Get(Keys.Yaw);
            }

            if (Equals(key, Keys.Pitch))
            {
                Y = InputManager.Get(Keys.Pitch);
            }
        }

        protected override void OnActivate()
        {
            base.OnActivate();

            Observable
                .Zip(X.Value, Y.Value)
                .Where(_ => Active && Valid)
                .Where(_ => CameraManager.Mode is IRotatableCamera)
                .Select(xs => new Vector2(xs[0], xs[1]))
                .Subscribe(OnRotate)
                .AddTo(Observers);
        }

        protected void OnRotate(Vector2 input)
        {
            OnRotate(input, (IRotatableCamera) CameraManager.Mode);
        }

        protected virtual void OnRotate(Vector2 input, IRotatableCamera camera)
        {
            camera.Heading += input.x;
            camera.Elevation += input.y;
        }

        public class Keys
        {
            public static IBindingKey<IAxisInput> Yaw = new BindingKey<IAxisInput>(Id + ".Yaw");

            public static IBindingKey<IAxisInput> Pitch = new BindingKey<IAxisInput>(Id + ".Pitch");
        }
    }
}