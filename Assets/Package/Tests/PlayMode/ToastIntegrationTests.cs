using System.Collections;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.UIElements;
using UnityEditor;
using NUnit.Framework;
using VARLab.Velcro;

public class ToastIntegrationTests
{
    private int sceneCounter = 0;

    private GameObject toastSimpleObj;
    private GameObject toastComplexObj;
    private UIDocument toastSimpleDoc;
    private UIDocument toastComplexDoc;
    private ToastSimple toastSimple;
    private ToastComplex toastComplex;
    private ToastSimpleSO toastSimpleSO;
    private ToastComplexSO toastComplexSO;

    [UnitySetUp]
    [Category("BuildServer")]
    public IEnumerator SetUp()
    {
        sceneCounter = TestUtils.ClearScene(sceneCounter, "ToastScene");

        //Set up <Toast> UI
        toastSimpleObj = new GameObject("Simple Toast");
        toastComplexObj = new GameObject("Complex Toast");
        toastSimpleDoc = toastSimpleObj.AddComponent<UIDocument>();
        toastComplexDoc = toastComplexObj.AddComponent<UIDocument>();
        toastSimple = toastSimpleObj.AddComponent<ToastSimple>();
        toastComplex = toastComplexObj.AddComponent<ToastComplex>();

        //Load required assets from project files
        VisualTreeAsset toastSimpleUXML = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/VELCRO UI/UI/Toasts/ToastSimple.uxml");
        VisualTreeAsset toastComplexUXML = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/VELCRO UI/UI/Toasts/ToastComplex.uxml");
        PanelSettings panelSettings = AssetDatabase.LoadAssetAtPath<PanelSettings>("Assets/VELCRO UI/Settings/Panel Settings.asset");

        //Reference panel settings and source asset as SerializedFields
        SerializedObject so = new SerializedObject(toastSimpleDoc);
        so.FindProperty("m_PanelSettings").objectReferenceValue = panelSettings;
        so.FindProperty("sourceAsset").objectReferenceValue = toastSimpleUXML;
        so.ApplyModifiedProperties();

        //Reference panel settings and source asset as SerializedFields
        SerializedObject so2 = new SerializedObject(toastComplexDoc);
        so2.FindProperty("m_PanelSettings").objectReferenceValue = panelSettings;
        so2.FindProperty("sourceAsset").objectReferenceValue = toastComplexUXML;
        so2.ApplyModifiedProperties();

        toastSimpleSO = ScriptableObject.CreateInstance<ToastSimpleSO>();
        toastSimpleSO.ToastType = ToastType.Hint;
        toastSimpleSO.Header = "Header";
        toastSimpleSO.Message = "Message";
        toastSimpleSO.Alignment = Align.FlexStart;

        toastComplexSO = ScriptableObject.CreateInstance<ToastComplexSO>();
        toastSimpleSO.ToastType = ToastType.Location;
        toastComplexSO.Header = "Header";
        toastComplexSO.Message = "Message";
        toastComplexSO.SecondaryMessage = "SecondaryMessage";
        toastComplexSO.Alignment = Align.FlexStart;

        toastSimpleUXML.CloneTree(toastSimpleDoc.rootVisualElement);
        toastComplexUXML.CloneTree(toastComplexDoc.rootVisualElement);
        yield return null;
    }

    [Test, Order(1)]
    [Category("BuildServer")]
    public void Start_SetsRootToDisplayNone()
    {
        //Arrange
        StyleEnum<DisplayStyle> expectedStyle = new StyleEnum<DisplayStyle>(DisplayStyle.None);

        //Assert
        Assert.AreEqual(expectedStyle, toastComplexDoc.rootVisualElement.style.display);
        Assert.AreEqual(expectedStyle, toastSimpleDoc.rootVisualElement.style.display);
    }

    [Test, Order(2)]
    [Category("BuildServer")]
    public void HandleDisplayUI_WithSimple_SetsRootToDisplayFlex()
    {
        //Arrange
        StyleEnum<DisplayStyle> expectedStyle = new StyleEnum<DisplayStyle>(DisplayStyle.Flex);

        //Act
        toastSimple.HandleDisplayUI(toastSimpleSO.Header, toastSimpleSO.Message);

        //Assert
        Assert.AreEqual(expectedStyle, toastSimpleDoc.rootVisualElement.style.display);
    }

    [Test, Order(3)]
    [Category("BuildServer")]
    public void HandleDisplayUI_WithComplex_SetsRootToDisplayFlex()
    {
        //Arrange
        StyleEnum<DisplayStyle> expectedStyle = new StyleEnum<DisplayStyle>(DisplayStyle.Flex);

        //Act
        toastComplex.HandleDisplayUI(toastComplexSO.Header, toastComplexSO.Message, toastComplexSO.SecondaryMessage);

        //Assert
        Assert.AreEqual(expectedStyle, toastComplexDoc.rootVisualElement.style.display);
    }

    [Test, Order(4)]
    [Category("BuildServer")]
    public void HandleDisplayUI_WithSimpleSO_PopulatesContent()
    {
        //Arrange
        string expectedHeader = "Header";
        string expectedMessage = "Message";

        //Act
        toastSimple.HandleDisplayUI(toastSimpleSO);

        //Assert
        Assert.AreEqual(expectedHeader, toastSimpleDoc.rootVisualElement.Q<Label>("HeaderLabel").text);
        Assert.AreEqual(expectedMessage, toastSimpleDoc.rootVisualElement.Q<Label>("MessageLabel").text);
    }

    [Test, Order(5)]
    [Category("BuildServer")]
    public void HandleDisplayUI_WithComplexSO_PopulatesContent()
    {
        //Arrange
        string expectedHeader = "Header";
        string expectedMessage = "Message";
        string expectedSecondaryMessage = "SecondaryMessage";

        //Act
        toastComplex.HandleDisplayUI(toastComplexSO);

        //Assert
        Assert.AreEqual(expectedHeader, toastComplexDoc.rootVisualElement.Q<Label>("HeaderLabel").text);
        Assert.AreEqual(expectedMessage, toastComplexDoc.rootVisualElement.Q<Label>("MessageLabel").text);
        Assert.AreEqual(expectedSecondaryMessage, toastComplexDoc.rootVisualElement.Q<Label>("SecondaryMessageLabel").text);
    }

    [Test, Order(6)]
    [Category("BuildServer")]
    public void SetContent_WithSimple_PopulatesContent()
    {
        //Arrange
        string expectedHeader = "Header";
        string expectedMessage = "Message";

        //Act
        toastSimple.SetContent(toastSimpleSO.ToastType, toastSimpleSO.Header, toastSimpleSO.Message);

        //Assert
        Assert.AreEqual(expectedHeader, toastSimpleDoc.rootVisualElement.Q<Label>("HeaderLabel").text);
        Assert.AreEqual(expectedMessage, toastSimpleDoc.rootVisualElement.Q<Label>("MessageLabel").text);
    }

    [Test, Order(7)]
    [Category("BuildServer")]
    public void SetContent_WithComplex_PopulatesContent()
    {
        //Arrange
        string expectedHeader = "Header";
        string expectedMessage = "Message";
        string expectedSecondaryMessage = "SecondaryMessage";

        //Act
        toastComplex.SetContent(toastComplexSO.ToastType, toastComplexSO.Header, toastComplexSO.Message, toastComplexSO.SecondaryMessage);

        //Assert
        Assert.AreEqual(expectedHeader, toastComplexDoc.rootVisualElement.Q<Label>("HeaderLabel").text);
        Assert.AreEqual(expectedMessage, toastComplexDoc.rootVisualElement.Q<Label>("MessageLabel").text);
        Assert.AreEqual(expectedSecondaryMessage, toastComplexDoc.rootVisualElement.Q<Label>("SecondaryMessageLabel").text);
    }

    [Test, Order(8)]
    [Category("BuildServer")]
    public void SetCustomToast_WithSimple_ChangesStyle()
    {
        //Arrange
        StyleColor expectedBackgroundColor = new StyleColor(Color.black);
        StyleColor expectedHeaderColor = new StyleColor(Color.white);
        StyleColor expectedMessageColor = new StyleColor(Color.white);
        StyleEnum<DisplayStyle> expectedStyle = new StyleEnum<DisplayStyle>(DisplayStyle.None);

        //Act
        toastSimple.SetCustomToast(Color.black, Color.white, Color.white, null);
        toastSimple.HandleDisplayUI("Header", "Message", ToastType.Custom);

        //Assert
        Assert.AreEqual(expectedBackgroundColor, toastSimpleDoc.rootVisualElement.Q<VisualElement>("Toast").style.backgroundColor);
        Assert.AreEqual(expectedHeaderColor, toastSimpleDoc.rootVisualElement.Q<Label>("HeaderLabel").style.color);
        Assert.AreEqual(expectedMessageColor, toastSimpleDoc.rootVisualElement.Q<Label>("MessageLabel").style.color);
        Assert.AreEqual(expectedStyle, toastSimpleDoc.rootVisualElement.Q<VisualElement>("IconContainer").style.display);
    }

    [Test, Order(9)]
    [Category("BuildServer")]
    public void SetCustomToast_WithComplex_ChangesStyle()
    {
        //Arrange
        StyleColor expectedBackgroundColor = new StyleColor(Color.black);
        StyleColor expectedHeaderColor = new StyleColor(Color.white);
        StyleColor expectedMessageColor = new StyleColor(Color.white);
        StyleEnum<DisplayStyle> expectedStyle = new StyleEnum<DisplayStyle>(DisplayStyle.None);

        //Act
        toastComplex.SetCustomToast(Color.black, Color.white, Color.white, null);
        toastComplex.HandleDisplayUI("Header", "Message", "SecondaryMessage", ToastType.Custom);

        //Assert
        Assert.AreEqual(expectedBackgroundColor, toastComplexDoc.rootVisualElement.Q<VisualElement>("Toast").style.backgroundColor);
        Assert.AreEqual(expectedHeaderColor, toastComplexDoc.rootVisualElement.Q<Label>("HeaderLabel").style.color);
        Assert.AreEqual(expectedMessageColor, toastComplexDoc.rootVisualElement.Q<Label>("MessageLabel").style.color);
        Assert.AreEqual(expectedMessageColor, toastComplexDoc.rootVisualElement.Q<Label>("SecondaryMessageLabel").style.color);
        Assert.AreEqual(expectedStyle, toastComplexDoc.rootVisualElement.Q<VisualElement>("IconContainer").style.display);
    }

    [Test, Order(10)]
    [Category("BuildServer")]
    public void ResetToast_WithSimple_ResetsStyle()
    {
        //Arrange
        StyleColor expectedColor = StyleKeyword.Null;
        StyleBackground expectedBackground = StyleKeyword.Null;
        StyleEnum<DisplayStyle> expectedStyle = new StyleEnum<DisplayStyle>(DisplayStyle.Flex);

        //Act
        toastSimple.ResetToast();

        //Assert
        Assert.AreEqual(expectedColor, toastSimpleDoc.rootVisualElement.Q<VisualElement>("Toast").style.backgroundColor);
        Assert.AreEqual(expectedColor, toastSimpleDoc.rootVisualElement.Q<Label>("HeaderLabel").style.color);
        Assert.AreEqual(expectedColor, toastSimpleDoc.rootVisualElement.Q<Label>("MessageLabel").style.color);
        Assert.AreEqual(expectedBackground, toastSimpleDoc.rootVisualElement.Q<VisualElement>("Icon").style.backgroundImage);
        Assert.AreEqual(expectedStyle, toastSimpleDoc.rootVisualElement.Q<VisualElement>("IconContainer").style.display);
    }

    [Test, Order(11)]
    [Category("BuildServer")]
    public void ResetToast_WithComplex_ResetsStyle()
    {
        //Arrange
        StyleColor expectedColor = StyleKeyword.Null;
        StyleBackground expectedBackground = StyleKeyword.Null;
        StyleEnum<DisplayStyle> expectedStyle = new StyleEnum<DisplayStyle>(DisplayStyle.Flex);

        //Act
        toastComplex.ResetToast();

        //Assert
        Assert.AreEqual(expectedColor, toastComplexDoc.rootVisualElement.Q<VisualElement>("Toast").style.backgroundColor);
        Assert.AreEqual(expectedColor, toastComplexDoc.rootVisualElement.Q<Label>("HeaderLabel").style.color);
        Assert.AreEqual(expectedColor, toastComplexDoc.rootVisualElement.Q<Label>("MessageLabel").style.color);
        Assert.AreEqual(expectedColor, toastComplexDoc.rootVisualElement.Q<Label>("SecondaryMessageLabel").style.color);
        Assert.AreEqual(expectedBackground, toastComplexDoc.rootVisualElement.Q<VisualElement>("Icon").style.backgroundImage);
        Assert.AreEqual(expectedStyle, toastComplexDoc.rootVisualElement.Q<VisualElement>("IconContainer").style.display);
    }

    [UnityTest, Order(12)]
    [Category("BuildServer")]
    public IEnumerator FadeIn_SetsDisplayAndOpacity()
    {
        //Arrange
        StyleEnum<DisplayStyle> expectedStyle = new StyleEnum<DisplayStyle>(DisplayStyle.Flex);
        StyleFloat expectedOpacity = new StyleFloat(1.0f);

        //Act
        yield return toastSimple.FadeIn();
        yield return toastComplex.FadeIn();

        //Assert
        Assert.AreEqual(expectedStyle, toastSimpleDoc.rootVisualElement.style.display);
        Assert.AreEqual(expectedStyle, toastComplexDoc.rootVisualElement.style.display);
        Assert.AreEqual(expectedOpacity, toastSimpleDoc.rootVisualElement.Q<VisualElement>("Toast").style.opacity);
        Assert.AreEqual(expectedOpacity, toastComplexDoc.rootVisualElement.Q<VisualElement>("Toast").style.opacity);
    }

    [UnityTest, Order(13)]
    [Category("BuildServer")]
    public IEnumerator FadeOut_SetsDisplayAndOpacity()
    {
        //Arrange
        StyleEnum<DisplayStyle> expectedStyle = new StyleEnum<DisplayStyle>(DisplayStyle.None);
        StyleFloat expectedOpacity = new StyleFloat(0.0f);

        //Act
        yield return toastSimple.FadeOut();
        yield return toastComplex.FadeOut();

        //Assert
        Assert.AreEqual(expectedStyle, toastSimpleDoc.rootVisualElement.style.display);
        Assert.AreEqual(expectedStyle, toastComplexDoc.rootVisualElement.style.display);
        Assert.AreEqual(expectedOpacity, toastSimpleDoc.rootVisualElement.Q<VisualElement>("Toast").style.opacity);
        Assert.AreEqual(expectedOpacity, toastComplexDoc.rootVisualElement.Q<VisualElement>("Toast").style.opacity);
    }

    [Test, Order(14)]
    [Category("BuildServer")]
    public void CloseToast_WithSimple_SetsDisplayToNone()
    {
        //Arrange
        StyleEnum<DisplayStyle> expectedStyle = new StyleEnum<DisplayStyle>(DisplayStyle.None);

        //Act
        toastSimple.CloseToast();

        //Assert
        Assert.AreEqual(expectedStyle, toastSimpleDoc.rootVisualElement.style.display);
    }

    [Test, Order(15)]
    [Category("BuildServer")]
    public void CloseToast_WithComplex_SetsDisplayToNone()
    {
        //Arrange
        StyleEnum<DisplayStyle> expectedStyle = new StyleEnum<DisplayStyle>(DisplayStyle.None);

        //Act
        toastComplex.CloseToast();

        //Assert
        Assert.AreEqual(expectedStyle, toastComplexDoc.rootVisualElement.style.display);
    }

    [Test, Order(16)]
    [Category("BuildServer")]
    public void Show_SetsDisplayFlex()
    {
        //Arrange
        StyleEnum<DisplayStyle> expectedStyle = new StyleEnum<DisplayStyle>(DisplayStyle.Flex);

        //Act
        toastSimple.Show();
        toastComplex.Show();

        //Assert
        Assert.AreEqual(expectedStyle, toastSimpleDoc.rootVisualElement.style.display);
        Assert.AreEqual(expectedStyle, toastComplexDoc.rootVisualElement.style.display);
    }

    [Test, Order(17)]
    [Category("BuildServer")]
    public void Hide_SetsDisplayNone()
    {
        //Arrange
        StyleEnum<DisplayStyle> expectedStyle = new StyleEnum<DisplayStyle>(DisplayStyle.None);

        //Act
        toastSimple.Hide();
        toastComplex.Hide();

        //Assert
        Assert.AreEqual(expectedStyle, toastSimpleDoc.rootVisualElement.style.display);
        Assert.AreEqual(expectedStyle, toastComplexDoc.rootVisualElement.style.display);
    }
}