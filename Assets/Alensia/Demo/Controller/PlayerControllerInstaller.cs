using Alensia.Core.Actor;
using Alensia.Core.Camera;
using Alensia.Core.Control;
using Alensia.Core.Locomotion;
using Alensia.Core.Physics;
using UnityEngine;
using Zenject;

namespace Alensia.Demo.Controller
{
    public class PlayerControllerInstaller : MonoInstaller<PlayerControllerInstaller>
    {
        public Camera Camera;

        public ViewSensitivity ViewSensitivity;

        public WalkSpeedSettings WalkSpeed;

        public GroundDetectionSettings GroundDetection;

        public AnimatedLocomotion.Settings AnimationSettings;

        public HeadMountedCamera.Settings FirstPersonCamera;

        public ThirdPersonCamera.Settings ThirdPersonCamera;

        public override void InstallBindings()
        {
            var parent = transform.parent;

            Container.Bind<Transform>().FromInstance(parent);
            Container.Bind<Collider>().FromInstance(parent.GetComponent<Collider>());
            Container.Bind<Rigidbody>().FromInstance(parent.GetComponent<Rigidbody>());
            Container.Bind<Animator>().FromInstance(parent.GetComponent<Animator>());
            Container.Bind<Camera>().FromInstance(Camera);

            Container.BindInterfacesAndSelfTo<DesktopInputManager>().AsSingle();
            Container.BindInterfacesAndSelfTo<FirstAndThirdPersonController>().AsSingle().NonLazy();

            Container.Bind<IHumanoid>().To<Humanoid>().AsSingle();

            Container.DeclareSignal<GroundHitEvent>();
            Container.DeclareSignal<GroundLeaveEvent>();

            Container.Bind<GroundDetectionSettings>().FromInstance(GroundDetection);
            Container.BindInterfacesAndSelfTo<RayCastingGroundDetector>().AsSingle();

            Container.DeclareSignal<PacingChangeEvent>();
            Container.BindInterfacesAndSelfTo<WalkingLocomotion>().AsSingle();

            Container.DeclareSignal<CollisionEnterEvent>();
            Container.DeclareSignal<CollisionExitEvent>();

            Container.Bind<ViewSensitivity>().FromInstance(ViewSensitivity);
            Container.Bind<WalkSpeedSettings>().FromInstance(WalkSpeed);
            Container.Bind<AnimatedLocomotion.Settings>().FromInstance(AnimationSettings);

            Container.Bind<HeadMountedCamera.Settings>().FromInstance(FirstPersonCamera);
            Container.Bind<ThirdPersonCamera.Settings>().FromInstance(ThirdPersonCamera);

            Container.BindInterfacesAndSelfTo<HeadMountedCamera>().AsSingle();
            Container.BindInterfacesAndSelfTo<ThirdPersonCamera>().AsSingle();

            Container.DeclareSignal<CameraChangeEvent>();
            Container.Bind<ICameraManager>().To<CameraManager>().AsSingle();
        }
    }
}