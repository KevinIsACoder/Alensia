﻿﻿using Alensia.Core.Actor;
using Alensia.Core.Camera;
using Alensia.Core.Common;
using NUnit.Framework;
using UnityEngine;
using TestRange = NUnit.Framework.RangeAttribute;

namespace Alensia.Tests.Camera
{
    public abstract class OrbitingCameraTest<TCamera, TActor> : TrackingCameraTest<TCamera, TActor>
        where TCamera : OrbitingCamera, ITrackingCamera
        where TActor : IActor
    {
        public float ActualDistance
        {
            get { return Vector3.Distance(Camera.Transform.position, Camera.Pivot); }
        }

        public float ActualHeading
        {
            get
            {
                var offset = (Camera.Pivot - Camera.Transform.position).normalized;

                Vector3 direction;
                float heading;

                if (Mathf.Approximately(Camera.Elevation, -90))
                {
                    direction = Camera.Transform.up;
                    heading = Vector3.Angle(Camera.AxisForward, direction);
                }
                else if (Mathf.Approximately(Camera.Elevation, 90))
                {
                    direction = -Camera.Transform.up;
                    heading = -Vector3.Angle(Camera.AxisForward, direction);
                }
                else
                {
                    direction = Vector3.ProjectOnPlane(offset, Camera.AxisUp);
                    heading = Vector3.Angle(Camera.AxisForward, direction);
                }

                var cross = Camera.Target.Transform.InverseTransformDirection(
                    Vector3.Cross(Camera.AxisForward, direction));

                if (cross.y < 0) heading = -heading;

                return GeometryUtils.NormalizeAspectAngle(heading);
            }
        }

        public float ActualElevation
        {
            get
            {
                var offset = (Camera.Pivot - Camera.Transform.position).normalized;
                var direction = Quaternion.AngleAxis(-ActualHeading, Camera.AxisUp) * offset;

                var elevation = Vector3.Angle(Camera.AxisForward, direction);
                var cross = Camera.Target.Transform.InverseTransformDirection(
                    Vector3.Cross(Camera.AxisForward, direction));

                if (cross.x > 0) elevation = -elevation;

                return GeometryUtils.NormalizeAspectAngle(elevation);
            }
        }

        [Test, Description("It should return camera's rotation as heading when elevation is -90/90 degrees.")]
        public void ShouldUseCameraRotationAsHeadingWhenElevationIs90Degrees(
            [Values(5, 10)] float distance,
            [TestRange(-180, 180, 45)] float heading,
            [Values(-90, 90)] float elevation)
        {
            Actor.Transform.eulerAngles = new Vector3
            {
                x = Random.Range(-180, 180),
                y = Random.Range(-180, 180),
                z = Random.Range(-180, 180)
            };

            Camera.RotationalConstraints.Up = 90;
            Camera.RotationalConstraints.Down = 90;

            Camera.Heading = heading;
            Camera.Elevation = elevation;
            Camera.Distance = distance;

            var expected = GeometryUtils.NormalizeAspectAngle(heading);

            Expect(
                ActualHeading,
                Is.EqualTo(expected).Within(Tolerance),
                "Unexpected camera heading.");
        }

        [Test, Description("Changing Heading/Elevation/Distance should move the camera to a proper position.")]
        public void ShouldMoveCameraToProperPosition(
            [Values(5, 10)] float distance,
            [TestRange(-180, 180, 45)] float heading,
            [TestRange(-80, 80, 40)] float elevation)
        {
            Camera.Heading = heading;
            Camera.Elevation = elevation;
            Camera.Distance = distance;

            var aspectAngle = GeometryUtils.NormalizeAspectAngle(heading);

            Expect(
                ActualHeading,
                Is.EqualTo(aspectAngle).Within(Tolerance),
                "Unexpected camera heading.");
            Expect(
                ActualElevation,
                Is.EqualTo(elevation).Within(Tolerance),
                "Unexpected camera elevation.");
            Expect(
                ActualDistance,
                Is.EqualTo(distance).Within(Tolerance),
                "Unexpected camera distance.");
        }

        [Test, Description("It should adjust the camera position according to the anchor's position and rotation.")]
        public void ShouldReflectAnchorPositionAndRotation(
            [Values(1, 5)] float distance,
            [Values(-120, 60)] float heading,
            [Values(-40, 15)] float elevation)
        {
            Actor.Transform.eulerAngles = new Vector3
            {
                x = Random.Range(-180, 180),
                y = Random.Range(-180, 180),
                z = Random.Range(-180, 180)
            };

            Actor.Transform.position = new Vector3
            {
                x = Random.Range(-10, 10),
                y = Random.Range(-10, 10),
                z = Random.Range(-10, 10)
            };

            Camera.Heading = heading;
            Camera.Elevation = elevation;
            Camera.Distance = distance;

            Expect(
                ActualHeading,
                Is.EqualTo(heading).Within(Tolerance),
                "Unexpected camera heading.");
            Expect(
                ActualElevation,
                Is.EqualTo(elevation).Within(Tolerance),
                "Unexpected camera elevation.");
            Expect(
                ActualDistance,
                Is.EqualTo(distance).Within(Tolerance),
                "Unexpected camera distance.");
        }

        [Test, Description("The camera should follow the anchor's position and rotation per every tick.")]
        public void ShouldFollowAnchorPositionAndRotationPerEveryTick(
            [Values(1, 5)] float distance,
            [Values(-120, 60)] float heading,
            [Values(-40, 15)] float elevation)
        {
            Camera.Heading = heading;
            Camera.Elevation = elevation;
            Camera.Distance = distance;

            Actor.Transform.eulerAngles = new Vector3
            {
                x = Random.Range(-180, 180),
                y = Random.Range(-180, 180),
                z = Random.Range(-180, 180)
            };

            Actor.Transform.position = new Vector3
            {
                x = Random.Range(-10, 10),
                y = Random.Range(-10, 10),
                z = Random.Range(-10, 10)
            };

            Camera.LateTick();

            Expect(
                ActualHeading,
                Is.EqualTo(heading).Within(Tolerance),
                "Unexpected camera heading.");
            Expect(
                ActualElevation,
                Is.EqualTo(elevation).Within(Tolerance),
                "Unexpected camera elevation.");
            Expect(
                ActualDistance,
                Is.EqualTo(distance).Within(Tolerance),
                "Unexpected camera distance.");
        }

        [Test, Description("Heading property should be clamped between the min. and the max. values.")]
        public void ShouldClampHeadingProperty()
        {
            Camera.RotationalConstraints.Side = 60;

            Camera.Heading = -80;

            Expect(
                Camera.Heading,
                Is.EqualTo(-60).Within(Tolerance),
                "Unexpected camera heading.");

            Camera.Heading = 80;

            Expect(
                Camera.Heading,
                Is.EqualTo(60).Within(Tolerance),
                "Unexpected camera heading.");
        }

        [Test, Description("Elevation property should be clamped between the min. and the max. values.")]
        public void ShouldClampElevationProperty()
        {
            Camera.RotationalConstraints.Up = 60;
            Camera.RotationalConstraints.Down = 30;

            Camera.Elevation = 80;

            Expect(
                Camera.Elevation,
                Is.EqualTo(60).Within(Tolerance),
                "Unexpected camera elevation.");

            Camera.Elevation = -50;

            Expect(
                Camera.Elevation,
                Is.EqualTo(-30).Within(Tolerance),
                "Unexpected camera elevation.");
        }

        [Test, Description("Distance property should be clamped between the min. and the max. values.")]
        public void ShouldClampDistanceProperty()
        {
            Camera.Distance = Camera.DistanceSettings.Minimum - 1;

            Expect(
                Camera.Distance,
                Is.EqualTo(Camera.DistanceSettings.Minimum).Within(Tolerance),
                "Unexpected camera distance.");

            Camera.Distance = Camera.DistanceSettings.Maximum + 1;

            Expect(
                Camera.Distance,
                Is.EqualTo(Camera.DistanceSettings.Maximum).Within(Tolerance),
                "Unexpected camera distance.");
        }
    }
}