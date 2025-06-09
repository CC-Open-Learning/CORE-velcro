using NUnit.Framework;
using System.Collections;
using UnityEditor;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.UIElements;
using VARLab.Velcro;

public class TutorialIntegrationTests
{
    private int sceneCounter = 0;

    private GameObject tutorialObj;
    private UIDocument tutorialDoc;
    private Tutorial tutorial;
    private TutorialSO tutorialSO;
    private Sprite sprite;

    [UnitySetUp]
    [Category("BuildServer")]
    public IEnumerator SetUp()
    {
        sceneCounter = TestUtils.ClearScene(sceneCounter, "TutorialScene");

        //Set up <Tutorial> UI
        tutorialObj = new GameObject("Tutorial");
        tutorialDoc = tutorialObj.AddComponent<UIDocument>();
        tutorial = tutorialObj.AddComponent<Tutorial>();

        //Load required assets from project files
        VisualTreeAsset tutorialUXML = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/VELCRO UI/UI/Tutorial/Tutorial.uxml");
        VisualTreeAsset carouselIndicatorUXML = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/VELCRO UI/UI/Tutorial/Button-Tutorial-Carousel-Indicator.uxml");
        PanelSettings panelSettings = AssetDatabase.LoadAssetAtPath<PanelSettings>("Assets/VELCRO UI/Settings/Panel Settings.asset");
        sprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Package/Samples/20 - Tutorial Demo/UI/Tutorial_Demo_Sprite.png");

        //Reference panel settings and source asset as SerializedFields
        SerializedObject so = new SerializedObject(tutorialDoc);
        so.FindProperty("m_PanelSettings").objectReferenceValue = panelSettings;
        so.FindProperty("sourceAsset").objectReferenceValue = tutorialUXML;
        so.ApplyModifiedProperties();

        //Create new SO
        tutorialSO = ScriptableObject.CreateInstance<TutorialSO>();
        tutorialSO.Name = "Name";
        tutorialSO.SecondaryBtnText = "Previous";
        tutorialSO.TertiaryBtnText = "Skip";
        tutorialSO.IsBackgroundDimmed = true;

        for (int i = 0; i < 3; i++)
        {
            TutorialSection newSection = new TutorialSection();
            newSection.Image = sprite;
            newSection.Header = i.ToString();
            newSection.Description = i.ToString();
            newSection.PrimaryBtnText = "Next";
            tutorialSO.TutorialSections.Add(newSection);
        }

        //Reference SO, and carouselIndicator button as SerializedFields
        SerializedObject so2 = new SerializedObject(tutorial);
        so2.FindProperty("carouselIndicatorTemplate").objectReferenceValue = carouselIndicatorUXML;
        so2.FindProperty("tutorialSO").objectReferenceValue = tutorialSO;
        so2.ApplyModifiedProperties();

        tutorialUXML.CloneTree(tutorialDoc.rootVisualElement);
        yield return null;
    }

    [Test, Order(1)]
    [Category("BuildServer")]
    public void Start_SetsRootToDisplayNone()
    {
        //Arrange
        StyleEnum<DisplayStyle> expectedStyle = new StyleEnum<DisplayStyle>(DisplayStyle.None);

        //Assert
        Assert.AreEqual(expectedStyle, tutorialDoc.rootVisualElement.style.display);
    }

    [Test, Order(2)]
    [Category("BuildServer")]
    public void HandleDisplayUI_SetsRootToDisplayFlex()
    {
        //Arrange
        StyleEnum<DisplayStyle> expectedStyle = new StyleEnum<DisplayStyle>(DisplayStyle.Flex);

        //Act
        tutorial.HandleDisplayUI();

        //Assert
        Assert.AreEqual(expectedStyle, tutorialDoc.rootVisualElement.style.display);
    }

    [Test, Order(3)]
    [Category("BuildServer")]
    public void HandleDisplayUI_PopulatesStrings()
    {
        //Arrange
        string expectedName = "Name";
        string expectedHeader = "0";
        string expectedDescription = "0";
        string expectedPrimaryBtnText = "Next";
        string expectedSecondaryBtnText = "Previous";
        string expectedTertiaryBtnText = "Skip";
        string expectedStepMin = "1";
        string expectedStepMax = "3";

        //Act
        tutorial.HandleDisplayUI();

        //Assert
        Assert.AreEqual(expectedName, tutorialDoc.rootVisualElement.Q<Label>("NameLabel").text);
        Assert.AreEqual(expectedHeader, tutorialDoc.rootVisualElement.Q<Label>("HeaderLabel").text);
        Assert.AreEqual(expectedDescription, tutorialDoc.rootVisualElement.Q<Label>("DescriptionLabel").text);
        Assert.AreEqual(expectedPrimaryBtnText, tutorialDoc.rootVisualElement.Q<Button>("ButtonTutorialPrimary").Q<Label>().text);
        Assert.AreEqual(expectedSecondaryBtnText, tutorialDoc.rootVisualElement.Q<Button>("ButtonTutorialSecondary").Q<Label>().text);
        Assert.AreEqual(expectedTertiaryBtnText, tutorialDoc.rootVisualElement.Q<Button>("ButtonTutorialSkip").text);
        Assert.AreEqual(expectedStepMin, tutorialDoc.rootVisualElement.Q<Label>("StepMinLabel").text);
        Assert.AreEqual(expectedStepMax, tutorialDoc.rootVisualElement.Q<Label>("StepMaxLabel").text);
    }

    [Test, Order(4)]
    [Category("BuildServer")]
    public void PopulateTutorialSection_WithSecondIndex_PopulatesNewSection()
    {
        //Arrange
        string expectedHeader = "1";
        string expectedDescription = "1";
        string expectedPrimaryBtnText = "Next";
        StyleBackground expectedSprite = new StyleBackground(sprite);

        //Act
        tutorial.PopulateTutorialSection(1);

        //Assert
        Assert.AreEqual(expectedHeader, tutorialDoc.rootVisualElement.Q<Label>("HeaderLabel").text);
        Assert.AreEqual(expectedDescription, tutorialDoc.rootVisualElement.Q<Label>("DescriptionLabel").text);
        Assert.AreEqual(expectedSprite.value, tutorialDoc.rootVisualElement.Q<VisualElement>("Image").style.backgroundImage.value);
        Assert.AreEqual(expectedPrimaryBtnText, tutorialDoc.rootVisualElement.Q<Button>("ButtonTutorialPrimary").Q<Label>().text);
    }

    [Test, Order(5)]
    [Category("BuildServer")]
    public void PopulateTutorialSection_WithBelowMinInt_LogsError()
    {
        //Arrange
        string expectedLogMessage = "Tutorial.PopulateTutorialSection() - Out of Bounds sectionIndex!";

        //Act
        tutorial.PopulateTutorialSection(-1);

        //Assert
        LogAssert.Expect(LogType.Error, expectedLogMessage);
    }

    [Test, Order(6)]
    [Category("BuildServer")]
    public void PopulateTutorialSection_WithAboveMaxInt_LogsError()
    {
        //Arrange
        string expectedLogMessage = "Tutorial.PopulateTutorialSection() - Out of Bounds sectionIndex!";

        //Act
        tutorial.PopulateTutorialSection(3);

        //Assert
        LogAssert.Expect(LogType.Error, expectedLogMessage);
    }

    [Test, Order(7)]
    [Category("BuildServer")]
    public void PopulateCarouselIndicators_ClonesBtnTemplates()
    {
        //Arrange
        int expectedCount = 3;

        //Act
        tutorial.PopulateCarouselIndicators();

        //Assert
        Assert.AreEqual(expectedCount, tutorialDoc.rootVisualElement.Q<VisualElement>("IndicatorContainer").childCount);
    }

    [Test, Order(8)]
    [Category("BuildServer")]
    public void PopulateCarouselIndicators_SetsBtnTooltips()
    {
        //Act
        tutorial.PopulateCarouselIndicators();

        //Assert
        for (int i = 0; i < tutorialSO.TutorialSections.Count; i++)
        {
            Assert.AreEqual(i.ToString(), tutorialDoc.rootVisualElement.Q<VisualElement>("IndicatorContainer").ElementAt(i).Q<Button>().tooltip);
        }
    }

    [Test, Order(9)]
    [Category("BuildServer")]
    public void PopulateCarouselIndicators_ShouldSetActiveClass()
    {
        //Arrange
        string expectedStyleClass = "tutorial-button-carousel-active";

        //Act
        tutorial.PopulateCarouselIndicators();

        //Assert
        Assert.IsTrue(tutorialDoc.rootVisualElement.Q<VisualElement>("IndicatorContainer").ElementAt(0).Q<Button>().ClassListContains(expectedStyleClass));
    }

    [Test, Order(10)]
    [Category("BuildServer")]
    public void ChangeActiveCarouselIndicators_ChangesActiveClasses()
    {
        //Arrange
        string expectedStyleClass = "tutorial-button-carousel-active";

        //Act
        tutorial.SetContent();
        tutorial.JumpToIndex(1);

        //Assert
        Assert.IsTrue(tutorialDoc.rootVisualElement.Q<VisualElement>("IndicatorContainer").ElementAt(1).Q<Button>().ClassListContains(expectedStyleClass));
    }

    [Test, Order(11)]
    [Category("BuildServer")]
    public void ChangePreviousButtonState_At0_SetsBtnToDisplayNone()
    {
        //Arrange
        StyleEnum<DisplayStyle> expectedStyle = new StyleEnum<DisplayStyle>(DisplayStyle.None);

        //Act
        tutorial.SetContent();
        tutorial.ChangePreviousButtonState();

        //Assert
        Assert.AreEqual(expectedStyle, tutorialDoc.rootVisualElement.Q<Button>("ButtonTutorialSecondary").style.display);
    }

    [Test, Order(12)]
    [Category("BuildServer")]
    public void ChangePreviousButtonState_At1_SetsBtnToDisplayFlex()
    {
        //Arrange
        StyleEnum<DisplayStyle> expectedStyle = new StyleEnum<DisplayStyle>(DisplayStyle.Flex);

        //Act
        tutorial.SetContent();
        tutorial.JumpToIndex(1);    
        tutorial.ChangePreviousButtonState();

        //Assert
        Assert.AreEqual(expectedStyle, tutorialDoc.rootVisualElement.Q<Button>("ButtonTutorialSecondary").style.display);
    }

    [Test, Order(13)]
    [Category("BuildServer")]
    public void JumpToIndex_PopulatesNewSection()
    {
        //Arrange
        string expectedHeader = "2";
        string expectedDescription = "2";
        string expectedPrimaryBtnText = "Next";
        StyleBackground expectedSprite = new StyleBackground(sprite);

        //Act
        tutorial.PopulateTutorialSection(2);

        //Assert
        Assert.AreEqual(expectedHeader, tutorialDoc.rootVisualElement.Q<Label>("HeaderLabel").text);
        Assert.AreEqual(expectedDescription, tutorialDoc.rootVisualElement.Q<Label>("DescriptionLabel").text);
        Assert.AreEqual(expectedSprite.value, tutorialDoc.rootVisualElement.Q<VisualElement>("Image").style.backgroundImage.value); 
        Assert.AreEqual(expectedPrimaryBtnText, tutorialDoc.rootVisualElement.Q<Button>("ButtonTutorialPrimary").Q<Label>().text);
    }

    [Test, Order(14)]
    [Category("BuildServer")]
    public void AdvanceTutorial_AtLastIndex_SetsRootToDisplayNone()
    {
        //Arrange
        StyleEnum<DisplayStyle> expectedStyle = new StyleEnum<DisplayStyle>(DisplayStyle.None);

        //Act
        tutorial.SetContent();
        tutorial.AdvanceTutorial();
        tutorial.AdvanceTutorial();
        tutorial.AdvanceTutorial();

        //Assert
        Assert.AreEqual(expectedStyle, tutorialDoc.rootVisualElement.style.display);
    }

    [Test, Order(15)]
    [Category("BuildServer")]
    public void AdvanceTutorial_AtFirstIndex_LogsWarning()
    {
        //Arrange
        string expectedLogMessage = "Tutorial.ReverseTutorial() - Already at First Tutorial Section Index!";

        //Act
        tutorial.SetContent();
        tutorial.ReverseTutorial();

        //Assert
        LogAssert.Expect(LogType.Warning, expectedLogMessage);
    }

    [Test, Order(16)]
    [Category("BuildServer")]
    public void Show_SetsDisplayFlex()
    {
        //Arrange
        StyleEnum<DisplayStyle> expectedStyle = new StyleEnum<DisplayStyle>(DisplayStyle.Flex);

        //Act
        tutorial.Show();

        //Assert
        Assert.AreEqual(expectedStyle, tutorialDoc.rootVisualElement.style.display);
    }

    [Test, Order(17)]
    [Category("BuildServer")]
    public void Hide_SetsDisplayNone()
    {
        //Arrange
        StyleEnum<DisplayStyle> expectedStyle = new StyleEnum<DisplayStyle>(DisplayStyle.None);

        //Act
        tutorial.Hide();

        //Assert
        Assert.AreEqual(expectedStyle, tutorialDoc.rootVisualElement.style.display);
    }
}