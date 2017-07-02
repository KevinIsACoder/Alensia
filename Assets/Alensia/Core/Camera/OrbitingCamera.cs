﻿using Alensia.Core.Geom;
using UnityEngine;
using Zenject;

namespace Alensia.Core.Camera
{
    public abstract class OrbitingCamera : RotatableCamera, IZoomableCamera, ILateTickable
    {
        public abstract DistanceSettings DistanceSettings { get; }

        public override float Heading
        {
            get { return _heading; }
            set
            {
                var heading = Mathf.Clamp(
                    GeometryUtils.NormalizeAspectAngle(value),
                    -RotationalConstraints.Side,
                    RotationalConstraints.Side);

                _heading = heading;

                UpdatePosition(heading, Elevation, Distance);
            }
        }

        public override float Elevation
        {
            get { return _elevation; }
            set
            {
                var elevation = Mathf.Clamp(
                    GeometryUtils.NormalizeAspectAngle(value),
                    -RotationalConstraints.Down,
                    RotationalConstraints.Up);

                _elevation = elevation;

                UpdatePosition(Heading, elevation, Distance);
            }
        }

        public float Distance
        {
            get { return _distance; }
            set
            {
                var distance = Mathf.Clamp(
                    value,
                    DistanceSettings.Minimum,
                    DistanceSettings.Maximum);

                _distance = distance;

                UpdatePosition(Heading, Elevation, distance);
            }
        }

        private float _heading;

        private float _elevation;

        private float _distance;

        protected OrbitingCamera(
            UnityEngine.Camera camera) : base(camera)
        {
        }

        public override void Reset()
        {
            base.Reset();

            Distance = DistanceSettings.Default;
        }

        protected virtual void UpdatePosition(
            float heading, float elevation, float distance)
        {
            Transform.rotation = Quaternion.LookRotation(AxisForward, AxisUp);
            Transform.position = Pivot - AxisForward * distance;

            Transform.RotateAround(Pivot, Transform.right, -elevation);

            if (Mathf.Approximately(Mathf.Abs(elevation), 90))
            {
                Transform.Rotate(Vector3.forward, -heading);
            }
            else
            {
                Transform.RotateAround(Pivot, AxisUp, heading);
                Transform.LookAt(Pivot, AxisUp);
            }
        }

        public virtual void LateTick()
        {
            if (Active) UpdatePosition(Heading, Elevation, Distance);
        }
    }
}