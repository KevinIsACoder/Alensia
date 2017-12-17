using System;
using System.Collections.Generic;
using System.Linq;
using Alensia.Core.Common;
using UniRx;
using UnityEngine;

namespace Alensia.Core.Character.Morph
{
    public abstract class MorphSet : ManagedMonoBehavior, IMorphSet
    {
        public IMorphableRace Race
        {
            get { return _race.Value; }
            set
            {
                if (Equals(_race.Value, value)) return;

                if (Initialized) ChangeRace(value);

                _race.Value = value;
            }
        }

        public Sex Sex
        {
            get { return _sex.Value; }
            set
            {
                if (_sex.Value == value) return;

                if (Initialized) ChangeSex(value);

                _sex.Value = value;
            }
        }

        public IEnumerable<IMorph> Morphs => _morphs.Value;

        public IObservable<IRace> OnRaceChange => _race;

        public IObservable<Sex> OnSexChange => _sex;

        public IObservable<IMorph> OnMorph => _morphChange;

        public IObservable<IEnumerable<IMorph>> OnMorphsChange =>
            _morphs.Cast<IReadOnlyList<IMorph>, IEnumerable<IMorph>>();

        private readonly IReactiveProperty<Sex> _sex;

        private readonly IReactiveProperty<IMorphableRace> _race;

        private readonly IReactiveProperty<IReadOnlyList<IMorph>> _morphs;

        private readonly ISubject<IMorph> _morphChange;

        private readonly CompositeDisposable _morphListeners;

        protected MorphSet()
        {
            _sex = new ReactiveProperty<Sex>();
            _race = new ReactiveProperty<IMorphableRace>();
            _morphs = new ReactiveProperty<IReadOnlyList<IMorph>>(
                Enumerable.Empty<IMorph>().ToList());

            _morphChange = new Subject<IMorph>();
            _morphListeners = new CompositeDisposable();
        }

        protected void UpdateMorphs()
        {
            lock (this)
            {
                _morphListeners.Clear();

                _morphs.Value = CreateMorphs().ToList().AsReadOnly();

                foreach (var morph in Morphs)
                {
                    morph.OnChange
                        .Select(_ => morph)
                        .Subscribe(HandleMorphChange, Debug.LogError)
                        .AddTo(_morphListeners);
                }
            }
        }

        private void HandleMorphChange(IMorph morph)
        {
            ApplyMorph(morph);
            _morphChange.OnNext(morph);
        }

        protected override void OnDisposed()
        {
            _morphChange.OnCompleted();
            _morphListeners.Dispose();

            base.OnDisposed();
        }

        protected abstract IEnumerable<IMorph> CreateMorphs();

        protected abstract void ChangeSex(Sex sex);

        protected abstract void ChangeRace(IMorphableRace race);

        protected abstract void ApplyMorph(IMorph morph);
    }
}