using UnityEngine;
using UnityEngine.UIElements;

namespace VARLab.Velcro
{
    public class FilterTag : MonoBehaviour
    {
        private const string SelectedClass = "filter-tag-selected";
        private const string NonSelectedClass = "filter-tag";

        public UIDocument Document;
        public VisualElement Root;

        private Button filterTag;
        private bool tagSelected = false;

        [Header("Text Settings")]
        [SerializeField, Tooltip("This is the text that displays on the filter tag.")]
        private string tagText = "All";

        private void Awake()
        {
            Document = GetComponent<UIDocument>();
            Root = Document.rootVisualElement;
            filterTag = Root.Q<Button>("Filter-Tag");
            filterTag.text = tagText;

            filterTag.RemoveFromClassList(SelectedClass);
            filterTag.AddToClassList(NonSelectedClass);
            filterTag.EnableInClassList(NonSelectedClass, true);

            filterTag.RegisterCallback<ClickEvent>(OnTagPressed);
        }

        private void OnTagPressed(ClickEvent cl)
        {
            if (tagSelected == true)
            {
                tagSelected = false;
                TagUnSelected();
            }
            else
            {
                tagSelected = true;
                TagSelected();
            }
        }
        private void TagSelected()
        {
            filterTag.AddToClassList(SelectedClass);
            filterTag.EnableInClassList(SelectedClass, true);
        }
        private void TagUnSelected()
        {
            filterTag.EnableInClassList(SelectedClass, false);
            filterTag.RemoveFromClassList(SelectedClass);
        }

    }
}
