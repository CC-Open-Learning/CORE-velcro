using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.UIElements;
using UnityEditor;
using NUnit.Framework;
using VARLab.Velcro;

public class ToolbarStaticIntegrationTests
{
    private GameObject toolbarObj;
    private UIDocument toolbarDoc;
    private ToolbarStatic toolbarComp;
    private VisualElement toolbarRoot;
    private Sprite testSprite;

    private int sceneCounter;

    [UnitySetUp]
    [Category("BuildServer")]
    public IEnumerator SetUp()
    {
        sceneCounter = TestUtils.ClearScene(sceneCounter, "ToolbarStaticScene");

        toolbarObj = new GameObject("Toolbar Object");
        toolbarDoc = toolbarObj.AddComponent<UIDocument>();
        toolbarComp = toolbarObj.AddComponent<ToolbarStatic>();

        VisualTreeAsset toolbarUXML = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/VELCRO UI/UI/Toolbars/ToolbarStatic.uxml");
        PanelSettings panelSettings = AssetDatabase.LoadAssetAtPath<PanelSettings>("Assets/VELCRO UI/Settings/Panel Settings.asset");
        VisualTreeAsset buttonTemplate = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/VELCRO UI/UI/Toolbars/ToolbarStaticButton.uxml");

        testSprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/VELCRO UI/Sprites/Icons/Settings_Sprite.png");

        SerializedObject so = new SerializedObject(toolbarDoc);
        so.FindProperty("m_PanelSettings").objectReferenceValue = panelSettings;
        so.FindProperty("sourceAsset").objectReferenceValue = toolbarUXML;
        so.ApplyModifiedProperties();

        so = new SerializedObject(toolbarComp);
        so.FindProperty("buttonTemplate").objectReferenceValue = buttonTemplate;
        so.ApplyModifiedProperties();

        toolbarUXML.CloneTree(toolbarDoc.rootVisualElement);

        toolbarRoot = toolbarDoc.rootVisualElement;

        yield return null;
    }

    [Test, Order(1)]
    [Category("BuildServer")]
    public void Start_SetsDisplayToNone()
    {
        // Assert
        Assert.AreEqual(new StyleEnum<DisplayStyle>(DisplayStyle.None), toolbarComp.Root.style.display);
    }

    [Test, Order(2)]
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

    [Test, Order(3)]
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

    [Test, Order(4)]
    [Category("BuildServer")]
    public void SetContent_SetsEmptyText()
    {
        // Arrange
        ToolbarSO so = ScriptableObject.CreateInstance<ToolbarSO>();
        so.Buttons = new List<ToolbarButton>() { new ToolbarButton() };

        // Act
        toolbarComp.SetContent(so);
        Label buttonLabel = toolbarRoot.Q<Label>("Text");

        // Assert
        Assert.AreEqual("", buttonLabel.text);
        Assert.True(string.IsNullOrEmpty(buttonLabel.text));
    }

    [Test, Order(5)]
    [Category("BuildServer")]
    public void HandleDisplayUI_SetsDisplayToFlex()
    {
        // Arrange
        ToolbarSO so = ScriptableObject.CreateInstance<ToolbarSO>();

        // Act
        toolbarComp.HandleDisplayUI(so);

        // Assert
        Assert.AreEqual(new StyleEnum<DisplayStyle>(DisplayStyle.Flex), toolbarComp.Root.style.display);
    }

    [Test, Order(6)]
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

    [Test, Order(7)]
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

    [Test, Order(8)]
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

    [Test, Order(9)]
    [Category("BuildServer")]
    public void SetContent_AddsButtonElementToUI()
    {
        // Arrange
        ToolbarSO so = ScriptableObject.CreateInstance<ToolbarSO>();
        so.Buttons = new List<ToolbarButton>();
        so.Buttons.AddRange(Enumerable.Repeat(new ToolbarButton(), 10));

        // Act
        toolbarComp.SetContent(so);

        // Assert
        Assert.NotZero(toolbarComp.Buttons.Count());
        Assert.NotZero(toolbarRoot.Q("Toolbar").childCount);
    }

    [Test, Order(10)]
    [Category("BuildServer")]
    public void ClearButtons_RemovesAllButtonsFromToolbar()
    {
        // Arrange
        ToolbarSO so = ScriptableObject.CreateInstance<ToolbarSO>();
        so.Buttons = new List<ToolbarButton>();
        so.Buttons.AddRange(Enumerable.Repeat(new ToolbarButton(), 10));
        toolbarComp.SetContent(so);

        // Act
        toolbarComp.ClearButtons();

        // Assert
        Assert.Zero(toolbarComp.Buttons.Count());
        Assert.Zero(toolbarRoot.Q("Toolbar").childCount);
    }

    [Test, Order(11)]
    [Category("BuildServer")]
    public void ToolbarButton_Icon_UpdatesButtonIconImage()
    {
        // Arrange
        ToolbarSO so = ScriptableObject.CreateInstance<ToolbarSO>();
        so.Buttons = new List<ToolbarButton>() { new ToolbarButton(null, string.Empty) };
        toolbarComp.SetContent(so);

        // Act
        toolbarComp.Buttons.ElementAt(0).Icon = testSprite;

        // Assert
        Assert.AreEqual(testSprite, toolbarRoot.Q("Image").style.backgroundImage.value.sprite);
    }

    [Test, Order(12)]
    [Category("BuildServer")]
    public void ToolbarButton_Text_UpdatesButtonIconImage()
    {
        // Arrange
        ToolbarSO so = ScriptableObject.CreateInstance<ToolbarSO>();
        so.Buttons = new List<ToolbarButton>() { new ToolbarButton(null, string.Empty) };
        toolbarComp.SetContent(so);
        string expectedText = "Lorem Ipsum";

        // Act
        toolbarComp.Buttons.ElementAt(0).Text = "Lorem Ipsum";

        // Assert
        Assert.AreEqual(expectedText, toolbarRoot.Q<Label>("Text").text);
    }

    [Test, Order(13)]
    [Category("BuildServer")]
    public void ToolbarButton_Show_SetsButtonDisplayToFlex()
    {
        // Arrange
        ToolbarSO so = ScriptableObject.CreateInstance<ToolbarSO>();
        so.Buttons = new List<ToolbarButton>() { new ToolbarButton(null, string.Empty) };
        toolbarComp.SetContent(so);

        // Act
        toolbarComp.Buttons.ElementAt(0).Show();

        // Assert
        Assert.AreEqual(new StyleEnum<DisplayStyle>(DisplayStyle.Flex), toolbarRoot.Q("Toolbar").Children().First().style.display);
    }

    [Test, Order(14)]
    [Category("BuildServer")]
    public void ToolbarButton_Hide_SetsButtonDisplayToFlex()
    {
        // Arrange
        ToolbarSO so = ScriptableObject.CreateInstance<ToolbarSO>();
        so.Buttons = new List<ToolbarButton>() { new ToolbarButton(null, string.Empty) };
        toolbarComp.SetContent(so);

        // Act
        toolbarComp.Buttons.ElementAt(0).Hide();

        // Assert
        Assert.AreEqual(new StyleEnum<DisplayStyle>(DisplayStyle.None), toolbarRoot.Q("Toolbar").Children().First().style.display);
    }

    [Test, Order(14)]
    [Category("BuildServer")]
    public void SetContent_AddsGrowClassToNewButtons()
    {
        // Arrange
        ToolbarSO so = ScriptableObject.CreateInstance<ToolbarSO>();
        so.Buttons = new List<ToolbarButton>() { new ToolbarButton(null, string.Empty) };

        // Act
        toolbarComp.SetContent(so);

        // Assert
        Assert.IsTrue(toolbarRoot.Q("Toolbar").Children().First().ClassListContains("grow"));
    }

    [Test, Order(15)]
    [Category("BuildServer")]
    public void RemoveButton_WithValidIndex_RemovesButtonFromToolbar()
    {
        // Arrange
        ToolbarSO so = ScriptableObject.CreateInstance<ToolbarSO>();
        so.Buttons = new List<ToolbarButton>() { new ToolbarButton(null, string.Empty) };
        toolbarComp.SetContent(so);

        // Act
        toolbarComp.RemoveButton(0);

        // Assert
        Assert.Zero(toolbarRoot.Q("Toolbar").childCount);
        Assert.Zero(toolbarComp.ButtonCount);
    }

    [Test, Order(16)]
    [Category("BuildServer")]
    public void RemoveButton_WithInvalidIndex_LogsError()
    {
        // Arrange
        string expectedMessage = "Unable to remove toolbar button at invalid index 308";

        // Act
        toolbarComp.RemoveButton(308);

        // Assert
        LogAssert.Expect(LogType.Error, expectedMessage);
    }

    [Test, Order(17)]
    [Category("BuildServer")]
    public void AddButton_AddsNewButtonToToolbar()
    {
        // Act
        toolbarComp.AddButton(testSprite, "Button");

        // Assert
        Assert.NotZero(toolbarRoot.Q("Toolbar").childCount);
        Assert.NotZero(toolbarComp.ButtonCount);
    }

    private void Ping()
    {
        Debug.Log("Toolbar event triggered!");
    }
}
