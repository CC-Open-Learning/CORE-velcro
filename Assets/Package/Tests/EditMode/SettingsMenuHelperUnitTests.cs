using NUnit.Framework;
using System;
using VARLab.Velcro;

public class SettingsMenuHelperUnitTests
{
    [Test, Order(1)]
    public void ConvertLinearVolumeToLog_WithMinimumValue_ShouldReturn80()
    {
        //Arrange
        float expectedValue = -80.0f;

        //Act
        float actualValue = SettingsMenuHelper.ConvertLinearVolumeToLog(0.0001f);

        //Assert
        Assert.AreEqual(expectedValue, actualValue);
    }

    [Test, Order(2)]
    public void ConvertLinearVolumeToLog_WithMaximumValue_ShouldReturn0()
    {
        //Arrange
        float expectedValue = 0.0f;

        //Act
        float actualValue = SettingsMenuHelper.ConvertLinearVolumeToLog(1.0f);

        //Assert
        Assert.AreEqual(expectedValue, actualValue);
    }

    [Test, Order(3)]
    public void ConvertLogVolumeToLinear_WithHalfValue_ShouldReturnHalfSlider()
    {
        //Arrange
        float expectedValue = 0.5f;

        //Act
        float actualValue = SettingsMenuHelper.ConvertLogVolumeToLinear(-6.0f);

        //Assert
        Assert.AreEqual(expectedValue, Math.Round(actualValue, 1));
    }

    [Test, Order(4)]
    public void ConvertLogVolumeToLinear_WithMaximumValue_ShouldReturn0()
    {
        //Arrange
        float expectedValue = 1.0f;

        //Act
        float actualValue = SettingsMenuHelper.ConvertLogVolumeToLinear(0.0001f);

        //Assert
        Assert.AreEqual(expectedValue, Math.Round(actualValue, 1));
    }

    [Test, Order(5)]
    public void ConvertLogVolumeToLinear_WithMinimumValue_ShouldReturn80()
    {
        //Arrange
        float expectedValue = 0.0001f;

        //Act
        float actualValue = SettingsMenuHelper.ConvertLogVolumeToLinear(-80.0f);

        //Assert
        Assert.AreEqual(expectedValue, actualValue);
    }
}