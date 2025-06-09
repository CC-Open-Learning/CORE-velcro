using UnityEngine;

namespace VARLab.Velcro.Demos
{
    public class InfoPopupDemoObject : MonoBehaviour
    {
        [SerializeField] private InfoPopupSmall infoPopup;
        [SerializeField] private InfoPopupSO infoPopupSO;

        private void OnMouseDown()
        {
            infoPopup.HandleDisplayUI(infoPopupSO, gameObject);
        }
    }
}