using System.Collections;
using UnityEditor;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.UIElements;
using VARLab.Velcro;
using NUnit.Framework;

public class CountupTimerIntegrationTests
{
    private int sceneCounter = 0;

    private GameObject timerObj;
    private UIDocument timerDoc;
    private CountupTimer timer;

    [UnitySetUp]
    [Category("BuildServer")]
    public IEnumerator SetUp()
    {
        sceneCounter = TestUtils.ClearScene(sceneCounter, "CountupTimerScene");

        //Set up <CountupTimer> UI
        timerObj = new GameObject("Timer Object");
        timerDoc = timerObj.AddComponent<UIDocument>();
        timer = timerObj.AddComponent<CountupTimer>();

        //Load required assets from project files
        VisualTreeAsset timerUXML = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/VELCRO UI/UI/Timers/CountupTimer.uxml");
        PanelSettings panelSettings = AssetDatabase.LoadAssetAtPath<PanelSettings>("Assets/Package/Samples/Settings/Panel Settings.asset");

        //Reference panel settings and source asset as SerializedFields
        SerializedObject so = new SerializedObject(timerDoc);
        so.FindProperty("m_PanelSettings").objectReferenceValue = panelSettings;
        so.FindProperty("sourceAsset").objectReferenceValue = timerUXML;
        so.ApplyModifiedProperties();

        timerUXML.CloneTree(timerDoc.rootVisualElement);
        yield return null;
    }

    [Test, Order(1)]
    [Category("BuildServer")]
    public void Start_SetsTimerToDisplayNone()
    {
        //Arrange
        StyleEnum<DisplayStyle> expectedStyle = new StyleEnum<DisplayStyle>(DisplayStyle.None);

        //Assert
        Assert.AreEqual(expectedStyle, timerDoc.rootVisualElement.style.display);
    }

    [Test, Order(2)]
    [Category("BuildServer")]
    public void Start_SetsCloseUSSClasses()
    {
        //Arrange
        string expectedBodyUSSClassName = "timer-closed";
        string expectedArrowUSSClassName = "timer-arrow-closed";

        //Assert
        Assert.IsTrue(timerDoc.rootVisualElement.Q("CountupTimer").ClassListContains(expectedBodyUSSClassName));
        Assert.IsTrue(timerDoc.rootVisualElement.Q("Arrow").ClassListContains(expectedArrowUSSClassName));
    }

    [Test, Order(3)]
    [Category("BuildServer")]
    public void HandleDisplayUI_SetsRootToDisplayFlex()
    {
        //Arrange
        StyleEnum<DisplayStyle> expectedStyle = new StyleEnum<DisplayStyle>(DisplayStyle.Flex);

        //Act
        timer.HandleDisplayUI();

        //Assert
        Assert.AreEqual(expectedStyle, timerDoc.rootVisualElement.style.display);
    }

    [Test, Order(4)]
    [Category("BuildServer")]
    public void HandleDisplayUI_SetsIsOpenIsPaused()
    {
        //Arrange
        bool expectedOpenState = false;
        bool expectedPauseState = true;

        //Act
        timer.HandleDisplayUI(false, true);

        //Assert
        Assert.AreEqual(expectedOpenState, timer.IsOpen);
        Assert.AreEqual(expectedPauseState, timer.IsPaused);
    }

    [Test, Order(5)]
    [Category("BuildServer")]
    public void HandleDisplayUI_WithStartToggled_DisablesClosedClass()
    {
        //Arrange
        string expectedBodyUSSClassName = "timer-closed";
        string expectedArrowUSSClassName = "timer-arrow-closed";

        //Act
        timer.HandleDisplayUI(true, false);

        //Assert
        Assert.IsFalse(timerDoc.rootVisualElement.Q("CountupTimer").ClassListContains(expectedBodyUSSClassName));
        Assert.IsFalse(timerDoc.rootVisualElement.Q("Arrow").ClassListContains(expectedArrowUSSClassName));
    }

    [Test, Order(6)]
    [Category("BuildServer")]
    public void ToggleTimer_TogglesUSSClasses()
    {
        //Arrange
        string expectedBodyUSSClassName = "timer-closed";
        string expectedArrowUSSClassName = "timer-arrow-closed";

        //Act
        timer.HandleDisplayUI();
        timer.ToggleTimer();

        //Assert
        Assert.IsTrue(timerDoc.rootVisualElement.Q("CountupTimer").ClassListContains(expectedBodyUSSClassName));
        Assert.IsTrue(timerDoc.rootVisualElement.Q("Arrow").ClassListContains(expectedArrowUSSClassName));
    }

    [Test, Order(7)]
    [Category("BuildServer")]
    public void Pause_SetsIsPausedTrue()
    {
        //Arrange
        bool expectedPauseState = true;

        //Act
        timer.HandleDisplayUI();
        timer.Pause();

        //Assert
        Assert.AreEqual(expectedPauseState, timer.IsPaused);
    }

    [Test, Order(8)]
    [Category("BuildServer")]
    public void Resume_SetsIsPausedFalse()
    {
        //Arrange
        bool expectedPauseState = false;

        //Act
        timer.HandleDisplayUI();
        timer.Pause();
        timer.Resume();

        //Assert
        Assert.AreEqual(expectedPauseState, timer.IsPaused);
    }

    [Test, Order(9)]
    [Category("BuildServer")]
    public void ResetTimer_ResetsAllValues()
    {
        //Arrange
        StyleEnum<DisplayStyle> expectedStyle = new StyleEnum<DisplayStyle>(DisplayStyle.None);
        string expectedBodyUSSClassName = "timer-closed";
        string expectedArrowUSSClassName = "timer-arrow-closed";
        bool expectedOpenState = false;
        bool expectedPauseState = true;
        double expectedElapsedTime = 0d;

        //Act
        timer.HandleDisplayUI();
        timer.ResetTimer();

        //Assert
        Assert.AreEqual(expectedStyle, timerDoc.rootVisualElement.style.display);
        Assert.AreEqual(expectedElapsedTime, timer.ElapsedTime);
        Assert.AreEqual(expectedOpenState, timer.IsOpen);
        Assert.AreEqual(expectedPauseState, timer.IsPaused);
        Assert.IsTrue(timerDoc.rootVisualElement.Q("CountupTimer").ClassListContains(expectedBodyUSSClassName));
        Assert.IsTrue(timerDoc.rootVisualElement.Q("Arrow").ClassListContains(expectedArrowUSSClassName));
    }

    [Test, Order(10)]
    [Category("BuildServer")]
    public void SetElapsedTime_SetsNewTime()
    {
        //Arrange
        double expectedElapsedTime = 10d;

        //Act
        timer.HandleDisplayUI();
        timer.SetElapsedTime(10d);

        //Assert
        Assert.AreEqual(expectedElapsedTime, timer.ElapsedTime);
    }

    [Test, Order(11)]
    [Category("BuildServer")]
    public void Show_SetsDisplayFlex()
    {
        //Arrange
        StyleEnum<DisplayStyle> expectedStyle = new StyleEnum<DisplayStyle>(DisplayStyle.Flex);

        //Act
        timer.Show();

        //Assert
        Assert.AreEqual(expectedStyle, timerDoc.rootVisualElement.style.display);
    }

    [Test, Order(12)]
    [Category("BuildServer")]
    public void Hide_SetsDisplayNone()
    {
        //Arrange
        StyleEnum<DisplayStyle> expectedStyle = new StyleEnum<DisplayStyle>(DisplayStyle.None);

        //Act
        timer.Show();
        timer.Hide();

        //Assert
        Assert.AreEqual(expectedStyle, timerDoc.rootVisualElement.style.display);
    }
}