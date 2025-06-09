using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace VARLab.Velcro.Demos
{
    public class IconsDemoHelper : MonoBehaviour
    {
        const string ClickedV1ClassName = "icon-square-rounded-clicked";
        const string ClickedV2ClassName = "icon-square-rounded-clickedv2";
        const string NoBorderClickedClassName = "icon-noborders-clicked";

        [SerializeField]
        private List<string> clickedV1Icons;

        [SerializeField]
        private List<string> clickedV2Icons;

        [SerializeField]
        private List<string> noBorderClickedIcons;

        void Start()
        {
            UIDocument doc = GetComponent<UIDocument>();
            VisualElement root = doc.rootVisualElement;
            foreach (string str in clickedV1Icons) {
                root.Q<VisualElement>(str).Q<Button>().AddToClassList(ClickedV1ClassName);
            }
            foreach (string str in clickedV2Icons) {
                root.Q<VisualElement>(str).Q<Button>().AddToClassList(ClickedV2ClassName);
            }
            foreach (string str in noBorderClickedIcons) {
                root.Q<VisualElement>(str).Q<Button>().AddToClassList(NoBorderClickedClassName);
            }
        }
    }
}
