using UnityEngine;
using UnityEngine.UIElements;

namespace VARLab.Velcro.Demos
{
    public class InfoPopupDemo : Toolbar
    {
        [SerializeField] private UIDocument infoPopup;

        private void Start()
        {
            SetupBaseToolbar();
        }
    }
}