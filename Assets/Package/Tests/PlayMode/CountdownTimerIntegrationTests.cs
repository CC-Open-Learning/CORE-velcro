using NUnit.Framework;
using System;
using System.Collections;
using UnityEditor;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.UIElements;
using VARLab.Velcro;

public class CountdownTimerIntegrationTests
{
    private int sceneCounter = 0;

    private GameObject timerObj;
    private UIDocument timerDoc;
    private CountdownTimer timer;
    private CountdownTimerElement timerElement;

    [UnitySetUp]
    [Category("BuildServer")]
    public IEnumerator SetUp()
    {
        sceneCounter = TestUtils.ClearScene(sceneCounter, "CountdownTimerScene");

        //Set up <CountdownTimer> UI
        timerObj = new GameObject("Timer Object");
        timerDoc = timerObj.AddComponent<UIDocument>();
        timer = timerObj.AddComponent<CountdownTimer>();

        //Load required assets from project files
        VisualTreeAsset timerUXML = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/VELCRO UI/UI/Timers/CountdownTimer.uxml");
        PanelSettings panelSettings = AssetDatabase.LoadAssetAtPath<PanelSettings>("Assets/VELCRO UI/Settings/Panel Settings.asset");

        //Reference panel settings and source asset as SerializedFields
        SerializedObject so = new SerializedObject(timerDoc);
        so.FindProperty("m_PanelSettings").objectReferenceValue = panelSettings;
        so.FindProperty("sourceAsset").objectReferenceValue = timerUXML;
        so.ApplyModifiedProperties();

        timerUXML.CloneTree(timerDoc.rootVisualElement);
        timerElement = timerDoc.rootVisualElement.Q<CountdownTimerElement>();
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
    public void HandleDisplayUI_SetsRootToDisplayFlex()
    {
        //Arrange
        StyleEnum<DisplayStyle> expectedStyle = new StyleEnum<DisplayStyle>(DisplayStyle.Flex);

        //Act
        timer.HandleDisplayUI();

        //Assert
        Assert.AreEqual(expectedStyle, timerDoc.rootVisualElement.style.display);
    }

    [Test, Order(3)]
    [Category("BuildServer")]
    public void HandleDisplayUI_StartTimeOutOfBounds_ClampsStartTime()
    {
        //Arrange
        float expectedTime = 0f;

        //Act
        timer.HandleDisplayUI(-72);

        //Assert
        Assert.AreEqual(expectedTime, timerElement.StartTime);
    }

    [Test, Order(4)]
    [Category("BuildServer")]
    public void HandleDisplayUI_SetsStartTime()
    {
        //Arrange
        float expectedStartTime = 3.0f;

        //Act
        timer.HandleDisplayUI();

        //Assert
        Assert.AreEqual(expectedStartTime, timerElement.StartTime);
    }

    [UnityTest, Order(5)]
    [Category("BuildServer")]
    public IEnumerator HandleDisplayUI_StartsTimerAndUpdatesCurrentTime()
    {
        //Arrange
        float expectedCurrentTime = 2.5f;

        //Act
        timer.HandleDisplayUI();
        yield return new WaitForSecondsRealtime(0.5f);
 
        //Assert
        Assert.AreEqual(expectedCurrentTime, Math.Round(timerElement.CurrentTime, 1));
    }

    [Test, Order(6)]
    [Category("BuildServer")]
    public void SetContent_SetsStartTime()
    {
        //Arrange
        float expectedStartTime = 3.0f;

        //Act
        timer.SetContent();

        //Assert
        Assert.AreEqual(expectedStartTime, timerElement.StartTime);
    }

    [Test, Order(7)]
    [Category("BuildServer")]
    public void ResetTimer_SetsCurrentTimeTo0()
    {
        //Arrange
        float expectedStartTime = 0.0f;

        //Act
        timer.ResetTimer();

        //Assert
        Assert.AreEqual(expectedStartTime, timerElement.CurrentTime);
    }

    [Test, Order(8)]
    [Category("BuildServer")]
    public void ResetTimer_SetsDisplayNone()
    {
        //Arrange
        StyleEnum<DisplayStyle> expectedStyle = new StyleEnum<DisplayStyle>(DisplayStyle.None);

        //Act
        timer.Show();
        timer.ResetTimer();

        //Assert
        Assert.AreEqual(expectedStyle, timerDoc.rootVisualElement.style.display);
    }

    [Test, Order(9)]
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

    [Test, Order(10)]
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