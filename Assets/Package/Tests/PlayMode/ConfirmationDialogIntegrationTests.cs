using System.Linq;
using System.Collections;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.UIElements;
using UnityEditor;
using NUnit.Framework;
using VARLab.Velcro;
using System.Collections.Generic;

public class ConfirmationDialogIntegrationTests
{
    private int sceneCounter;

    private GameObject dialogObj;
    private UIDocument dialogDoc;
    private ConfirmationDialog dialog;
    private ConfirmationDialogSO confirmationDialogSO;

    private VisualTreeAsset primaryButtonAsset;
    private VisualTreeAsset secondary1ButtonAsset;
    private VisualTreeAsset secondary2ButtonAsset;
    private VisualTreeAsset negative1ButtonAsset;
    private VisualTreeAsset negative2ButtonAsset;

    [UnitySetUp]
    [Category("BuildServer")]
    public IEnumerator SetUp()
    {
        sceneCounter = TestUtils.ClearScene(sceneCounter, "ConfirmationDialogScene");

        //Set up <ConfirmationDialog> UI
        dialogObj = new GameObject("Dialog Object");
        dialogDoc = dialogObj.AddComponent<UIDocument>();
        dialog = dialogObj.AddComponent<ConfirmationDialog>();

        //Load required assets from project files
        VisualTreeAsset dialogUXML = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/VELCRO UI/UI/Modals/ConfirmationDialog.uxml");
        PanelSettings panelSettings = AssetDatabase.LoadAssetAtPath<PanelSettings>("Assets/Package/Samples/Settings/Panel Settings.asset");

        //Reference panel settings and source asset as SerializedFields
        SerializedObject so = new SerializedObject(dialogDoc);
        so.FindProperty("m_PanelSettings").objectReferenceValue = panelSettings;
        so.FindProperty("sourceAsset").objectReferenceValue = dialogUXML;
        so.ApplyModifiedProperties();

        //Setup the list with all the button templates
        SerializedObject so2 = new SerializedObject(dialog);
        SerializedProperty buttonTemplates = so2.FindProperty("buttonTemplates");
        buttonTemplates.InsertArrayElementAtIndex(0);
        buttonTemplates.InsertArrayElementAtIndex(1);
        buttonTemplates.InsertArrayElementAtIndex(2);
        buttonTemplates.InsertArrayElementAtIndex(3);
        buttonTemplates.InsertArrayElementAtIndex(4);

        primaryButtonAsset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/VELCRO UI/Templates/Button Primary/Button-Primary-Small.uxml");
        secondary1ButtonAsset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/VELCRO UI/Templates/Button Secondary 1/Button-Secondary-Small-1.uxml");
        secondary2ButtonAsset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/VELCRO UI/Templates/Button Secondary 2/Button-Secondary-Small-2.uxml");
        negative1ButtonAsset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/VELCRO UI/Templates/Button Negative 1/Button-Negative-Small-1.uxml");
        negative2ButtonAsset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/VELCRO UI/Templates/Button Negative 2/Button-Negative-Small-2.uxml");

        buttonTemplates.GetArrayElementAtIndex(0).objectReferenceValue = primaryButtonAsset;
        buttonTemplates.GetArrayElementAtIndex(1).objectReferenceValue = secondary1ButtonAsset;
        buttonTemplates.GetArrayElementAtIndex(2).objectReferenceValue = secondary2ButtonAsset;
        buttonTemplates.GetArrayElementAtIndex(3).objectReferenceValue = negative1ButtonAsset;
        buttonTemplates.GetArrayElementAtIndex(4).objectReferenceValue = negative2ButtonAsset;
        so2.ApplyModifiedProperties();

        confirmationDialogSO = ScriptableObject.CreateInstance<ConfirmationDialogSO>();
        confirmationDialogSO.Name = "Name";
        confirmationDialogSO.Description = "Description";
        confirmationDialogSO.PrimaryBtnText = "PrimaryBtn";
        confirmationDialogSO.SecondaryBtnText = "SecondaryBtn";
        confirmationDialogSO.PrimaryBtnType = ButtonType.Primary;
        confirmationDialogSO.SecondaryBtnType = ButtonType.Secondary1;
        confirmationDialogSO.IsBackgroundDimmed = true;
        confirmationDialogSO.IsCloseBtnVisible = true;

        dialogUXML.CloneTree(dialogDoc.rootVisualElement);
        yield return null;
    }

    [Test, Order(1)]
    [Category("BuildServer")]
    public void Start_SetsRootToDisplayNone()
    {
        //Assert
        Assert.AreEqual(new StyleEnum<DisplayStyle>(DisplayStyle.None), dialogDoc.rootVisualElement.style.display);
    }

    [Test, Order(2)]
    [Category("BuildServer")]
    public void HandleDisplayUI_SetsRootToDisplayFlex()
    {
        //Act
        dialog.HandleDisplayUI(confirmationDialogSO);

        //Assert
        Assert.AreEqual(new StyleEnum<DisplayStyle>(DisplayStyle.Flex), dialogDoc.rootVisualElement.style.display);
    }

    [Test, Order(3)]
    [Category("BuildServer")]
    public void HandleDisplayUI_WithSO_ShouldPopulateStrings()
    {
        //Arrange
        string expectedName = "Name";
        string expectedDescription = "Description";
        string expectedPrimaryBtnText = "PrimaryBtn";
        string expectedSecondaryBtnText = "SecondaryBtn";

        //Act
        dialog.HandleDisplayUI(confirmationDialogSO);

        //Assert
        Assert.AreEqual(expectedName, dialogDoc.rootVisualElement.Q<Label>("NameLabel").text);
        Assert.AreEqual(expectedDescription, dialogDoc.rootVisualElement.Q<Label>("DescriptionLabel").text);

        List<TemplateContainer> buttons = dialogDoc.rootVisualElement.Query<TemplateContainer>().ToList();
        Assert.AreEqual(expectedSecondaryBtnText, buttons[2].Q<Button>("Button").text);
        Assert.AreEqual(expectedPrimaryBtnText, buttons[3].Q<Button>("Button").text);
    }

    [Test, Order(4)]
    [Category("BuildServer")]
    public void HandleDisplayUI_SecondaryButtonHasRightMarginClass()
    {
        //Arrange
        string expectedClass = "mr-20";

        //Act
        dialog.HandleDisplayUI(confirmationDialogSO);

        //Assert
        Assert.IsTrue(dialogDoc.rootVisualElement.Q("ButtonContainer").Children().First().ClassListContains(expectedClass));
    }

    [Test, Order(5)]
    [Category("BuildServer")]
    public void SetContent_AddsCorrectButtonCount()
    {
        //Arrange
        int expectedButtonCount = 2;

        //Act
        dialog.HandleDisplayUI(confirmationDialogSO);

        //Assert
        Assert.AreEqual(expectedButtonCount, dialogDoc.rootVisualElement.Q("ButtonContainer").childCount);
    }

    [Test, Order(6)]
    [Category("BuildServer")]
    public void SetCloseButton_WithCloseBtnVisible_SetsDisplayToFlex()
    {
        //Arrange
        StyleEnum<DisplayStyle> expectedStyle = new StyleEnum<DisplayStyle>(DisplayStyle.Flex);

        //Act
        dialog.SetCloseButton(true);

        //Assert
        Assert.AreEqual(expectedStyle, dialogDoc.rootVisualElement.Q<Button>("CloseBtn").style.display);
    }

    [Test, Order(7)]
    [Category("BuildServer")]
    public void SetCloseButton_WithoutCloseBtnVisible_SetsDisplayToNone()
    {
        //Arrange
        StyleEnum<DisplayStyle> expectedStyle = new StyleEnum<DisplayStyle>(DisplayStyle.None);

        //Act
        dialog.SetCloseButton(false);

        //Assert
        Assert.AreEqual(expectedStyle, dialogDoc.rootVisualElement.Q<Button>("CloseBtn").style.display);
    }

    [Test, Order(8)]
    [Category("BuildServer")]
    public void Show_SetsDisplayFlex()
    {
        //Arrange
        StyleEnum<DisplayStyle> expectedStyle = new StyleEnum<DisplayStyle>(DisplayStyle.Flex);

        //Act
        dialog.Show();

        //Assert
        Assert.AreEqual(expectedStyle, dialogDoc.rootVisualElement.style.display);
    }

    [Test, Order(9)]
    [Category("BuildServer")]
    public void Hide_SetsDisplayNone()
    {
        //Arrange
        StyleEnum<DisplayStyle> expectedStyle = new StyleEnum<DisplayStyle>(DisplayStyle.None);

        //Act
        dialog.Show();
        dialog.Hide();

        //Assert
        Assert.AreEqual(expectedStyle, dialogDoc.rootVisualElement.style.display);
    }
}