using System.Linq;
using System.Collections;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.UIElements;
using UnityEditor;
using NUnit.Framework;
using VARLab.Velcro;
using System.Collections.Generic;
using NUnit.Framework.Internal;

public class ToolbarCollapseIntegrationTests
{
    private GameObject toolbarObj;
    private UIDocument toolbarDoc;
    private ToolbarCollapse toolbarComp;
    private VisualElement toolbarRoot;
    private Sprite testSprite;

    private int sceneCounter;

    [UnitySetUp]
    [Category("BuildServer")]
    public IEnumerator SetUp()
    {
        sceneCounter = TestUtils.ClearScene(sceneCounter, "ToolbarCollapseScene");

        toolbarObj = new GameObject("Toolbar Object");
        toolbarDoc = toolbarObj.AddComponent<UIDocument>();
        toolbarComp = toolbarObj.AddComponent<ToolbarCollapse>();

        VisualTreeAsset toolbarUXML = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/VELCRO UI/UI/Toolbars/ToolbarCollapse.uxml");
        PanelSettings panelSettings = AssetDatabase.LoadAssetAtPath<PanelSettings>("Assets/VELCRO UI/Settings/Panel Settings.asset");
        testSprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/VELCRO UI/Sprites/Icons/Settings_Sprite.png");
        VisualTreeAsset buttonTemplate = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/VELCRO UI/UI/Toolbars/ToolbarCollapseButton.uxml");
        VisualTreeAsset simpleButtonTemplate = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/VELCRO UI/UI/Toolbars/ToolbarCollapseButtonSimple.uxml");

        SerializedObject so = new SerializedObject(toolbarDoc);
        so.FindProperty("m_PanelSettings").objectReferenceValue = panelSettings;
        so.FindProperty("sourceAsset").objectReferenceValue = toolbarUXML;
        so.ApplyModifiedProperties();

        so = new SerializedObject(toolbarComp);
        so.FindProperty("buttonTemplate").objectReferenceValue = buttonTemplate;
        so.FindProperty("simpleButtonTemplate").objectReferenceValue = simpleButtonTemplate;
        so.ApplyModifiedProperties();

        toolbarUXML.CloneTree(toolbarDoc.rootVisualElement);

        toolbarRoot = toolbarDoc.rootVisualElement.Children().First();

        yield return null;
    }

    [Test, Order(1)]
    [Category("BuildServer")]
    public void Start_ClassListContainsOpenClass()
    {
        // Arrange
        string closedClass = "toolbar-collapse-closed";
        string openClass = "toolbar-collapse";
        VisualElement toolbarElement = toolbarRoot.Q<VisualElement>("Toolbar");

        // Assert
        Assert.IsTrue(toolbarElement.ClassListContains(openClass));
        Assert.IsFalse(toolbarElement.ClassListContains(closedClass));
    }

    [Test, Order(2)]
    [Category("BuildServer")]
    public void Start_ToolbarNotHidden()
    {
        // Arrange
        bool expected = true;

        // Assert
        Assert.AreEqual(expected, toolbarRoot.visible);
    }

    [Test, Order(3)]
    [Category("BuildServer")]
    public void SetContent_AppliesSpriteToButtonImage()
    {
        // Arrange
        ToolbarSO so = ScriptableObject.CreateInstance<ToolbarSO>();
        so.Buttons = new List<ToolbarButton>() { new ToolbarButton(testSprite, string.Empty) };
        Sprite exptectedSprite = testSprite;

        // Act
        toolbarComp.SetContent(so);
        VisualElement buttonImage = toolbarRoot.Q("Image");

        // Assert
        Assert.AreEqual(exptectedSprite, buttonImage.style.backgroundImage.value.sprite);
    }

    [Test, Order(4)]
    [Category("BuildServer")]
    public void SetContent_AppliesTextToButtonLabel()
    {
        // Arrange
        ToolbarSO so = ScriptableObject.CreateInstance<ToolbarSO>();
        so.Buttons = new List<ToolbarButton>() { new ToolbarButton(null, "Lorem Ipsum") };
        string expectedText = "Lorem Ipsum";

        // Act
        toolbarComp.SetContent(so);
        Label buttonLabel = toolbarRoot.Q<Label>("Text");

        // Assert
        Assert.AreEqual(expectedText, buttonLabel.text);
    }

    [Test, Order(5)]
    [Category("BuildServer")]
    public void SetContent_WithComplexButton_AddsButtonToComplexContainer()
    {
        // Arrange
        ToolbarSO so = ScriptableObject.CreateInstance<ToolbarSO>();
        so.Buttons = new List<ToolbarButton>() { new ToolbarButton(null, "Lorem Ipsum") };

        // Act
        toolbarComp.SetContent(so);

        // Assert
        Assert.NotZero(toolbarRoot.Q("ComplexButtons").childCount);
    }

    [Test, Order(6)]
    [Category("BuildServer")]
    public void SetContent_WithSimpleButton_AddsButtonToSimpleContainer()
    {
        // Arrange
        ToolbarSO so = ScriptableObject.CreateInstance<ToolbarSO>();
        so.Buttons = new List<ToolbarButton>() { new ToolbarButton(null, string.Empty) };

        // Act
        toolbarComp.SetContent(so);

        // Assert
        Assert.NotZero(toolbarRoot.Q("SimpleButtons").childCount);
    }

    [Test, Order(7)]
    [Category("BuildServer")]
    public void SetContent_WithSimpleButton_ShowsSeperator()
    {
        // Arrange
        ToolbarSO so = ScriptableObject.CreateInstance<ToolbarSO>();
        so.Buttons = new List<ToolbarButton>() { new ToolbarButton(null, string.Empty) };

        // Act
        toolbarComp.SetContent(so);

        // Assert
        Assert.AreEqual(new StyleEnum<DisplayStyle>(DisplayStyle.Flex), toolbarRoot.Q("Seperator").style.display);
    }

    [Test, Order(8)]
    [Category("BuildServer")]
    public void ClearButtons_RemovesAllSimpleButtonsFromToolbar()
    {
        // Arrange
        ToolbarSO so = ScriptableObject.CreateInstance<ToolbarSO>();
        so.Buttons = new List<ToolbarButton>();
        so.Buttons.AddRange(Enumerable.Repeat(new ToolbarButton(), 10));
        toolbarComp.SetContent(so);

        // Act
        toolbarComp.ClearButtons();

        // Assert
        Assert.Zero(toolbarRoot.Q("SimpleButtons").childCount);
    }

    [Test, Order(9)]
    [Category("BuildServer")]
    public void ClearButtons_HidesSeperator()
    {
        // Arrange
        ToolbarSO so = ScriptableObject.CreateInstance<ToolbarSO>();
        so.Buttons = new List<ToolbarButton>();
        so.Buttons.AddRange(Enumerable.Repeat(new ToolbarButton(), 10));
        toolbarComp.SetContent(so);

        // Act
        toolbarComp.ClearButtons();

        // Assert
        Assert.AreEqual(new StyleEnum<DisplayStyle>(DisplayStyle.None), toolbarRoot.Q("Seperator").style.display);
    }

    [Test, Order(10)]
    [Category("BuildServer")]
    public void Hide_SetsDisplayToNone()
    {
        // Arrange
        toolbarComp.Show();

        // Act
        toolbarComp.Hide();

        // Assert
        Assert.AreEqual(new StyleEnum<DisplayStyle>(DisplayStyle.None), toolbarComp.Root.style.display);
    }

    [Test, Order(11)]
    [Category("BuildServer")]
    public void Show_InvokesOnShowEvent()
    {
        // Arrange
        toolbarComp.OnToolbarShown.AddListener(Ping);
        string expectedMessage = "Toolbar event triggered!";

        // Act
        toolbarComp.Show();

        // Assert
        LogAssert.Expect(LogType.Log, expectedMessage);

        // Cleanup
        toolbarComp.OnToolbarShown.RemoveListener(Ping);
    }

    [Test, Order(12)]
    [Category("BuildServer")]
    public void Hide_InvokesOnHideEvent()
    {
        // Arrange
        toolbarComp.OnToolbarHidden.AddListener(Ping);
        string expectedMessage = "Toolbar event triggered!";

        // Act
        toolbarComp.Hide();

        // Assert
        LogAssert.Expect(LogType.Log, expectedMessage);

        // Cleanup
        toolbarComp.OnToolbarHidden.RemoveListener(Ping);
    }

    [Test, Order(13)]
    [Category("BuildServer")]
    public void Expand_EnablesOpenClassAndDisablesClosedClass()
    {
        // Arrange
        string closedClass = "toolbar-collapse-closed";
        string openClass = "toolbar-collapse";

        VisualElement toolbarElement = toolbarRoot.Q<VisualElement>("Toolbar");
        toolbarElement.EnableInClassList(closedClass, true);
        toolbarElement.EnableInClassList(openClass, false);

        // Act
        toolbarComp.Expand();

        // Assert
        Assert.IsTrue(toolbarElement.ClassListContains(openClass));
        Assert.IsFalse(toolbarElement.ClassListContains(closedClass));
    }

    [UnityTest, Order(14)]
    [Category("BuildServer")]
    public IEnumerator Collapse_EnablesClosedClassAndDisablesOpenClass()
    {
        // Arrange
        string closedClass = "toolbar-collapse-closed";
        string openClass = "toolbar-collapse";
        float animationSpeed = 0.15f;

        VisualElement toolbarElement = toolbarRoot.Q<VisualElement>("Toolbar");
        toolbarElement.EnableInClassList(closedClass, false);
        toolbarElement.EnableInClassList(openClass, true);

        // Act
        toolbarComp.Collapse();

        // Wait
        yield return new WaitForSeconds(animationSpeed);

        // Assert
        Assert.IsTrue(toolbarElement.ClassListContains(closedClass));
        Assert.IsFalse(toolbarElement.ClassListContains(openClass));
    }

    [UnityTest, Order(15)]
    [Category("BuildServer")]
    public IEnumerator Expand_InvokesOnExpandEvent()
    {
        // Arrange
        toolbarComp.OnToolbarExpand.AddListener(Ping);
        string expectedMessage = "Toolbar event triggered!";
        float animationSpeed = 0.15f;

        // Act
        toolbarComp.Expand();

        // Wait
        yield return new WaitForSeconds(animationSpeed);

        // Assert
        LogAssert.Expect(LogType.Log, expectedMessage);

        // Cleanup
        toolbarComp.OnToolbarExpand.RemoveListener(Ping);
    }

    [UnityTest, Order(16)]
    [Category("BuildServer")]
    public IEnumerator Collapse_InvokesOnCollapseEvent()
    {
        // Arrange
        toolbarComp.OnToolbarCollapse.AddListener(Ping);
        string expectedMessage = "Toolbar event triggered!";
        float animationSpeed = 0.15f;

        // Act
        toolbarComp.Collapse();

        // Wait
        yield return new WaitForSeconds(animationSpeed);

        // Assert
        LogAssert.Expect(LogType.Log, expectedMessage);

        // Cleanup
        toolbarComp.OnToolbarCollapse.RemoveListener(Ping);
    }

    [Test, Order(17)]
    [Category("BuildServer")]
    public void AddButton_WithText_AddsNewComplexButtonToToolbar()
    {
        // Act
        toolbarComp.AddButton(testSprite, "Button");

        // Assert
        Assert.NotZero(toolbarRoot.Q("ComplexButtons").childCount);
        Assert.NotZero(toolbarComp.ButtonCount);
    }

    [Test, Order(18)]
    [Category("BuildServer")]
    public void AddButton_WithBlankText_AddsNewSimpleButtonToToolbar()
    {
        // Act
        toolbarComp.AddButton(testSprite, string.Empty);

        // Assert
        Assert.NotZero(toolbarRoot.Q("Toolbar").childCount);
        Assert.NotZero(toolbarComp.ButtonCount);
    }

    [Test, Order(19)]
    [Category("BuildServer")]
    public void AddButton_WithNullText_AddsNewSimpleButtonToToolbar()
    {
        // Act
        toolbarComp.AddButton(testSprite, null);

        // Assert
        Assert.NotZero(toolbarRoot.Q("Toolbar").childCount);
        Assert.NotZero(toolbarComp.ButtonCount);
    }

    private void Ping()
    {
        Debug.Log("Toolbar event triggered!");
    }
}
