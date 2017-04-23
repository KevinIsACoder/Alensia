﻿using Alensia.Core.Physics;
using UnityEngine;

namespace Alensia.Core.Locomotion
{
    public interface IWalkingLocomotion : ILocomotion
    {
        WalkSpeedSettings MaximumSpeed { get; }

        IGroundDetector GroundDetector { get; }

        Pacing Pacing { get; set; }

        PacingChangeEvent PacingChanged { get; }

        void Walk(Vector2 direction, float heading);

        void Jump(Vector2 direction);
    }
}