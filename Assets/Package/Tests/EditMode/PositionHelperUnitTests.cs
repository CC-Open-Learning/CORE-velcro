using NUnit.Framework;
using UnityEngine.TestTools;
using UnityEngine;
using UnityEngine.UIElements;
using VARLab.Velcro;

public class PositionHelperUnitTests
{
    VisualElement validElement;
    VisualElement invalidElement;

    [SetUp]
    public void SetUp()
    {
        validElement = new VisualElement();
        invalidElement = null;
    }

    [Test, Order(1)]
    public void SetAbsoluteVerticalPosition_WithFlexStart_ShouldSetTopToZero()
    {
        //Arrange
        Length expectedTop = new Length(0);
        StyleKeyword expectedBottom = StyleKeyword.Auto;

        //Act
        PositionHelper.SetAbsoluteVerticalPosition(validElement, Align.FlexStart);

        //Assert
        Assert.AreEqual(expectedTop, validElement.style.top.value);
        Assert.AreEqual(expectedBottom, validElement.style.bottom.keyword);
    }

    [Test, Order(2)]
    public void SetAbsoluteVerticalPosition_WithFlexEnd_ShouldSetTopToZero()
    {
        //Arrange
        Length expectedBottom = new Length(0);
        StyleKeyword expectedTop = StyleKeyword.Auto;

        //Act
        PositionHelper.SetAbsoluteVerticalPosition(validElement, Align.FlexEnd);

        //Assert
        Assert.AreEqual(expectedTop, validElement.style.top.keyword);
        Assert.AreEqual(expectedBottom, validElement.style.bottom.value);
    }

    [Test, Order(3)]
    public void SetAbsoluteVerticalPosition_WithCenter_ShouldCenterElement()
    {
        //Arrange
        Length expectedTopBottom = Length.Percent(50);

        //Act
        PositionHelper.SetAbsoluteVerticalPosition(validElement, Align.Center);

        //Assert
        Assert.AreEqual(expectedTopBottom, validElement.style.top.value);
        Assert.AreEqual(expectedTopBottom, validElement.style.bottom.value);
    }

    [Test, Order(8)]
    public void SetAbsoluteVerticalPosition_InvalidElement_ShouldSetTopToZero()
    {
        //Act
        PositionHelper.SetAbsoluteVerticalPosition(invalidElement, Align.Center);

        //Assert
        LogAssert.Expect(LogType.Error, "PositionHelper.SetAbsoluteVerticalPosition() - Incoming VisualElement Is Null!");
    }
}