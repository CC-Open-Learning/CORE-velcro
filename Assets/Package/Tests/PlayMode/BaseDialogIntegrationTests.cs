using NUnit.Framework;
using System.Collections;
using UnityEditor;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.UIElements;
using VARLab.Velcro;

public class BaseDialogIntegrationTests : MonoBehaviour
{
    private int sceneCounter = 0;

    private GameObject dialogObj;
    private UIDocument dialogDoc;

    private BaseDialog dialog;
    private BaseDialogSO baseDialogSO;

    [UnitySetUp]
    [Category("BuildServer")]
    public IEnumerator SetUp()
    {
        sceneCounter = TestUtils.ClearScene(sceneCounter, "BaseDialogScene");

        //Set up <BaseDialog> UI
        dialogObj = new GameObject("Base Dialog");
        dialogDoc = dialogObj.AddComponent<UIDocument>();
        dialog = dialogObj.AddComponent<BaseDialog>();

        //Load required assets from project files
        //Uses InformationDialog doc, but could use ImageDialog alternatively
        VisualTreeAsset dialogUXML = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/VELCRO UI/UI/Modals/InformationDialog.uxml");
        PanelSettings panelSettings = AssetDatabase.LoadAssetAtPath<PanelSettings>("Assets/VELCRO UI/Settings/Panel Settings.asset");

        //Reference panel settings and source asset as SerializedFields
        SerializedObject so = new SerializedObject(dialogDoc);
        so.FindProperty("m_PanelSettings").objectReferenceValue = panelSettings;
        so.FindProperty("sourceAsset").objectReferenceValue = dialogUXML;
        so.ApplyModifiedProperties();

        dialogUXML.CloneTree(dialogDoc.rootVisualElement);
        dialog.InitializeDialog("information-dialog-canvas");

        baseDialogSO = ScriptableObject.CreateInstance<BaseDialogSO>();
        baseDialogSO.Name = "Name";
        baseDialogSO.Title = "Title";
        baseDialogSO.Description = "Description";
        baseDialogSO.PrimaryBtnText = "PrimaryBtn";
        baseDialogSO.IsBackgroundDimmed = true;
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
    public void SetContent_WithSO_ShouldPopulateStrings()
    {
        //Arrange
        string expectedName = "Name";
        string expectedTitle = "Title";
        string expectedDescription = "Description";
        string expectedPrimaryBtnText = "PrimaryBtn";

        //Act
        dialog.SetContent(baseDialogSO);

        //Assert
        Assert.AreEqual(expectedName, dialogDoc.rootVisualElement.Q<Label>("NameLabel").text);
        Assert.AreEqual(expectedTitle, dialogDoc.rootVisualElement.Q<Label>("TitleLabel").text);
        Assert.AreEqual(expectedDescription, dialogDoc.rootVisualElement.Q<Label>("DescriptionLabel").text);
        Assert.AreEqual(expectedPrimaryBtnText, dialogDoc.rootVisualElement.Q<Button>("Button").text);
    }

    [Test, Order(3)]
    [Category("BuildServer")]
    public void SetContent_WithDescriptionBolded_SetsBold()
    {
        //Arrange
        string expectedClass = "fw-700";
        baseDialogSO.IsDescriptionBolded = true;

        //Act
        dialog.SetContent(baseDialogSO);

        //Assert
        Assert.IsTrue(dialogDoc.rootVisualElement.Q<Label>("DescriptionLabel").ClassListContains(expectedClass));
    }

    [Test, Order(4)]
    [Category("BuildServer")]
    public void SetContent_WithDescriptionRegular_SetsRegular()
    {
        //Arrange
        string expectedClass = "fw-400";
        baseDialogSO.IsDescriptionBolded = false;

        //Act
        dialog.SetContent(baseDialogSO);

        //Assert
        Assert.IsTrue(dialogDoc.rootVisualElement.Q<Label>("DescriptionLabel").ClassListContains(expectedClass));
    }

    [Test, Order(5)]
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

    [Test, Order(6)]
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
