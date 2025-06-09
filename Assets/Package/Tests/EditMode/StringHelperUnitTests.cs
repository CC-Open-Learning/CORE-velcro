using NUnit.Framework;
using VARLab.Velcro;

public class StringHelperUnitTests
{
    [Test, Order(1)]
    public void FormatTime_With_HHMMSS_Format()
    {
        // Arrange
        string expectedTime = "01:23:20";

        // Assert
        Assert.AreEqual(expectedTime, StringHelper.FormatTime(5000d, "HH:mm:ss"));
    }

    [Test, Order(2)]
    public void FormatTime_With_HMM_Format()
    {
        // Arrange
        string expectedTime = "1:38";

        // Assert
        Assert.AreEqual(expectedTime, StringHelper.FormatTime(5882d, "H:mm"));
    }
}