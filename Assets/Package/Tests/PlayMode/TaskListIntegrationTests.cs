using NUnit.Framework;
using System.Collections;
using UnityEditor;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.UIElements;
using VARLab.Velcro;

public class TaskListIntegrationTests
{
    private int sceneCounter;

    private GameObject listObj;
    private UIDocument listDoc;
    private TaskList listComp;
    private VisualElement listElement;

    private TaskListSO taskListTopRight;
    private TaskListSO taskListTopLeft;

    [UnitySetUp]
    [Category("BuildServer")]
    public IEnumerator SetUp()
    {
        sceneCounter = TestUtils.ClearScene(sceneCounter, "TaskListScene");

        listObj = new GameObject("Timer Object");
        listDoc = listObj.AddComponent<UIDocument>();
        listComp = listObj.AddComponent<TaskList>();


        VisualTreeAsset taskListUXML = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/VELCRO UI/UI/Task List/TaskList.uxml");
        VisualTreeAsset tasklistElementUXML = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/VELCRO UI/UI/Task List/TaskListElement.uxml");
        PanelSettings panelSettings = AssetDatabase.LoadAssetAtPath<PanelSettings>("Assets/VELCRO UI/Settings/Panel Settings.asset");

        {
            //Reference panel settings and source asset as SerializedFields
            SerializedObject so = new SerializedObject(listDoc);
            so.FindProperty("m_PanelSettings").objectReferenceValue = panelSettings;
            so.FindProperty("sourceAsset").objectReferenceValue = taskListUXML;
            so.ApplyModifiedProperties();
        }
        {
            SerializedObject so = new SerializedObject(listComp);
            so.FindProperty("taskUIComponent").objectReferenceValue = tasklistElementUXML;
            so.ApplyModifiedProperties();
        }

        taskListUXML.CloneTree(listDoc.rootVisualElement);

        listElement = listDoc.rootVisualElement.Q("TaskList");

        // Set up the task list SOs
        var task1 = new TaskListSO.Task();
        task1.TaskName = "Task 1";
        task1.TaskHint = "Task 1 hint";
        var task2 = new TaskListSO.Task();
        task2.TaskName = "Task 2";
        task2.TaskHint = "Task 2 hint";
        var task3 = new TaskListSO.Task();
        task3.TaskName = "Task 3";
        task3.TaskHint = "Task 3 hint";
        var task4 = new TaskListSO.Task();
        task4.TaskName = "Task 4";
        task4.TaskHint = "Task 4 hint";

        taskListTopLeft = ScriptableObject.CreateInstance<TaskListSO>();
        taskListTopLeft.ListTitle = "List Top Left";
        taskListTopLeft.ProgressText = "Completed";
        taskListTopLeft.ListDescription = "List Description";
        taskListTopLeft.Alignment = TopAnchor.TopLeft;
        taskListTopLeft.Tasks = new()
        {
            task1,
            task2,
            task3,
            task4
        };

        taskListTopRight = ScriptableObject.CreateInstance<TaskListSO>();
        taskListTopRight.ListTitle = "List Top Right";
        taskListTopRight.ProgressText = "Completed";
        taskListTopRight.ListDescription = "List Description";
        taskListTopRight.Alignment = TopAnchor.TopRight;
        taskListTopRight.Tasks = new()
        {
            task1,
            task2,
            task3,
            task4
        };


        yield return null;
    }

    [Test, Order(1)]
    [Category("BuildServer")]
    public void Start_SetsListToDisplayNone()
    {
        // Assert
        Assert.AreEqual(new StyleEnum<DisplayStyle>(DisplayStyle.None), listElement.style.display);
    }

    [Test, Order(2)]
    [Category("BuildServer")]
    public void HandleDisplayUI_SetsTitleText()
    {
        // Arrange
        string expectedText = "List Top Left";
        listComp.CreateTaskListFromSO(taskListTopLeft);

        // Act
        listComp.Show();

        // Assert
        Assert.AreEqual(expectedText, listElement.Q<Label>("TitleLabel").text);
    }

    [Test, Order(3)]
    [Category("BuildServer")]
    public void HandleDisplayUI_SetsProgressText()
    {
        // Arrange
        string expectedText = "Completed";

        // Act
        listComp.CreateTaskListFromSO(taskListTopLeft);
        listComp.Show();

        // Assert
        Assert.IsTrue(listElement.Q<Label>("ProgressLabel").text.Contains(expectedText));
    }

    [Test, Order(4)]
    [Category("BuildServer")]
    public void HandleDisplayUI_SetsProgressDescription()
    {
        // Arrange
        string expectedText = "List Description";

        // Act
        listComp.CreateTaskListFromSO(taskListTopLeft);
        listComp.Show();

        // Assert
        Assert.AreEqual(expectedText, listElement.Q<Label>("ProgressDescription").text);
    }

    [Test, Order(5)]
    [Category("BuildServer")]
    public void HandleDisplayUI_UpdatesProgressLabel()
    {
        // Arrange
        string expectedText = "0/4";

        // Act
        listComp.CreateTaskListFromSO(taskListTopLeft);
        listComp.Show();

        // Assert
        Assert.IsTrue(listElement.Q<Label>("ProgressLabel").text.Contains(expectedText));
    }

    [Test, Order(6)]
    [Category("BuildServer")]
    public void AddTask_AddsNewTaskToList()
    {
        // Arrange
        listComp.CreateTaskListFromSO(taskListTopLeft);
        int oldCount = listComp.Count;

        // Act
        listComp.AddTask("Task 5", "Hint Task 5");

        // Assert
        Assert.Less(oldCount, listComp.Count);
    }

    [Test, Order(7)]
    [Category("BuildServer")]
    public void AddTask_AddsNewVisualElementToHolder()
    {
        // Arrange
        VisualElement taskHolder = listElement.Q("TaskHolder");
        listComp.CreateTaskListFromSO(taskListTopLeft);
        int oldCount = taskHolder.childCount;

        // Act
        listComp.AddTask("Task 5", "Hint 5");
        listComp.AddTask("Task 6", "Hint 6");
        listComp.AddTask("Task 7", "Hint 7");

        // Assert
        Assert.Less(oldCount, taskHolder.childCount);
    }

    [Test, Order(8)]
    [Category("BuildServer")]
    public void AddTask_InvokesTaskAddedEvent()
    {
        // Arrange
        listComp.OnTaskAdded.AddListener(Ping);
        listComp.CreateTaskListFromSO(taskListTopLeft);

        // Act
        listComp.AddTask("Task 1", "Hint 1");

        // Assert
        LogAssert.Expect(LogType.Log, "Progress event triggered");

        // Clean up
        listComp.OnTaskAdded.RemoveListener(Ping);
    }

    [Test, Order(9)]
    [Category("BuildServer")]
    public void RemoveTask_RemovesNewTaskFromList()
    {
        // Arrange
        listComp.CreateTaskListFromSO(taskListTopLeft);
        int oldCount = listComp.Count;

        // Act
        listComp.RemoveTask("Task 1");

        // Assert
        Assert.Greater(oldCount, listComp.Count);
    }

    [Test, Order(10)]
    [Category("BuildServer")]
    public void RemoveTask_RemovesNewVisualElementFromHolder()
    {
        // Arrange
        listComp.CreateTaskListFromSO(taskListTopLeft);
        VisualElement taskHolder = listElement.Q("TaskHolder");
        int oldCount = taskHolder.childCount;

        // Act
        listComp.RemoveTask("Task 1");

        // Assert
        Assert.Greater(oldCount, taskHolder.childCount);
    }

    [Test, Order(11)]
    [Category("BuildServer")]
    public void RemoveTask_InvokesTaskRemovedEvent()
    {
        // Arrange
        listComp.CreateTaskListFromSO(taskListTopLeft);
        listComp.OnTaskRemoved.AddListener(Ping);

        // Act
        listComp.RemoveTask("Task 1");

        // Assert
        LogAssert.Expect(LogType.Log, "Progress event triggered");

        // Clean up
        listComp.OnTaskRemoved.RemoveListener(Ping);
    }

    [Test, Order(12)]
    [Category("BuildServer")]
    public void RemoveTask_WithInvalidName_LogsError()
    {
        // Arrange
        listComp.CreateTaskListFromSO(taskListTopLeft);
        string expectedMessage = "Unable to remove task Task 10";

        // Act
        listComp.RemoveTask("Task 10");

        // Assert
        LogAssert.Expect(LogType.Error, expectedMessage);
    }

    [Test, Order(13)]
    [Category("BuildServer")]
    public void SetTaskComplete_WithTrue_EnablesCompletedClasses()
    {
        // Arrange
        VisualElement taskHolder = listElement.Q("TaskHolder");
        listComp.CreateTaskListFromSO(taskListTopLeft);

        // Act
        listComp.SetTaskComplete("Task 1", true);

        // Assert
        Assert.IsTrue(taskHolder.Q("TaskIcon").ClassListContains("list-element-task-icon-completed"));
        Assert.IsTrue(taskHolder.Q("TaskLabel").ClassListContains("list-element-task-label-completed"));
        Assert.IsTrue(taskHolder.Q("Icon").ClassListContains("list-element-hint-icon-completed"));
    }

    [Test, Order(14)]
    [Category("BuildServer")]
    public void SetTaskComplete_SetsTaskToComplete()
    {
        // Arrange
        listComp.CreateTaskListFromSO(taskListTopLeft);

        // Act
        listComp.SetTaskComplete("Task 2", true);

        // Assert
        Assert.IsTrue(listComp.IsTaskComplete("Task 2"));
    }

    [Test, Order(15)]
    [Category("BuildServer")]
    public void IsTaskComplete_WithInvalidName_LogsError()
    {
        // Arrange
        listComp.CreateTaskListFromSO(taskListTopLeft);
        string expectedMessage = "Unable to check if task Task 8 is complete";

        // Act
        listComp.IsTaskComplete("Task 8");

        // Assert
        LogAssert.Expect(LogType.Error, expectedMessage);
    }

    [Test, Order(16)]
    [Category("BuildServer")]
    public void GetTaskIndex_WithValidName_ReturnsTaskName()
    {
        // Arrange
        int expectedIndex = 1;
        listComp.CreateTaskListFromSO(taskListTopLeft);

        // Assert
        Assert.AreEqual(expectedIndex, listComp.GetTaskIndex("Task 2"));
    }

    [Test, Order(17)]
    [Category("BuildServer")]
    public void GetTaskIndex_WithInvalidName_LogsError()
    {
        // Arrange
        listComp.CreateTaskListFromSO(taskListTopLeft);
        int expectedMessage = -1;

        // Act
        int taskIndex = listComp.GetTaskIndex("Task 8");

        // Assert
        Assert.AreEqual(taskIndex, expectedMessage);
    }

    [Test, Order(18)]
    [Category("BuildServer")]
    public void CompletedCount_ReturnsTheNumberOfTasksCompleted()
    {
        // Arrange
        int expectedCount = 3;
        listComp.CreateTaskListFromSO(taskListTopLeft);

        // Act
        listComp.SetTaskComplete("Task 1", true);
        listComp.SetTaskComplete("Task 2", true);
        listComp.SetTaskComplete("Task 3", true);

        // Assert
        Assert.AreEqual(expectedCount, listComp.CompletedCount);
    }

    [Test, Order(19)]
    [Category("BuildServer")]
    public void IsComplete_WithAllTasksComplete_ReturnsTrue()
    {
        // Arrange
        listComp.CreateTaskListFromSO(taskListTopLeft);

        // Act
        listComp.SetTaskComplete("Task 1", true);
        listComp.SetTaskComplete("Task 2", true);
        listComp.SetTaskComplete("Task 3", true);
        listComp.SetTaskComplete("Task 4", true);

        // Assert
        Assert.IsTrue(listComp.IsComplete);
    }

    [Test, Order(20)]
    [Category("BuildServer")]
    public void IsComplete_WithoutAllTasksComplete_ReturnsFalse()
    {
        // Arrange
        listComp.CreateTaskListFromSO(taskListTopLeft);

        // Act
        listComp.SetTaskComplete("Task 1", true);
        listComp.SetTaskComplete("Task 2", true);
        listComp.SetTaskComplete("Task 3", true);

        // Assert
        Assert.IsFalse(listComp.IsComplete);
    }

    [Test, Order(21)]
    [Category("BuildServer")]
    public void Show_InvokesListShownEvent()
    {
        // Arrange
        listComp.CreateTaskListFromSO(taskListTopLeft);
        listComp.OnListShown.AddListener(Ping);

        // Act
        listComp.Show();

        // Assert
        LogAssert.Expect(LogType.Log, "Progress event triggered");

        // Clean up
        listComp.OnListShown.RemoveListener(Ping);
    }

    [Test, Order(22)]
    [Category("BuildServer")]
    public void HideUI()
    {
        // Arrange
        listComp.CreateTaskListFromSO(taskListTopLeft);
        listComp.OnListHidden.AddListener(Ping);

        // Act
        listComp.Hide();

        // Assert
        LogAssert.Expect(LogType.Log, "Progress event triggered");

        // Clean up
        listComp.OnListHidden.RemoveListener(Ping);
    }

    [Test, Order(23)]
    [Category("BuildServer")]
    public void SetTaskComplete_WithTrue_InvokesTaskCompletedEvent()
    {
        // Arrange
        listComp.CreateTaskListFromSO(taskListTopLeft);
        listComp.OnTaskCompleted.AddListener(Ping);

        // Act
        listComp.SetTaskComplete("Task 2", true);

        // Assert
        LogAssert.Expect(LogType.Log, "Progress event triggered");

        // Clean up
        listComp.OnTaskCompleted.RemoveListener(Ping);
    }

    [Test, Order(24)]
    [Category("BuildServer")]
    public void SetTaskComplete_WithInvalidName_LogsError()
    {
        // Arrange
        listComp.CreateTaskListFromSO(taskListTopLeft);
        string expectedMessage = "Unable to set completion status of task Task 10";

        // Act
        listComp.SetTaskComplete("Task 10", true);

        // Assert
        LogAssert.Expect(LogType.Error, expectedMessage);
    }

    [Test, Order(25)]
    [Category("BuildServer")]
    public void SetListDisplayAnchor_TopLeft()
    {
        // Arrange
        listComp.CreateTaskListFromSO(taskListTopLeft);
        var expected = Align.FlexStart;

        // Act
        listComp.Show();
        bool contains = expected.ToString().Contains(listElement.style.alignSelf.ToString());

        // Assert
        Assert.IsTrue(contains);
    }

    [Test, Order(26)]
    [Category("BuildServer")]
    public void SetListDisplayAnchor_TopRight()
    {
        // Arrange
        listComp.CreateTaskListFromSO(taskListTopRight);
        var expected = Align.FlexEnd;

        // Act
        listComp.Show();
        bool contains = expected.ToString().Contains(listElement.style.alignSelf.ToString());

        // Assert
        Assert.IsTrue(contains);
    }

    private void Ping()
    {
        Debug.Log("Progress event triggered");
    }

    private void Ping(int arg0)
    {
        Ping();
    }
}
