using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

namespace VARLab.Velcro
{
    /// <summary>
    /// Tooltips come in 4 different types
    ///     - Top: Tooltip is offset from the bottom of the UI/pointer/gameObject and pointing up
    ///     - Right: Tooltip is offset from the left of the UI/pointer/gameObject and pointing right
    ///     - Bottom: Tooltip is offset from the top of the UI/pointer/gameObject and pointing down
    ///     - Left: Tooltip is offset from the right of the UI/pointer/gameObject and pointing left
    ///     
    /// Intended use of this class is to connect TooltipUI.HandleDisplayUI() to a UnityEvent 
    /// in your DLX. Methods changing content, position, fade in/out, and show/hide have been exposed 
    /// if DLX would prefer to serialize and control individual stages of the tooltip
    /// </summary>
    [RequireComponent(typeof(UIDocument))]
    public class TooltipUI : MonoBehaviour, IUserInterface
    {
        [SerializeField, Tooltip("Offset of the tooltip's position in pixels")]
        private float positionOffset = 10.0f;
        [SerializeField, Tooltip("Time in seconds for tooltip to go from 0->1 or 1->0 opacity")]
        private float fadeDuration = 0.25f;

        [HideInInspector] public VisualElement Root { private set; get; }
        public UnityEvent OnTooltipShown;
        public UnityEvent OnTooltipHidden;

        private VisualElement tooltip;
        private Label label;

        private static readonly Dictionary<TooltipType, string> tooltipClasses = new Dictionary<TooltipType, string>()
        {
            [TooltipType.Top] = "tooltip-top",
            [TooltipType.Bottom] = "tooltip-bottom",
            [TooltipType.Left] = "tooltip-left",
            [TooltipType.Right] = "tooltip-right"
        };

        private const float Transparent = 0f;
        private const float Opaque = 1f;

        private void Start()
        {
            Root = gameObject.GetComponent<UIDocument>().rootVisualElement;
            tooltip = Root.Q("Container");
            label = tooltip.Q<Label>("Label");

            OnTooltipShown ??= new UnityEvent();
            OnTooltipHidden ??= new UnityEvent();

            //Make sure position offset is positive
            positionOffset = Mathf.Abs(positionOffset);
            tooltip.style.transitionDuration = new List<TimeValue>() { new TimeValue(fadeDuration) };
            tooltip.Hide();
        }

        /// <summary>
        /// This method offers the most control and is intended for use with UI Toolkit. It is a single 
        /// access public method that changes all properties of the tooltip. This includes the parent 
        /// VisualElement, tooltip direction, message, and font size
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="tooltipType"></param>
        /// <param name="message"></param>
        /// <param name="fontSize"></param>
        public void HandleDisplayUI(VisualElement parent, TooltipType tooltipType, string message, FontSize fontSize)
        {
            SetUpTooltip(tooltipType, message, fontSize);
            StartCoroutine(SetPosition(parent, tooltipType));
        }

        /// <summary>
        /// This method is intended for use with UI Toolkit. It is a single access public method 
        /// that changes all properties of the tooltip excluding font size. The default font size 
        /// (Medium = 24) is used
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="tooltipType"></param>
        public void HandleDisplayUI(VisualElement parent, TooltipType tooltipType, string message)
        {
            SetUpTooltip(tooltipType, message, FontSize.Medium);
            StartCoroutine(SetPosition(parent, tooltipType));
        }

        /// <summary>
        /// This method is intended for use with TooltipObject. It is a single access public method 
        /// changes tooltip direction, message, and font size
        /// </summary>
        /// <param name="tooltipType"></param>
        /// <param name="message"></param>
        public void HandleDisplayUI(TooltipType tooltipType, string message, FontSize fontSize = FontSize.Medium)
        {
            SetUpTooltip(tooltipType, message, fontSize);
        }

        /// <summary>
        /// This method handles the assigning of tooltip properties from the various HandleDisplayUI overloads
        /// </summary>
        /// <param name="tooltipType"></param>
        /// <param name="message"></param>
        /// <param name="fontSize"></param>
        private void SetUpTooltip(TooltipType tooltipType, string message, FontSize fontSize)
        {
            ClearClasses();
            SetContent(tooltipType, message);
            label.SetElementFontSize(fontSize);
            StopAllCoroutines();
            StartCoroutine(FadeIn());
        }

        /// <summary>
        /// There are 4 different types of tooltips that determine the direction of the tooltip 
        /// offset and the arrow image used as the background. This method changes the label and
        /// applies the correct USS class for the direction
        /// </summary>
        /// <param name="tooltipType"></param>
        /// <param name="message"></param>
        public void SetContent(TooltipType tooltipType, string message)
        {
            label.SetElementText(message);
            
            if (tooltipClasses.TryGetValue(tooltipType, out string tooltipClass))
            {
                tooltip.EnableInClassList(tooltipClass, true);
            }
        }

        /// <summary>
        /// Moves the tooltip to a new position
        /// </summary>
        /// <param name="position"></param>
        public void ForceSetPosition(Vector2 position)
        {
            tooltip.transform.position = position;
        }

        /// <summary>
        /// Moves the tooltip to the new position. The new position will be the edge of the parent 
        /// element's position, centered and offset from the direction of the tooltip
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="tooltipType"></param>
        public IEnumerator SetPosition(VisualElement parent, TooltipType tooltipType)
        {
            //Wait a frame so that the tooltip.layout property can be updated after setting the class
            yield return null;

            Vector2 tooltipPosition = TooltipHelper.CalculateVisualElementCenter(parent);
            ApplyOffsetAndOrigin(tooltipType, ref tooltipPosition, parent);
            ForceSetPosition(tooltipPosition);
        }

        /// <summary>
        /// Adjusts the given position based on the tooltip's correct origin point and position offset.
        /// </summary>
        /// <param name="tooltipType"></param>
        /// <param name="tooltipPosition"></param>
        /// <param name="parent"></param>
        public void ApplyOffsetAndOrigin(TooltipType tooltipType, ref Vector2 tooltipPosition, VisualElement parent = null)
        {
            //If no parent supplied, then this is for TooltipObject, and the center is converted
            //from mouse position or object center position
            float parentRadiusY = parent == null ? 0.0f : (parent.layout.size.y / 2);
            float parentRadiusX = parent == null ? 0.0f : (parent.layout.size.x / 2);

            switch (tooltipType)
            {
                case TooltipType.Top:
                    tooltipPosition.y += parentRadiusY + positionOffset;
                    tooltipPosition.x -= tooltip.layout.width / 2f;
                    break;

                case TooltipType.Bottom:
                    tooltipPosition.y -= parentRadiusY + positionOffset;
                    tooltipPosition -= new Vector2(tooltip.layout.width / 2f, tooltip.layout.height);
                    break;

                case TooltipType.Left:
                    tooltipPosition.x += parentRadiusX + positionOffset;
                    tooltipPosition.y -= tooltip.layout.height / 2f;
                    break;

                case TooltipType.Right:
                    tooltipPosition.x -= parentRadiusX + positionOffset;
                    tooltipPosition -= new Vector2(tooltip.layout.width, tooltip.layout.height / 2f);
                    break;

                default:
                    Debug.LogWarning("Tooltip.ApplyOffsetAndOrigin() - No tooltip type matched the available options");
                    break;
            }
        }

        /// <summary>
        /// Closes the tooltip with a fade out animation
        /// </summary>
        public void CloseTooltip()
        {
            StopAllCoroutines();
            StartCoroutine(FadeOut());
        }

        /// <summary>
        /// Tooltips fade in based on the properties serialized in the script. On enter, the 
        /// tooltip goes from 0 to 1 opacity in fadeDuration time
        /// </summary>
        public IEnumerator FadeIn()
        {
            Show();
            tooltip.style.opacity = Opaque;
            yield return null;
        }

        /// <summary>
        /// Tooltips fade out based on the properties serialized in the script. On exit, the 
        /// tooltip goes from 1 to 0 opacity in fadeDuration time
        /// </summary>
        public IEnumerator FadeOut()
        {
            tooltip.style.opacity = Transparent;
            yield return new WaitForSeconds(fadeDuration);
            Hide();
        }

        public void Show()
        {
            tooltip.Show();
            OnTooltipShown.Invoke();
        }

        public void Hide()
        {
            tooltip.Hide();
            OnTooltipHidden.Invoke();
        }

        /// <summary>
        /// Disables all directional classes from the tooltip
        /// </summary>
        public void ClearClasses()
        {
            foreach (string tooltipClass in tooltipClasses.Values)
            {
                tooltip.EnableInClassList(tooltipClass, false);
            }
        }
    }
}