using System.Collections.Generic;
using UnityEngine;

namespace VARLab.Velcro
{
    [CreateAssetMenu(fileName = "TutorialSO", menuName = "ScriptableObjects/TutorialSO")]
    public class TutorialSO : ScriptableObject
    {
        [Header("Content"), Space(5)]
        [TextArea(1, 3), Tooltip("The [Name] label of the tutorial")]
        public string Name;

        [TextArea(1, 3), Tooltip("The text of the secondary button (Previous button in the bottom right)")]
        public string SecondaryBtnText = "Previous";

        [TextArea(1, 3), Tooltip("The text of the tertiary button (Skip button in the bottom left)")]
        public string TertiaryBtnText = "Skip";

        [Header("Section Content"), Space(5)]
        [Tooltip("The [Header]/[Description] label and [Image] VisualElement for each tutorial page")]
        public List<TutorialSection> TutorialSections = new List<TutorialSection>();

        [Header("Additional Options"), Space(5)]
        [Tooltip("Whether the background behind the UI is dimmed or not")]
        public bool IsBackgroundDimmed = true;
    }
}