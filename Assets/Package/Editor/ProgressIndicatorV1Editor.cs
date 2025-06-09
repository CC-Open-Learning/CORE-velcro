using UnityEditor;

namespace VARLab.Velcro
{
    [CustomEditor(typeof(ProgressIndicatorV1))]
    public class ProgressIndicatorV1Editor : Editor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.ApplyModifiedProperties();
            base.OnInspectorGUI();
        }
    }
}
