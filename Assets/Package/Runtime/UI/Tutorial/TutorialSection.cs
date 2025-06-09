using System;
using UnityEngine;

namespace VARLab.Velcro
{
    [Serializable]
    public class TutorialSection
    {
        [Tooltip("The [Image] VisualElement of the tutorial")]
        public Sprite Image;

        [TextArea(1, 3), Tooltip("The [Header] label of the tutorial")]
        public string Header;

        [TextArea(1, 10), Tooltip("The [Description] label of the tutorial")]
        public string Description;
        
        [TextArea(1, 3), Tooltip("The text of the primary button (Next button in the bottom right)")]
        public string PrimaryBtnText = "Next";
    }
}