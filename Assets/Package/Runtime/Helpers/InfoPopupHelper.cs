using System;
using UnityEngine.UIElements;
using UnityEngine;

namespace VARLab.Velcro
{
    public static class InfoPopupHelper
    {
        const float UI_MARGIN = 20f;
        const float WINDOW_WIDTH = 550f;
        const float WINDOW_HEIGHT = 550f;

        /// <summary>
        /// Moves a small info popup UI element beside on object.
        /// Popup will appear to the left or right of the object depending on the object's position on screen.
        /// </summary>
        public static void MovePopupBesideObject(VisualElement popup, GameObject selectedObject)
        {
            float canvasWidth = popup.parent.resolvedStyle.width;
            float canvasHeight = popup.parent.resolvedStyle.height;

            (popup.style.left, popup.style.top) = GetWindowLocation(selectedObject, new Vector2(canvasWidth, canvasHeight));
        }

        /// <summary>
        /// Determines where the popup will be placed relative to game object passed in parameter,
        /// gathers results from sub-methods and passes to CalculateWindowLocation as parameters
        /// </summary>
        /// <param name="selectedObject"></param>
        /// <param name="uiCanvasSize"></param>
        /// <returns></returns>
        private static (float, float) GetWindowLocation(GameObject selectedObject, Vector2 uiCanvasSize)
        {
            Bounds bounds = GetGameObjectBounds(selectedObject);
            Rect rect = BoundsInScreenSpace(bounds);
            Vector2 screen = new Vector2(Screen.width, Screen.height);
            ScreenZone screenZone = GetScreenZone(rect.center, screen);
            
            return CalculateWindowLocation(rect, screenZone, screen, uiCanvasSize);
        }

        /// <summary>
        /// Returns bounds for the object that the popup is going to be placed beside,
        /// tries for renderer, collider and mesh filter bounds of the object (in that order)
        /// </summary>
        /// <param name="selectedObject"></param>
        /// <returns></returns>
        /// <exception cref="MissingComponentException"></exception>
        private static Bounds GetGameObjectBounds(GameObject selectedObject)
        {
            if (selectedObject.TryGetComponent(out Renderer renderer))
            {
                return renderer.bounds;
            }
            else if (selectedObject.TryGetComponent(out Collider collider))
            {
                return collider.bounds;
            }
            else if (selectedObject.TryGetComponent(out MeshFilter meshFilter))
            {
                return meshFilter.mesh.bounds;
            }
            else
            {
                throw new MissingComponentException("The selected game object does not have " +
                    "a component (Collider, MeshFilter.Mesh, Renderer) with UnityEngine.Bounds");
            }
        }

        /// <summary>
        /// Converts object's 3D bounds (from GetBoundsWorldPosition) into 2D screen space from world space
        /// </summary>
        /// <param name="bounds"></param>
        /// <returns></returns>
        private static Rect BoundsInScreenSpace(Bounds bounds)
        {
            Camera mainCamera = Camera.main;
            Vector3[] worldSpaceCorners = GetBoundsWorldPosition(bounds);

            Vector3[] screenSpaceCorners = new Vector3[8];
            for (int i = 0; i < 8; i++)
            {
                screenSpaceCorners[i] = mainCamera.WorldToScreenPoint(worldSpaceCorners[i]);
            }

            return GetBoundsScreenSpaceRange(screenSpaceCorners);
        }

        /// <summary>
        /// Computes corners of 3D box of world space positions
        /// </summary>
        /// <param name="bounds"></param>
        /// <returns></returns>
        private static Vector3[] GetBoundsWorldPosition(Bounds bounds)
        {
            Vector3[] vectors = new Vector3[8];

            vectors[0] = new Vector3(bounds.center.x + bounds.extents.x, bounds.center.y + bounds.extents.y, bounds.center.z + bounds.extents.z);
            vectors[1] = new Vector3(bounds.center.x + bounds.extents.x, bounds.center.y + bounds.extents.y, bounds.center.z - bounds.extents.z);
            vectors[2] = new Vector3(bounds.center.x + bounds.extents.x, bounds.center.y - bounds.extents.y, bounds.center.z + bounds.extents.z);
            vectors[3] = new Vector3(bounds.center.x + bounds.extents.x, bounds.center.y - bounds.extents.y, bounds.center.z - bounds.extents.z);
            vectors[4] = new Vector3(bounds.center.x - bounds.extents.x, bounds.center.y + bounds.extents.y, bounds.center.z + bounds.extents.z);
            vectors[5] = new Vector3(bounds.center.x - bounds.extents.x, bounds.center.y + bounds.extents.y, bounds.center.z - bounds.extents.z);
            vectors[6] = new Vector3(bounds.center.x - bounds.extents.x, bounds.center.y - bounds.extents.y, bounds.center.z + bounds.extents.z);
            vectors[7] = new Vector3(bounds.center.x - bounds.extents.x, bounds.center.y - bounds.extents.y, bounds.center.z - bounds.extents.z);
            
            return vectors;
        }

        // Really not sure what this does, appears to compute the minimum possible sized rectangle that fit the object clicked?
        // Or maybe something else, this might need to be reviewed to be sure
        private static Rect GetBoundsScreenSpaceRange(Vector3[] screenSpaceCorners)
        {
            if (screenSpaceCorners.Length != 8)
            {
                throw new ArgumentException("The Vector3 array must have exactly 8 elements.");
            }

            float min_x = screenSpaceCorners[0].x;
            float min_y = screenSpaceCorners[0].y;
            float max_x = screenSpaceCorners[0].x;
            float max_y = screenSpaceCorners[0].y;
            for (int i = 1; i < 8; i++)
            {
                // Old if-statement blocks just checked which was lower for each block (of 4 total, one for each float defined above)
                // so switched to cleaner Math library calls here
                min_x = Math.Min(min_x, screenSpaceCorners[i].x);
                min_y = Math.Min(min_y, screenSpaceCorners[i].y);
                max_x = Math.Max(max_x, screenSpaceCorners[i].x);
                max_y = Math.Max(max_y, screenSpaceCorners[i].y);
            }

            return Rect.MinMaxRect(min_x, min_y, max_x, max_y);
        }

        /// <summary>
        /// Determines whether to place popup on left or right side of the screen
        /// </summary>
        /// <param name="point"></param>
        /// <param name="screenSize"></param>
        /// <returns></returns>
        private static ScreenZone GetScreenZone(Vector2 point, Vector2 screenSize)
        {
            return (point.x < Screen.width / 2f) ? ScreenZone.Left : ScreenZone.Right;
        }

        /// <summary>
        /// Uses other method computations to determine where popup should be placed (targets X and Y in return statement),
        /// which the original calling method then applies to the popup's style attributes
        /// </summary>
        /// <param name="rect"></param>
        /// <param name="screenSize"></param>
        /// <param name="uiCanvasSize"></param>
        /// <param name="zone"></param>
        /// <returns></returns>
        private static (float, float) CalculateWindowLocation(Rect rect, ScreenZone zone, Vector2 screenSize, Vector2 uiCanvasSize)
        {
            // Converting the left and right bounds of the object from screen space to UI space (uiCanvasSize used here)
            float uiObjectLeft = rect.xMin / screenSize.x * uiCanvasSize.x;
            float uiObjectRight = rect.xMax / screenSize.x * uiCanvasSize.x;

            // Y axis starts at bottom on screen, but UI starts at top, so it is flipped here
            float uiObjectCenterY = (1f - rect.center.y / screenSize.y) * uiCanvasSize.y;

            float targetX = 0f;
            float targetY = uiObjectCenterY - WINDOW_HEIGHT / 2f; // No longer setting Y Target to be 0 since the UI space is now being used

            switch (zone)
            {
                //UI appear right side of the object and align top
                case ScreenZone.Right:
                    targetX = uiObjectLeft - WINDOW_WIDTH - UI_MARGIN;
                    if (targetX < UI_MARGIN)
                        targetX = uiObjectRight + UI_MARGIN;
                    break;

                //UI appear left side of the object and align top
                case ScreenZone.Left:
                    targetX = uiObjectRight + UI_MARGIN;
                    if (targetX > uiCanvasSize.x - WINDOW_WIDTH - UI_MARGIN)
                        targetX = uiObjectLeft - WINDOW_WIDTH - UI_MARGIN;
                    break;
            }

            //Move UI back into the screen if part of the UI is off the screen.
            //Note using Math.clamp now, old if-statement block structure did exactly the same thing, this is just cleaner
            targetX = Math.Clamp(targetX, UI_MARGIN, uiCanvasSize.x - WINDOW_WIDTH - UI_MARGIN);
            targetY = Math.Clamp(targetY, UI_MARGIN, uiCanvasSize.y - WINDOW_HEIGHT - UI_MARGIN);

            return (targetX, targetY);
        }
    }
}

