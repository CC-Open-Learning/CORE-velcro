using UnityEngine;
using NUnit.Framework;
using UnityEngine.UIElements;
using VARLab.Velcro;
using UnityEngine.TestTools;
using UnityEditor;

public class VisualElementExtensionsUnitTests
{
    VisualElement validElement;
    VisualElement invalidElement;
    Sprite sprite;

    [SetUp]
    public void SetUp()
    {
        sprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/VELCRO UI/Sprites/Arrows/ArrowBack_Sprite.png");
        validElement = new VisualElement();
        invalidElement = null;
    }

    [Test, Order(1)]
    public void Show_WithValidVisualElement_SetsElementToFlex()
    {
        //Arrange
        StyleEnum<DisplayStyle> expectedStyle = new StyleEnum<DisplayStyle>(DisplayStyle.Flex);

        //Act
        SetStyleToNone(validElement);
        validElement.Show();

        //Assert
        Assert.AreEqual(expectedStyle, validElement.style.display);
    }

    [Test, Order(2)]
    public void Hide_WithValidVisualElement_SetsElementToNone() 
    {
        //Arrange
        StyleEnum<DisplayStyle> expectedStyle = new StyleEnum<DisplayStyle>(DisplayStyle.None);

        //Act
        SetStyleToFlex(validElement);
        validElement.Hide(              );

        //Assert
        Assert.AreEqual(expectedStyle, validElement.style.display);
    }

    [Test, Order(3)]
    public void Show_WithInvalidVisualElement_LogsErrorToConsole()
    {
        //Arrange
        string expectedLogMessage = "VisualElementExtensions.Show() - Incoming VisualElement Is Null!";

        //Act
        invalidElement.Show();

        //Assert
        LogAssert.Expect(LogType.Error, expectedLogMessage);
    }

    [Test, Order(4)]
    public void Hide_WithInvalidVisualElement_LogsErrorToConsole() 
    {
        //Arrange
        string expectedLogMessage = "VisualElementExtensions.Hide() - Incoming VisualElement Is Null!";

        //Act
        invalidElement.Hide();

        //Assert
        LogAssert.Expect(LogType.Error, expectedLogMessage);
    }

    [Test, Order(5)]
    public void SetElementSprite_WithValidVisualElement_SetsBackgroundImage()
    {
        //Arrange
        StyleBackground expectedSprite = new StyleBackground(sprite);

        //Act
        validElement.SetElementSprite(sprite);

        //Assert
        Assert.AreEqual(expectedSprite, validElement.style.backgroundImage);
    }

    [Test, Order(6)]
    public void SetElementSprite_WithInvalidVisualElement_LogsError()
    {
        //Arrange
        string expectedLogMessage = "VisualElementExtensions.SetElementSprite() - Incoming VisualElement is Null!";

        //Act
        invalidElement.SetElementSprite(sprite);

        //Assert
        LogAssert.Expect(LogType.Error, expectedLogMessage);
    }

    [Test, Order(7)]
    public void SetElementSprite_WithNullSprite_SetsElementToNone()
    {
        //Arrange
        StyleEnum<DisplayStyle> expectedStyle = new StyleEnum<DisplayStyle>(DisplayStyle.None);

        //Act
        SetStyleToFlex(validElement);
        validElement.SetElementSprite(null);

        //Assert
        Assert.AreEqual(expectedStyle, validElement.style.display);
    }

    [Test, Order(8)]
    public void SetBackgroundColour_WithValidVisualElement_SetsBackgroundColour()
    {
        //Arrange
        StyleColor expectedBackgroundColour = Color.red;

        //Act
        validElement.SetBackgroundColour(Color.red);

        //Assert
        Assert.AreEqual(expectedBackgroundColour, validElement.style.backgroundColor);
    }

    [Test, Order(9)]
    public void SetBackgroundColour_WithInvalidVisualElement_LogsError()
    {
        //Arrange
        string expectedError = "VisualElementExtensions.SetBackgroundColour() - Incoming VisualElement is Null!";

        //Act
        invalidElement.SetBackgroundColour(Color.red);

        //Assert
        LogAssert.Expect(LogType.Error, expectedError);
    }

    [Test, Order(10)]
    public void SetBorderColour_WithValidVisualElement_SetsBorderColour()
    {
        //Arrange
        StyleColor expectedBorderColour = Color.blue;

        //Act
        validElement.SetBorderColour(Color.blue);

        //Assert
        Assert.AreEqual(expectedBorderColour, validElement.style.borderBottomColor);
        Assert.AreEqual(expectedBorderColour, validElement.style.borderTopColor);
        Assert.AreEqual(expectedBorderColour, validElement.style.borderLeftColor);
        Assert.AreEqual(expectedBorderColour, validElement.style.borderRightColor);
    }

    [Test, Order(11)]
    public void SetBorderColour_WithInvalidVisualElement_LogsError()
    {
        //Arrange
        string expectedError = "VisualElementExtensions.SetBorderColour() - Incoming VisualElement is Null!";

        //Act
        invalidElement.SetBorderColour(Color.blue);

        //Assert
        LogAssert.Expect(LogType.Error, expectedError);
    }

    private void SetStyleToNone(VisualElement visualElement)
    {
        visualElement.style.display = DisplayStyle.None;
    }

    private void SetStyleToFlex(VisualElement visualElement)
    {
        visualElement.style.display = DisplayStyle.Flex;
    }
}