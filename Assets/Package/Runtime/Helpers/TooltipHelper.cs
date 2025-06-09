using UnityEngine;
using UnityEngine.UIElements;

namespace VARLab.Velcro
{
    public static class TooltipHelper
    {
        /// <summary>
        /// Editor Window has origin 0, 0 in the top left with Y increasing downwards.
        /// UnityEngine.Screen has origin 0,0 in the bottom left with Y increasing upwards
        /// This method converts that
        /// </summary>
        /// <param name="sourcePosition">The source position of the mouse pointer</param>
        /// <param name="panelSize">The size of the root panel</param>
        /// <param name="screenWidth">Width of the unity play window</param>
        /// <param name="screenHeight">Height of the unity play window</param>
        /// <returns>The final position not including margin offsets</returns>
        public static Vector2 ConvertToEditorCoordinates(Rect sourcePosition, Vector2 panelSize, float screenWidth, float screenHeight)
        {
            Vector2 screenPosition = new Vector2(sourcePosition.x / screenWidth, sourcePosition.y / screenHeight);
            Vector2 editorPosition = new Vector2(screenPosition.x, 1f - screenPosition.y);
            Vector2 finalPosition = editorPosition * panelSize;
            return finalPosition;
        }

        /// <summary>
        /// Gets the center position of a GameObject from the view of the camera.
        /// </summary>
        /// <param name="camera">The camera currently viewing the GameObject.</param>
        /// <param name="renderer">The renderer component of the GameObject.</param>
        /// <returns>The center position of the object in screen space.</returns>
        public static Vector2 GetObjectCenterInScreenSpace(Camera camera, Renderer renderer)
        {
            Bounds bigBounds = renderer.bounds;
            Vector3[] screenSpaceCorners = new Vector3[8];

            if (camera == null)
            {
                camera = Camera.main;
            }

            screenSpaceCorners[0] = camera.WorldToScreenPoint(new Vector3(bigBounds.center.x + bigBounds.extents.x, bigBounds.center.y + bigBounds.extents.y, bigBounds.center.z + bigBounds.extents.z));
            screenSpaceCorners[1] = camera.WorldToScreenPoint(new Vector3(bigBounds.center.x + bigBounds.extents.x, bigBounds.center.y + bigBounds.extents.y, bigBounds.center.z - bigBounds.extents.z));
            screenSpaceCorners[2] = camera.WorldToScreenPoint(new Vector3(bigBounds.center.x + bigBounds.extents.x, bigBounds.center.y - bigBounds.extents.y, bigBounds.center.z + bigBounds.extents.z));
            screenSpaceCorners[3] = camera.WorldToScreenPoint(new Vector3(bigBounds.center.x + bigBounds.extents.x, bigBounds.center.y - bigBounds.extents.y, bigBounds.center.z - bigBounds.extents.z));
            screenSpaceCorners[4] = camera.WorldToScreenPoint(new Vector3(bigBounds.center.x - bigBounds.extents.x, bigBounds.center.y + bigBounds.extents.y, bigBounds.center.z + bigBounds.extents.z));
            screenSpaceCorners[5] = camera.WorldToScreenPoint(new Vector3(bigBounds.center.x - bigBounds.extents.x, bigBounds.center.y + bigBounds.extents.y, bigBounds.center.z - bigBounds.extents.z));
            screenSpaceCorners[6] = camera.WorldToScreenPoint(new Vector3(bigBounds.center.x - bigBounds.extents.x, bigBounds.center.y - bigBounds.extents.y, bigBounds.center.z + bigBounds.extents.z));
            screenSpaceCorners[7] = camera.WorldToScreenPoint(new Vector3(bigBounds.center.x - bigBounds.extents.x, bigBounds.center.y - bigBounds.extents.y, bigBounds.center.z - bigBounds.extents.z));

            float min_x = screenSpaceCorners[0].x;
            float min_y = screenSpaceCorners[0].y;
            float max_x = screenSpaceCorners[0].x;
            float max_y = screenSpaceCorners[0].y;

            for (int i = 1; i < screenSpaceCorners.Length; i++)
            {
                if (screenSpaceCorners[i].x < min_x)
                {
                    min_x = screenSpaceCorners[i].x;
                }
                if (screenSpaceCorners[i].y < min_y)
                {
                    min_y = screenSpaceCorners[i].y;
                }
                if (screenSpaceCorners[i].x > max_x)
                {
                    max_x = screenSpaceCorners[i].x;
                }
                if (screenSpaceCorners[i].y > max_y)
                {
                    max_y = screenSpaceCorners[i].y;
                }
            }

            Rect rect = Rect.MinMaxRect(min_x, min_y, max_x, max_y);
            return new Vector2(rect.x + rect.width / 2f, rect.y + rect.height / 2f);
        }

        /// <summary>
        /// Calculates the center position of the visual element based on the provided VisualElement
        /// </summary>
        /// <param name="element"></param>
        /// <returns>Center position of the provided visual element</returns>
        public static Vector2 CalculateVisualElementCenter(VisualElement element)
        {
            if (element == null)
            {
                return Vector2.zero;
            }

            Vector2 position = element.worldTransform.GetPosition();
            float width = element.layout.width;
            float height = element.layout.height;

            position.x += width / 2f;
            position.y += height / 2f;

            return position;
        }
    }
}