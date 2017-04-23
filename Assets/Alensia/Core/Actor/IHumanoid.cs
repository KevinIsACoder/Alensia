﻿using Alensia.Core.Locomotion;
using UnityEngine;

namespace Alensia.Core.Actor
{
    public interface IHumanoid : IActor, IWalker
    {
        Transform Head { get; }

        Transform LeftEye { get; }

        Transform RightEye { get; }

        Vector3 Viewpoint { get; }

        Transform GetBodyPart(HumanBodyBones bone);
    }
}