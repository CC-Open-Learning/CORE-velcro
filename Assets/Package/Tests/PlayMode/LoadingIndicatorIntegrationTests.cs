using System.Collections;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.UIElements;
using UnityEditor;
using NUnit.Framework;
using VARLab.Velcro;
using System;

public class LoadingIndicatorIntegrationTests
{
    private int sceneCounter = 0;

    private GameObject loadingObj;
    private UIDocument loadingDoc;
    private LoadingIndicator loadingIndicator;
    private RadialLoadingProgress loadingElement;

    [UnitySetUp]
    [Category("BuildServer")]
    public IEnumerator SetUp()
    {
        sceneCounter = TestUtils.ClearScene(sceneCounter, "LoadingScene");

        //Set up <LoadingIndicator> UI
        loadingObj = new GameObject("Loading Object");
        loadingDoc = loadingObj.AddComponent<UIDocument>();
        loadingIndicator = loadingObj.AddComponent<LoadingIndicator>();

        //Load required assets from project files
        VisualTreeAsset loadingUXML = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/VELCRO UI/UI/Loading Indicators/LoadingIndicator.uxml");
        PanelSettings panelSettings = AssetDatabase.LoadAssetAtPath<PanelSettings>("Assets/Package/Samples/Settings/Panel Settings.asset");

        //Reference panel settings and source asset as SerializedFields
        SerializedObject so = new SerializedObject(loadingDoc);
        so.FindProperty("m_PanelSettings").objectReferenceValue = panelSettings;
        so.FindProperty("sourceAsset").objectReferenceValue = loadingUXML;
        so.ApplyModifiedProperties();

        loadingUXML.CloneTree(loadingDoc.rootVisualElement);
        loadingElement = loadingDoc.rootVisualElement.Q("LoadingContainer").Q<RadialLoadingProgress>();
        yield return null;
    }

    [Test, Order(1)]
    [Category("BuildServer")]
    public void Start_SetsContainerToDisplayNone()
    {
        //Arrange
        StyleEnum<DisplayStyle> expectedStyle = new StyleEnum<DisplayStyle>(DisplayStyle.None);

        //Assert
        Assert.AreEqual(expectedStyle, loadingDoc.rootVisualElement.style.display);
    }

    [Test, Order(2)]
    [Category("BuildServer")]
    public void HandleDisplayUI_WithDefault_SetsRootToDisplayFlex()
    {
        //Arrange
        StyleEnum<DisplayStyle> expectedStyle = new StyleEnum<DisplayStyle>(DisplayStyle.Flex);

        //Act
        loadingIndicator.HandleDisplayUI();

        //Assert
        Assert.AreEqual(expectedStyle, loadingDoc.rootVisualElement.style.display);
    }

    [Test, Order(3)]
    [Category("BuildServer")]
    public void HandleDisplayUI_WithValidElements_StartsTimer()
    {
        //Arrange
        StyleEnum<DisplayStyle> expectedStyle = new StyleEnum<DisplayStyle>(DisplayStyle.Flex);
        float expectedProgress = 50.0f;
        bool expectedLoadingState = true;

        //Act
        loadingIndicator.HandleDisplayUI(50.0f);

        //Assert
        Assert.AreEqual(expectedStyle, loadingDoc.rootVisualElement.style.display);
        Assert.AreEqual(expectedLoadingState, loadingIndicator.IsLoading);
        Assert.AreEqual(expectedProgress, Math.Round(loadingElement.Progress, 0));
    }

    [Test, Order(4)]
    [Category("BuildServer")]
    public void SetContent_AsDimmed_SetsCanvasToDimmed()
    {
        //Arrange
        string expectedUSSClass = "loading-indicator-canvas-dimmed";

        SerializedObject so = new SerializedObject(loadingIndicator);
        so.FindProperty("isBackgroundDimmed").boolValue = true;
        so.ApplyModifiedProperties();

        //Act
        loadingIndicator.SetContent(50.0f);

        //Assert
        Assert.IsTrue(loadingDoc.rootVisualElement.Q<VisualElement>("Canvas").ClassListContains(expectedUSSClass));
    }

    [Test, Order(5)]
    [Category("BuildServer")]
    public void SetContent_AsOpaque_SetsCanvasToOpaque()
    {
        //Arrange
        string expectedUSSClass = "loading-indicator-canvas-opaque";

        SerializedObject so = new SerializedObject(loadingIndicator);
        so.FindProperty("isBackgroundDimmed").boolValue = false;
        so.ApplyModifiedProperties();

        //Act
        loadingIndicator.SetContent(50.0f);

        //Assert
        Assert.IsTrue(loadingDoc.rootVisualElement.Q<VisualElement>("Canvas").ClassListContains(expectedUSSClass));
    }

    [Test, Order(6)]
    [Category("BuildServer")]
    public void SetContent_PopulatesContent()
    {
        //Arrange
        string expectedLabel = "Loading...";
        float expectedProgress = 50.0f;

        //Act
        loadingIndicator.SetContent(50.0f);

        //Assert
        Assert.AreEqual(expectedLabel, loadingDoc.rootVisualElement.Q<Label>("LoadingTextLabel").text);
        Assert.AreEqual(expectedProgress, loadingElement.Progress);
    }

    [Test, Order(7)]
    [Category("BuildServer")]
    public void SetContent_WithNegativeValue_ClampsToZero()
    {
        //Arrange
        float expectedProgress = 0.0f;

        //Act
        loadingIndicator.SetContent(-50.0f);

        //Assert
        Assert.AreEqual(expectedProgress, loadingElement.Progress);
    }

    [Test, Order(8)]
    [Category("BuildServer")]
    public void SetContent_Over100_ClampsTo100()
    {
        //Arrange
        float expectedprogress = 100.0f;

        //Act
        loadingIndicator.SetContent(308.0f);

        //Assert
        Assert.AreEqual(expectedprogress, loadingElement.Progress);
    }

    [Test, Order(9)]
    [Category("BuildServer")]
    public void StartLoad_StartsIncrementingProgress()
    {
        //Arrange
        float startProgress = 0.0f;
        
        //Act
        loadingIndicator.SetContent(startProgress);
        loadingIndicator.StartLoad();

        //Assert
        Assert.Greater(loadingElement.Progress, startProgress);
    }

    [Test, Order(10)]
    [Category("BuildServer")]
    public void SetComplete_SetsLoadingStateToFalse()
    {
        //Arrange
        bool expectedLoadingState = false;

        //Act
        loadingIndicator.SetComplete();

        //Assert
        Assert.AreEqual(expectedLoadingState, loadingIndicator.IsLoading);
    }

    [Test, Order(11)]
    [Category("BuildServer")]
    public void SetComplete_SetsProgressTo100()
    {
        //Arrange
        float expectedProgress = 100.0f;

        //Act
        loadingIndicator.SetComplete();

        //Assert
        Assert.AreEqual(expectedProgress, loadingElement.Progress);
    }

    [Test, Order(12)]
    [Category("BuildServer")]
    public void SetComplete_SetsLabelToComplete()
    {
        //Arrange
        string expectedLabel = "Loading Complete...";

        //Act
        loadingIndicator.SetComplete();

        //Assert
        Assert.AreEqual(expectedLabel, loadingDoc.rootVisualElement.Q<Label>("LoadingTextLabel").text);
    }

    [Test, Order(13)]
    [Category("BuildServer")]
    public void SetFailed_SetsIsLoadingToFalse()
    {
        //Arrange
        bool expectedLoadingState = false;

        //Act
        loadingIndicator.SetFailed();

        //Assert
        Assert.AreEqual(expectedLoadingState, loadingIndicator.IsLoading);
    }

    [Test, Order(14)]
    [Category("BuildServer")]
    public void SetFailed_SetsHasFailedToTrue()
    {
        //Arrange
        bool expectedFailedState = true;

        //Act
        loadingIndicator.SetFailed();

        //Assert
        Assert.AreEqual(expectedFailedState, loadingIndicator.HasFailed);
    }

    [Test, Order(15)]
    [Category("BuildServer")]
    public void SetFailed_SetsProgressToZero()
    {
        //Arrange
        float expectedProgress = 0.0f;

        //Act
        loadingIndicator.SetFailed();

        //Assert
        Assert.AreEqual(expectedProgress, loadingElement.Progress);
    }

    [Test, Order(16)]
    [Category("BuildServer")]
    public void SetFailed_EnablesFailClasses()
    {
        //Arrange
        string expectedFailedUSSClass = "loading-indicator-failed";
        string expectedIconUSSClass = "loading-indicator-failed-icon";

        //Act
        loadingIndicator.SetFailed();

        //Assert
        Assert.IsTrue(loadingDoc.rootVisualElement.Q<RadialLoadingProgress>().ClassListContains(expectedFailedUSSClass));
        Assert.IsTrue(loadingDoc.rootVisualElement.Q<VisualElement>("FailedIcon").ClassListContains(expectedIconUSSClass));
    }

    [Test, Order(17)]
    [Category("BuildServer")]
    public void SetFailed_SetsLabelToFailed()
    {
        //Arrange
        string expectedLabel = "Loading Failed...";

        //Act
        loadingIndicator.SetFailed();

        //Assert
        Assert.AreEqual(expectedLabel, loadingDoc.rootVisualElement.Q<Label>("LoadingTextLabel").text);
    }

    [Test, Order(18)]
    [Category("BuildServer")]
    public void ResetLoadingIndicator_ResetsALlValues()
    {
        //Arrange
        bool expectedLoadingState = false;
        bool expectedFailedState = false;
        float expectedProgress = 0.0f;
        string expectedLabel = "Loading...";
        string expectedFailedUSSClass = "loading-indicator-failed";
        string expectedIconUSSClass = "loading-indicator-failed-icon";

        //Act
        loadingIndicator.ResetLoadingIndicator();

        //Assert
        Assert.AreEqual(expectedLoadingState, loadingIndicator.IsLoading);
        Assert.AreEqual(expectedFailedState, loadingIndicator.HasFailed);
        Assert.AreEqual(expectedProgress, loadingElement.Progress);
        Assert.AreEqual(expectedLabel, loadingDoc.rootVisualElement.Q<Label>("LoadingTextLabel").text);
        Assert.IsFalse(loadingDoc.rootVisualElement.Q<RadialLoadingProgress>().ClassListContains(expectedFailedUSSClass));
        Assert.IsFalse(loadingDoc.rootVisualElement.Q<VisualElement>("FailedIcon").ClassListContains(expectedIconUSSClass));
    }

    [Test, Order(19)]
    [Category("BuildServer")]
    public void Show_SetsDisplayFlex()
    {
        //Arrange
        StyleEnum<DisplayStyle> expectedStyle = new StyleEnum<DisplayStyle>(DisplayStyle.Flex);

        //Act
        loadingIndicator.Show();

        //Assert
        Assert.AreEqual(expectedStyle, loadingDoc.rootVisualElement.style.display);
    }

    [Test, Order(20)]
    [Category("BuildServer")]
    public void Hide_SetsDisplayNone()
    {
        //Arrange
        StyleEnum<DisplayStyle> expectedStyle = new StyleEnum<DisplayStyle>(DisplayStyle.None);

        //Act
        loadingIndicator.Show();
        loadingIndicator.Hide();

        //Assert
        Assert.AreEqual(expectedStyle, loadingDoc.rootVisualElement.style.display);
    }
}