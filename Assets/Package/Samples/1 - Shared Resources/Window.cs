using UnityEngine;
using UnityEngine.UIElements;

namespace VARLab.Velcro.Demos
{
    public class Window : MonoBehaviour
    {
        private VisualElement root;

        void Start()
        {
            root = gameObject.GetComponent<UIDocument>().rootVisualElement;

            if (root.Q<Button>("CloseBtn") != null)
            {
                Button closeBtn = root.Q<Button>("CloseBtn");
                closeBtn.clicked += Hide;
            }

            Hide();
        }

        public void Show()
        {
            root.Show();
        }

        public void Hide()
        {
            root.Hide();
        }
    }
}