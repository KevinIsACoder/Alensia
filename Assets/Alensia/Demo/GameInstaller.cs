using Alensia.Core.Camera;
using Alensia.Core.Control;
using Alensia.Core.Input;
using Alensia.Core.UI;
using UnityEngine;
using UnityEngine.Assertions;
using Zenject;

namespace Alensia.Demo
{
    public class GameInstaller : MonoInstaller<GameInstaller>
    {
        public ViewSensitivity ViewSensitivity;

        public HeadMountedCamera.Settings FirstPersonCamera;

        public ThirdPersonCamera.Settings ThirdPersonCamera;

        public UIManager.Settings UI;

        protected void OnValidate()
        {
            Assert.IsNotNull(ViewSensitivity, "ViewSensitivity != null");
            Assert.IsNotNull(FirstPersonCamera, "FirstPersonCamera != null");
            Assert.IsNotNull(ThirdPersonCamera, "ThirdPersonCamera != null");
        }

        public override void InstallBindings()
        {
            InstallUI();
            InstallControls();
            InstallCameras();
        }

        protected void InstallUI()
        {
            Container.Bind<GuiRenderer>().FromNewComponentOnNewGameObject().AsSingle().NonLazy();

            Container.Bind<UIManager.Settings>().FromInstance(UI);

            Container.BindInterfacesAndSelfTo<UIManager>().AsSingle();
        }

        protected void InstallCameras()
        {
            Container.Bind<Camera>().FromInstance(Camera.main);

            Container.Bind<HeadMountedCamera.Settings>().FromInstance(FirstPersonCamera);
            Container.Bind<ThirdPersonCamera.Settings>().FromInstance(ThirdPersonCamera);

            Container.BindInterfacesAndSelfTo<HeadMountedCamera>().AsSingle();
            Container.BindInterfacesAndSelfTo<ThirdPersonCamera>().AsSingle();

            Container.DeclareSignal<CameraChangeEvent>();
            Container.Bind<ICameraManager>().To<CameraManager>().AsSingle();
        }

        protected void InstallControls()
        {
            Container.Bind<ViewSensitivity>().FromInstance(ViewSensitivity);

            Container.DeclareSignal<BindingChangeEvent>();
            Container.BindInterfacesAndSelfTo<InputManager>().AsSingle();

            Container.BindInterfacesAndSelfTo<DemoControl>().AsSingle().NonLazy();
            Container.BindInterfacesAndSelfTo<Controller>().AsSingle().NonLazy();
        }
    }
}