using UnityEngine;

namespace OutlinesPack
{
    [System.Serializable]
    public class BlurOutlineSettings
    {
        [SerializeField, ColorUsage(true, true)]
        public Color color = Color.green;

        [SerializeField, Range(0, 32)]
        public float width = 10;

        [SerializeField]
        public bool blur = true;

        [SerializeField, Range(0, 64)]
        public float intensity = 1;

    }
}