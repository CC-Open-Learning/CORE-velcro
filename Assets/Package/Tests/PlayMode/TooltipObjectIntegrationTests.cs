using System.Linq;
using System.Collections;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.UIElements;
using UnityEditor;
using NUnit.Framework;
using VARLab.Velcro;

public class TooltipObjectIntegrationTests
{
    private GameObject tooltipObj;
    private UIDocument tooltipDoc;
    private TooltipUI tooltipUI;
    private VisualElement tooltipRoot;
    private GameObject tooltipHoverObj;
    private TooltipObject tooltipObject;

    private int sceneCounter;

    [UnitySetUp]
    [Category("BuildServer")]
    public IEnumerator SetUp()
    {
        sceneCounter = TestUtils.ClearScene(sceneCounter, "TooltipObjectScene");

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

        // Tooltip root is meant to represent the "Container" element in the tooltip template
        // It is not to be mistaken with the actual root of the uxml document
        tooltipRoot = tooltipDoc.rootVisualElement.Children().First();
        tooltipHoverObj = new GameObject("Tooltip Object");
        tooltipHoverObj.AddComponent<BoxCollider>();
        tooltipObject = tooltipHoverObj.AddComponent<TooltipObject>();

        SerializedObject tooltipObjectSO = new SerializedObject(tooltipObject);
        tooltipObjectSO.FindProperty("tooltipUI").objectReferenceValue = tooltipUI;
        tooltipObjectSO.ApplyModifiedProperties();

        yield return null;
    }

    [Test, Order(1)]
    [Category("BuildServer")]
    public void UpdateTooltipPosition_UpdatesTooltipPositon()
    {
        //Arrange
        tooltipRoot.transform.position = new Vector3(-999, -999, 0);
        Vector3 oldPosition = tooltipRoot.transform.position;

        //Act
        tooltipObject.UpdateTooltipPosition(Input.mousePosition);

        //Assert
        Assert.AreNotEqual(oldPosition, tooltipRoot.transform.position);
    }

    [Test, Order(2)]
    [Category("BuildServer")]
    public void SetTooltipText_WithValidString_SetsText()
    {
        //Arrange
        string expectedString = "Hello! Test string";

        //Act
        tooltipObject.SetTooltipText(expectedString);

        //Assert
        Assert.AreNotEqual(expectedString, tooltipRoot.Q<Label>("Label").text);
    }

    [Test, Order(3)]
    [Category("BuildServer")]
    public void SetTooltipText_WithInvalidString_MaintainsOldText()
    {
        //Arrange
        string expectedString = "Hello! Test string";

        //Act
        tooltipObject.SetTooltipText(expectedString);
        tooltipObject.SetTooltipText(null);

        //Assert
        Assert.AreNotEqual(expectedString, tooltipRoot.Q<Label>("Label").text);
    }
}