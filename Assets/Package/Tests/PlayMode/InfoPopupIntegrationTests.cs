using System.Collections;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.UIElements;
using UnityEditor;
using NUnit.Framework;
using VARLab.Velcro;

public class InfoPopupIntegrationTests
{
    private int sceneCounter = 0;

    private GameObject popupObj;
    private UIDocument popupDoc;
    private InfoPopupSmall popupSmall;
    private InfoPopupSO infoPopupSO;

    private Sprite defaultImg;
    private GameObject dummyObject; // Used for tests which call HandleDisplayUI

    [UnitySetUp]
    [Category("BuildServer")]
    public IEnumerator SetUp()
    {
        sceneCounter = TestUtils.ClearScene(sceneCounter, "InformationPopupScene");

        // Required for the MovePopupBesideObject call in HandleDisplayUI
        var cameraObj = new GameObject("Camera");
        var camera = cameraObj.AddComponent<Camera>();
        cameraObj.tag = "MainCamera";

        //Set up <InformationDialog> UI
        popupObj = new GameObject("Information Popup");
        popupDoc = popupObj.AddComponent<UIDocument>();

        //Load required assets from project files
        VisualTreeAsset infoPopupUXML = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/VELCRO UI/UI/Info Popups/InfoPopupSmall.uxml");
        PanelSettings panelSettings = AssetDatabase.LoadAssetAtPath<PanelSettings>("Assets/VELCRO UI/Settings/Panel Settings.asset");
        defaultImg = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/VELCRO UI/Sprites/Icons/Default_Sprite.png");

        //Reference panel settings and source asset as SerializedFields
        SerializedObject so = new SerializedObject(popupDoc);
        so.FindProperty("m_PanelSettings").objectReferenceValue = panelSettings;
        so.FindProperty("sourceAsset").objectReferenceValue = infoPopupUXML;
        so.ApplyModifiedProperties();

        infoPopupSO = ScriptableObject.CreateInstance<InfoPopupSO>();
        infoPopupSO.Image = defaultImg;
        infoPopupSO.Title = "There is text here!";

        infoPopupUXML.CloneTree(popupDoc.rootVisualElement);

        popupSmall = popupObj.AddComponent<InfoPopupSmall>();
        dummyObject = GameObject.CreatePrimitive(PrimitiveType.Cube); // Required since a standard new GameObject() does not have valid bounds properties

        yield return null;
    }

    [Test, Order(1)]
    [Category("BuildServer")]
    public void Start_SetsRootToDisplayNone()
    {
        //Assert
        Assert.AreEqual(new StyleEnum<DisplayStyle>(DisplayStyle.None), popupDoc.rootVisualElement.style.display);
    }

    [Test, Order(2)]
    [Category("BuildServer")]
    public void HandleDisplayUI_WithValidElements_SetsContent_RootToDisplayFlex()
    {
        //Arrange
        string expectedTitle = "Test 2 Title text";
        StyleBackground expectedImg = new StyleBackground(defaultImg);

        //Act
        popupSmall.HandleDisplayUI(expectedTitle, defaultImg, dummyObject);

        //Assert
        Assert.AreEqual(expectedTitle, popupDoc.rootVisualElement.Q<Label>("TitleLabel").text);
        Assert.AreEqual(expectedImg, popupDoc.rootVisualElement.Q<VisualElement>("Image").style.backgroundImage);
        Assert.AreEqual(new StyleEnum<DisplayStyle>(DisplayStyle.Flex), popupDoc.rootVisualElement.style.display);
    }

    [Test, Order(3)]
    [Category("BuildServer")]
    public void HandleDisplayUI_WithSO_SetsContent_RootToDisplayFlex()
    {
        //Arrange
        StyleBackground expectedImg = new StyleBackground(infoPopupSO.Image);

        //Act
        popupSmall.HandleDisplayUI(infoPopupSO, dummyObject);

        //Assert
        Assert.AreEqual(infoPopupSO.Title, popupDoc.rootVisualElement.Q<Label>("TitleLabel").text);
        Assert.AreEqual(expectedImg, popupDoc.rootVisualElement.Q<VisualElement>("Image").style.backgroundImage);
        Assert.AreEqual(new StyleEnum<DisplayStyle>(DisplayStyle.Flex), popupDoc.rootVisualElement.style.display);
    }

    [Test, Order(4)]
    [Category("BuildServer")]
    public void SetContent_WithCustomIconAndTitle_SetsValues()
    {
        //Arrange
        string Title = "Setting Content With This Text";
        Sprite img = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/VELCRO UI/Sprites/Icons/Rabbit_Sprite.png"); // using rabbit icon image as test sprite
        StyleBackground expectedImg = new StyleBackground(img);

        //Act
        popupSmall.SetContent(Title, img);

        //Assert
        Assert.AreEqual(Title, popupDoc.rootVisualElement.Q<Label>("TitleLabel").text);
        Assert.AreEqual(expectedImg, popupDoc.rootVisualElement.Q<VisualElement>("Image").style.backgroundImage);
    }

    [Test, Order(5)]
    [Category("BuildServer")]
    public void Show_Triggers_OnPopupShown()
    {
        //Arrange
        popupSmall.OnPopupShown.AddListener(Ping);

        //Act
        popupSmall.Show();

        //Assert
        LogAssert.Expect(LogType.Log, "Event triggered!");

        //Clean Up
        popupSmall.OnPopupShown.RemoveListener(Ping);
    }

    [Test, Order(6)]
    [Category("BuildServer")]
    public void Hide_Triggers_OnPopupHidden()
    {
        //Arrange
        popupSmall.OnPopupHidden.AddListener(Ping);

        //Act
        popupSmall.Hide();

        //Assert
        LogAssert.Expect(LogType.Log, "Event triggered!");

        //Clean Up
        popupSmall.OnPopupHidden.RemoveListener(Ping);
    }

    private void Ping()
    {
        Debug.Log("Event triggered!");
    }
}
