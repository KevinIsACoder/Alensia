﻿using Alensia.Core.Camera;
using NUnit.Framework;
using UnityEngine;

namespace Alensia.Tests.Camera
{
    public abstract class CameraTest<T> : TestBase where T : class, ICameraMode
    {
        public T Camera { get; private set; }

        protected override void PrepareScene()
        {
            base.PrepareScene();

            var gameObject = new GameObject();
            var camera = gameObject.AddComponent<UnityEngine.Camera>();

            Camera = CreateCamera(camera);
            Camera.Activate();
        }

        [TearDown]
        public virtual void TearDown()
        {
            if (Camera != null)
            {
                if (Camera.Active.Value) Camera.Deactivate();

                Object.DestroyImmediate(Camera.Transform.gameObject);
            }

            Camera = default(T);
        }

        protected abstract T CreateCamera(UnityEngine.Camera camera);

        [Test, Description("Activate() should make the camera in active state.")]
        public void ActivateShouldMakeCameraInActiveStatus()
        {
            Expect(Camera.Active, Is.True, "Unexpected camera state.");
        }

        [Test, Description("Deactivate() should make the camera in inactive state.")]
        public void DeactivateShouldMakeCameraInInactiveStatus()
        {
            Camera.Deactivate();

            Expect(Camera.Active, Is.False, "Unexpected camera state.");
        }
    }
}