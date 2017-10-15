﻿using Alensia.Core.Animation;
using Alensia.Core.Entity;
using Alensia.Core.Locomotion;
using Alensia.Core.Locomotion.Generic;
using Alensia.Core.Sensor;
using Alensia.Core.Sensor.Generic;
using UnityEngine;

namespace Alensia.Core.Character
{
    public interface ICharacter : IEntity, IAnimatable, ISeeing, ILocomotive
    {
        Race Race { get; }

        Sex Sex { get; }

        Transform Head { get; }
    }

    namespace Generic
    {
        public interface ICharacter<out TVision, out TLocomotion> : ICharacter,
            ISeeing<TVision>, ILocomotive<TLocomotion>
            where TVision : IVision
            where TLocomotion : ILocomotion
        {
        }
    }
}