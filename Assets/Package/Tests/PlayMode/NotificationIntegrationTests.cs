using NUnit.Framework;
using NUnit.Framework.Internal;
using System.Collections;
using UnityEditor;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.UIElements;
using VARLab.Velcro;

public class NotificationIntegrationTests
{
    private int sceneCounter = 0;

    private GameObject notifObj;
    private UIDocument notifDoc;
    private Notification notification;

    private Sprite successIcon;
    private Sprite errorIcon;
    private Sprite infoIcon;
    private Sprite customIcon;

    [UnitySetUp]
    [Category("BuildServer")]
    public IEnumerator SetUp()
    {
        sceneCounter = TestUtils.ClearScene(sceneCounter, "NotificationScene");

        //Set up <Notification> UI
        notifObj = new GameObject("Notification Object");
        notifDoc = notifObj.AddComponent<UIDocument>();
        notification = notifObj.AddComponent<Notification>();
        
        //Load required assets from project files
        successIcon = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/VELCRO UI/Sprites/Icons/Success_Sprite.png");
        errorIcon = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/VELCRO UI/Sprites/Icons/Warning_Sprite.png");
        infoIcon = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/VELCRO UI/Sprites/Icons/Info_Sprite.png");
        customIcon = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/VELCRO UI/Sprites/Checkmarks/Checkmark_Sprite.png");
        VisualTreeAsset notificationUXML = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/VELCRO UI/UI/Notifications/Notification.uxml");
        PanelSettings panelSettings = AssetDatabase.LoadAssetAtPath<PanelSettings>("Assets/VELCRO UI/Settings/Panel Settings.asset");

        //Reference panel settings and source asset as SerializedFields
        SerializedObject so = new SerializedObject(notifDoc);
        so.FindProperty("m_PanelSettings").objectReferenceValue = panelSettings;
        so.FindProperty("sourceAsset").objectReferenceValue = notificationUXML;
        so.ApplyModifiedProperties();

        //Reference icons as SerializedFields
        SerializedObject so2 = new SerializedObject(notification);
        so2.FindProperty("successIcon").objectReferenceValue = successIcon;
        so2.FindProperty("errorIcon").objectReferenceValue = errorIcon;
        so2.FindProperty("infoIcon").objectReferenceValue = infoIcon;
        so2.FindProperty("customIcon").objectReferenceValue = customIcon;
        so2.ApplyModifiedProperties();

        notificationUXML.CloneTree(notifDoc.rootVisualElement);
        yield return null;
    }

    [Test, Order(1)]
    [Category("BuildServer")]
    public void Start_SetsRootToDisplayNone()
    {
        //Assert
        Assert.AreEqual(new StyleEnum<DisplayStyle>(DisplayStyle.None), notifDoc.rootVisualElement.style.display);
    }

    [Test, Order(2)]
    [Category("BuildServer")]
    public void HandleDisplayUI_WithValidElements_SetsRootToDisplayFlex()
    {
        //Arrange
        StyleEnum<DisplayStyle> expectedStyle = new StyleEnum<DisplayStyle>(DisplayStyle.Flex);

        //Act
        notification.HandleDisplayUI(NotificationType.Success, "Test Message", FontSize.Large, Align.FlexStart);

        //Assert
        Assert.AreEqual(expectedStyle, notifDoc.rootVisualElement.style.display);
    }

    [Test, Order(3)]
    [Category("BuildServer")]
    public void HandleDisplayUI_WithSO_SetsRootToDisplayFlex()
    {
        //Arrange
        StyleEnum<DisplayStyle> expectedStyle = new StyleEnum<DisplayStyle>(DisplayStyle.Flex);
        NotificationSO notificationSO = ScriptableObject.CreateInstance<NotificationSO>();
        notificationSO.NotificationType = NotificationType.Success;
        notificationSO.Alignment = Align.FlexStart;
        notificationSO.FontSize = FontSize.Medium;
        notificationSO.Message = "Test notification";

        //Act
        notification.HandleDisplayUI(notificationSO);

        //Assert
        Assert.AreEqual(expectedStyle, notifDoc.rootVisualElement.style.display);
    }

    [Test, Order(4)]
    [Category("BuildServer")]
    public void SetContent_WithSuccessType_ShouldPopulateNotification()
    {
        //Arrange
        StyleBackground expectedIcon = new StyleBackground(successIcon);
        string expectedMessage = "Test Message";

        //Act
        notification.SetContent(NotificationType.Success, "Test Message");

        //Assert
        Assert.AreEqual(expectedIcon, notifDoc.rootVisualElement.Q<VisualElement>("Icon").style.backgroundImage);
        Assert.AreEqual(expectedMessage, notifDoc.rootVisualElement.Q<Label>("Text").text);
    }

    [Test, Order(5)]
    [Category("BuildServer")]
    public void SetContent_WithInfoType_ShouldPopulateNotification()
    {
        //Arrange
        StyleBackground expectedIcon = new StyleBackground(infoIcon);
        string expectedMessage = "Test Message";

        //Act
        notification.SetContent(NotificationType.Info, "Test Message");

        //Assert
        Assert.AreEqual(expectedIcon, notifDoc.rootVisualElement.Q<VisualElement>("Icon").style.backgroundImage);
        Assert.AreEqual(expectedMessage, notifDoc.rootVisualElement.Q<Label>("Text").text);
    }

    [Test, Order(6)]
    [Category("BuildServer")]
    public void SetContent_WithErrorType_ShouldPopulateNotification()
    {
        //Arrange
        StyleBackground expectedIcon = new StyleBackground(errorIcon);
        string expectedMessage = "Test Message";

        //Act
        notification.SetContent(NotificationType.Error, "Test Message");

        //Assert
        Assert.AreEqual(expectedIcon, notifDoc.rootVisualElement.Q<VisualElement>("Icon").style.backgroundImage);
        Assert.AreEqual(expectedMessage, notifDoc.rootVisualElement.Q<Label>("Text").text);
    }

    [Test, Order(7)]
    [Category("BuildServer")]
    public void FadeIn_ShouldSetOpacityToOne()
    {
        //Arrange
        float expectedOpacity = 1.0f;

        //Act
        notification.FadeIn();

        //Assert
        Assert.AreEqual(expectedOpacity, notifDoc.rootVisualElement.resolvedStyle.opacity);
    }

    [Test, Order(8)]
    [Category("BuildServer")]
    public void Show_Triggers_OnNotificationOpened()
    {
        //Arrange
        notification.OnNotificationShown.AddListener(Ping);

        //Act
        notification.Show();

        //Assert
        LogAssert.Expect(LogType.Log, "Event triggered!");

        //Clean Up
        notification.OnNotificationShown.RemoveListener(Ping);
    }

    [Test, Order(9)]
    [Category("BuildServer")]
    public void Hide_Triggers_OnNotificationClosed()
    {
        //Arrange
        notification.OnNotificationHidden.AddListener(Ping);

        //Act
        notification.Hide();

        //Assert
        LogAssert.Expect(LogType.Log, "Event triggered!");

        //Clean Up
        notification.OnNotificationHidden.RemoveListener(Ping);
    }

    [Test, Order(10)]
    [Category("BuildServer")]
    public void SetContent_WithCustomType_ShouldPopulateNotification()
    {
        //Arrange
        StyleBackground expectedIcon = new StyleBackground(customIcon);
        string expectedMessage = "Test Message";

        //Act
        notification.SetContent(NotificationType.Custom, "Test Message");

        //Assert
        Assert.AreEqual(expectedIcon, notifDoc.rootVisualElement.Q<VisualElement>("Icon").style.backgroundImage);
        Assert.AreEqual(expectedMessage, notifDoc.rootVisualElement.Q<Label>("Text").text);
    }

    [Test, Order(11)]
    [Category("BuildServer")]
    public void SetContent_WithCustomType_ShouldSetInlineValues()
    {
        //Act
        notification.SetContent(NotificationType.Custom, "Test Message");

        //Assert
        Assert.AreNotEqual(StyleKeyword.Null, notifDoc.rootVisualElement.Q<VisualElement>("Icon").style.backgroundImage);
        Assert.AreNotEqual(StyleKeyword.Null, notifDoc.rootVisualElement.Q<VisualElement>("Container").style.backgroundColor);
        Assert.AreNotEqual(StyleKeyword.Null, notifDoc.rootVisualElement.Q<Label>("Text").style.color);
    }

    [Test, Order(12)]
    [Category("BuildServer")]
    public void SetCustomNotification_WithIcon_SetsValues() 
    {
        // Arrange
        StyleBackground expectedIcon = new StyleBackground(customIcon);
        StyleColor expectedBackgroundColour = new StyleColor(Color.red);
        StyleColor expectedTextColour = new StyleColor(Color.green);

        //Act
        notification.SetCustomNotification(Color.red, Color.green, customIcon);
        notification.HandleDisplayUI(NotificationType.Custom, "Test Message");

        //Assert
        Assert.AreEqual(expectedIcon, notifDoc.rootVisualElement.Q<VisualElement>("Icon").style.backgroundImage);
        Assert.AreEqual(expectedBackgroundColour, notifDoc.rootVisualElement.Q<VisualElement>("Container").style.backgroundColor);
        Assert.AreEqual(expectedTextColour, notifDoc.rootVisualElement.Q<Label>("Text").style.color);
    }

    [Test, Order(13)]
    [Category("BuildServer")]
    public void SetCustomNotification_WithoutIcon_SetsValues()
    {
        // Arrange
        StyleKeyword expectedStyle = StyleKeyword.Null;
        StyleColor expectedBackgroundColour = new StyleColor(Color.red);
        StyleColor expectedTextColour = new StyleColor(Color.green);

        //Act
        notification.SetCustomNotification(Color.red, Color.green);
        notification.HandleDisplayUI(NotificationType.Custom, "Test Message");

        //Assert
        Assert.AreEqual(expectedStyle.ToString(), notifDoc.rootVisualElement.Q<VisualElement>("Icon").style.backgroundImage.ToString());
        Assert.AreEqual(expectedBackgroundColour, notifDoc.rootVisualElement.Q<VisualElement>("Container").style.backgroundColor);
        Assert.AreEqual(expectedTextColour, notifDoc.rootVisualElement.Q<Label>("Text").style.color);
    }

    [Test, Order(14)]
    [Category("BuildServer")]
    public void ClearClasses_WithSuccess_RemovesClass()
    {
        // Arrange
        string disabledClass = "notification-success";

        //Act
        notification.HandleDisplayUI(NotificationType.Success, "Success Message");
        notification.ClearClasses();

        //Assert
        Assert.IsFalse(notifDoc.rootVisualElement.Q<VisualElement>("Container").ClassListContains(disabledClass));
    }

    [Test, Order(15)]
    [Category("BuildServer")]
    public void ClearClasses_WithInfo_RemovesClass()
    {
        // Arrange
        string disabledClass = "notification-info";

        //Act
        notification.HandleDisplayUI(NotificationType.Info, "Info Message");
        notification.ClearClasses();

        //Assert
        Assert.IsFalse(notifDoc.rootVisualElement.Q<VisualElement>("Container").ClassListContains(disabledClass));
    }

    [Test, Order(16)]
    [Category("BuildServer")]
    public void ClearClasses_WithError_RemovesClass()
    {
        // Arrange
        string disabledClass = "notification-error";

        //Act
        notification.HandleDisplayUI(NotificationType.Error, "Error Message");
        notification.ClearClasses();

        //Assert
        Assert.IsFalse(notifDoc.rootVisualElement.Q<VisualElement>("Container").ClassListContains(disabledClass));
    }

    private void Ping()
    {
        Debug.Log("Event triggered!");
    }
}