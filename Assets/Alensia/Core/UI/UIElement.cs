using System;
using System.Collections.Generic;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.EventSystems;
using Object = UnityEngine.Object;

namespace Alensia.Core.UI
{
    [ExecuteInEditMode]
    public abstract class UIElement : UIBehaviour, IUIElement
    {
        public string Name => name;

        public IUIContext Context { get; private set; }

        public bool Visible
        {
            get { return enabled; }
            set { enabled = value; }
        }

        public RectTransform RectTransform => _rectTransform ?? (_rectTransform = GetComponent<RectTransform>());

        public Transform Transform => transform;

        public GameObject GameObject => gameObject;

        public UniRx.IObservable<Unit> OnShow => this.OnEnableAsObservable();

        public UniRx.IObservable<Unit> OnHide => this.OnDisableAsObservable();

        public UniRx.IObservable<bool> OnVisibilityChange =>
            OnShow.Select(_ => true).Merge(OnHide.Select(_ => false));

        protected virtual IList<Object> Peers => new List<Object>();

        protected virtual HideFlags PeerFlags => HideFlags.HideInHierarchy | HideFlags.HideInInspector;

        protected virtual bool InitializeInEditor => false;

        private RectTransform _rectTransform;

        public virtual void Initialize(IUIContext context)
        {
            Assert.IsNotNull(context, "context != null");

            lock (this)
            {
                if (Context != null)
                {
                    throw new InvalidOperationException(
                        $"The component has already been initialized: '{Name}'.");
                }
            }

            Context = context;
        }

        protected override void OnEnable()
        {
            base.OnEnable();

            if (Context != null || Application.isPlaying || !InitializeInEditor) return;

            var context = CreateEditorUIContext();

            if (context != null)
            {
                Initialize(context);
            }
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            if (!Application.isPlaying)
            {
                Context = null;
            }
        }

        protected virtual EditorUIContext CreateEditorUIContext()
        {
            return Resources.Load<EditorUIContext>("UI/EditorUIContext");
        }

        protected virtual void UpdateEditor()
        {
        }

        protected virtual void ApplyHideFlags()
        {
            foreach (var peer in Peers)
            {
                peer.hideFlags = PeerFlags;
            }
        }

//TODO It seems that those 'magic methods' of MonoBehaviour confuse the hell out of the compiler, so it we remove this method, the player build fails.  
#pragma warning disable 108,114
        protected virtual void Awake()
        {
            ApplyHideFlags();
        }

        protected virtual void Reset()
        {
            ApplyHideFlags();
        }

        protected virtual void OnValidate()
        {
            UpdateEditor();
            ApplyHideFlags();
        }
#pragma warning restore 108,114

        public void Show() => Visible = true;

        public void Hide() => Visible = false;
    }
}