﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Alensia.Core.Actor;
using Alensia.Core.Common;
using UnityEngine.Assertions;

namespace Alensia.Core.Camera
{
    public class CameraManager : ICameraManager
    {
        private ICameraMode _mode;

        public ICameraMode Mode
        {
            get { return _mode; }

            private set
            {
                if (value == null || value == _mode) return;

                if (_mode != null) _mode.Deactivate();

                _mode = value;

                if (_mode == null) return;

                _mode.Activate();

                if (CameraChanged != null)
                {
                    CameraChanged(this, new CameraChangeEventArgs(_mode));
                }
            }
        }

        public ReadOnlyCollection<ICameraMode> AvailableModes { get; private set; }

        public event EventHandler<CameraChangeEventArgs> CameraChanged;

        public CameraManager(List<ICameraMode> modes)
        {
            Assert.IsNotNull(modes, "modes != null");
            Assert.IsTrue(modes.Count > 0, "modes.Count > 0");

            AvailableModes = modes.AsReadOnly();
        }

        public T Switch<T>() where T : class, ICameraMode
        {
            return AvailableModes.FirstOrDefault(m => m is T) as T;
        }

        public IFirstPersonCamera ToFirstPerson(IActor target)
        {
            var camera = Switch<IFirstPersonCamera>();

            if (camera == null) return null;

            camera.Initialize((IHumanoid) target);

            Mode = camera;

            return camera;
        }

        public IThirdPersonCamera ToThirdPerson(IActor target)
        {
            var camera = Switch<IThirdPersonCamera>();

            if (camera == null) return null;

            camera.Initialize(target);

            Mode = camera;

            return camera;
        }

        public void Follow(ITransformable target)
        {
            throw new NotImplementedException();
        }
    }
}