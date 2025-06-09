using UnityEngine;

namespace VARLab.Velcro
{
    /// <summary>
    /// Intended use of this class is to add this component to any GameObject that requires a tooltip. Set the 
    /// tooltip properties to your desired look and feel and reference the TooltipUI prefab
    /// </summary>
    [RequireComponent(typeof(Collider))]
    public class TooltipObject : MonoBehaviour
    {
        [SerializeField, Tooltip("Reference to the tooltip UI component")]
        private TooltipUI tooltipUI;
        [SerializeField, Tooltip("Direction the tooltip will point (Up, Right, Down, Left")]
        private TooltipType tooltipType;
        [SerializeField, Tooltip("Size of the tooltip's font (Medium = 24, Large = 28)")]
        private FontSize tooltipFontSize = FontSize.Medium;
        [SerializeField, Tooltip("Whether the tooltip should follow the cursor while it is hovering over the object")]
        private bool followCursor = true;
        [SerializeField, Tooltip("The Camera that is viewing the 3D object. Only needed if followCursor is set to false.\nIf set to null, Camera.main will be used instead.")]
        private Camera mainCamera;
        [SerializeField, Tooltip("Time in seconds to wait until tooltip is displayed on the GameObject")]
        private float tooltipHoverDelay = 0.0f;
        [SerializeField] private string tooltipText;

        public bool IsTooltipEnabled = true;
        private Renderer objectRender;
        private float hoverDuration = 0;
        private bool isTooltipDisplayed = false;

        private const float DefaultHoverDelay = 0.0f;

        private void Start()
        {
            mainCamera ??= Camera.main;
            objectRender = GetComponent<Renderer>();
            tooltipHoverDelay = Mathf.Abs(tooltipHoverDelay);
        }

        private void OnMouseEnter()
        {
            if (!IsTooltipEnabled) { return; }

            if (tooltipHoverDelay == DefaultHoverDelay)
            {
                isTooltipDisplayed = true;
                tooltipUI.HandleDisplayUI(tooltipType, tooltipText, tooltipFontSize);
            }

            hoverDuration = 0;
        }

        private void OnMouseExit()
        {
            if (!IsTooltipEnabled) { return; }

            tooltipUI.CloseTooltip();
            isTooltipDisplayed = false;
        }

        /// <summary>
        /// Update the tooltip position based on the cursor while the mouse is over the object's collider
        /// </summary>
        private void OnMouseOver()
        {
            if (!IsTooltipEnabled)
            {
                if (isTooltipDisplayed)
                {
                    tooltipUI.CloseTooltip();
                    isTooltipDisplayed = false;
                }

                return;
            }

            hoverDuration += Time.deltaTime;

            if (hoverDuration >= tooltipHoverDelay && !isTooltipDisplayed)
            {
                tooltipUI.HandleDisplayUI(null, tooltipType, tooltipText, tooltipFontSize);
                isTooltipDisplayed = true;
            }

            if (followCursor)
            {
                UpdateTooltipPosition(Input.mousePosition);
            }
            else
            {
                Vector2 objectCenter = TooltipHelper.GetObjectCenterInScreenSpace(mainCamera, objectRender);
                UpdateTooltipPosition(objectCenter);
            }
        }

        /// <summary>
        /// Update the tooltip's position based on the mouse position
        /// </summary>
        /// <param name="position"></param>
        public void UpdateTooltipPosition(Vector2 position)
        {
            Rect sourcePosition = new Rect(position, Vector2.zero);
            Vector2 newPosition = TooltipHelper.ConvertToEditorCoordinates(sourcePosition, tooltipUI.Root.panel.visualTree.layout.size, Screen.width, Screen.height);
            tooltipUI.ApplyOffsetAndOrigin(tooltipType, ref newPosition);
            tooltipUI.ForceSetPosition(newPosition);
        }

        /// <summary>
        /// Public setter method to change the tooltip text that is displayed for this instance of TooltipObject
        /// </summary>
        /// <param name="message"></param>
        public void SetTooltipText(string message)
        {
            if (message == null) { return; }

            tooltipText = message;
        }

        private void OnDisable()
        {
            if (!IsTooltipEnabled) { return; }
            if (tooltipUI) { tooltipUI.CloseTooltip(); }
            
            isTooltipDisplayed = false;
        }
    }
}