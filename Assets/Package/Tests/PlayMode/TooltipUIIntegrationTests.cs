using System.Linq;
using System.Collections;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.UIElements;
using UnityEditor;
using NUnit.Framework;
using VARLab.Velcro;

public class TooltipUIIntegrationTests
{
    private GameObject tooltipObj;
    private UIDocument tooltipDoc;
    private TooltipUI tooltipUI;
    private VisualElement tooltipRoot;
    private VisualElement tooltipParent;

    private int sceneCounter;

    [UnitySetUp]
    [Category("BuildServer")]
    public IEnumerator SetUp()
    {
        sceneCounter = TestUtils.ClearScene(sceneCounter, "TooltipUIScene");

        tooltipObj = new GameObject("Tooltip Object");
        tooltipDoc = tooltipObj.AddComponent<UIDocument>();
        tooltipUI = tooltipObj.AddComponent<TooltipUI>();

        VisualTreeAsset tooltipUXML = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/VELCRO UI/UI/Tooltips/Tooltip.uxml");
        PanelSettings panelSettings = AssetDatabase.LoadAssetAtPath<PanelSettings>("Assets/VELCRO UI/Settings/Panel Settings.asset");

        //Reference panel settings and source asset as SerializedFields
        SerializedObject so = new SerializedObject(tooltipDoc);
        so.FindProperty("m_PanelSettings").objectReferenceValue = panelSettings;
        so.FindProperty("sourceAsset").objectReferenceValue = tooltipUXML;
        so.ApplyModifiedProperties();

        tooltipUXML.CloneTree(tooltipDoc.rootVisualElement);

        //Tooltip root is meant to represent the "Container" element in the tooltip template
        //It is not to be mistaken with the actual root of the uxml document
        tooltipRoot = tooltipDoc.rootVisualElement.Children().First();

        //We can just use a blank parent for testing
        tooltipParent = new VisualElement();

        yield return null;
    }

    [Test, Order(1)]
    [Category("BuildServer")]
    public void Start_SetsRootToDisplayNone()
    {
        //Assert
        Assert.AreEqual(new StyleEnum<DisplayStyle>(DisplayStyle.None), tooltipRoot.style.display);
    }

    [Test, Order(2)]
    [Category("BuildServer")]
    public void Start_OpacityShouldBeZero()
    {
        //Arrange
        float expectedOpacity = 0f;

        //Assert
        Assert.AreEqual(expectedOpacity, tooltipRoot.resolvedStyle.opacity);
    }

    [Test, Order(3)]
    [Category("BuildServer")]
    public void SetContent_WithTopClass_ShouldBeEnabled()
    {
        //Arrange
        string expectedClass = "tooltip-top";

        //Act
        tooltipUI.SetContent(TooltipType.Top, "Hello");

        //Assert
        Assert.IsTrue(tooltipRoot.ClassListContains(expectedClass));
    }

    [Test, Order(4)]
    [Category("BuildServer")]
    public void SetContent_WithBottomClass_ShouldBeEnabled()
    {
        //Arrange
        string expectedClass = "tooltip-bottom";

        //Act
        tooltipUI.SetContent(TooltipType.Bottom, "Hello");

        //Assert
        Assert.IsTrue(tooltipRoot.ClassListContains(expectedClass));
    }

    [Test, Order(5)]
    [Category("Build Server")]
    public void SetContent_WithLeftClass_ShouldBeEnabled()
    {
        //Arrange
        string expectedClass = "tooltip-left";

        //Act
        tooltipUI.SetContent(TooltipType.Left, "Hello");

        //Assert
        Assert.IsTrue(tooltipRoot.ClassListContains(expectedClass));
    }

    [Test, Order(6)]
    [Category("BuildServer")]
    public void SetContent_WithRightClass_ShouldBeEnabled()
    {
        //Arrange
        string expectedClass = "tooltip-right";

        //Act
        tooltipUI.SetContent(TooltipType.Right, "Hello");

        //Assert
        Assert.IsTrue(tooltipRoot.ClassListContains(expectedClass));
    }

    [Test, Order(7)]
    [Category("BuildServer")]
    public void ClearClasses_WithTopClass_ShouldBeDisabled()
    {
        //Arrange
        string disabledClass = "tooltip-top";

        //Act
        tooltipUI.HandleDisplayUI(tooltipParent, TooltipType.Top, "Hello");
        tooltipUI.ClearClasses();

        //Assert
        Assert.IsFalse(tooltipRoot.ClassListContains(disabledClass));
    }

    [Test, Order(8)]
    [Category("BuildServer")]
    public void ClearClasses_WithBottomClass_ShouldBeDisabled()
    {
        //Arrange
        string disabledClass = "tooltip-bottom";

        //Act
        tooltipUI.HandleDisplayUI(tooltipParent, TooltipType.Bottom, "Hello");
        tooltipUI.ClearClasses();

        //Assert
        Assert.IsFalse(tooltipRoot.ClassListContains(disabledClass));
    }

    [Test, Order(9)]
    [Category("BuildServer")]
    public void ClearClasses_WithLeftClass_ShouldBeDisabled()
    {
        //Arrange
        string disabledClass = "tooltip-left";

        //Act
        tooltipUI.HandleDisplayUI(tooltipParent, TooltipType.Left, "Hello");
        tooltipUI.ClearClasses();

        //Assert
        Assert.IsFalse(tooltipRoot.ClassListContains(disabledClass));
    }

    [Test, Order(10)]
    [Category("BuildServer")]
    public void ClearClasses_WithRightClass_ShouldBeDisabled()
    {
        //Arrange
        string disabledClass = "tooltip-right";

        //Act
        tooltipUI.HandleDisplayUI(tooltipParent, TooltipType.Right, "Hello");
        tooltipUI.ClearClasses();

        //Assert
        Assert.IsFalse(tooltipRoot.ClassListContains(disabledClass));
    }

    [Test, Order(11)]
    [Category("BuildServer")]
    public void SetPosition_ShouldMoveTooltip()
    {
        //Arrange
        tooltipRoot.transform.position = Vector2.zero;
        Vector2 oldPositon = tooltipRoot.transform.position;
        tooltipParent.transform.position = new Vector2(40, 40);

        //Act
        tooltipUI.SetPosition(tooltipParent, TooltipType.Top);

        //Assert
        Assert.AreNotEqual(oldPositon, tooltipRoot.transform.position);
    }

    [Test, Order(12)]
    [Category("BuildServer")]
    public void ForceSetPosition_ChangesTooltipPosition()
    {
        //Arrange
        Vector3 expectedPosition = new Vector3(99, 67, 0);

        //Act
        tooltipUI.ForceSetPosition(new Vector2(99, 67));

        //Assert
        Assert.AreEqual(expectedPosition, tooltipRoot.transform.position);
    }

    [Test, Order(13)]
    [Category("BuildServer")]
    public void FadeIn_ShouldSetOpacityToOne()
    {
        //Arrange
        StyleFloat expectedOpacity = new StyleFloat(1.0f);

        //Act
        tooltipUI.StartCoroutine(tooltipUI.FadeIn());

        //Assert
        Assert.AreEqual(expectedOpacity, tooltipRoot.style.opacity);
    }

    [Test, Order(14)]
    [Category("BuildServer")]
    public void FadeOut_ShouldSetOpacityToZero()
    {
        //Arrange
        StyleFloat expectedOpacity = new StyleFloat(0.0f);

        //Act
        tooltipUI.StartCoroutine(tooltipUI.FadeOut());

        //Assert
        Assert.AreEqual(expectedOpacity, tooltipRoot.style.opacity);
    }

    [Test, Order(15)]
    [Category("BuildServer")]
    public void Show_TriggersEvent_OnTooltipShown()
    {
        //Arrange
        tooltipUI.OnTooltipShown.AddListener(Ping);

        //Act
        tooltipUI.Show();

        //Assert
        LogAssert.Expect(LogType.Log, "Tooltip event triggered");

        //Clean Up
        tooltipUI.OnTooltipShown.RemoveListener(Ping);
    }

    [Test, Order(16)]
    [Category("BuildServer")]
    public void Hide_TriggersEvent_OnTooltipHidden()
    {
        //Arrange
        tooltipUI.OnTooltipHidden.AddListener(Ping);

        //Act
        tooltipUI.Hide();

        //Assert
        LogAssert.Expect(LogType.Log, "Tooltip event triggered");

        // Clean Up
        tooltipUI.OnTooltipHidden.RemoveListener(Ping);
    }

    [Test, Order(17)]
    [Category("BuildServer")]
    public void HandleDisplayUI_WithValidElements_SetsRootToDisplayFlex()
    {
        //Arrange
        StyleEnum<DisplayStyle> expectedStyle = new StyleEnum<DisplayStyle>(DisplayStyle.Flex);

        //Act
        tooltipUI.HandleDisplayUI(tooltipParent, TooltipType.Top, "Test Tooltip", FontSize.Medium);

        //Assert
        Assert.AreEqual(expectedStyle, tooltipRoot.style.display);
    }

    [Test, Order(18)]
    [Category("BuildServer")]
    public void HandleDisplayUI_WithAllProperties_AppliesChanges()
    {
        //Arrange
        string expectedClass = "tooltip-top";
        string expectedText = "Tooltip Text";
        StyleLength expectedFontSize = new StyleLength(24.0f);

        //Act
        tooltipUI.HandleDisplayUI(tooltipParent, TooltipType.Top, "Tooltip Text", FontSize.Medium);

        //Assert
        Assert.IsTrue(tooltipRoot.ClassListContains(expectedClass));
        Assert.AreEqual(expectedText, tooltipRoot.Q<Label>("Label").text);
        Assert.AreEqual(expectedFontSize, tooltipRoot.Q<Label>("Label").style.fontSize);
    }

    [Test, Order(19)]
    [Category("BuildServer")]
    public void HandleDisplayUI_WithoutFontSize_AppliesChanges()
    {
        //Arrange
        string expectedClass = "tooltip-top";
        string expectedText = "Tooltip Text";
        StyleLength expectedFontSize = new StyleLength(24.0f);

        //Act
        tooltipUI.HandleDisplayUI(tooltipParent, TooltipType.Top, "Tooltip Text");

        //Assert
        Assert.IsTrue(tooltipRoot.ClassListContains(expectedClass));
        Assert.AreEqual(expectedText, tooltipRoot.Q<Label>("Label").text);
        Assert.AreEqual(expectedFontSize, tooltipRoot.Q<Label>("Label").style.fontSize);
    }

    [Test, Order(20)]
    [Category("BuildServer")]
    public void HandleDisplayUI_WithoutParent_AppliesChanges()
    {
        //Arrange
        string expectedClass = "tooltip-top";
        string expectedText = "Tooltip Text";
        StyleLength expectedFontSize = new StyleLength(24.0f);

        //Act
        tooltipUI.HandleDisplayUI(TooltipType.Top, "Tooltip Text");

        //Assert
        Assert.IsTrue(tooltipRoot.ClassListContains(expectedClass));
        Assert.AreEqual(expectedText, tooltipRoot.Q<Label>("Label").text);
        Assert.AreEqual(expectedFontSize, tooltipRoot.Q<Label>("Label").style.fontSize);
    }

    [Test, Order(21)]
    public void ApplyOffsetAndOrigin_WithBottomType_SetsCorrectPosition()
    {
        //Arrange
        Vector2 expectedPosition = new Vector2(0, -30f);
        Vector2 position = Vector2.zero;
        tooltipUI.ForceSetPosition(position);

        //Act
        SerializedObject so = new SerializedObject(tooltipUI);
        so.FindProperty("positionOffset").floatValue = 30f;
        so.ApplyModifiedProperties();

        tooltipUI.ApplyOffsetAndOrigin(TooltipType.Bottom, ref position);

        //Assert
        Assert.AreEqual(expectedPosition, position);
    }

    [Test, Order(22)]
    public void ApplyOffsetAndOrigin_WithTopType_SetsCorrectPosition()
    {
        //Arrange
        Vector2 expectedPosition = new Vector2(0, 30f);
        Vector2 position = Vector2.zero;

        //Act
        SerializedObject so = new SerializedObject(tooltipUI);
        so.FindProperty("positionOffset").floatValue = 30f;
        so.ApplyModifiedProperties();

        tooltipUI.ApplyOffsetAndOrigin(TooltipType.Top, ref position);

        //Assert
        Assert.AreEqual(expectedPosition, position);
    }

    [Test, Order(23)]
    public void ApplyOffsetAndOrigin_WithLeftType_SetsCorrectPosition()
    {
        //Arrange
        Vector2 expectedPosition = new Vector2(30f, 0);
        Vector2 position = Vector2.zero;

        //Act
        SerializedObject so = new SerializedObject(tooltipUI);
        so.FindProperty("positionOffset").floatValue = 30f;
        so.ApplyModifiedProperties();

        tooltipUI.ApplyOffsetAndOrigin(TooltipType.Left, ref position);

        //Assert
        Assert.AreEqual(expectedPosition, position);
    }

    [Test, Order(24)]
    public void ApplyOffsetAndOrigin_WithRightType_SetsCorrectPosition()
    {
        //Arrange
        Vector2 expectedPosition = new Vector2(-30f, 0);
        Vector2 position = Vector2.zero;

        //Act
        SerializedObject so = new SerializedObject(tooltipUI);
        so.FindProperty("positionOffset").floatValue = 30f;
        so.ApplyModifiedProperties();

        tooltipUI.ApplyOffsetAndOrigin(TooltipType.Right, ref position);

        //Assert
        Assert.AreEqual(expectedPosition, position);
    }

    private void Ping()
    {
        Debug.Log("Tooltip event triggered");
    }
}