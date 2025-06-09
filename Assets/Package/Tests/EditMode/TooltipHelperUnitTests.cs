using NUnit.Framework;
using System;
using System.Collections;
using UnityEditor;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.UIElements;
using VARLab.Velcro;

public class TooltipHelperUnitTests
{
    private GameObject obj;
    private UIDocument doc;

    [Test, Order(1)]
    public void ConvertToEditorCoordinates_WithValues_CalculatesCorrectPosition()
    {
        //Arrange
        float screenWidth = 1920f;
        float screenHeight = 1080f;

        Rect sourcePosition = new Rect(960f, 540f, 0f, 0f);
        Vector2 expectedPosition = new Vector2(150f, 100f);
        Vector2 panelSize = new Vector2(300f, 200f);

        //Act
        Vector2 actualPosition = TooltipHelper.ConvertToEditorCoordinates(sourcePosition, panelSize, screenWidth, screenHeight);

        //Assert
        Assert.AreEqual(expectedPosition, actualPosition);
    }

    /// <summary>
    /// This method calculates the center of a VisualElement in the UI layer so tooltips can be displayed next to it
    /// </summary>
    [UnityTest, Order(2)]
    public IEnumerator Test_CalculateVisualElementCenter_OffsetFromOrigin_CalculatesCorrectCenter()
    {
        // Screen setup
        PlayModeWindow.SetCustomRenderingResolution(1920, 1080, "1080p");

        SetupUI();
        yield return null;

        //Arrange
        //Below values change depending on layout of the scene/size since the parent grows to fill the space
        //X is width of label "Tooltip Demo" in the Tooltip Scene + padding on parent
        //Y is height of label "Tooltip Demo" in the Tooltip Scene + padding on parent
        Vector2 expectedCenterPosition = new Vector2(641f, 294f);

        //Act
        //This is using the green hover element in scene that invokes a tooltip pointing right
        Vector2 unroundedCenterPosition = TooltipHelper.CalculateVisualElementCenter(doc.rootVisualElement.Q<VisualElement>("TooltipRight"));
        float actualX = MathF.Round(unroundedCenterPosition.x, 1);
        float actualY = MathF.Round(unroundedCenterPosition.y, 1);
        Vector2 actualCenterPosition = new Vector2(actualX, actualY);

        //Assert
        Assert.AreEqual(expectedCenterPosition, actualCenterPosition);
    }

    /// <summary>
    /// This method calculates the center of a GameObject in the UI layer so tooltips can be displayed next to objects
    /// </summary>
    [Test, Order(3)]
    public void GetObjectCenterInScreenSpace_CalculatesObjectCenterForUILayer()
    {
        // Screen setup
        PlayModeWindow.SetCustomRenderingResolution(1920, 1080, "1080p");

        //Arrange
        GameObject obj = GameObject.CreatePrimitive(PrimitiveType.Cube);
        Vector2 expectedCenter = new Vector2(960f, 443.9f);

        //Act
        Vector2 unroundedActualCenter = TooltipHelper.GetObjectCenterInScreenSpace(new Camera(), obj.GetComponent<Renderer>());
        float actualX = MathF.Round(unroundedActualCenter.x, 1);
        float actualY = MathF.Round(unroundedActualCenter.y, 1);
        Vector2 actualCenter = new Vector2(actualX, actualY);

        //Assert
        Assert.AreEqual(expectedCenter, actualCenter);
    }

    private void SetupUI()
    {
        //Set up UI
        obj = new GameObject("Object");
        doc = obj.AddComponent<UIDocument>();

        VisualTreeAsset uxml = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Package/Samples/4 - Tooltip Demo/UI/TooltipDemo.uxml");
        PanelSettings panelSettings = AssetDatabase.LoadAssetAtPath<PanelSettings>("Assets/VELCRO UI/Settings/Panel Settings.asset");

        //Reference panel settings and source asset as SerializedFields
        SerializedObject so = new SerializedObject(doc);
        so.FindProperty("m_PanelSettings").objectReferenceValue = panelSettings;
        so.FindProperty("sourceAsset").objectReferenceValue = uxml;
        so.ApplyModifiedProperties();

        uxml.CloneTree(doc.rootVisualElement);
        doc.sortingOrder = 1;
    }
}