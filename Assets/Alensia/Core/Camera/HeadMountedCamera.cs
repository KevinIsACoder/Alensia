﻿using System;
using Alensia.Core.Actor;
using Alensia.Core.Common;
using UnityEngine;
using UnityEngine.Assertions;
using Zenject;

namespace Alensia.Core.Camera
{
    public class HeadMountedCamera : BaseCameraMode, IFirstPersonCamera, ILateTickable
    {
        public const string MountPointName = "CameraMount";

        public float Heading
        {
            get { return _heading; }

            set
            {
                var heading = Mathf.Clamp(
                    GeometryUtils.NormalizeAspectAngle(value),
                    -RotationalConstraints.Side,
                    RotationalConstraints.Side);

                _heading = heading;

                UpdatePosition(heading, Elevation);
            }
        }

        public float Elevation
        {
            get { return _elevation; }

            set
            {
                var elevation = Mathf.Clamp(
                    GeometryUtils.NormalizeAspectAngle(value),
                    -RotationalConstraints.Down,
                    RotationalConstraints.Up);

                _elevation = elevation;

                UpdatePosition(Heading, elevation);
            }
        }

        public float LookAhead
        {
            get { return _lookAhead; }
        }

        public override bool Valid
        {
            get { return base.Valid && Head != null; }
        }

        public RotationalConstraints RotationalConstraints
        {
            get { return _rotationalConstraints; }
        }

        public ITransformable Target { get; private set; }

        public Transform Head { get; private set; }

        public Transform Pivot { get; private set; }

        protected Vector3 FocalPoint
        {
            get
            {
                var rotation = Target.Transform.rotation * Quaternion.Euler(-Elevation, Heading, 0);

                return Head.position + rotation * Vector3.forward * LookAhead;
            }
        }

        private float _heading;

        private float _elevation;

        private Quaternion _initialRotation;

        private readonly float _lookAhead;

        private readonly RotationalConstraints _rotationalConstraints;

        public HeadMountedCamera(
            UnityEngine.Camera camera) : this(new Settings(), camera)
        {
        }

        [Inject]
        public HeadMountedCamera(
            Settings settings,
            UnityEngine.Camera camera) : base(camera)
        {
            Assert.IsNotNull(settings, "settings != null");

            _rotationalConstraints = settings.Rotation;
            _lookAhead = settings.LookAhead;
        }

        public void Initialize(ITransformable target)
        {
            Assert.IsNotNull(target, "target != null");

            Target = target;

            var character = target as IHumanoid;

            if (character == null)
            {
                Head = Target.Transform;
            }
            else
            {
                Head = character.GetBodyPart(HumanBodyBones.Head) ?? Target.Transform;
            }

            Pivot = FindMountPoint(Head) ?? Head;

            _initialRotation = Head.localRotation;
        }

        protected override void OnDeactivate()
        {
            base.OnDeactivate();

            Head.localRotation = _initialRotation;
        }

        protected virtual Transform FindMountPoint(Transform parent)
        {
            Assert.IsNotNull(parent, "parent != null");

            return parent.FindChild(MountPointName);
        }

        protected virtual void UpdatePosition(float heading, float elevation)
        {
            if (Head == Pivot)
            {
                Head.localRotation = Quaternion.Euler(new Vector3(-_elevation, _heading, 0));
            }
            else
            {
                Head.localRotation = _initialRotation *
                                     Pivot.localRotation *
                                     Quaternion.Euler(new Vector3(-_elevation, _heading, 0)) *
                                     Quaternion.Inverse(Pivot.localRotation);
            }

            Transform.position = Pivot.position;
            Transform.rotation = Pivot.rotation;

            Transform.LookAt(FocalPoint);
        }

        public virtual void LateTick()
        {
            if (Active) UpdatePosition(Heading, Elevation);
        }

        [Serializable]
        public class Settings : IEditorSettings
        {
            [Range(0.1f, 10f)]
            public float LookAhead = 10f;

            public RotationalConstraints Rotation = new RotationalConstraints
            {
                Down = 65,
                Side = 85,
                Up = 60
            };
        }
    }
}