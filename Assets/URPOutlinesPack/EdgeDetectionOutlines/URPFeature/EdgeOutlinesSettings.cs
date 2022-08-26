using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OutlinesPack
{
    [System.Serializable]
    public class EdgeOutlinesSettings
    {
        [SerializeField, ColorUsage(true, true)]
        public Color Color = Color.green;

        [SerializeField, Range(0, 5)]
        public float Width = 2;

        [SerializeField, Range(0.2f, 40f)]
        public float DepthThreshold = 10;

        [SerializeField, Range(0f, 3f)]
        public float NormalThreshold = 1;

    }
}