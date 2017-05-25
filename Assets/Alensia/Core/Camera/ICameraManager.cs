﻿using System.Collections.ObjectModel;
using Alensia.Core.Actor;
using Alensia.Core.Geom;

namespace Alensia.Core.Camera
{
    public interface ICameraManager
    {
        ICameraMode Mode { get; }

        ReadOnlyCollection<ICameraMode> AvailableModes { get; }

        CameraChangeEvent CameraChanged { get; }

        T Switch<T>() where T : class, ICameraMode;

        IFirstPersonCamera ToFirstPerson(IActor target);

        IThirdPersonCamera ToThirdPerson(IActor target);

        void Follow(ITransformable target);
    }
}