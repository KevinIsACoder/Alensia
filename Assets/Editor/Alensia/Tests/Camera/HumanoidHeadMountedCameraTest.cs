using Alensia.Core.Actor;
using Alensia.Core.Common;
using Alensia.Tests.Actor;
using NUnit.Framework;
using UnityEngine;
using TestRange = NUnit.Framework.RangeAttribute;

namespace Alensia.Tests.Camera
{
    [TestFixture, Description("Test suite for HeadMountedCamera class with a humanoid.")]
    public class HumanoidHeadMountedCameraTest : HeadMountedCameraTest<IHumanoid>
    {
        public override float ActualHeading
        {
            get
            {
                var body = Camera.Target.Transform;

                Vector3 direction;

                if (Mathf.Approximately(Camera.Elevation, -90))
                {
                    direction = Camera.AxisUp;
                }
                else if (Mathf.Approximately(Camera.Elevation, 90))
                {
                    direction = -Camera.AxisUp;
                }
                else
                {
                    direction = Vector3.ProjectOnPlane(Camera.AxisForward, body.up);
                }

                var heading = Vector3.Angle(body.forward, direction);
                var cross = body.InverseTransformDirection(Vector3.Cross(body.forward, direction));

                if (cross.y < 0) heading = -heading;

                return GeometryUtils.NormalizeAspectAngle(heading);
            }
        }

        public override float ActualElevation
        {
            get
            {
                var body = Camera.Target.Transform;

                var direction = Quaternion.AngleAxis(-ActualHeading, body.up) * Camera.AxisForward;

                var elevation = Vector3.Angle(body.forward, direction);
                var cross = body.InverseTransformDirection(Vector3.Cross(body.forward, direction));

                if (cross.x > 0) elevation = -elevation;

                return GeometryUtils.NormalizeAspectAngle(elevation);
            }
        }

        protected override IHumanoid CreateActor()
        {
            return new DummyHumanoid();
        }

        [Test, Description("It should place the camera at the target actor's head with proper offset.")]
        public void ShouldPlaceCameraAtHeadWithProperOffset()
        {
            Camera.CameraOffset = new Vector3(0.5f, 0.2f, 0.1f);

            var offset = Actor.Head.TransformDirection(Camera.CameraOffset) *
                Camera.CameraOffset.magnitude;

            Expect(
                Camera.Pivot,
                Is.EqualTo(Actor.Head.position + offset),
                "Unexpected pivot position.");
            Expect(
                Camera.AxisForward,
                Is.EqualTo(Actor.Head.forward),
                "Unexpected pivot axis (forward).");
            Expect(
                Camera.AxisUp,
                Is.EqualTo(Actor.Head.up),
                "Unexpected pivot axis (up).");
            Expect(
                Camera.Head,
                Is.EqualTo(Actor.GetBodyPart(HumanBodyBones.Head)),
                "Unexpected head transform."
            );
        }
    }
}