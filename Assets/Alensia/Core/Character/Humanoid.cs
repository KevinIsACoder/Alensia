﻿using Alensia.Core.Locomotion;
using Alensia.Core.Sensor;
using UnityEngine;
using UnityEngine.Assertions;
using Zenject;

namespace Alensia.Core.Character
{
    public class Humanoid : Character<IBinocularVision, ILeggedLocomotion>, IHumanoid
    {
        public Transform Head { get; }

        public override IBinocularVision Vision { get; }

        public override ILeggedLocomotion Locomotion { get; }

        public Humanoid(
            [InjectOptional] IBinocularVision vision,
            [InjectOptional] ILeggedLocomotion locomotion,
            Animator animator,
            Transform transform) : base(animator, transform)
        {
            Assert.IsNotNull(vision, "vision != null");
            Assert.IsNotNull(locomotion, "locomotion != null");

            Head = GetBodyPart(HumanBodyBones.Head);

            Vision = vision;
            Locomotion = locomotion;
        }

        public Transform GetBodyPart(HumanBodyBones bone) => Animator.GetBoneTransform(bone);
    }
}