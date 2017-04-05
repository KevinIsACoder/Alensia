using Alensia.Core.Actor;
using Alensia.Core.Common;
using Alensia.Tests.Actor;
using NUnit.Framework;
using TestRange = NUnit.Framework.RangeAttribute;

namespace Alensia.Tests.Camera
{
    [TestFixture, Description("Test suite for HeadMountedCamera class with a non humanoid actor.")]
    public class GenericHeadMountedCameraTest : BaseHeadMountedCameraTest<IActor>
    {
        protected override IActor CreateActor()
        {
            return new DummyActor();
        }

        public override float ActualHeading
        {
            get { return GeometryUtils.NormalizeAspectAngle(Actor.Transform.localEulerAngles.y); }
        }

        public override float ActualElevation
        {
            get { return -GeometryUtils.NormalizeAspectAngle(Actor.Transform.localEulerAngles.x); }
        }

        [Test, Description("It should use the target actor itself as the pivot point.")]
        public void ShouldUseTargetActorItselfAsPivotPoint()
        {
            Expect(
                Camera.Pivot,
                Is.EqualTo(Actor.Transform),
                "Unexpected pivot object."
            );
            Expect(
                Camera.Head,
                Is.EqualTo(Actor.Transform),
                "Unexpected head transform."
            );
        }
    }
}