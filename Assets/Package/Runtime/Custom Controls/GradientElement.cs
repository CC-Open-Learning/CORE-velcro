using UnityEngine;
using UnityEngine.UIElements;

namespace VARLab.Velcro
{
    [UxmlElement]
    public partial class GradientElement : VisualElement
    {
        private static CustomStyleProperty<Color> topLeftColourProperty = new CustomStyleProperty<Color>("--top-left-colour");
        private static CustomStyleProperty<Color> topRightColourProperty = new CustomStyleProperty<Color>("--top-right-colour");
        private static CustomStyleProperty<Color> bottomLeftColourProperty = new CustomStyleProperty<Color>("--bottom-left-colour");
        private static CustomStyleProperty<Color> bottomRightColourProperty = new CustomStyleProperty<Color>("--bottom-right-colour");

        private Color topLeftColour = Color.blue;
        private Color topRightColour = Color.green;
        private Color bottomLeftColour = Color.red;
        private Color bottomRightColour = Color.yellow;

        private static readonly Vertex[] vertices = new Vertex[4];
        private static readonly ushort[] indices = { 0, 1, 2, 2, 3, 0 };

        public GradientElement()
        {
            RegisterCallback<CustomStyleResolvedEvent>(CustomStylesResolved);
            generateVisualContent += OnGenerateVisualContent;
            MarkDirtyRepaint();
        }

        private static void CustomStylesResolved(CustomStyleResolvedEvent evt)
        {
            GradientElement element = evt.currentTarget as GradientElement;
            element.UpdateCustomStyles();
        }

        private void UpdateCustomStyles()
        {
            bool repaint = false;

            if (customStyle.TryGetValue(topLeftColourProperty, out topLeftColour))
            {
                repaint = true;
            }

            if (customStyle.TryGetValue(topRightColourProperty, out topRightColour))
            {
                repaint = true;
            }

            if (customStyle.TryGetValue(bottomLeftColourProperty, out bottomLeftColour))
            {
                repaint = true;
            }

            if (customStyle.TryGetValue(bottomRightColourProperty, out bottomRightColour))
            {
                repaint = true;
            }

            if (repaint)
            {
                MarkDirtyRepaint();
            }
        }

        private void OnGenerateVisualContent(MeshGenerationContext context)
        {
            Rect r = contentRect;
            if (r.width < 0.01f || r.height < 0.01f)
            {
                return;
            }

            vertices[0].tint = bottomLeftColour;
            vertices[1].tint = topLeftColour;
            vertices[2].tint = topRightColour;
            vertices[3].tint = bottomRightColour;

            float left = 0;
            float right = r.width;
            float top = 0;
            float bottom = r.height;

            vertices[0].position = new Vector3(left, bottom, Vertex.nearZ);
            vertices[1].position = new Vector3(left, top, Vertex.nearZ);
            vertices[2].position = new Vector3(right, top, Vertex.nearZ);
            vertices[3].position = new Vector3(right, bottom, Vertex.nearZ);

            MeshWriteData mwd = context.Allocate(vertices.Length, indices.Length);
            mwd.SetAllVertices(vertices);
            mwd.SetAllIndices(indices);
        }

        // Cleanup
        ~GradientElement()
        {
            UnregisterCallback<CustomStyleResolvedEvent>(CustomStylesResolved);
            generateVisualContent -= OnGenerateVisualContent;
        }
    }
}