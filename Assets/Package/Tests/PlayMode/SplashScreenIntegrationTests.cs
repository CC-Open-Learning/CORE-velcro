using System.Collections;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.UIElements;
using UnityEditor;
using NUnit.Framework;
using VARLab.Velcro;
using System.Collections.Generic;

public class SplashScreenIntegrationTests
{
    private int sceneCounter;

    private GameObject splashObj;
    private UIDocument splashDoc;
    private SplashScreen splash;

    [UnitySetUp]
    [Category("BuildServer")]
    public IEnumerator SetUp()
    {
        sceneCounter = TestUtils.ClearScene(sceneCounter, "SplashScene");

        //Set up <SplashScreen> UI
        splashObj = new GameObject("Splash Object");
        splashDoc = splashObj.AddComponent<UIDocument>();
        splash = splashObj.AddComponent<SplashScreen>();

        //Load required assets from project files
        VisualTreeAsset splashUXML = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/VELCRO UI/UI/Splash Screen/SplashScreen.uxml");
        PanelSettings panelSettings = AssetDatabase.LoadAssetAtPath<PanelSettings>("Assets/Package/Samples/Settings/Panel Settings.asset");

        //Reference panel settings and source asset as SerializedFields
        SerializedObject so = new SerializedObject(splashDoc);
        so.FindProperty("m_PanelSettings").objectReferenceValue = panelSettings;
        so.FindProperty("sourceAsset").objectReferenceValue = splashUXML;
        so.ApplyModifiedProperties();

        //Setup the list with all the string messages
        SplashScreenSO splashScreenSO = ScriptableObject.CreateInstance<SplashScreenSO>();
        splashScreenSO.IntroText = "Developed By";
        splashScreenSO.OrganizationText = "CENTRE FOR VIRTUAL REALITY INNOVATION";
        splashScreenSO.Messages = new List<string>
        {
            "Message One",
            "Message Two",
            "Message Three"
        };

        //Reference SerializedFields in <SplashScreen>
        SerializedObject so2 = new SerializedObject(splash);
        so2.FindProperty("messageDuration").floatValue = 0.05f;
        so2.FindProperty("fadeDuration").floatValue = 0.05f;
        so2.FindProperty("splashScreenSO").objectReferenceValue = splashScreenSO;
        so2.ApplyModifiedProperties();

        splashUXML.CloneTree(splashDoc.rootVisualElement);
        yield return null;
    }

    [Test, Order(1)]
    [Category("BuildServer")]
    public void Start_SetsRootToDisplayFlex()
    { 
        //Arrange
        StyleEnum<DisplayStyle> expectedStyle = new StyleEnum<DisplayStyle>(DisplayStyle.Flex);

        //Assert
        //The actual value of display.style and display.resolvedStyle is Flex, but for some reason it displays as " Flex".
        //This is the reason is is converted to string and trimmed
        Assert.AreEqual(expectedStyle.ToString().Trim(), splashDoc.rootVisualElement.style.display.value.ToString().Trim());
    }

    [UnityTest, Order(2)]
    [Category("BuildServer")]
    public IEnumerator HandleTaskComplete_ChangesLabel()
    {
        //Arrange
        string expectedMessage = "Message Two";

        //Act
        splash.HandleTaskComplete();
        yield return new WaitForSecondsRealtime(0.06f);

        //Assert
        Assert.AreEqual(expectedMessage, splashDoc.rootVisualElement.Q<Label>("MessageLabel").text);
    }

    [UnityTest, Order(3)]
    [Category("BuildServer")]
    public IEnumerator HandleAllTaskComplete_ChangesLabel()
    {
        //Arrange
        string expectedMessage = "Message Three";

        //Act
        splash.HandleAllTaskComplete();
        yield return new WaitForSecondsRealtime(0.15f);

        //Assert
        Assert.AreEqual(expectedMessage, splashDoc.rootVisualElement.Q<Label>("MessageLabel").text);
    }

    [UnityTest, Order(4)]
    [Category("BuildServer")]
    public IEnumerator FadeOut_ChangesOpacityTo0()
    {
        //Arrange
        float expectedOpacity = 0.0f;

        //Act
        splash.HandleAllTaskComplete();
        yield return new WaitForSecondsRealtime(0.15f);

        //Assert
        Assert.AreEqual(expectedOpacity, splashDoc.rootVisualElement.Q<VisualElement>("Canvas").style.opacity.value);
    }

    [Test, Order(5)]
    [Category("BuildServer")]
    public void Show_SetsDisplayFlex()
    {
        //Arrange
        StyleEnum<DisplayStyle> expectedStyle = new StyleEnum<DisplayStyle>(DisplayStyle.Flex);

        //Act
        splash.Show();

        //Assert
        Assert.AreEqual(expectedStyle, splashDoc.rootVisualElement.style.display);
    }

    [Test, Order(6)]
    [Category("BuildServer")]
    public void Hide_SetsDisplayNone()
    {
        //Arrange
        StyleEnum<DisplayStyle> expectedStyle = new StyleEnum<DisplayStyle>(DisplayStyle.None);

        //Act
        splash.Show();
        splash.Hide();

        //Assert
        Assert.AreEqual(expectedStyle, splashDoc.rootVisualElement.style.display);
    }
}