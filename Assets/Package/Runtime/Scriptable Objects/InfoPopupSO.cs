using UnityEngine;

namespace VARLab.Velcro
{
    [CreateAssetMenu(fileName = "InfoPopupSO", menuName = "ScriptableObjects/InfoPopupSO")]
    public class InfoPopupSO : ScriptableObject
    {
        [Header("Text Elemets")]
        [TextArea(1, 10), Tooltip("The [Title] label on the Info Popup.")]
        public string Title;

        [Header("Image Elements"), Space(5)]
        [Tooltip("The sprite to be displayed on the body of the info popup.")]
        public Sprite Image;
    }
}
