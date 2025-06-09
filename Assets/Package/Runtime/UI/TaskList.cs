using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

namespace VARLab.Velcro
{
    /// <summary>
    /// Task List can be set to 2 different positions, top left or top right 
    ///     - an unlimited amount of tasks can be added to the task list
    ///     - tasks have a indicator to show if they have been completed
    ///     - tasks have a hint button that display a notification with a hint on hot to complete the task
    ///     
    ///     -UI Anchors
    ///         - TopAnchor.Right (UI will be anchored to the top right corner of the screen with a 40px margin to the top and right) 
    ///         - TopAnchor.Left (UI will be anchored to the top left corner of the screen with a 40px margin to the top and left) 
    /// 
    /// Intended use of this class is to dislay a task list with a progress indicator where the learner can track their progress and 
    /// request for hints on how to complete a task.
    /// </summary>
    [RequireComponent(typeof(UIDocument))]
    public class TaskList : MonoBehaviour, IUserInterface
    {
        #region Serilialized Fields
        [Header("Task List Settings")]
        [SerializeField, Tooltip("Reference to the task list entry UI element")]
        private VisualTreeAsset taskUIComponent;

        [SerializeField, Tooltip("Reference to the notification UI element"), Space(5f)]
        private Notification notification;
        #endregion

        [HideInInspector]
        public List<Task> Tasks = new List<Task>();

        private TaskListSO taskListSO;
        private (Label, VisualElement, Button, VisualElement) taskUIElements;

        public int Count
        {
            get => Tasks.Count;
        }

        public int CompletedCount
        {
            get => Tasks.Count(t => t.Completed);
        }

        public bool IsComplete
        {
            get => CompletedCount == Count;
        }

        #region UI Elements
        /// <summary>
        /// The root visual element of the task list.
        /// </summary>
        [HideInInspector]
        public VisualElement Root;

        private VisualElement taskHolder;
        private VisualElement taskList;
        private Label titleLabel;
        private Label progressLabel;
        private Label progressDescription;
        private VisualElement progressBarFill;
        #endregion

        #region Unity Events
        [Header("Events"), Space(4f)]
        [Tooltip("Invoked when the task list is shown.")]
        public UnityEvent OnListShown;

        [Tooltip("Invoked when the task list is closed.")]
        public UnityEvent OnListHidden;

        [Tooltip("Invoked when a new task is added to the list.")]
        public UnityEvent<int> OnTaskAdded;

        [Tooltip("Invoked when a task is removed from the list.")]
        public UnityEvent OnTaskRemoved;

        [Tooltip("Invoked when a task is completed.")]
        public UnityEvent<int> OnTaskCompleted;

        [Tooltip("Invoked when a hint has been requested for a task.")]
        public UnityEvent<string> OnHintRequested;
        #endregion

        #region Constants
        private const int TaskComplete = 1;
        private const int TaskIncomplete = 0;
        private const string TaskIconCompletedClass = "list-element-task-icon-completed";
        private const string TaskLabelCompletedClass = "list-element-task-label-completed";
        private const string TaskHintIconCompletedClass = "list-element-hint-icon-completed";
        #endregion

        #region Interface Implementation
        public void Show()
        {
            taskList.Show();
            UpdateProgressBar();
            OnListShown?.Invoke();
        }
        public void Hide()
        {
            taskList.Hide();
            OnListHidden?.Invoke();
        }
        #endregion

        private void Start()
        {
            UIDocument doc = GetComponent<UIDocument>();
            Root = doc.rootVisualElement;

            GetReferences();
            taskList.Hide();
                
            OnListShown ??= new UnityEvent();
            OnListHidden ??= new UnityEvent();
            OnTaskAdded ??= new UnityEvent<int>();
            OnTaskRemoved ??= new UnityEvent();
            OnTaskCompleted ??= new UnityEvent<int>();
            OnHintRequested ??= new UnityEvent<string>();
        }

        #region Set up task list
        /// <summary>
        /// Method for creating a task list from a SO
        /// </summary>
        /// <param name="taskListScriptableObject">Task list scriptable object</param>
        public void CreateTaskListFromSO(TaskListSO taskListScriptableObject)
        {
            // Clear list before creating a new one.
            ClearList();

            PositionHelper.SetUITopAnchor(taskList, taskListScriptableObject.Alignment);

            taskListSO = taskListScriptableObject;

            titleLabel.SetElementText(taskListSO.ListTitle);
            progressLabel.SetElementText(taskListSO.ProgressText);
            progressDescription.SetElementText(taskListSO.ListDescription);

            foreach (var task in taskListSO.Tasks)
            {
                AddTask(task.TaskName, task.TaskHint, taskListScriptableObject.NotificationAlignment);
            }
        }

        /// <summary>
        /// Create a copy and populates the task visual element
        /// </summary>
        /// <param name="taskName">Name of the task</param>
        /// <param name="taskHint">Task hint message</param>
        private void CreateTaskUIElement(string taskName, string taskHint, Align notificationAlignment)
        {
            // Create a copy of the task element 
            VisualElement taskElement = taskUIComponent.CloneTree();

            taskElement.tooltip = taskName;

            GetTaskElementReferences(taskElement);

            // Set the task name
            taskUIElements.Item1.text = taskName;
            taskUIElements.Item1.EnableInClassList(TaskLabelCompletedClass, false);

            // Set the task icon to incomplete
            taskUIElements.Item2.EnableInClassList(TaskIconCompletedClass, false);

            // Register Button click event
            taskUIElements.Item3.clicked += () =>
            {
                notification.HandleDisplayUI(NotificationType.Info, taskHint, FontSize.Large, notificationAlignment);
                OnHintRequested?.Invoke(taskName);
            };

            // Set the hint icon to enabled
            taskUIElements.Item4.EnableInClassList(TaskHintIconCompletedClass, false);

            // add the task element to the task container
            taskHolder.Add(taskElement);
        }
        #endregion

        /// <summary>
        /// Sets the completion status of a task.
        /// </summary>
        /// <param name="taskIndex">The task to change the completion status of.</param>
        /// <param name="complete">Complete the task or not.</param>
        public void SetTaskComplete(string taskName, bool complete)
        {
            Task task = Tasks.Find(t => t.Name == taskName);
            int index = Tasks.FindIndex(t => t.Name == taskName);
            if (task == null)
            {
                Debug.LogError($"Unable to set completion status of task {taskName}");
                return;
            }

            task.Progress = complete ? TaskComplete : TaskIncomplete;

            List<VisualElement> currentTask = taskHolder.Query<VisualElement>().Where(element => element.tooltip == taskName).ToList();

            currentTask.ForEach(task =>
            {
                GetTaskElementReferences(task);
                taskUIElements.Item1.EnableInClassList(TaskLabelCompletedClass, complete);
                taskUIElements.Item2.EnableInClassList(TaskIconCompletedClass, complete);
                taskUIElements.Item3.SetEnabled(!complete);
                taskUIElements.Item4.EnableInClassList(TaskHintIconCompletedClass, complete);
            });

            UpdateProgressBar();

            if (complete)
            {
                OnTaskCompleted?.Invoke(index);
            }
        }

        #region Helpers
        /// <summary>
        /// Adds a new task to the list.
        /// </summary>
        /// <param name="taskName">The name of the new task</param>
        /// <param name="taskHint">Task hint message</param>
        public void AddTask(string taskName, string taskHint, Align notificationAlignment = Align.Center)
        {
            Task task = new(taskName);
            Tasks.Add(task);

            CreateTaskUIElement(taskName, taskHint, notificationAlignment);
            UpdateProgressBar();
            OnTaskAdded?.Invoke(Count - 1);
        }

        /// <summary>
        /// Removes a task from the list
        /// </summary>
        /// <param name="taskIndex"></param>
        public void RemoveTask(string taskName)
        {
            int index = GetTaskIndex(taskName);
            Task task = GetTaskByTaskName(taskName);

            if (task == null)
            {
                Debug.LogError($"Unable to remove task {taskName}");
                return;
            }

            Tasks.Remove(task);
            taskHolder.RemoveAt(index);

            UpdateProgressBar();
            OnTaskRemoved?.Invoke();
        }

        /// <summary>
        /// Get referece to the task visual elements
        /// </summary>
        /// <param name="taskElement"></param>
        private void GetTaskElementReferences(VisualElement taskElement)
        {
            taskUIElements.Item1 = taskElement.Q<Label>("TaskLabel");
            taskUIElements.Item2 = taskElement.Q("TaskIcon");
            taskUIElements.Item3 = taskElement.Q<Button>("Hint");
            taskUIElements.Item4 = taskElement.Q("Icon");
        }

        /// <summary>
        /// Get reference to the task list UI elements
        /// </summary>
        private void GetReferences()
        {
            taskList = Root.Q("TaskList");
            taskHolder = taskList.Q("TaskHolder");
            titleLabel = taskList.Q<Label>("TitleLabel");
            progressLabel = taskList.Q<Label>("ProgressLabel");
            progressDescription = taskList.Q<Label>("ProgressDescription");
            progressBarFill = taskList.Q("Fill");
        }

        /// <summary>
        /// Update the UI progress bar
        /// </summary>
        private void UpdateProgressBar()
        {
            Length percent = Length.Percent((float)CompletedCount / Count * 100f);
            progressBarFill.style.width = percent;
            progressLabel.text = $"{taskListSO.ProgressText} {CompletedCount}/{Count}";

            if (percent.value > 0f)
            {
                progressBarFill.SetBackgroundColour(StyleKeyword.Null);
                progressBarFill.SetBorderColour(StyleKeyword.Null);
            }
            else
            {
                progressBarFill.SetBackgroundColour(Color.clear);
                progressBarFill.SetBorderColour(Color.clear);
            }
        }

        public int GetTaskIndex(string taskName)
        {
            int index = Tasks.FindIndex(t => t.Name == taskName);
            return index;
        }

        public Task GetTaskByTaskName(string taskName)
        {
            Task task = Tasks.Find(t => t.Name == taskName);
            return task;
        }

        public bool IsTaskComplete(string taskName)
        {
            Task task = GetTaskByTaskName(taskName);

            if (task == null)
            {
                Debug.LogError($"Unable to check if task {taskName} is complete");
                return false;
            }

            return task.Completed;
        }

        private void ClearList()
        {
            Tasks.Clear();
            taskListSO = null;
            taskHolder.Clear();
        }
        #endregion
    }
}