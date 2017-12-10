﻿using System;
using Malee;
using UnityEngine;

namespace Alensia.Core.Common
{
    [Serializable]
    internal class StringList : ReorderableArray<string>
    {
        public StringList() : base(0)
        {
        }

        public StringList(int length): base(length)
        {
        }
    }

    [Serializable]
    internal class Texture2DList : ReorderableArray<Texture2D>
    {
        public Texture2DList() : base(0)
        {
        }

        public Texture2DList(int length): base(length)
        {
        }
    }
}