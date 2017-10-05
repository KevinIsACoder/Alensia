﻿using Alensia.Core.Common;
using UnityEngine;
using Zenject;

namespace Alensia.Core.Camera
{
    public abstract class CameraMode : ActivatableMonoBehavior, ICameraMode
    {
        public UnityEngine.Camera Camera => _camera ?? UnityEngine.Camera.main;

        public Transform Transform => Camera.transform;

        public GameObject GameObject => Transform.gameObject;

        public virtual bool Valid => true;

        [InjectOptional] private UnityEngine.Camera _camera;

        public virtual void ResetCamera()
        {
        }
    }
}