using Alensia.Core.Actor;
using Alensia.Core.Locomotion;
using Alensia.Core.Physics;
using UnityEngine;
using Zenject;

namespace Alensia.Demo.Controller
{
    public class PlayerControllerInstaller : MonoInstaller<PlayerControllerInstaller>
    {
        public WalkingLocomotion.Settings Locomotion;

        public GroundDetectionSettings GroundDetection;

        public override void InstallBindings()
        {
            InstallModel();
            InstallAnimator();
            InstallPhysics();

            InstallLocomotion();
            InstallCharacter();
        }

        protected void InstallModel()
        {
            Container.Bind<Transform>().FromInstance(GetComponent<Transform>()).AsSingle();
        }

        protected void InstallPhysics()
        {
            Container.Bind<CapsuleCollider>().FromInstance(GetComponent<CapsuleCollider>()).AsSingle();
            Container.Bind<Rigidbody>().FromInstance(GetComponent<Rigidbody>()).AsSingle();

            Container.DeclareSignal<GroundHitEvent>();
            Container.DeclareSignal<GroundLeaveEvent>();

            Container.Bind<GroundDetectionSettings>().FromInstance(GroundDetection).AsSingle();
            Container.BindInterfacesAndSelfTo<CapsuleColliderGroundDetector>().AsSingle();
        }

        protected void InstallAnimator()
        {
            Container.Bind<Animator>().FromInstance(GetComponent<Animator>()).AsSingle();
        }

        protected void InstallLocomotion()
        {
            Container.Bind<WalkingLocomotion.Settings>().FromInstance(Locomotion);

            Container.DeclareSignal<PacingChangeEvent>();
            Container.BindInterfacesAndSelfTo<WalkingLocomotion>().AsSingle();
        }

        protected void InstallCharacter()
        {
            Container.Bind<IHumanoid>().To<Humanoid>().AsSingle();
        }
    }
}