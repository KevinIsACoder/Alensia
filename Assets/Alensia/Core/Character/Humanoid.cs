﻿using Alensia.Core.Locomotion;
using UnityEngine;
using UnityEngine.Assertions;

namespace Alensia.Core.Character
{
    public class Humanoid : Character<ILeggedLocomotion>, IHumanoid
    {
        public Transform Head { get; }

        public Transform LeftEye { get; }

        public Transform RightEye { get; }

        public Vector3 Viewpoint
        {
            get
            {
                if (LeftEye && RightEye)
                {
                    return (LeftEye.position + RightEye.position) / 2;
                }

                return Head ? Head.position : Transform.position;
            }
        }

        public override ILeggedLocomotion Locomotion { get; }

        public Humanoid(
            ILeggedLocomotion locomotion,
            Animator animator,
            Transform transform) : base(animator, transform)
        {
            Assert.IsNotNull(locomotion, "locomotion != null");

            Head = GetBodyPart(HumanBodyBones.Head);

            LeftEye = GetBodyPart(HumanBodyBones.LeftEye);
            RightEye = GetBodyPart(HumanBodyBones.RightEye);

            Locomotion = locomotion;
        }

        public Transform GetBodyPart(HumanBodyBones bone)
        {
            return Animator.GetBoneTransform(bone);
        }
    }
}