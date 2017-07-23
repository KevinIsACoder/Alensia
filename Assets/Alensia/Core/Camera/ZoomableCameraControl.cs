﻿using System;
using System.Collections.Generic;
using Alensia.Core.Input;
using Alensia.Core.Input.Generic;
using UniRx;

namespace Alensia.Core.Camera
{
    public class ZoomableCameraControl : CameraControl
    {
        public IBindingKey<IAxisInput> Zoom => Keys.Zoom;

        protected IAxisInput Scroll { get; private set; }

        public override bool Valid => base.Valid && Scroll != null;

        public ZoomableCameraControl(
            ViewSensitivity sensitivity,
            ICameraManager cameraManager,
            IInputManager inputManager) : base(sensitivity, cameraManager, inputManager)
        {
        }

        protected override bool Supports(ICameraMode camera) => camera is IZoomableCamera;

        protected override ICollection<IBindingKey> PrepareBindings() => new List<IBindingKey> {Zoom};

        protected override void RegisterDefaultBindings()
        {
            base.RegisterDefaultBindings();

            InputManager.Register(Zoom, new AxisInput("Mouse ScrollWheel", 0.15f));
        }

        protected override void OnBindingChange(IBindingKey key)
        {
            base.OnBindingChange(key);

            if (Equals(key, Zoom))
            {
                Scroll = InputManager.Get(Zoom);
            }
        }

        protected override void Subscribe(ICollection<IDisposable> disposables)
        {
            Scroll.OnChange
                .Where(_ => Valid)
                .Select(v => v * -15)
                .Subscribe(OnZoom)
                .AddTo(disposables);
        }

        protected virtual void OnZoom(float input) => OnZoom(input, (IZoomableCamera) CameraManager.Mode);

        protected virtual void OnZoom(float input, IZoomableCamera camera)
        {
            camera.Distance += input * Sensitivity.Zoom;
        }

        public class Keys : RotatableCameraControl.Keys
        {
            public static IBindingKey<IAxisInput> Zoom = new BindingKey<IAxisInput>(Category + ".Zoom");
        }
    }
}