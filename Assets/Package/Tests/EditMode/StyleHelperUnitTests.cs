using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.UIElements;
using VARLab.Velcro;

public class StyleHelperUnitTests
{
    [Test, Order(1)]
    public void ToggleTransitionProperties_WithoutValidVisualElement_LogsError()
    {
        //Arrange
        VisualElement element = null;
        string expectedError = "StyleHelper.ToggleTransitionProperties() - Incoming VisualElement is Null!";

        //Act
        StyleHelper.ToggleTransitionProperties(element, 0.0f, 0.0f);

        //Assert
        LogAssert.Expect(LogType.Error, expectedError);
    }

    [Test, Order(2)]
    public void ToggleTransitionProperties_SetsOpacityAndTranslate()
    {
        //Arrange
        VisualElement element = new VisualElement();
        StyleFloat expectedOpacity = new StyleFloat(1.0f);
        StyleTranslate expectedTranslate = new Translate(new Length(0.0f), new Length(0.0f));

        //Act
        StyleHelper.ToggleTransitionProperties(element, 1.0f, 0.0f);

        //Assert
        Assert.AreEqual(expectedOpacity, element.style.opacity);
        Assert.AreEqual(expectedTranslate, element.style.translate);
    }
}