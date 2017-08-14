using System.Collections.Generic;
using Alensia.Core.UI.Property;
using UniRx;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

namespace Alensia.Core.UI
{
    public class Panel : UIContainer
    {
        public ImageAndColor Background
        {
            get { return _background.Value; }
            set
            {
                Assert.IsNotNull(value, "value != null");

                _background.Value = value;
            }
        }

        protected Image PeerImage => _peerImage;

        protected override IList<Object> Peers
        {
            get
            {
                var peers = base.Peers;

                if (PeerImage != null) peers.Add(PeerImage);

                return peers;
            }
        }

        [SerializeField] private ImageAndColorReactiveProperty _background;

        [SerializeField, HideInInspector] private Image _peerImage;

        protected override void InitializePeers()
        {
            base.InitializePeers();

            _peerImage = GetComponentInChildren<Image>();

            _background
                .Subscribe(b => b.Update(PeerImage))
                .AddTo(this);
        }

        protected override void ValidateProperties()
        {
            base.ValidateProperties();

            Background.Update(PeerImage);
        }

        protected override void Reset()
        {
            base.Reset();

            var source = CreateInstance();

            Background.Load(source.PeerImage);
            Background.Update(PeerImage);

            DestroyImmediate(source.gameObject);
        }

        public static Panel CreateInstance()
        {
            var prefab = Resources.Load<GameObject>("UI/Components/Panel");

            Assert.IsNotNull(prefab, "prefab != null");

            return Instantiate(prefab).GetComponent<Panel>();
        }
    }
}