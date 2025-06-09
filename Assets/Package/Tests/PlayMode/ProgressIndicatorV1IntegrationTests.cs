using NUnit.Framework;
using System.Collections;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.UIElements;
using VARLab.Velcro;

public class ProgressIndicatorV1IntegrationTests
{
    private int sceneCounter;

    private GameObject progressObj;
    private UIDocument progressDoc;
    private ProgressIndicatorV1 progressComp;
    private VisualElement progressElement;
    private VisualElement categoryHolder;

    [UnitySetUp]
    [Category("BuildServer")]
    public IEnumerator SetUp()
    {
        sceneCounter = TestUtils.ClearScene(sceneCounter, "ProgressV1Scene");

        progressObj = new GameObject("Timer Object");
        progressDoc = progressObj.AddComponent<UIDocument>();
        progressComp = progressObj.AddComponent<ProgressIndicatorV1>();

        VisualTreeAsset progressUXML = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/VELCRO UI/UI/Progress Indicators/ProgressIndicatorV1.uxml");
        PanelSettings panelSettings = AssetDatabase.LoadAssetAtPath<PanelSettings>("Assets/VELCRO UI/Settings/Panel Settings.asset");
        VisualTreeAsset progressCategoryUXML = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/VELCRO UI/UI/Progress Indicators/ProgressIndicatorV1_Category.uxml");
        VisualTreeAsset progressTaskUXML = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/VELCRO UI/UI/Progress Indicators/ProgressIndicatorV1_Task.uxml");

        {
            SerializedObject so = new SerializedObject(progressDoc);
            so.FindProperty("m_PanelSettings").objectReferenceValue = panelSettings;
            so.FindProperty("sourceAsset").objectReferenceValue = progressUXML;
            so.ApplyModifiedProperties();
        }
        {
            SerializedObject so = new SerializedObject(progressComp);
            so.FindProperty("categoryAsset").objectReferenceValue = progressCategoryUXML;
            so.FindProperty("taskAsset").objectReferenceValue = progressTaskUXML;
            so.ApplyModifiedProperties();
        }

        progressUXML.CloneTree(progressDoc.rootVisualElement);

        progressElement = progressDoc.rootVisualElement.Q("ProgressIndicator");
        categoryHolder = progressElement.Q("CategoryHolder");

        yield return null;
    }

    [Test, Order(1)]
    [Category("BuildServer")]
    public void Start_SetsProgressToDisplayNone()
    {
        // Assert
        Assert.AreEqual(new StyleEnum<DisplayStyle>(DisplayStyle.None), progressElement.style.display);
    }

    [Test, Order(2)]
    [Category("BuildServer")]
    public void AddCategory_ChangesCategoryCount()
    {
        // Arrange
        int oldCount = progressComp.CategoryCount;

        // Act
        progressComp.AddCategory("Category 1");

        // Assert
        Assert.Less(oldCount, progressComp.CategoryCount);
    }

    [Test, Order(3)]
    [Category("BuildServer")]
    public void AddCategory_AddsVisualElementToCategoryHolder()
    {
        // Arrange
        int oldCount = categoryHolder.childCount;

        // Act
        progressComp.AddCategory("Category 1");

        // Assert
        Assert.Less(oldCount, categoryHolder.childCount);
    }

    [Test, Order(4)]
    [Category("BuildServer")]
    public void AddTask_WithValidIndex_ChangesTaskCount()
    {
        // Arrange & Act
        progressComp.AddCategory("Category 1");
        int oldCount = progressComp.GetTaskCount(0);

        progressComp.AddTask(0, "Task 1-0", 5);

        // Assert
        Assert.Less(oldCount, progressComp.GetTaskCount(0));
    }

    [Test, Order(5)]
    [Category("BuildServer")]
    public void AddTask_WithValidIndex_AddsVisualElementToTaskHolder()
    {
        // Arrange & Act
        progressComp.AddCategory("Category 1");

        VisualElement taskHolder = categoryHolder.Children().First().Q("TaskHolder");
        int oldCount = taskHolder.childCount;

        progressComp.AddTask(0, "Task 1-0", 5);

        // Assert
        Assert.Less(oldCount, taskHolder.childCount);
    }

    [Test, Order(6)]
    [Category("BuildServer")]
    public void FindCategoryWithName_WithValidName_ReturnsCategoryIndex()
    {
        // Arrange
        int expectedIndex = 1;
        progressComp.AddCategory("Category 1");
        progressComp.AddCategory("Category 2");

        // Act
        progressComp.FindCategoryWithName("Category 2", out int index);

        // Assert
        Assert.AreEqual(expectedIndex, index);
    }

    [Test, Order(7)]
    [Category("BuildServer")]
    public void FindCategoryWithName_WithoutValidName_ReturnsInvalidIndex()
    {
        // Arrange
        int expectedIndex = -1;
        progressComp.AddCategory("Category 2");

        // Act
        progressComp.FindCategoryWithName("Invalid Category", out int index);

        // Assert
        Assert.AreEqual(expectedIndex, index);
    }

    [Test, Order(8)]
    [Category("BuildServer")]
    public void FindTaskWithName_WithCategoryAndValidIndex_WithValidName_ReturnsTaskIndex()
    {
        // Arrange
        int expectedIndex = 1;
        progressComp.AddCategory("Category 2");
        progressComp.AddTask(0, "Task 1-0", 5);
        progressComp.AddTask(0, "Task 2-0", 10);
        progressComp.AddTask(0, "Task 3-0", 10);

        // Act
        bool found = progressComp.FindTaskWithName("Task 2-0", 0, out int index);

        // Assert
        Assert.IsTrue(found);
        Assert.AreEqual(expectedIndex, index);
    }

    [Test, Order(9)]
    [Category("BuildServer")]
    public void FindTaskWithName_WithCategoryAndValidIndex_WithoutValidName_ReturnsInvalidIndex()
    {
        // Arrange
        int expectedIndex = -1;
        progressComp.AddCategory("Category 2");
        progressComp.AddTask(0, "Task 0", 5);

        // Act
        bool found = progressComp.FindTaskWithName("Invalid Category", 0, out int index);

        // Assert
        Assert.IsFalse(found);
        Assert.AreEqual(expectedIndex, index);
    }

    [Test, Order(10)]
    [Category("BuildServer")]
    public void FindTaskWithName_WithoutCategory_WithValidName_ReturnsTaskIndex()
    {
        // Arrange
        int expectedIndex = 4;
        progressComp.AddCategory("Category 2");
        progressComp.AddTask(0, "Task 1-0", 5);
        progressComp.AddTask(0, "Task 2-0", 5);
        progressComp.AddTask(0, "Task 3-0", 5);
        progressComp.AddTask(0, "Task 4-0", 5);
        progressComp.AddTask(0, "Task 5-0", 5);
        progressComp.AddTask(0, "Task 6-0", 5);

        // Act
        bool found = progressComp.FindTaskWithName("Task 5-0", out int category, out int task);

        // Assert
        Assert.IsTrue(found);
        Assert.AreEqual(expectedIndex, task);
    }

    [Test, Order(11)]
    [Category("BuildServer")]
    public void FindTaskWithName_WithoutCategory_WithoutValidName_ReturnsInvalidIndex()
    {
        // Arrange
        int expectedIndex = -1;

        // Act
        bool found = progressComp.FindTaskWithName("Invalid Task", out int category, out int task);

        // Assert
        Assert.IsFalse(found);
        Assert.AreEqual(expectedIndex, task);
    }

    [Test, Order(12)]
    [Category("BuildServer")]
    public void HandleDisplayUI_SetsProgressDisplayToFlex()
    {
        // Act
        progressComp.HandleDisplayUI("My Progress");

        // Assert
        Assert.AreEqual(new StyleEnum<DisplayStyle>(DisplayStyle.Flex), progressElement.style.display);
    }

    [Test, Order(13)]
    [Category("BuildServer")]
    public void HandleDisplayUI_InvokesShowEvent()
    {
        // Arrange
        progressComp.OnShow.AddListener(Ping);

        // Act
        progressComp.HandleDisplayUI("My Progress");

        // Assert
        LogAssert.Expect(LogType.Log, "Progress event triggered");

        // Clean Up
        progressComp.OnShow.RemoveListener(Ping);
    }

    [Test, Order(14)]
    [Category("BuildServer")]
    public void Hide_InvokesHideEvent()
    {
        // Arrange
        progressComp.OnHide.AddListener(Ping);

        // Act
        progressComp.Hide();

        // Assert
        LogAssert.Expect(LogType.Log, "Progress event triggered");

        // Clean Up
        progressComp.OnHide.RemoveListener(Ping);
    }

    [Test, Order(15)]
    [Category("BuildServer")]
    public void AddCategory_InvokesCategoryAddedEvent()
    {
        // Arrange
        progressComp.OnCategoryAdded.AddListener(Ping);

        // Act
        progressComp.AddCategory("Category 1");

        // Assert
        LogAssert.Expect(LogType.Log, "Progress event triggered");

        // Clean Up
        progressComp.OnCategoryAdded.RemoveListener(Ping);
    }

    [Test, Order(16)]
    [Category("BuildServer")]
    public void AddTask_WithValidIndices_InvokesTaskAddedEvent()
    {
        // Arrange
        progressComp.OnTaskAdded.AddListener(Ping);
        progressComp.AddCategory("Category 1");

        // Act
        progressComp.AddTask(0, "Task 1", 5);

        // Assert
        LogAssert.Expect(LogType.Log, "Progress event triggered");

        // Clean Up
        progressComp.OnTaskAdded.RemoveListener(Ping);
    }

    [Test, Order(17)]
    [Category("BuildServer")]
    public void GetTaskCount_WithValidIndex_ReturnsTheTaskCount()
    {
        // Arrange
        int expectedCount = 5;
        progressComp.AddCategory("Category 1");
        progressComp.AddTask(0, "Task 1", 5);
        progressComp.AddTask(0, "Task 2", 5);
        progressComp.AddTask(0, "Task 3", 5);
        progressComp.AddTask(0, "Task 4", 5);
        progressComp.AddTask(0, "Task 5", 5);

        // Assert
        Assert.AreEqual(expectedCount, progressComp.GetTaskCount(0));
    }

    [Test, Order(18)]
    [Category("BuildServer")]
    public void AddProgressToTask_WithValidIndices_AddsProgressToTask()
    {
        // Arrange & Act
        progressComp.AddCategory("Category 1");
        progressComp.AddTask(0, "Task 5", 5);

        int oldProgress = progressComp.GetTaskProgress(0, 0);
        progressComp.AddProgressToTask(0, 0);

        // Assert
        Assert.Less(oldProgress, progressComp.GetTaskProgress(0, 0));
    }

    [Test, Order(19)]
    [Category("BuildServer")]
    public void AddProgressToTask_WithValidIndices_InvokesTaskProgressUpdatedEvent()
    {
        // Arrange
        progressComp.OnProgressUpdated.AddListener(Ping);
        progressComp.AddCategory("Category 1");
        progressComp.AddTask(0, "Task 5", 5);

        // Act
        progressComp.AddProgressToTask(0, 0);

        // Assert
        LogAssert.Expect(LogType.Log, "Progress event triggered");

        // Clean up
        progressComp.OnProgressUpdated.RemoveListener(Ping);
    }

    [Test, Order(20)]
    [Category("BuildServer")]
    public void AddCategory_RemovesClassesOnOldLastCategory()
    {
        // Arrange
        string CategoryLastBorderRadiusClass = "progress-v1-category-last";
        string CategoryLastBottomClass = "progress-v1-category-last-bottom";

        // Act
        progressComp.AddCategory("Category 1");
        progressComp.AddCategory("Category 2");

        VisualElement lastCategory = categoryHolder.Children().ToArray()[categoryHolder.childCount - 2].Q("Category");

        // Assert
        Assert.IsFalse(lastCategory.ClassListContains(CategoryLastBorderRadiusClass));
        Assert.IsFalse(lastCategory.ClassListContains(CategoryLastBottomClass));
    }

    [Test, Order(21)]
    [Category("BuildServer")]
    public void AddProgressToTask_UpdatesProgressLabels()
    {
        // Arrange
        progressComp.AddCategory("Category 1");
        progressComp.AddTask(0, "Task 1", 5);

        VisualElement category = categoryHolder.Children().First();
        ProgressIndicatorV1Bar progressBar = category.Q<ProgressIndicatorV1Bar>("ProgressBar");
        Label progressLabel = category.Q<Label>("ProgressLabel");
        Label taskProgressLabel = category.Q("TaskHolder").Children().First().Q<Label>("ProgressLabel");

        float oldProgress = progressBar.Value;
        string oldProgressLabelText = progressLabel.text;
        string oldTaskProgressLabelText = taskProgressLabel.text;

        // Act
        progressComp.AddProgressToTask(0, 0, 5);

        // Assert
        Assert.AreNotEqual(oldProgress, progressBar.Value);
        Assert.AreNotEqual(oldProgressLabelText, progressLabel.text);
        Assert.AreNotEqual(oldTaskProgressLabelText, taskProgressLabel.text);
    }

    [Test, Order(22)]
    [Category("BuildServer")]
    public void GetCategoryName_WithValidIndex_ReturnsCategoryName()
    {
        // Arrange
        string expectedName = "Category 1";
        progressComp.AddCategory("Category 1");

        // Assert
        Assert.AreEqual(expectedName, progressComp.GetCategoryName(0));
    }

    [Test, Order(23)]
    [Category("BuildServer")]
    public void GetCategoryName_WithInvalidIndex_LogsError()
    {
        // Act
        string name = progressComp.GetCategoryName(-6);

        // Assert
        Assert.IsNull(name);
        LogAssert.Expect(LogType.Error, "Unable to get category name at invalid category index -6");
    }

    [Test, Order(24)]
    [Category("BuildServer")]
    public void GetTaskName_WithValidIndices_ReturnsTaskName()
    {
        // Arrange
        string expectedName = "Task 1";
        progressComp.AddCategory("Category 1");
        progressComp.AddTask(0, "Task 1", 5);

        // Assert
        Assert.AreEqual(expectedName, progressComp.GetTaskName(0, 0));
    }

    [Test, Order(25)]
    [Category("BuildServer")]
    public void GetTaskName_WithInvalidCategoryIndex_LogsError()
    {
        // Arrange
        progressComp.AddCategory("Category 1");
        progressComp.AddTask(0, "Task 1", 5);

        // Act
        string name = progressComp.GetTaskName(5, 0);

        // Assert
        Assert.IsNull(name);
        LogAssert.Expect(LogType.Error, "Unable to get task name at invalid category index 5");
    }

    [Test, Order(26)]
    [Category("BuildServer")]
    public void GetTaskName_WithInvalidTaskIndex_LogsError()
    {
        // Arrange
        progressComp.AddCategory("Category 1");
        progressComp.AddTask(0, "Task 1", 5);

        // Act
        string name = progressComp.GetTaskName(0, 4);

        // Assert
        Assert.IsNull(name);
        LogAssert.Expect(LogType.Error, "Unable to get task name at invalid task index 4");
    }

    [Test, Order(27)]
    [Category("BuildServer")]
    public void GetTaskProgress_WithValidIndex_ReturnsTaskProgress()
    {
        // Arrange
        int expectedProgress = 4;
        progressComp.AddCategory("Category 1");
        progressComp.AddTask(0, "Task 1", 5);
        progressComp.AddProgressToTask(0, 0, 4);

        // Assert
        Assert.AreEqual(expectedProgress, progressComp.GetTaskProgress(0, 0));
    }

    [Test, Order(28)]
    [Category("BuildServer")]
    public void GetTaskProgress_WithInvalidCategoryIndex_LogsError()
    {
        // Arrange
        progressComp.AddCategory("Category 1");
        progressComp.AddTask(0, "Task 1", 5);

        // Act
        int progress = progressComp.GetTaskProgress(5, 0);

        // Assert
        Assert.AreEqual(-1, progress);
        LogAssert.Expect(LogType.Error, "Unable to get task progress at invalid category index 5");
    }

    [Test, Order(29)]
    [Category("BuildServer")]
    public void GetTaskProgress_WithInvalidTaskIndex_LogsError()
    {
        // Arrange
        progressComp.AddCategory("Category 1");
        progressComp.AddTask(0, "Task 1", 5);

        // Act
        int progress = progressComp.GetTaskProgress(0, 4);

        // Assert
        Assert.AreEqual(-1, progress);
        LogAssert.Expect(LogType.Error, "Unable to get task progress at invalid task index 4");
    }

    [Test, Order(30)]
    [Category("BuildServer")]
    public void GetTaskMaxProgress_WithValidIndex_ReturnsTaskProgress()
    {
        // Arrange
        int expectedProgress = 5;
        progressComp.AddCategory("Category 1");
        progressComp.AddTask(0, "Task 1", 5);
        progressComp.AddProgressToTask(0, 0, 4);

        // Assert
        Assert.AreEqual(expectedProgress, progressComp.GetTaskMaxProgress(0, 0));
    }

    [Test, Order(31)]
    [Category("BuildServer")]
    public void GetTaskMaxProgress_WithInvalidCategoryIndex_LogsError()
    {
        // Arrange
        progressComp.AddCategory("Category 1");
        progressComp.AddTask(0, "Task 1", 5);

        // Act
        int maxProgress = progressComp.GetTaskMaxProgress(5, 0);

        // Assert
        Assert.AreEqual(-1, maxProgress);
        LogAssert.Expect(LogType.Error, "Unable to get task max progress at invalid category index 5");
    }

    [Test, Order(32)]
    [Category("BuildServer")]
    public void GetTaskMaxProgress_WithInvalidTaskIndex_LogsError()
    {
        // Arrange
        progressComp.AddCategory("Category 1");
        progressComp.AddTask(0, "Task 1", 5);

        // Act
        int maxProgress = progressComp.GetTaskMaxProgress(0, 4);

        // Assert
        Assert.AreEqual(-1, maxProgress);
        LogAssert.Expect(LogType.Error, "Unable to get task max progress at invalid task index 4");
    }

    [Test, Order(33)]
    [Category("BuildServer")]
    public void AddTask_WithInvalidCategoryIndex_LogsError()
    {

        // Act
        progressComp.AddTask(6, "Task 1", 5);

        // Assert
        LogAssert.Expect(LogType.Error, "Unable to add task at invalid category index 6");
    }

    [Test, Order(34)]
    [Category("BuildServer")]
    public void RemoveCategory_WithValidIndex_ChangesCategoryCount()
    {
        // Arrange
        progressComp.AddCategory("Category 1");
        int oldCount = progressComp.CategoryCount;

        // Act
        progressComp.RemoveCategory(0);

        // Assert
        Assert.Greater(oldCount, progressComp.CategoryCount);
    }

    [Test, Order(35)]
    [Category("BuildServer")]
    public void RemoveCategory_WithValidIndex_AddsVisualElementToCategoryHolder()
    {
        // Arrange
        progressComp.AddCategory("Category 1");
        int oldCount = categoryHolder.childCount;

        // Act
        progressComp.RemoveCategory(0);

        // Assert
        Assert.Greater(oldCount, categoryHolder.childCount);
    }

    [Test, Order(36)]
    [Category("BuildServer")]
    public void RemoveCategory_WithInvalidCategoryIndex_LogsError()
    {
        // Act
        progressComp.RemoveCategory(-4);

        // Assert
        LogAssert.Expect(LogType.Error, "Unable to remove category at invalid category index -4");
    }

    [Test, Order(37)]
    [Category("BuildServer")]
    public void RemoveTask_WithValidIndex_ChangesTaskCount()
    {
        // Arrange
        progressComp.AddCategory("Category 1");
        progressComp.AddTask(0, "Task 1-0", 5);
        int oldCount = progressComp.GetTaskCount(0);

        // Act
        progressComp.RemoveTask(0, 0);

        // Assert
        Assert.Greater(oldCount, progressComp.GetTaskCount(0));
    }

    [Test, Order(38)]
    [Category("BuildServer")]
    public void RemoveTask_WithValidIndex_AddsVisualElementToTaskHolder()
    {
        // Arrange
        progressComp.AddCategory("Category 1");

        VisualElement taskHolder = categoryHolder.Children().First().Q("TaskHolder");
        progressComp.AddTask(0, "Task 1-0", 5);
        int oldCount = taskHolder.childCount;

        // Act
        progressComp.RemoveTask(0, 0);

        // Assert
        Assert.Greater(oldCount, taskHolder.childCount);
    }

    [Test, Order(39)]
    [Category("BuildServer")]
    public void RemoveTask_WithInvalidCategoryIndex_LogsError()
    {
        // Act
        progressComp.RemoveTask(-3, 0);

        // Assert
        LogAssert.Expect(LogType.Error, "Unable to remove task at invalid category index -3");
    }

    [Test, Order(40)]
    [Category("BuildServer")]
    public void RemoveTask_WithInvalidTaskIndex_LogsError()
    {
        // Arrange
        progressComp.AddCategory("Category 1");

        // Act
        progressComp.RemoveTask(0, 8);

        // Assert
        LogAssert.Expect(LogType.Error, "Unable to remove task at invalid task index 8");
    }

    [Test, Order(41)]
    [Category("BuildServer")]
    public void AddProgressToTask_WithInvalidCategoryIndex_LogsError()
    {
        // Act
        progressComp.AddProgressToTask(99, 0);

        // Assert
        LogAssert.Expect(LogType.Error, "Unable to add progress to task at invalid category index 99");
    }

    [Test, Order(42)]
    [Category("BuildServer")]
    public void AddProgressToTask_WithInvalidTaskIndex_LogsError()
    {
        // Arrange
        progressComp.AddCategory("Category 1");

        // Act
        progressComp.AddProgressToTask(0, 99);

        // Assert
        LogAssert.Expect(LogType.Error, "Unable to add progress to task at invalid task index 99");
    }

    [Test, Order(43)]
    [Category("BuildServer")]
    public void FindTaskWithName_WithInvalidCategoryIndex_ReturnsFalse()
    {
        // Arrange
        progressComp.AddCategory("Category 1");

        // Act
        bool found = progressComp.FindTaskWithName("Task 100", 7, out int taskIndex);

        // Assert
        Assert.IsFalse(found);
        Assert.AreEqual(-1, taskIndex);
    }

    [Test, Order(44)]
    [Category("BuildServer")]
    public void GetTaskCount_WithInvalidCategoryIndex_LogsError()
    {
        // Arrange
        progressComp.AddCategory("Category 1");

        // Act
        int count = progressComp.GetTaskCount(-30);

        // Assert
        Assert.AreEqual(-1, count);
        LogAssert.Expect(LogType.Error, "Unable to get task at invalid category index -30");
    }

    [Test, Order(45)]
    [Category("BuildServer")]
    public void RemoveCategory_WithValidIndex_InvokesCategoryRemovedEvent()
    {
        // Arrange
        progressComp.AddCategory("Category 1");
        progressComp.OnCategoryRemoved.AddListener(Ping);

        // Act
        progressComp.RemoveCategory(0);

        // Assert
        LogAssert.Expect(LogType.Log, "Progress event triggered");

        // Clean Up
        progressComp.OnCategoryRemoved.RemoveListener(Ping);
    }

    [Test, Order(46)]
    [Category("BuildServer")]
    public void RemoveTask_WithValidIndices_InvokesTaskRemovedEvent()
    {
        // Arrange
        progressComp.AddCategory("Category 1");
        progressComp.AddTask(0, "Task 1", 5);
        progressComp.OnTaskRemoved.AddListener(Ping);

        // Act
        progressComp.RemoveTask(0, 0);

        // Assert
        LogAssert.Expect(LogType.Log, "Progress event triggered");

        // Clean Up
        progressComp.OnTaskRemoved.RemoveListener(Ping);
    }

    [Test, Order(47)]
    [Category("BuildServer")]
    public void RemoveProgressFromTask_WithValidIndices_RemovesProgressFromTask()
    {
        // Arrange & Act
        progressComp.AddCategory("Category 1");
        progressComp.AddTask(0, "Task 5", 5);
        progressComp.AddProgressToTask(0, 0, 5);

        int oldProgress = progressComp.GetTaskProgress(0, 0);
        progressComp.RemoveProgressFromTask(0, 0, 3);

        // Assert
        Assert.Greater(oldProgress, progressComp.GetTaskProgress(0, 0));
    }

    [Test, Order(48)]
    [Category("BuildServer")]
    public void RemoveProgressFromTask_WithValidIndices_InvokesTaskProgressUpdatedEvent()
    {
        // Arrange
        progressComp.AddCategory("Category 1");
        progressComp.AddTask(0, "Task 5", 5);
        progressComp.AddProgressToTask(0, 0, 5);
        progressComp.OnProgressUpdated.AddListener(Ping);

        // Act
        progressComp.RemoveProgressFromTask(0, 0);

        // Assert
        LogAssert.Expect(LogType.Log, "Progress event triggered");

        // Clean up
        progressComp.OnProgressUpdated.RemoveListener(Ping);
    }

    [Test, Order(49)]
    [Category("BuildServer")]
    public void RemoveProgressFromTask_UpdatesProgressLabels()
    {
        // Arrange
        progressComp.AddCategory("Category 1");
        progressComp.AddTask(0, "Task 1", 5);
        progressComp.AddProgressToTask(0, 0, 5);

        VisualElement category = categoryHolder.Children().First();
        ProgressIndicatorV1Bar progressBar = category.Q<ProgressIndicatorV1Bar>("ProgressBar");
        Label progressLabel = category.Q<Label>("ProgressLabel");
        Label taskProgressLabel = category.Q("TaskHolder").Children().First().Q<Label>("ProgressLabel");

        float oldProgress = progressBar.Value;
        string oldProgressLabelText = progressLabel.text;
        string oldTaskProgressLabelText = taskProgressLabel.text;

        // Act
        progressComp.RemoveProgressFromTask(0, 0, 2);

        // Assert
        Assert.AreNotEqual(oldProgress, progressBar.Value);
        Assert.AreNotEqual(oldProgressLabelText, progressLabel.text);
        Assert.AreNotEqual(oldTaskProgressLabelText, taskProgressLabel.text);
    }

    [Test, Order(50)]
    [Category("BuildServer")]
    public void RemoveProgressFromTask_WithInvalidCategoryIndex_LogsError()
    {
        // Act
        progressComp.RemoveProgressFromTask(99, 0);

        // Assert
        LogAssert.Expect(LogType.Error, "Unable to remove progress from task at invalid category index 99");
    }

    [Test, Order(51)]
    [Category("BuildServer")]
    public void RemoveProgressFromTask_WithInvalidTaskIndex_LogsError()
    {
        // Arrange
        progressComp.AddCategory("Category 1");

        // Act
        progressComp.RemoveProgressFromTask(0, 99);

        // Assert
        LogAssert.Expect(LogType.Error, "Unable to remove progress from task at invalid task index 99");
    }

    [Test, Order(52)]
    [Category("BuildServer")]
    public void HandleDisplayUI_SetsTitleLabelText()
    {
        // Arrange
        string expectedTitle = "My Progress";

        // Act
        progressComp.HandleDisplayUI("My Progress");

        // Assert
        Assert.AreEqual(expectedTitle, progressElement.Q<Label>("TitleLabel").text);
    }

    private void Ping()
    {
        Debug.Log("Progress event triggered");
    }

    private void Ping(int arg0)
    {
        Debug.Log("Progress event triggered");
    }

    private void Ping(int arg0, int arg1)
    {
        Debug.Log("Progress event triggered");
    }
}
