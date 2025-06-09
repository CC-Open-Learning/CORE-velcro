using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

namespace VARLab.Velcro
{
    /// <summary>
    /// Class that allows for progress indicator V1 functionality in UI.
    /// </summary>
    [RequireComponent(typeof(UIDocument))]
    public class ProgressIndicatorV1 : MonoBehaviour, IUserInterface
    {
        [Header("Progress Indicator Settings")]
        [SerializeField]
        private VisualTreeAsset categoryAsset;

        [SerializeField]
        private VisualTreeAsset taskAsset;

        [SerializeField]
        private List<Category> categories;

        private Dictionary<Category, VisualElement> categoryElements = new Dictionary<Category, VisualElement>();
        private Dictionary<Category, CategoryUI> categoryUIs = new Dictionary<Category, CategoryUI>();
        private Dictionary<Task, TaskUI> taskUIs = new Dictionary<Task, TaskUI>();

        /// <summary>
        /// The root visual element of the progress indicator.
        /// </summary>
        public VisualElement Root { get; set; }

        private VisualElement progressIndicator;
        private Label titleLabel;
        private VisualElement categoriesHolder;

        private const string ArrowClosedClass = "progress-v1-category-arrow-closed";
        private const string CategoryLastBorderRadiusClass = "progress-v1-category-last";
        private const string CategoryLastBottomClass = "display-hidden";
        private const string TaskCompletedLabelClass = "progress-v1-task-completed-label";
        private const string TaskProgressLabelClass = "progress-v1-task-progress-label";
        private const string TaskProgressLabelHighlightClass = "progress-v1-task-progress-label-highlight";
        private const string TaskCheckmarkCheckedClass = "progress-v1-task-checkmark-checked";

        [Header("Events"), Space(4f)]
        [Tooltip("Invokes when the progress indicator is set to visible.")]
        public UnityEvent OnShow;

        [Tooltip("Invokes when the progress indicator is set to hidden.")]
        public UnityEvent OnHide;

        [Tooltip("Invokes when the progress of a task has been updated. Event paramters: The index of category containing the updated task, The index of the updated task.")]
        public UnityEvent<int, int> OnProgressUpdated;

        [Tooltip("Invokes when a new category has been added to the progress indicator. Event parameter: The index of the newly created category.")]
        public UnityEvent<int> OnCategoryAdded;

        [Tooltip("Invokes when a new task has been added to a category. Event parameters: The index of the category the task has been added to, The index of the newly created task.")]
        public UnityEvent<int, int> OnTaskAdded;

        [Tooltip("Invokes when a category has been removed from the progress indicator.")]
        public UnityEvent OnCategoryRemoved;

        [Tooltip("Invokes when a task has been removed from a category. Event parameter: The index of the category the task was removed from.")]
        public UnityEvent<int> OnTaskRemoved;

        /// <summary>
        /// The number of categories in the progress indicator.
        /// </summary>
        public int CategoryCount
        {
            get => categories.Count;
        }

        private void Start()
        {
            UIDocument document = GetComponent<UIDocument>();
            Root = document.rootVisualElement;
            progressIndicator = Root.Q<VisualElement>("ProgressIndicator");
            categoriesHolder = Root.Q<ScrollView>("CategoryHolder");
            titleLabel = Root.Q<Label>("TitleLabel");

            if (categories == null) { categories = new List<Category>(); }
            if (OnShow == null) { OnShow = new UnityEvent(); }
            if (OnHide == null) { OnHide = new UnityEvent(); }
            if (OnProgressUpdated == null) { OnProgressUpdated = new UnityEvent<int, int>(); }
            if (OnCategoryAdded == null) { OnCategoryAdded = new UnityEvent<int>(); }
            if (OnCategoryRemoved == null) { OnCategoryRemoved = new UnityEvent(); }
            if (OnTaskAdded == null) { OnTaskAdded = new UnityEvent<int, int>(); }
            if (OnTaskRemoved == null) { OnTaskRemoved = new UnityEvent<int>(); }

            OnCategoryAdded.AddListener(OnCategoryAddedToList);
            OnCategoryRemoved.AddListener(OnCategoryRemoveFromList);

            GenerateCategories();

            progressIndicator.Hide();
        }

        /// <summary>
        /// Displays the progress indicator with all its categories and tasks.
        /// </summary>
        public void HandleDisplayUI(string title)
        {
            SetTitle(title);
            Show();
        }

        public void SetTitle(string title)
        {
            titleLabel.text = title;
        }

        // Generate the categories already initialized in the categories list.
        private void GenerateCategories()
        {
            foreach (Category category in categories)
            {
                GenerateCategory(category);

                foreach (Task task in category.Tasks)
                {
                    GenerateTask(category, task);
                }
            }
        }

        // Internal method for generating the UI part of the category.
        private void GenerateCategory(Category category)
        {
            VisualElement newCategory = categoryAsset.CloneTree();
            newCategory.Q("Category").EnableInClassList(CategoryLastBorderRadiusClass, false);
            newCategory.Q("Bottom").EnableInClassList(CategoryLastBottomClass, false);
            newCategory.Q<Label>("TitleLabel").text = category.Name;

            VisualElement taskHolder = newCategory.Q("TaskHolder");
            categoriesHolder.Add(newCategory);

            // Arrow
            VisualElement arrowHolder = newCategory.Q("ArrowHolder");
            VisualElement arrow = newCategory.Q("Arrow");
            arrow.EnableInClassList(ArrowClosedClass, false);
            arrowHolder.RegisterCallback<ClickEvent, VisualElement>(ArrowAnimation, arrow);
            arrowHolder.RegisterCallback<ClickEvent, VisualElement>(ToggleTaskView, taskHolder);

            // Progress Bar and Progress Label
            ProgressIndicatorV1Bar progressBar = newCategory.Q<ProgressIndicatorV1Bar>("ProgressBar");
            Label progressLabel = newCategory.Q<Label>("ProgressLabel");

            int taskCount = category.Tasks.Count;
            progressBar.MinValue = 0;
            progressBar.MaxValue = taskCount;
            progressBar.Value = 0;
            progressLabel.text = $"0/{taskCount}";

            VisualElement progressBarFill = progressBar.Q("Fill");

            // UI
            CategoryUI categoryUI = new CategoryUI()
            {
                ProgressBar = progressBar,
                ProgressBarFill = progressBarFill,
                ProgressLabel = progressLabel,
                TaskHolder = taskHolder,
            };

            // Add to lists
            categoryElements.Add(category, newCategory);
            categoryUIs.Add(category, categoryUI);
        }

        // Internal method for generating the UI part of the task.
        private void GenerateTask(Category category, Task task)
        {
            VisualElement newTask = taskAsset.CloneTree();

            // Task name label
            Label taskNameLabel = newTask.Q<Label>("TaskLabel");
            taskNameLabel.text = task.Name;
            taskNameLabel.EnableInClassList(TaskCompletedLabelClass, false);

            // Task progress label
            Label taskProgressLabel = newTask.Q<Label>("ProgressLabel");
            taskProgressLabel.text = $"(0/{task.MaxProgress})";
            taskProgressLabel.EnableInClassList(TaskProgressLabelHighlightClass, false);
            taskProgressLabel.EnableInClassList(TaskCompletedLabelClass, false);

            // Task checkmark
            VisualElement checkmark = newTask.Q("Checkmark");
            checkmark.EnableInClassList(TaskCheckmarkCheckedClass, false);
            VisualElement checkmarkIcon = checkmark.Q("Icon");

            TaskUI taskUI = new()
            {
                ProgressLabel = taskProgressLabel,
                TaskNameLabel = taskNameLabel,
                TaskCheckmark = checkmark,
                TaskCheckmarkIcon = checkmarkIcon
            };

            categoryUIs[category].TaskHolder.Add(newTask);
            taskUIs.Add(task, taskUI);
        }

        // Arrow animation
        private void ArrowAnimation(ClickEvent e, VisualElement arrow)
        {
            arrow.ToggleInClassList(ArrowClosedClass);
        }

        // Arrow functionality
        private void ToggleTaskView(ClickEvent e, VisualElement taskHolder)
        {
            if (taskHolder.style.display == DisplayStyle.None)
            {
                taskHolder.Show();
            }
            else
            {
                taskHolder.Hide();
            }
        }

        /// <summary>
        /// Adds a new category to the progress indicator.
        /// </summary>
        /// <param name="name">The name of the new category.</param>
        public void AddCategory(string name)
        {
            Category category = new Category(name);

            categories.Add(category);
            GenerateCategory(category);
            OnCategoryAdded.Invoke(CategoryCount - 1);
        }

        /// <summary>
        /// Adds a new task to a specific category in the progress indicator.
        /// </summary>
        /// <param name="categoryIndex">The index of the category to add the task to.</param>
        /// <param name="taskName">The name of the new task.</param>
        /// <param name="maxProgress">The maximum amount of progress required to complete the new task.</param>
        public void AddTask(int categoryIndex, string taskName, int maxProgress)
        {
            Category category = categories.ElementAtOrDefault(categoryIndex);
            if (category == null)
            {
                Debug.LogError($"Unable to add task at invalid category index {categoryIndex}");
                return;
            }

            Task task = new Task(taskName, maxProgress);

            category.Tasks.Add(task);
            GenerateTask(categories[categoryIndex], task);
            UpdateUI(categories[categoryIndex], task);
            OnTaskAdded.Invoke(categoryIndex, GetTaskCount(categoryIndex) - 1);
        }

        /// <summary>
        /// Removes a category at the specified index.
        /// </summary>
        /// <param name="categoryIndex">The index of the category to remove.</param>
        public void RemoveCategory(int categoryIndex)
        {
            Category category = categories.ElementAtOrDefault(categoryIndex);
            if (category == null)
            {
                Debug.LogError($"Unable to remove category at invalid category index {categoryIndex}");
                return;
            }

            for (int i = 0; i < GetTaskCount(categoryIndex); i++)
            {
                RemoveTask(categoryIndex, i);
            }

            categoryUIs.Remove(category);
            categoryElements.Remove(category);
            categoriesHolder.RemoveAt(categoryIndex);
            categories.RemoveAt(categoryIndex);
            OnCategoryRemoved.Invoke();
        }

        /// <summary>
        /// Removes a task at the specified category and task indices.
        /// </summary>
        /// <param name="categoryIndex">The category index of the task.</param>
        /// <param name="taskIndex">The index of the task to remove.</param>
        public void RemoveTask(int categoryIndex, int taskIndex)
        {
            Category category = categories.ElementAtOrDefault(categoryIndex);
            if (category == null)
            {
                Debug.LogError($"Unable to remove task at invalid category index {categoryIndex}");
                return;
            }

            Task task = category.Tasks.ElementAtOrDefault(taskIndex);
            if (task == null)
            {
                Debug.LogError($"Unable to remove task at invalid task index {taskIndex}");
                return;
            }

            taskUIs.Remove(category.Tasks[taskIndex]);
            category.Tasks.RemoveAt(taskIndex);
            categoryUIs[category].TaskHolder.RemoveAt(taskIndex);
            UpdateCategoryUI(category);
            OnTaskRemoved.Invoke(categoryIndex);
        }

        /// <summary>
        /// Adds progress to a specific task in the progress indicator.
        /// </summary>
        /// <param name="categoryIndex">The category containing the task.</param>
        /// <param name="taskIndex">The task to add progress to.</param>
        /// <param name="amount">The amount of progress to add to the task's progression (default value is 1).</param>
        public void AddProgressToTask(int categoryIndex, int taskIndex, int amount = 1)
        {
            Category category = categories.ElementAtOrDefault(categoryIndex);
            if (category == null)
            {
                Debug.LogError($"Unable to add progress to task at invalid category index {categoryIndex}");
                return;
            }

            Task task = category.Tasks.ElementAtOrDefault(taskIndex);
            if (task == null)
            {
                Debug.LogError($"Unable to add progress to task at invalid task index {taskIndex}");
                return;
            }

            task.Progress += amount;
            OnProgressUpdated.Invoke(categoryIndex, taskIndex);

            UpdateUI(categories[categoryIndex], task);
        }

        /// <summary>
        /// Removes progress from a specific task in the progress indicator.
        /// </summary>
        /// <param name="categoryIndex">The category containing the task.</param>
        /// <param name="taskIndex">The task to remove progress from.</param>
        /// <param name="amount">The amount of progress to remove from the task's progression (default value is 1).</param>
        public void RemoveProgressFromTask(int categoryIndex, int taskIndex, int amount = 1)
        {
            Category category = categories.ElementAtOrDefault(categoryIndex);
            if (category == null)
            {
                Debug.LogError($"Unable to remove progress from task at invalid category index {categoryIndex}");
                return;
            }

            Task task = category.Tasks.ElementAtOrDefault(taskIndex);
            if (task == null)
            {
                Debug.LogError($"Unable to remove progress from task at invalid task index {taskIndex}");
                return;
            }

            task.Progress -= amount;
            task.Completed = false;

            OnProgressUpdated.Invoke(categoryIndex, taskIndex);

            UpdateUI(categories[categoryIndex], task);
        }

        // Internal method for updating category UI
        private void UpdateCategoryUI(Category category)
        {
            int taskCount = category.Tasks.Count;

            int tasksComplete = 0;
            foreach (Task t in category.Tasks)
            {
                if (t.Completed)
                {
                    tasksComplete++;
                }
            }

            CategoryUI categoryUI = categoryUIs[category];
            categoryUI.ProgressBar.MaxValue = taskCount;
            categoryUI.ProgressBar.Value = tasksComplete;
            categoryUI.ProgressLabel.text = $"{tasksComplete}/{taskCount}";
        }

        // Internal method for updating both category and task UI when tasks have been updated (new task added, progress added, etc).
        private void UpdateUI(Category category, Task task)
        {
            TaskUI taskUI = taskUIs[task];

            //This styling is always enabled or disabled depending on completed state
            //Both label green colour classes, and checkmark icon styling
            taskUI.ProgressLabel.text = $"({task.Progress}/{task.MaxProgress})";
            taskUI.TaskCheckmarkIcon.style.visibility = task.Completed ? Visibility.Visible : Visibility.Hidden;
            taskUI.ProgressLabel.EnableInClassList(TaskCompletedLabelClass, task.Completed);    //Completed (Green) colour for X/Y
            taskUI.TaskNameLabel.EnableInClassList(TaskCompletedLabelClass, task.Completed);    //Completed (Green) colour for task
            taskUI.TaskCheckmark.EnableInClassList(TaskCheckmarkCheckedClass, task.Completed);  //Completed border-width around checkmark

            //Completed. Green styling
            if (task.Completed)
            {
                taskUI.ProgressLabel.EnableInClassList(TaskProgressLabelHighlightClass, false); //In progress (Yellow) colour for X/Y
                taskUI.ProgressLabel.EnableInClassList(TaskProgressLabelClass, false);          //Not started (White) colour for X/Y
            }
            //In progress. Yellow styling
            else if (task.Progress > 0)
            {
                taskUI.ProgressLabel.EnableInClassList(TaskProgressLabelHighlightClass, true);  //In progress (Yellow) colour for X/Y
                taskUI.ProgressLabel.EnableInClassList(TaskProgressLabelClass, false);          //Not started (White) colour for X/Y
            }
            //Not started. Default white/grey styling
            else
            {
                taskUI.ProgressLabel.EnableInClassList(TaskProgressLabelHighlightClass, false); //In progress (Yellow) colour for X/Y
                taskUI.ProgressLabel.EnableInClassList(TaskProgressLabelClass, true);           //Not started (White) colour for X/Y
            }

            UpdateCategoryUI(category);
        }

        /// <summary>
        /// Gets the name of the category at the specified index.
        /// </summary>
        /// <param name="categoryIndex"></param>
        /// <returns>The name of the category.</returns>
        public string GetCategoryName(int categoryIndex)
        {
            Category category = categories.ElementAtOrDefault(categoryIndex);
            if (category == null)
            {
                Debug.LogError($"Unable to get category name at invalid category index {categoryIndex}");
                return null;
            }
            
            return category.Name;
        }

        /// <summary>
        /// Gets the name of a task at the specified category with the specified task index.
        /// </summary>
        /// <param name="categoryIndex"></param>
        /// <param name="taskIndex"></param>
        /// <returns></returns>
        public string GetTaskName(int categoryIndex, int taskIndex)
        {
            Category category = categories.ElementAtOrDefault(categoryIndex);
            if (category == null)
            {
                Debug.LogError($"Unable to get task name at invalid category index {categoryIndex}");
                return null;
            }

            Task task = category.Tasks.ElementAtOrDefault(taskIndex);
            if (task == null)
            {
                Debug.LogError($"Unable to get task name at invalid task index {taskIndex}");
                return null;
            }

            return task.Name;
        }

        /// <summary>
        /// Gets the current progress of a task at the specified category with the specified task index.
        /// </summary>
        /// <param name="categoryIndex"></param>
        /// <param name="taskIndex"></param>
        /// <returns>The progress of the task.</returns>
        public int GetTaskProgress(int categoryIndex, int taskIndex)
        {
            Category category = categories.ElementAtOrDefault(categoryIndex);
            if (category == null)
            {
                Debug.LogError($"Unable to get task progress at invalid category index {categoryIndex}");
                return -1;
            }

            Task task = category.Tasks.ElementAtOrDefault(taskIndex);
            if (task == null)
            {
                Debug.LogError($"Unable to get task progress at invalid task index {taskIndex}");
                return -1;
            }

            return task.Progress;
        }

        /// <summary>
        /// Gets the max progress of a task at the specified category with the specified task index.
        /// </summary>
        /// <param name="categoryIndex"></param>
        /// <param name="taskIndex"></param>
        /// <returns>The max progress of the task.</returns>
        public int GetTaskMaxProgress(int categoryIndex, int taskIndex)
        {
            Category category = categories.ElementAtOrDefault(categoryIndex);
            if (category == null)
            {
                Debug.LogError($"Unable to get task max progress at invalid category index {categoryIndex}");
                return -1;
            }

            Task task = category.Tasks.ElementAtOrDefault(taskIndex);
            if (task == null)
            {
                Debug.LogError($"Unable to get task max progress at invalid task index {taskIndex}");
                return -1;
            }

            return task.MaxProgress;
        }

        /// <summary>
        /// Finds the index of the first category with the specified name.
        /// </summary>
        /// <param name="categoryName"></param>
        /// <param name="categoryIndex"></param>
        /// <returns></returns>
        public bool FindCategoryWithName(string categoryName, out int categoryIndex)
        {
            for (int i = 0; i < CategoryCount; i++)
            {
                if (categories[i].Name == categoryName)
                {
                    categoryIndex = i;
                    return true;
                }
            }

            categoryIndex = -1;
            return false;
        }

        /// <summary>
        /// Finds the index of the first task with the specified name in the specified category (O(N)).
        /// </summary>
        /// <param name="taskName"></param>
        /// <param name="categoryIndex"></param>
        /// <param name="taskIndex"></param>
        /// <returns></returns>
        public bool FindTaskWithName(string taskName, int categoryIndex, out int taskIndex)
        {
            Category category = categories.ElementAtOrDefault(categoryIndex);
            if (category == null)
            {
                taskIndex = -1;
                return false;
            }

            for (int i = 0; i < GetTaskCount(categoryIndex); i++)
            {
                if (category.Tasks[i].Name == taskName)
                {
                    taskIndex = i;
                    return true;
                }
            }

            taskIndex = -1;
            return false;
        }

        /// <summary>
        /// Finds the index of the first task with the specified name (O(N^2)).
        /// </summary>
        /// <param name="taskName"></param>
        /// <param name="categoryIndex"></param>
        /// <param name="taskIndex"></param>
        /// <returns></returns>
        public bool FindTaskWithName(string taskName, out int categoryIndex, out int taskIndex)
        {
            for (int i = 0; i < CategoryCount; i++)
            {
                for (int j = 0; j < GetTaskCount(i); j++)
                {
                    if (categories[i].Tasks[j].Name == taskName)
                    {
                        categoryIndex = i;
                        taskIndex = j;
                        return true;
                    }
                }
            }

            categoryIndex = -1;
            taskIndex = -1;
            return false;
        }

        // This function is for the round borders of the bottom element
        private void SetBottomCategory(VisualElement category, bool bottom)
        {
            VisualElement background = category.Q("Category");
            VisualElement bottomElement = category.Q("Bottom");
            VisualElement arrowHolder = category.Q("ArrowHolder");

            if (bottom)
            {
                arrowHolder.RegisterCallback<ClickEvent, (VisualElement, VisualElement)>(ToggleBorderClass, (background, bottomElement));
            }
            else
            {
                arrowHolder.UnregisterCallback<ClickEvent, (VisualElement, VisualElement)>(ToggleBorderClass);
            }
        }

        // Toggles the rounded corners on the last category element
        private void ToggleBorderClass(ClickEvent e, (VisualElement, VisualElement) elements)
        {
            elements.Item1.ToggleInClassList(CategoryLastBorderRadiusClass);
            elements.Item2.ToggleInClassList(CategoryLastBottomClass);
        }

        /// <summary>
        /// Shows the progress indicator.
        /// </summary>
        public void Show()
        {
            progressIndicator.Show();
            OnShow.Invoke();
        }

        /// <summary>
        /// Hides the progress indicator.
        /// </summary>
        public void Hide()
        {
            progressIndicator.Hide();
            OnHide.Invoke();
        }

        /// <summary>
        /// Returns the amount of tasks in a category.
        /// </summary>
        /// <param name="categoryIndex">The index of the category to check.</param>
        /// <returns>The amount of tasks in the specified category.</returns>
        public int GetTaskCount(int categoryIndex)
        {
            Category category = categories.ElementAtOrDefault(categoryIndex);
            if (category == null)
            {
                Debug.LogError($"Unable to get task at invalid category index {categoryIndex}");
                return -1;
            }

            return category.Tasks.Count;
        }

        // Add round corners to the last category in the list
        private void AddBottomClassesToLastCategory(int index)
        {
            if (index > 0)
            {
                SetBottomCategory(categoryElements[categories[index - 1]], false);
            }
            SetBottomCategory(categoryElements[categories[index]], true);
        }

        private void OnCategoryAddedToList(int index)
        {
            AddBottomClassesToLastCategory(index);
        }

        private void OnCategoryRemoveFromList()
        {
            if (CategoryCount > 0)
            {
                AddBottomClassesToLastCategory(CategoryCount - 1);
            }
        }

        // Cleanup
        private void OnDestroy()
        {
            OnCategoryAdded?.RemoveListener(OnCategoryAddedToList);
            OnCategoryRemoved?.RemoveListener(OnCategoryRemoveFromList);
        }
    }
}