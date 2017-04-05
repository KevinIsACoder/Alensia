﻿using System;
using Alensia.Core.Actor;
using Alensia.Core.Common;
using UnityEngine;
using UnityEngine.Assertions;
using Zenject;

namespace Alensia.Core.Camera
{
    public class ThirdPersonCamera : BaseOrbitingCamera, IThirdPersonCamera
    {
        public override RotationalConstraints RotationalConstraints
        {
            get { return _rotationalConstraints; }
        }

        public override DistanceSettings DistanceSettings
        {
            get { return _distanceSettings; }
        }

        public WallAvoidanceSettings WallAvoidanceSettings
        {
            get { return _wallAvoidanceSettings; }
        }

        public override bool Valid
        {
            get { return base.Valid && Target != null; }
        }

        public ITransformable Target { get; private set; }

        public override Transform Pivot
        {
            get { return _pivot; }
        }

        protected override Vector3 AxisForward
        {
            get { return Target.Transform.forward; }
        }

        protected override Vector3 AxisUp
        {
            get { return Target.Transform.up; }
        }

        private Transform _pivot;

        private readonly RotationalConstraints _rotationalConstraints;

        private readonly DistanceSettings _distanceSettings;

        private readonly WallAvoidanceSettings _wallAvoidanceSettings;

        public ThirdPersonCamera(UnityEngine.Camera camera) : this(new Settings(), camera)
        {
        }

        [Inject]
        public ThirdPersonCamera(
            Settings settings,
            UnityEngine.Camera camera) : base(camera)
        {
            Assert.IsNotNull(settings, "settings != null");

            _rotationalConstraints = settings.Rotation;
            _distanceSettings = settings.Distance;
            _wallAvoidanceSettings = settings.WallAvoidance;
        }

        public void Initialize(ITransformable target)
        {
            Assert.IsNotNull(target, "target != null");

            Target = target;

            var character = target as IHumanoid;

            if (character == null)
            {
                _pivot = Target.Transform;
            }
            else
            {
                _pivot = character.GetBodyPart(HumanBodyBones.Head) ?? Target.Transform;
            }

            Distance = DistanceSettings.Default;
        }

        protected override void UpdatePosition(
            float heading, float elevation, float distance)
        {
            var preferredDistance = distance;

            if (WallAvoidanceSettings.AvoidWalls)
            {
                var direction = (Transform.position - Pivot.position).normalized;
                var ray = new Ray(Pivot.position, direction);

                RaycastHit hit;

                if (UnityEngine.Physics.Raycast(ray, out hit, preferredDistance))
                {
                    preferredDistance =
                        Vector3.Distance(Pivot.position, hit.point) -
                        WallAvoidanceSettings.MinimumDistance;
                }
            }

            base.UpdatePosition(heading, elevation, preferredDistance);
        }

        [Serializable]
        public class Settings : IEditorSettings
        {
            public RotationalConstraints Rotation = new RotationalConstraints();

            public DistanceSettings Distance = new DistanceSettings();

            public WallAvoidanceSettings WallAvoidance = new WallAvoidanceSettings();
        }
    }
}