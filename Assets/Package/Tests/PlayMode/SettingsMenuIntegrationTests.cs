using System.Collections;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.UIElements;
using UnityEditor;
using NUnit.Framework;
using VARLab.Velcro;

public class SettingsMenuIntegrationTests 
{
    private int sceneCounter = 0;

    private GameObject simpleMenuObj;
    private GameObject complexMenuObj;
    private UIDocument simpleMenuDoc;
    private UIDocument complexMenuDoc;
    private SettingsMenuComplex complexMenu;
    private SettingsMenuSimple simpleMenu;

    [UnitySetUp]
    [Category("BuildServer")]
    public IEnumerator SetUp()
    {
        sceneCounter = TestUtils.ClearScene(sceneCounter, "SettingsMenuScene");

        //Set up <SettingsMenu> UI
        simpleMenuObj = new GameObject("Simple Menu");
        complexMenuObj = new GameObject("Complex Complex");
        simpleMenuDoc = simpleMenuObj.AddComponent<UIDocument>();
        complexMenuDoc = complexMenuObj.AddComponent<UIDocument>();
        simpleMenu = simpleMenuObj.AddComponent<SettingsMenuSimple>();
        complexMenu = complexMenuObj.AddComponent<SettingsMenuComplex>();

        //Load required assets from project files
        VisualTreeAsset simpleMenuUXML = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/VELCRO UI/UI/Settings Menus/SettingsMenuSimple.uxml");
        VisualTreeAsset complexMenuUXML = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/VELCRO UI/UI/Settings Menus/SettingsMenuComplex.uxml");
        PanelSettings panelSettings = AssetDatabase.LoadAssetAtPath<PanelSettings>("Assets/VELCRO UI/Settings/Panel Settings.asset");

        //Reference panel settings and source asset as SerializedFields
        SerializedObject so = new SerializedObject(simpleMenuDoc);
        so.FindProperty("m_PanelSettings").objectReferenceValue = panelSettings;
        so.FindProperty("sourceAsset").objectReferenceValue = simpleMenuUXML;
        so.ApplyModifiedProperties();

        SerializedObject so2 = new SerializedObject(complexMenuDoc);
        so2.FindProperty("m_PanelSettings").objectReferenceValue = panelSettings;
        so2.FindProperty("sourceAsset").objectReferenceValue = complexMenuUXML;
        so2.ApplyModifiedProperties();

        simpleMenuUXML.CloneTree(simpleMenuDoc.rootVisualElement);
        complexMenuUXML.CloneTree(complexMenuDoc.rootVisualElement);
        yield return null;
    }

    [Test, Order(1)]
    [Category("BuildServer")]
    public void Start_SimpleRoot_SetsDisplayNone()
    {
        //Assert
        Assert.AreEqual(new StyleEnum<DisplayStyle>(DisplayStyle.None), simpleMenuDoc.rootVisualElement.style.display);
    }

    [Test, Order(2)]
    [Category("BuildServer")]
    public void Start_ComplexRoot_SetsDisplayNone()
    {
        //Assert
        Assert.AreEqual(new StyleEnum<DisplayStyle>(DisplayStyle.None), complexMenuDoc.rootVisualElement.style.display);
    }

    [Test, Order(3)]
    [Category("BuildServer")]
    public void Show_SimpleRoot_InvokesOnSettingsMenuShown()
    {
        //Arrange
        simpleMenu.OnSettingsMenuShown.AddListener(Ping);

        //Act
        simpleMenu.Show();

        //Assert
        LogAssert.Expect(LogType.Log, "Event triggered!");

        //Clean up
        simpleMenu.OnSettingsMenuShown.RemoveListener(Ping);
    }

    [Test, Order(4)]
    [Category("BuildServer")]
    public void Show_SimpleRoot_SetsDisplayFlex()
    {
        //Act
        simpleMenu.Show();

        //Assert
        Assert.AreEqual(new StyleEnum<DisplayStyle>(DisplayStyle.Flex), simpleMenu.Root.style.display);
    }

    [Test, Order(4)]
    [Category("BuildServer")]
    public void Hide_SimpleRoot_InvokesOnSettingsMenuHidden()
    {
        //Arrange
        simpleMenu.OnSettingsMenuHidden.AddListener(Ping);

        //Act
        simpleMenu.Hide();

        //Assert
        LogAssert.Expect(LogType.Log, "Event triggered!");

        //Clean up
        simpleMenu.OnSettingsMenuHidden.RemoveListener(Ping);
    }

    [Test, Order(5)]
    [Category("BuildServer")]
    public void Hide_SimpleRoot_SetsDisplayNone()
    {
        //Act
        simpleMenu.Hide();

        //Assert
        Assert.AreEqual(new StyleEnum<DisplayStyle>(DisplayStyle.None), simpleMenu.Root.style.display);
    }

    [Test, Order(6)]
    [Category("BuildServer")]
    public void Show_ComplexRoot_InvokesOnSettingsMenuShown()
    {
        //Arrange
        complexMenu.OnSettingsMenuShown.AddListener(Ping);

        //Act
        complexMenu.Show();

        //Assert
        LogAssert.Expect(LogType.Log, "Event triggered!");

        //Clean up
        complexMenu.OnSettingsMenuShown.RemoveListener(Ping);
    }

    [Test, Order(7)]
    [Category("BuildServer")]
    public void Show_ComplexRoot_SetsDisplayFlex()
    {
        //Act
        complexMenu.Show();

        //Assert
        Assert.AreEqual(new StyleEnum<DisplayStyle>(DisplayStyle.Flex), complexMenu.Root.style.display);
    }

    [Test, Order(8)]
    [Category("BuildServer")]
    public void Hide_ComplexRoot_InvokesOnSettingsMenuHidden()
    {
        //Arrange
        complexMenu.OnSettingsMenuHidden.AddListener(Ping);

        //Act
        complexMenu.Hide();

        //Assert
        LogAssert.Expect(LogType.Log, "Event triggered!");

        //Clean up
        complexMenu.OnSettingsMenuHidden.RemoveListener(Ping);
    }

    [Test, Order(9)]
    [Category("BuildServer")]
    public void Hide_ComplexRoot_SetsDisplayNone()
    {
        //Act
        complexMenu.Hide();

        //Assert
        Assert.AreEqual(new StyleEnum<DisplayStyle>(DisplayStyle.None), complexMenu.Root.style.display);
    }

    [Test, Order(10)]
    [Category("BuildServer")]
    public void VolumeSliderCallback_SimpleRoot_InvokesOnVolumeSliderChanged()
    {
        //Arrange
        simpleMenu.OnVolumeSliderChanged.AddListener(PingVolumeSlider);
        float testVal = 0.9f;

        //Act
        simpleMenu.Root.Q<TemplateContainer>("VolumeSlider").Q<FillSlider>().value = testVal;

        //Assert
        LogAssert.Expect(LogType.Log, "String/Float Event triggered!");

        //Clean Up
        simpleMenu.OnVolumeSliderChanged.RemoveListener(PingVolumeSlider);
    }

    [Test, Order(11)]
    [Category("BuildServer")]
    public void CameraSensitivitySliderCallback_SimpleRoot_InvokesOnCameraSliderChanged()
    {
        //Arrange
        simpleMenu.OnCameraSliderChanged.AddListener(PingSlider);
        float testVal = 0.9f;

        //Act
        simpleMenu.Root.Q<TemplateContainer>("SensitivitySlider").Q<FillSlider>().value = testVal;

        //Assert
        LogAssert.Expect(LogType.Log, "Float Event triggered!");

        //Clean Up
        simpleMenu.OnCameraSliderChanged.RemoveListener(PingSlider);
    }

    [Test, Order(12)]
    [Category("BuildServer")]
    public void VolumeSliderCallback_ComplexRoot_InvokesOnVolumeSliderChanged()
    {
        //Arrange
        complexMenu.OnVolumeSliderChanged.AddListener(PingVolumeSlider);
        float testVal = 0.9f;

        //Act
        complexMenu.Root.Q<TemplateContainer>("VolumeSlider").Q<FillSlider>().value = testVal;

        //Assert
        LogAssert.Expect(LogType.Log, "String/Float Event triggered!");

        //Clean Up
        complexMenu.OnVolumeSliderChanged.RemoveListener(PingVolumeSlider);
    }

    [Test, Order(13)]
    [Category("BuildServer")]
    public void CameraSensitivitySliderCallback_ComplexRoot_InvokesOnCameraSliderChanged()
    {
        //Arrange
        complexMenu.OnCameraSliderChanged.AddListener(PingSlider);
        float testVal = 0.9f;

        //Act
        complexMenu.Root.Q<TemplateContainer>("SensitivitySlider").Q<FillSlider>().value = testVal;

        //Assert
        LogAssert.Expect(LogType.Log, "Float Event triggered!");

        //Clean Up
        complexMenu.OnCameraSliderChanged.RemoveListener(PingSlider);
    }

    [Test, Order(14)]
    [Category("BuildServer")]
    public void SoundEffectSliderCallback_ComplexRoot_InvokesOnSoundEffectsSliderChanged()
    {
        //Arrange
        complexMenu.OnSoundEffectsSliderChanged.AddListener(PingVolumeSlider);
        float testVal = 0.9f;

        //Act
        complexMenu.Root.Q<TemplateContainer>("SoundEffectSlider").Q<FillSlider>().value = testVal;

        //Assert
        LogAssert.Expect(LogType.Log, "String/Float Event triggered!");

        //Clean Up
        complexMenu.OnSoundEffectsSliderChanged.RemoveListener(PingVolumeSlider);
    }

    [Test, Order(15)]
    [Category("BuildServer")]
    public void DialogueSliderCallback_ComplexRoot_InvokesOnDialogueSliderChanged()
    {
        //Arrange
        complexMenu.OnDialogueSliderChanged.AddListener(PingVolumeSlider);
        float testVal = 0.9f;

        //Act
        complexMenu.Root.Q<TemplateContainer>("DialogueSlider").Q<FillSlider>().value = testVal;

        //Assert
        LogAssert.Expect(LogType.Log, "String/Float Event triggered!");

        //Clean Up
        complexMenu.OnDialogueSliderChanged.RemoveListener(PingVolumeSlider);
    }

    [Test, Order(16)]
    [Category("BuildServer")] 
    public void ThemeToggleCallback_SimpleRoot_InvokesOnThemeTogglePressed()
    {
        //Arrange
        simpleMenu.OnThemeTogglePressed.AddListener(PingToggle);
        bool testVal = true;

        //Act
        simpleMenu.Root.Q<TemplateContainer>("ThemeToggle").Q<SlideToggle>().value = testVal;

        //Assert
        LogAssert.Expect(LogType.Log, "Bool Event triggered!");

        //Clean Up
        simpleMenu.OnThemeTogglePressed.RemoveListener(PingToggle);
    }

    [Test, Order(17)]
    [Category("BuildServer")]
    public void SoundToggleCallback_SimpleRoot_InvokesOnSoundTogglePressed()
    {
        //Arrange
        simpleMenu.OnSoundTogglePressed.AddListener(PingToggle);
        bool testVal = true;

        //Act
        simpleMenu.Root.Q<TemplateContainer>("SoundToggle").Q<SlideToggle>().value = testVal;

        //Assert
        LogAssert.Expect(LogType.Log, "Bool Event triggered!");

        //Clean Up
        simpleMenu.OnSoundTogglePressed.RemoveListener(PingToggle);
    }

    [Test, Order(18)]
    [Category("BuildServer")]
    public void ThemeToggleCallback_ComplexRoot_InvokesOnThemeTogglePressed()
    {
        //Arrange
        complexMenu.OnThemeTogglePressed.AddListener(PingToggle);
        bool testVal = true;

        //Act
        complexMenu.Root.Q<TemplateContainer>("ThemeToggle").Q<SlideToggle>().value = testVal;

        //Assert
        LogAssert.Expect(LogType.Log, "Bool Event triggered!");

        //Clean Up
        complexMenu.OnThemeTogglePressed.RemoveListener(PingToggle);
    }

    [Test, Order(19)]
    [Category("BuildServer")]
    public void SoundToggleCallback_ComplexRoot_InvokesOnSoundTogglePressed()
    {
        //Arrange
        complexMenu.OnSoundTogglePressed.AddListener(PingToggle);
        bool testVal = true;

        //Act
        complexMenu.Root.Q<TemplateContainer>("SoundToggle").Q<SlideToggle>().value = testVal;

        //Assert
        LogAssert.Expect(LogType.Log, "Bool Event triggered!");

        //Clean Up
        complexMenu.OnSoundTogglePressed.RemoveListener(PingToggle);
    }

    [Test, Order(20)]
    [Category("BuildServer")]
    public void VolumeSliderCallback_Simple_SetValue_InvokesOnVolumeSliderChanged()
    {
        //Arrange
        simpleMenu.OnVolumeSliderChanged.AddListener(PingVolumeSlider);
        float testVal = 0.9f;

        //Act
        simpleMenu.SetVolumeSlider(testVal);

        //Assert
        LogAssert.Expect(LogType.Log, "String/Float Event triggered!");

        //Clean Up
        simpleMenu.OnVolumeSliderChanged.RemoveListener(PingVolumeSlider);
    }

    [Test, Order(21)]
    [Category("BuildServer")]
    public void VolumeSliderCallback_Simple_SetValue_WithoutNotify()
    {
        //Arrange
        simpleMenu.OnVolumeSliderChanged.AddListener(PingVolumeSlider);
        float testVal = 0.9f;

        //Act
        simpleMenu.SetVolumeSlider(testVal, notify: false);

        //Assert
        // Should not log any messages as the callback is not invoked
        LogAssert.NoUnexpectedReceived();

        //Clean Up
        simpleMenu.OnVolumeSliderChanged.RemoveListener(PingVolumeSlider);
    }

    [Test, Order(22)]
    [Category("BuildServer")]
    public void CameraSensitivitySliderCallback_Simple_SetValue_InvokesOnCameraSliderChanged()
    {
        //Arrange
        simpleMenu.OnCameraSliderChanged.AddListener(PingSlider);
        float testVal = 0.9f;

        //Act
        simpleMenu.SetCameraSlider(testVal);

        //Assert
        LogAssert.Expect(LogType.Log, "Float Event triggered!");

        //Clean Up
        simpleMenu.OnCameraSliderChanged.RemoveListener(PingSlider);
    }

    [Test, Order(22)]
    [Category("BuildServer")]
    public void CameraSensitivitySliderCallback_Simple_SetValue_WithoutNotify()
    {
        //Arrange
        simpleMenu.OnCameraSliderChanged.AddListener(PingSlider);
        float testVal = 0.9f;

        //Act
        simpleMenu.SetCameraSlider(testVal, false);

        //Assert
        LogAssert.NoUnexpectedReceived();

        //Clean Up
        simpleMenu.OnCameraSliderChanged.RemoveListener(PingSlider);
    }

    [Test, Order(23)]
    [Category("BuildServer")]
    public void VolumeSliderCallback_Complex_SetValue_InvokesOnVolumeSliderChanged()
    {
        //Arrange
        complexMenu.OnVolumeSliderChanged.AddListener(PingVolumeSlider);
        float testVal = 0.9f;

        //Act
        complexMenu.SetVolumeSlider(testVal);

        //Assert
        LogAssert.Expect(LogType.Log, "String/Float Event triggered!");

        //Clean Up
        complexMenu.OnVolumeSliderChanged.RemoveListener(PingVolumeSlider);
    }

    [Test, Order(24)]
    [Category("BuildServer")]
    public void VolumeSliderCallback_Complex_SetValue_WithoutNotify()
    {
        //Arrange
        complexMenu.OnVolumeSliderChanged.AddListener(PingVolumeSlider);
        float testVal = 0.9f;

        //Act
        complexMenu.SetVolumeSlider(testVal, false);

        //Assert
        LogAssert.NoUnexpectedReceived();

        //Clean Up
        complexMenu.OnVolumeSliderChanged.RemoveListener(PingVolumeSlider);
    }

    [Test, Order(25)]
    [Category("BuildServer")]
    public void CameraSensitivitySliderCallback_Complex_SetValue_InvokesOnCameraSliderChanged()
    {
        //Arrange
        complexMenu.OnCameraSliderChanged.AddListener(PingSlider);
        float testVal = 0.9f;

        //Act
        complexMenu.SetCameraSlider(testVal);

        //Assert
        LogAssert.Expect(LogType.Log, "Float Event triggered!");

        //Clean Up
        complexMenu.OnCameraSliderChanged.RemoveListener(PingSlider);
    }

    [Test, Order(26)]
    [Category("BuildServer")]
    public void CameraSensitivitySliderCallback_Complex_SetValue_WithoutNotify()
    {
        //Arrange
        complexMenu.OnCameraSliderChanged.AddListener(PingSlider);
        float testVal = 0.9f;

        //Act
        complexMenu.SetCameraSlider(testVal, false);

        //Assert
        LogAssert.NoUnexpectedReceived();

        //Clean Up
        complexMenu.OnCameraSliderChanged.RemoveListener(PingSlider);
    }

    [Test, Order(27)]
    [Category("BuildServer")]
    public void SoundEffectSliderCallback_Complex_SetValue_InvokesOnSoundEffectsSliderChanged()
    {
        //Arrange
        complexMenu.OnSoundEffectsSliderChanged.AddListener(PingVolumeSlider);
        float testVal = 0.9f;

        //Act
        complexMenu.SetSoundEffectsSlider(testVal);

        //Assert
        LogAssert.Expect(LogType.Log, "String/Float Event triggered!");

        //Clean Up
        complexMenu.OnSoundEffectsSliderChanged.RemoveListener(PingVolumeSlider);
    }

    [Test, Order(28)]
    [Category("BuildServer")]
    public void SoundEffectSliderCallback_Complex_SetValue_WithoutNotify()
    {
        //Arrange
        complexMenu.OnSoundEffectsSliderChanged.AddListener(PingVolumeSlider);
        float testVal = 0.9f;

        //Act
        complexMenu.SetSoundEffectsSlider(testVal, false);

        //Assert
        LogAssert.NoUnexpectedReceived();

        //Clean Up
        complexMenu.OnSoundEffectsSliderChanged.RemoveListener(PingVolumeSlider);
    }

    [Test, Order(29)]
    [Category("BuildServer")]
    public void DialogueSliderCallback_Complex_SetValue_InvokesOnDialogueSliderChanged()
    {
        //Arrange
        complexMenu.OnDialogueSliderChanged.AddListener(PingVolumeSlider);
        float testVal = 0.9f;

        //Act
        complexMenu.SetDialogueSlider(testVal);

        //Assert
        LogAssert.Expect(LogType.Log, "String/Float Event triggered!");

        //Clean Up
        complexMenu.OnDialogueSliderChanged.RemoveListener(PingVolumeSlider);
    }

    [Test, Order(30)]
    [Category("BuildServer")]
    public void DialogueSliderCallback_Complex_SetValue_WithoutNotify()
    {
        //Arrange
        complexMenu.OnDialogueSliderChanged.AddListener(PingVolumeSlider);
        float testVal = 0.9f;

        //Act
        complexMenu.SetDialogueSlider(testVal, false);

        //Assert
        LogAssert.NoUnexpectedReceived();

        //Clean Up
        complexMenu.OnDialogueSliderChanged.RemoveListener(PingVolumeSlider);
    }

    [Test, Order(31)]
    [Category("BuildServer")]
    public void ThemeToggleCallback_Simple_SetValue_InvokesOnThemeTogglePressed()
    {
        //Arrange
        simpleMenu.OnThemeTogglePressed.AddListener(PingToggle);
        bool testVal = true;

        //Act
        simpleMenu.SetThemeToggle(testVal);

        //Assert
        LogAssert.Expect(LogType.Log, "Bool Event triggered!");

        //Clean Up
        simpleMenu.OnThemeTogglePressed.RemoveListener(PingToggle);
    }

    [Test, Order(32)]
    [Category("BuildServer")]
    public void ThemeToggleCallback_Simple_SetValue_WithoutNotify()
    {
        //Arrange
        simpleMenu.OnThemeTogglePressed.AddListener(PingToggle);
        bool testVal = true;

        //Act
        simpleMenu.SetThemeToggle(testVal, false);

        //Assert
        LogAssert.NoUnexpectedReceived();

        //Clean Up
        simpleMenu.OnThemeTogglePressed.RemoveListener(PingToggle);
    }

    [Test, Order(33)]
    [Category("BuildServer")]
    public void SoundToggleCallback_Simple_SetValue_InvokesOnSoundTogglePressed()
    {
        //Arrange
        simpleMenu.OnSoundTogglePressed.AddListener(PingToggle);
        bool testVal = true;

        //Act
        simpleMenu.SetSoundToggle(testVal);

        //Assert
        LogAssert.Expect(LogType.Log, "Bool Event triggered!");

        //Clean Up
        simpleMenu.OnSoundTogglePressed.RemoveListener(PingToggle);
    }

    [Test, Order(34)]
    [Category("BuildServer")]
    public void SoundToggleCallback_Simple_SetValue_WithoutNotify()
    {
        //Arrange
        simpleMenu.OnSoundTogglePressed.AddListener(PingToggle);
        bool testVal = true;

        //Act
        simpleMenu.SetSoundToggle(testVal, false);

        //Assert
        LogAssert.NoUnexpectedReceived();

        //Clean Up
        simpleMenu.OnSoundTogglePressed.RemoveListener(PingToggle);
    }

    [Test, Order(35)]
    [Category("BuildServer")]
    public void ThemeToggleCallback_Complex_SetValue_InvokesOnThemeTogglePressed()
    {
        //Arrange
        complexMenu.OnThemeTogglePressed.AddListener(PingToggle);
        bool testVal = true;

        //Act
        complexMenu.SetThemeToggle(testVal);

        //Assert
        LogAssert.Expect(LogType.Log, "Bool Event triggered!");

        //Clean Up
        complexMenu.OnThemeTogglePressed.RemoveListener(PingToggle);
    }

    [Test, Order(35)]
    [Category("BuildServer")]
    public void ThemeToggleCallback_Complex_SetValue_WithoutNotify()
    {
        //Arrange
        complexMenu.OnThemeTogglePressed.AddListener(PingToggle);
        bool testVal = true;

        //Act
        complexMenu.SetThemeToggle(testVal, false);

        //Assert
        LogAssert.NoUnexpectedReceived();

        //Clean Up
        complexMenu.OnThemeTogglePressed.RemoveListener(PingToggle);
    }

    [Test, Order(36)]
    [Category("BuildServer")]
    public void SoundToggleCallback_Complex_SetValue_InvokesOnSoundTogglePressed()
    {
        //Arrange
        complexMenu.OnSoundTogglePressed.AddListener(PingToggle);
        bool testVal = true;

        //Act
        complexMenu.SetSoundToggle(testVal);

        //Assert
        LogAssert.Expect(LogType.Log, "Bool Event triggered!");

        //Clean Up
        complexMenu.OnSoundTogglePressed.RemoveListener(PingToggle);
    }

    [Test, Order(36)]
    [Category("BuildServer")]
    public void SoundToggleCallback_Complex_SetValue_WithoutNotify()
    {
        //Arrange
        complexMenu.OnSoundTogglePressed.AddListener(PingToggle);
        bool testVal = true;

        //Act
        complexMenu.SetSoundToggle(testVal, false);

        //Assert
        LogAssert.NoUnexpectedReceived();

        //Clean Up
        complexMenu.OnSoundTogglePressed.RemoveListener(PingToggle);
    }


    private void Ping()
    {
        Debug.Log("Event triggered!");
    }

    private void PingVolumeSlider(string tag, float val)
    {
        Debug.Log("String/Float Event triggered!");
    }

    private void PingSlider(float val)
    {
        Debug.Log("Float Event triggered!");
    }

    private void PingToggle(bool val)
    {
        Debug.Log("Bool Event triggered!");
    }
}