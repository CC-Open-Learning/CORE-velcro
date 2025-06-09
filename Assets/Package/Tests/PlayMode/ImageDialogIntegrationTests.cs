using NUnit.Framework;
using System.Collections;
using UnityEditor;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.UIElements;
using VARLab.Velcro;

public class ImageDialogIntegrationTests : MonoBehaviour
{
    private int sceneCounter = 0;

    private GameObject dialogObj;
    private UIDocument dialogDoc;
    private ImageDialog dialog;

    private ImageDialogSO imageDialogSO;

    [UnitySetUp]
    [Category("BuildServer")]
    public IEnumerator SetUp()
    {
        sceneCounter = TestUtils.ClearScene(sceneCounter, "ImageDialogScene");

        //Set up <ImageDialog> UI
        dialogObj = new GameObject("Image Dialog");
        dialogDoc = dialogObj.AddComponent<UIDocument>();
        dialog = dialogObj.AddComponent<ImageDialog>();

        //Load required assets from project files
        VisualTreeAsset dialogUXML = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/VELCRO UI/UI/Modals/ImageDialog.uxml");
        PanelSettings panelSettings = AssetDatabase.LoadAssetAtPath<PanelSettings>("Assets/VELCRO UI/Settings/Panel Settings.asset");

        //Reference panel settings and source asset as SerializedFields
        SerializedObject so = new SerializedObject(dialogDoc);
        so.FindProperty("m_PanelSettings").objectReferenceValue = panelSettings;
        so.FindProperty("sourceAsset").objectReferenceValue = dialogUXML;
        so.ApplyModifiedProperties();

        imageDialogSO = ScriptableObject.CreateInstance<ImageDialogSO>();
        imageDialogSO.Name = "Name";
        imageDialogSO.Title = "Title";
        imageDialogSO.Description = "Description";
        imageDialogSO.PrimaryBtnText = "PrimaryBtn";
        imageDialogSO.IsBackgroundDimmed = true;
        imageDialogSO.SubDescription = "SubDescription";
        imageDialogSO.Note = "Note";
        imageDialogSO.DialogImage = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/VELCRO UI/Sprites/Icons/Default_Sprite.png");
        imageDialogSO.NoteImage = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/VELCRO UI/Sprites/Checkmarks/Checkmark_Sprite.png");

        dialog.InitializeDialog("image-dialog-canvas");

        dialogUXML.CloneTree(dialogDoc.rootVisualElement);
        yield return null;
    }

    [Test, Order(1)]
    [Category("BuildServer")]
    public void HandleDisplayUI_SetsRootToDisplayFlex()
    {
        //Act
        dialog.HandleDisplayUI(imageDialogSO);

        //Assert
        Assert.AreEqual(new StyleEnum<DisplayStyle>(DisplayStyle.Flex), dialogDoc.rootVisualElement.style.display);
    }

    [Test, Order(2)]
    [Category("BuildServer")]
    public void HandleDisplayUniqueUI_WithSO_ShouldPopulateData()
    {
        //Arrange
        string expectedName = "Name";
        string expectedTitle = "Title";
        string expectedDescription = "Description";
        string expectedPrimaryBtnText = "PrimaryBtn";
        string expectedSubDescription = "SubDescription";
        string expectedNoteDescription = "Note";

        var dialogImageElement = dialogDoc.rootVisualElement.Q<VisualElement>("ImageContainer").Q<VisualElement>("ImageBackground").Q<VisualElement>("Image");
        var noteImageElement = dialogDoc.rootVisualElement.Q<VisualElement>("NoteContainer").Q<VisualElement>("NoteImage");

        //Act
        dialog.HandleDisplayUI(imageDialogSO);

        //Assert
        Assert.AreEqual(expectedName, dialogDoc.rootVisualElement.Q<Label>("NameLabel").text);
        Assert.AreEqual(expectedTitle, dialogDoc.rootVisualElement.Q<Label>("TitleLabel").text);
        Assert.AreEqual(expectedDescription, dialogDoc.rootVisualElement.Q<Label>("DescriptionLabel").text);
        Assert.AreEqual(expectedSubDescription, dialogDoc.rootVisualElement.Q<Label>("SubDescriptionLabel").text);
        Assert.AreEqual(expectedNoteDescription, dialogDoc.rootVisualElement.Q<Label>("NoteLabel").text);
        Assert.AreEqual(expectedPrimaryBtnText, dialogDoc.rootVisualElement.Q<Button>("Button").text);
        Assert.AreEqual(dialogImageElement.style.backgroundImage, dialogDoc.rootVisualElement.Q<VisualElement>("ImageContainer").Q<VisualElement>("ImageBackground").Q<VisualElement>("Image").style.backgroundImage);
        Assert.AreEqual(noteImageElement.style.backgroundImage, dialogDoc.rootVisualElement.Q<VisualElement>("NoteContainer").Q<VisualElement>("NoteImage").style.backgroundImage);
    }
}
