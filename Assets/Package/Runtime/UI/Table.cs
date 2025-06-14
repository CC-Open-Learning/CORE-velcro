using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

namespace VARLab.Velcro
{
    /// <summary>
    /// Class that allows for table functionality in UI.
    /// </summary>
    [RequireComponent(typeof(UIDocument))]
    public class Table : MonoBehaviour, IUserInterface
    {
        [Header("Table References")]
        [SerializeField]
        private VisualTreeAsset categoryTemplate;

        [SerializeField]
        private VisualTreeAsset entryTemplate;

        [SerializeField]
        private VisualTreeAsset elementTemplate;

        [SerializeField]
        private VisualTreeAsset headerLabelTemplate;

        // Getters for internal usage
        internal VisualTreeAsset EntryTemplate => entryTemplate;
        internal VisualTreeAsset ElementTemplate => elementTemplate;

        private VisualElement root;

        /// <summary>
        /// The root visual element of the table.
        /// </summary>
        public VisualElement Root => root;

        private VisualElement table;
        private VisualElement tableHeader;
        private VisualElement headerBlankSpace;
        private ScrollView categoryHolder;

        private List<TableCategory> categories = new List<TableCategory>();

        /// <summary>
        /// The list of categories contained in the table.
        /// </summary>
        public IEnumerable<TableCategory> Categories => categories;

        [Header("Events"), Space(4f)]
        [Tooltip("Invoked when the table is shown to the screen.")]
        public UnityEvent OnTableShown;

        [Tooltip("Invoked when the table is hidden from the screen")]
        public UnityEvent OnTableHidden;

        [Tooltip("Invoked when a new category is added to the table.")]
        public UnityEvent<TableCategory> OnCategoryAdded;

        [Tooltip("Invoked when a cateogry is removed from the table.")]
        public UnityEvent<TableCategory> OnCategoryRemoved;

        [Tooltip("Invoked when a category is expanded.")]
        public UnityEvent<TableCategory> OnCategoryExpanded;

        [Tooltip("Invoked when a category is collapsed.")]
        public UnityEvent<TableCategory> OnCategoryCollapsed;

        [Tooltip("Invoked when a new column is added to the table.")]
        public UnityEvent<int> OnColumnAdded;

        [Tooltip("Invoked when a column is removed from the table.")]
        public UnityEvent<int> OnColumnRemoved;

        private const string ExpandButtonClickedClass = "table-category-exapnd-button-clicked";

        /// <summary>
        /// The number of columns the table has.
        /// </summary>
        public int ColumnCount => tableHeader.childCount - 1;

        /// <summary>
        /// The number of categories contained inside the table.
        /// </summary>
        public int CategoryCount => categories.Count;

        private void Start()
        {
            UIDocument document = GetComponent<UIDocument>();
            root = document.rootVisualElement;
            table = root.Q("Table");
            tableHeader = table.Q("Header");
            headerBlankSpace = tableHeader.Q("BlankSpace");
            categoryHolder = table.Q<ScrollView>("CategoryHolder");

            OnTableShown ??= new UnityEvent();
            OnTableHidden ??= new UnityEvent();
            OnCategoryAdded ??= new UnityEvent<TableCategory>();
            OnCategoryRemoved ??= new UnityEvent<TableCategory>();
            OnCategoryExpanded ??= new UnityEvent<TableCategory>();
            OnCategoryCollapsed ??= new UnityEvent<TableCategory>();
            OnColumnAdded ??= new UnityEvent<int>();
            OnColumnRemoved ??= new UnityEvent<int>();

            table.Hide();
        }

        /// <summary>
        /// Displays the table to the screen with the specified columns.
        /// </summary>
        /// <param name="columnNames">The list of columns to initialize the table with.</param>
        public void HandleDisplayUI(params string[] columnNames)
        {
            RemoveAllCategories();
            RemoveAllColumns();

            if (columnNames != null)
            {
                foreach (string columnName in columnNames)
                {
                    AddColumn(columnName);
                }
            }

            Show();
        }

        public void AddCategory(string categoryName)
        {
            VisualElement newCategory = categoryTemplate.CloneTree();
            Label categoryLabel = newCategory.Q<Label>("CategoryNameLabel");
            VisualElement entryHolder = newCategory.Q("EntryHolder");

            Button expandButton = newCategory.Q<Button>("ExpandButton");
            expandButton.EnableInClassList(ExpandButtonClickedClass, false);

            categoryHolder.Add(newCategory);

            TableCategory category = new TableCategory();
            category.SetOwner(this);
            category.SetUIReferences(newCategory, categoryLabel, entryHolder);
            category.Name = categoryName;

            categories.Add(category);

            expandButton.RegisterCallback<ClickEvent, TableCategory>(ToggleCategory, category);
            expandButton.RegisterCallback<ClickEvent, VisualElement>(PlayExpandAnimation, expandButton);

            OnCategoryAdded.Invoke(category);
        }

        /// <summary>
        /// Removes all categories from the table.
        /// </summary>
        public void RemoveAllCategories()
        {
            for (int i = CategoryCount - 1; i > -1; i--)
            {
                categories[i].RemoveFromTable();
            }
        }

        /// <summary>
        /// Adds a new column to the table.
        /// </summary>
        /// <param name="columnName"></param>
        public void AddColumn(string columnName)
        {
            // Adding column to header
            VisualElement columnHeader = headerLabelTemplate.CloneTree();
            columnHeader.style.width = Length.Percent(100f);
            columnHeader.Q<Label>().text = columnName;
            
            tableHeader.Add(columnHeader);
            headerBlankSpace.BringToFront();

            // Adding column to entries
            foreach (TableCategory category in categories)
            {
                foreach (TableEntry entry in category.Entries)
                {
                    entry.AddElement();
                }
            }

            OnColumnAdded.Invoke(ColumnCount - 1);
        }

        /// <summary>
        /// Removes the column at the specified index from the table.
        /// </summary>
        /// <param name="columnIndex">The index of the column to be removed</param>
        public void RemoveColumn(int columnIndex)
        {
            if (columnIndex >= ColumnCount || columnIndex < 0)
            {
                Debug.LogError($"Unable to remove column at invalid index {columnIndex}.");
                return;
            }

            // Removing column from header
            tableHeader.RemoveAt(columnIndex);

            // Removing column from entries
            foreach (TableCategory category in categories)
            {
                foreach (TableEntry entry in category.Entries)
                {
                    entry.RemoveElement(columnIndex);
                }
            }

            OnColumnRemoved.Invoke(columnIndex);
        }

        /// <summary>
        /// Removes the first column with the specified name from the table.
        /// </summary>
        /// <param name="columnName">The name of the column to be removed</param>
        public void RemoveColumn(string columnName)
        {
            int columnIndex = 0;
            foreach (VisualElement columnElement in tableHeader.Children().SkipLast(1))
            {
                if (columnElement.Q<Label>().text == columnName)
                {
                    RemoveColumn(columnIndex);
                    return;
                }

                columnIndex++;
            }

            Debug.LogError($"Unable to find and remove column with name: {columnName}");
        }

        /// <summary>
        /// Removes all columns from the table.
        /// </summary>
        public void RemoveAllColumns()
        {
            for (int i = ColumnCount - 1; i > -1; i--)
            {
                RemoveColumn(i);
            }
        }

        /// <summary>
        /// Finds a category with the speccified name and returns it.
        /// </summary>
        /// <param name="categoryName">The name of the category to find.</param>
        /// <returns>The first category found with the specified name.</returns>
        public TableCategory FindCategoryByName(string categoryName)
        {
            return categories.Find(c => c.Name == categoryName);
        }

        // Button method for toggling the contents of a category
        private void ToggleCategory(ClickEvent evt, TableCategory category)
        {
            if (category.entryHolder.style.display == DisplayStyle.None)
            {
                category.entryHolder.Show();
                OnCategoryExpanded.Invoke(category);
            }
            else
            {
                category.entryHolder.Hide();
                OnCategoryCollapsed.Invoke(category);
            }
        }

        // Button method for playing the animation category expand/callapse animation
        private void PlayExpandAnimation(ClickEvent evt, VisualElement expandButton)
        {
            expandButton.ToggleInClassList(ExpandButtonClickedClass);
        }

        /// <summary>
        /// Shows the table to the screen.
        /// </summary>
        public void Show()
        {
            table.Show();
            OnTableShown.Invoke();
        }

        /// <summary>
        /// Hides the table from the screen.
        /// </summary>
        public void Hide()
        {
            table.Hide();
            OnTableHidden.Invoke();
        }

        // Internal use only
        // Used for the TableCategory.RemoveFromTable() method.
        internal void RemoveCategory(TableCategory category)
        {
            categories.Remove(category);
            OnCategoryRemoved.Invoke(category);
        }
    }
}