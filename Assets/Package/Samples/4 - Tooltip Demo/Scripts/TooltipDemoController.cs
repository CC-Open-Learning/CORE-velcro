using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

namespace VARLab.Velcro.Demos
{
    public class TooltipDemoController : MonoBehaviour
    {
        public UnityEvent<VisualElement, TooltipType, string, FontSize> DisplayTooltip;
        public UnityEvent HideTooltip;

        private VisualElement tooltipRight;
        private VisualElement tooltipTop;
        private VisualElement tooltipBottom;
        private VisualElement tooltipLeft;

        private void Start()
        {
            VisualElement root = gameObject.GetComponent<UIDocument>().rootVisualElement;

            DisplayTooltip ??= new UnityEvent<VisualElement, TooltipType, string, FontSize>();
            HideTooltip ??= new UnityEvent();

            tooltipRight = root.Q("TooltipRight");
            tooltipTop = root.Q("TooltipTop");
            tooltipBottom = root.Q("TooltipBottom");
            tooltipLeft = root.Q("TooltipLeft");

            tooltipRight.RegisterCallback<PointerEnterEvent>(OnEnteredRight);
            tooltipTop.RegisterCallback<PointerEnterEvent>(OnEnteredTop);
            tooltipBottom.RegisterCallback<PointerEnterEvent>(OnEnteredBottom);
            tooltipLeft.RegisterCallback<PointerEnterEvent>(OnEnteredLeft);

            tooltipRight.RegisterCallback<PointerLeaveEvent>(OnExitedUI);
            tooltipTop.RegisterCallback<PointerLeaveEvent>(OnExitedUI);
            tooltipBottom.RegisterCallback<PointerLeaveEvent>(OnExitedUI);
            tooltipLeft.RegisterCallback<PointerLeaveEvent>(OnExitedUI);
        }

        private void OnEnteredRight(PointerEnterEvent e)
        {
            DisplayTooltip?.Invoke(tooltipRight, TooltipType.Right, "Pointing right", FontSize.Medium);
        }

        private void OnEnteredTop(PointerEnterEvent e)
        {
            DisplayTooltip?.Invoke(tooltipTop, TooltipType.Top, "Pointing up with some longer text", FontSize.Medium);
        }

        private void OnEnteredBottom(PointerEnterEvent e)
        {
            DisplayTooltip?.Invoke(tooltipBottom, TooltipType.Bottom, "Pointing down with larger text", FontSize.Large);
        }

        private void OnEnteredLeft(PointerEnterEvent e)
        {
            DisplayTooltip?.Invoke(tooltipLeft, TooltipType.Left, "Pointing left", FontSize.Medium);
        }

        private void OnExitedUI(PointerLeaveEvent e)
        {
            HideTooltip?.Invoke();
        }

        private void OnDestroy()
        {
            tooltipRight.UnregisterCallback<PointerEnterEvent>(OnEnteredRight);
            tooltipTop.UnregisterCallback<PointerEnterEvent>(OnEnteredTop);
            tooltipBottom.UnregisterCallback<PointerEnterEvent>(OnEnteredBottom);
            tooltipLeft.UnregisterCallback<PointerEnterEvent>(OnEnteredLeft);

            tooltipRight.UnregisterCallback<PointerLeaveEvent>(OnExitedUI);
            tooltipTop.UnregisterCallback<PointerLeaveEvent>(OnExitedUI);
            tooltipBottom.UnregisterCallback<PointerLeaveEvent>(OnExitedUI);
            tooltipLeft.UnregisterCallback<PointerLeaveEvent>(OnExitedUI);
        }
    }
}