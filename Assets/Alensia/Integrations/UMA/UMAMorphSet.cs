using System;
using System.Collections.Generic;
using System.Linq;
using Alensia.Core.Character;
using Alensia.Core.Character.Morph;
using Alensia.Core.Character.Morph.Generic;
using UMA;
using UMA.CharacterSystem;
using UnityEngine.Assertions;

namespace Alensia.Integrations.UMA
{
    public class UMAMorphSet : MorphSet
    {
        public DynamicCharacterAvatar Avatar { get; }

        public UMAData Data => Avatar?.umaData;

        public RaceData RaceData => Avatar?.activeRace.data;

        public UMARaceRepository RaceRepository { get; }

        private readonly IDictionary<string, DNAKey> _dnaMappings;

        public UMAMorphSet(DynamicCharacterAvatar avatar, UMARaceRepository raceRepository)
        {
            Assert.IsNotNull(avatar, "avatar != null");
            Assert.IsNotNull(raceRepository, "raceRepository != null");

            Avatar = avatar;
            RaceRepository = raceRepository;

            _dnaMappings = new Dictionary<string, DNAKey>();

            var umaRace = avatar.activeRace.name;

            Race = RaceRepository.GetRaceFromUMARace(umaRace);
            Sex = Sex.Female;
        }

        protected override IEnumerable<IMorph> CreateMorphs()
        {
            _dnaMappings.Clear();

            var allDna = Data.GetAllDna();

            foreach (var dna in allDna)
            {
                var hash = dna.DNATypeHash;

                for (var i = 0; i < dna.Names.Length; i++)
                {
                    _dnaMappings[dna.Names[i]] = new DNAKey(hash, i);
                }
            }

            return allDna.SelectMany(CreateMorphs);
        }

        protected virtual IEnumerable<IMorph> CreateMorphs(UMADnaBase dna)
        {
            var values = dna.Names.Zip(dna.Values, (n, v) => new Tuple<string, float>(n, v));

            return values.Select(tuple =>
            {
                var name = tuple.Item1;
                var value = tuple.Item2;

                return new RangedMorph<float>(name, value, 0, 0, 1);
            });
        }

        protected override void ChangeSex(Sex sex) => ChangeUmaRace(Race, sex);

        protected override void ChangeRace(Race race) => ChangeUmaRace(race, Sex);

        protected override void ApplyMorph(IMorph morph)
        {
            var value = morph as IMorph<float>;

            if (value == null || !_dnaMappings.ContainsKey(morph.Name)) return;

            var mapping = _dnaMappings[morph.Name];

            ApplyMorph(mapping.Hash, mapping.Index, value.Value);
        }

        protected virtual void ApplyMorph(int dnaIndex, int valueIndex, float value)
        {
            Data.GetDna(dnaIndex).SetValue(valueIndex, value);
            Avatar.ForceUpdate(true);
        }

        protected virtual void ChangeUmaRace(Race race, Sex sex)
        {
            var name = RaceRepository.GetUMARace(race, sex);

            if (name == null)
            {
                throw new ArgumentException(
                    $"Cannot determine UMA race: sex = '{sex}', race = '{race}'.");
            }

            var raceData = RaceRepository.RaceLibrary.GetRace(name);

            if (raceData == null)
            {
                throw new ArgumentException($"Unknown UMA race: '{name}'.");
            }

            Avatar.ChangeRace(raceData);
        }

        private struct DNAKey
        {
            public readonly int Hash;

            public readonly int Index;

            public DNAKey(int hash, int index)
            {
                Hash = hash;
                Index = index;
            }
        }
    }
}