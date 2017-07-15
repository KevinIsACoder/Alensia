﻿using System.Collections.Generic;
using System.Linq;
using Alensia.Core.Common;
using UniRx;
using UnityEngine;
using UnityEngine.Assertions;

namespace Alensia.Core.Control
{
    public class Controller : BaseActivatable, IController
    {
        public IReadOnlyList<IControl> Controls { get; }

        private readonly IDictionary<string, IControl> _controls;

        private bool _active;

        //TODO Change to IList later: https://github.com/modesttree/Zenject/issues/281
        public Controller(List<IControl> controls)
        {
            Assert.IsNotNull(controls, "controls != null");
            Assert.IsTrue(controls.Any(), "controls.Any()");

            Controls = controls.AsReadOnly();

            _controls = new Dictionary<string, IControl>();

            foreach (var control in Controls)
            {
                _controls.Add(control.Name, control);
            }

            OnActiveStateChange.Subscribe(ChangeControlStatus).AddTo(this);

            OnInitialize.Subscribe(_ => Activate()).AddTo(this);
            OnDispose.Subscribe(_ => Deactivate()).AddTo(this);

            //TODO Find a better way to handle cursor mode & visibility.
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        public bool Contains(string key) => _controls.ContainsKey(key);

        public IControl this[string key] => _controls.ContainsKey(key) ? _controls[key] : null;

        public virtual void EnableControl(string name)
        {
            var control = this[name];

            if (control != null && !control.Active) control.Activate();
        }

        public virtual void DisableControl(string name)
        {
            var control = this[name];

            if (control != null && control.Active) control.Deactivate();
        }

        private void ChangeControlStatus(bool active)
        {
            foreach (var control in Controls)
            {
                control.Active = active;
            }
        }
    }
}