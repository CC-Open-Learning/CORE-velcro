using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.UIElements;
using VARLab.Velcro;

public class TextElementExtensionsUnitTests
{
    Button validButton;
    Button invalidButton;
    Label validLabel;
    Label invalidLabel;

    [SetUp]
    public void SetUp()
    {
        validButton = new Button();
        validLabel = new Label();
        invalidButton = null;
        invalidLabel = null;
    }

    [Test, Order(1)]
    public void SetElementText_WithValidButton_ChangesButtonText()
    {
        //Arrange
        string expectedText = "Edit mode label testing";

        //Act
        validButton.SetElementText("Edit mode label testing");

        //Assert
        Assert.AreEqual(expectedText, validButton.text);
    }

    [Test, Order(2)]
    public void SetElementText_WithValidLabel_ChangesLabelText()
    {
        //Arrange
        string expectedText = "Edit mode label testing";

        //Act
        validLabel.SetElementText("Edit mode label testing");

        //Assert
        Assert.AreEqual(expectedText, validLabel.text);
    }

    [Test, Order(3)]
    public void SetElementText_WithInvalidButton_LogsErrorToConsole()
    {
        //Arrange
        string expectedLogMessage = "TextElementExtensions.SetElementText() - Incoming TextElement is Null!";

        //Act
        invalidButton.SetElementText("Edit mode label testing");

        //Assert
        LogAssert.Expect(LogType.Error, expectedLogMessage);
    }

    [Test, Order(4)]
    public void SetElementText_WithInvalidLabel_LogsErrorToConsole()
    {
        //Arrange
        string expectedLogMessage = "TextElementExtensions.SetElementText() - Incoming TextElement is Null!";

        //Act
        invalidLabel.SetElementText("Edit mode label testing");

        //Assert
        LogAssert.Expect(LogType.Error, expectedLogMessage);
    }

    [Test, Order(5)]
    public void SetElementText_WithEmptyString_SetsLabelToNone()
    {
        //Arrange
        StyleEnum<DisplayStyle> expectedStyle = new StyleEnum<DisplayStyle>(DisplayStyle.None);

        //Act
        SetStyleToFlex(validLabel);
        validLabel.SetElementText("");

        //Assert
        Assert.AreEqual(expectedStyle, validLabel.style.display);
    }

    [Test, Order(6)]
    public void SetElementText_WithEmptyString_SetsButtonToNone()
    {
        //Arrange
        StyleEnum<DisplayStyle> expectedStyle = new StyleEnum<DisplayStyle>(DisplayStyle.None);

        //Act
        SetStyleToFlex(validButton);
        validButton.SetElementText("");

        //Assert
        Assert.AreEqual(expectedStyle, validButton.style.display);
    }

    [Test, Order(7)]
    public void SetElementFontSize_WithLabelMediumFontSize_ShouldSetFontSize()
    {
        //Arrange
        StyleLength expectedSize = new Length(24);

        //Act
        validLabel.SetElementFontSize(FontSize.Medium);

        //Assert
        Assert.AreEqual(expectedSize, validLabel.style.fontSize);
    }

    [Test, Order(8)]
    public void SetElementFontSize_WithButtonMediumFontSize_ShouldSetFontSize()
    {
        //Arrange
        StyleLength expectedSize = new Length(24);

        //Act
        validButton.SetElementFontSize(FontSize.Medium);

        //Assert
        Assert.AreEqual(expectedSize, validButton.style.fontSize);
    }

    [Test, Order(9)]
    public void SetElementFontSize_WithLabelLargeFontSize_ShouldSetFontSize()
    {
        //Arrange
        StyleLength expectedSize = new Length(28);

        //Act
        validLabel.SetElementFontSize(FontSize.Large);

        //Assert
        Assert.AreEqual(expectedSize, validLabel.style.fontSize);
    }

    [Test, Order(10)]
    public void SetElementFontSize_WithButtonLargeFontSize_ShouldSetFontSize()
    {
        //Arrange
        StyleLength expectedSize = new Length(28);

        //Act
        validButton.SetElementFontSize(FontSize.Large);

        //Assert
        Assert.AreEqual(expectedSize, validButton.style.fontSize);
    }

    [Test, Order(11)]
    public void SetElementFontSize_WithInvalidFontSize_ShouldLogWarning()
    {
        //Arrange
        string expectedWarning = "TextElementExtensions.SetElementFontSize() - Specified Font Size is not a Valid Enum Value";

        //Act
        validLabel.SetElementFontSize((FontSize)99);

        //Assert
        LogAssert.Expect(LogType.Warning, expectedWarning);
    }

    [Test, Order(12)]
    public void SetElementFontSize_WithInvalidLabel_LogsError()
    {
        //Arrange
        string expectedError = "TextElementExtensions.SetElementFontSize() - Incoming TextElement is Null!";

        //Act
        invalidLabel.SetElementFontSize(FontSize.Large);

        //Assert
        LogAssert.Expect(LogType.Error, expectedError);
    }

    [Test, Order(13)]
    public void SetElementFontSize_WithInvalidButton_LogsError()
    {
        //Arrange
        string expectedError = "TextElementExtensions.SetElementFontSize() - Incoming TextElement is Null!";

        //Act
        invalidButton.SetElementFontSize(FontSize.Large);

        //Assert
        LogAssert.Expect(LogType.Error, expectedError);
    }

    [Test, Order(14)]
    public void SetElementColour_WithValidTextElement_SetsColour()
    {
        //Arrange
        StyleColor expectedColour = Color.red;

        //Act
        validLabel.SetElementColour(Color.red);

        //Assert
        Assert.AreEqual(expectedColour, validLabel.style.color);
    }

    [Test, Order(15)]
    public void SetElementColour_WithInvalidTextElement_LogsError()
    {
        //Arrange
        string expectedError = "TextElementExtensions.SetElementColour() - Incoming TextElement is Null!";

        //Act
        invalidLabel.SetElementColour(Color.red);

        //Assert
        LogAssert.Expect(LogType.Error, expectedError);
    }

    private void SetStyleToFlex(VisualElement visualElement)
    {
        visualElement.style.display = DisplayStyle.Flex;
    }
}