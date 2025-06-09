using NUnit.Framework;
using System.Collections;
using UnityEditor;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.UIElements;
using VARLab.Velcro;

public class InformationDialogIntegrationTests
{
    private int sceneCounter = 0;

    private GameObject dialogObj;
    private UIDocument dialogDoc;

    private InformationDialog dialog;
    private InformationDialogSO informationDialogSO;

    [UnitySetUp]
    [Category("BuildServer")]
    public IEnumerator SetUp()
    {
        sceneCounter = TestUtils.ClearScene(sceneCounter, "InformationDialogScene");

        //Set up <InformationDialog> UI
        dialogObj = new GameObject("Information Dialog");
        dialogDoc = dialogObj.AddComponent<UIDocument>();
        dialog = dialogObj.AddComponent<InformationDialog>();

        //Load required assets from project files
        VisualTreeAsset dialogUXML = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/VELCRO UI/UI/Modals/InformationDialog.uxml");
        PanelSettings panelSettings = AssetDatabase.LoadAssetAtPath<PanelSettings>("Assets/VELCRO UI/Settings/Panel Settings.asset");

        //Reference panel settings and source asset as SerializedFields
        SerializedObject so = new SerializedObject(dialogDoc);
        so.FindProperty("m_PanelSettings").objectReferenceValue = panelSettings;
        so.FindProperty("sourceAsset").objectReferenceValue = dialogUXML;
        so.ApplyModifiedProperties();

        dialog.InitializeDialog("information-dialog-canvas");

        informationDialogSO = ScriptableObject.CreateInstance<InformationDialogSO>();
        informationDialogSO.Name = "Name";
        informationDialogSO.Title = "Title";
        informationDialogSO.Description = "Description";
        informationDialogSO.PrimaryBtnText = "PrimaryBtn";
        informationDialogSO.IsBackgroundDimmed = true;

        dialogUXML.CloneTree(dialogDoc.rootVisualElement);
        yield return null;
    }

    [Test, Order(1)]
    [Category("BuildServer")]
    public void HandleDisplayUniqueUI_SetsRootToDisplayFlex()
    {
        //Act
        dialog.HandleDisplayUI(informationDialogSO);

        //Assert
        Assert.AreEqual(new StyleEnum<DisplayStyle>(DisplayStyle.Flex), dialogDoc.rootVisualElement.style.display);
    }
}