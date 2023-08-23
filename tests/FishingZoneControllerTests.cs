using Xunit;

namespace FishingGame.Tests;

public class FishingZoneControllerTests
{
    [Theory]
    [InlineData(0, 24, 0, true)] // 00:00 ~ 00:00, 00:00
    [InlineData(0, 24, 12, true)] // 00:00 ~ 00:00, 12:00
    [InlineData(0, 24, 23.9993, true)] // 00:00 ~ 00:00, 23:59
    [InlineData(1, 1, 1.5, true)] // 01:00 ~ 02:00, 01:30
    [InlineData(1, 1, 0, false)] // 01:00 ~ 02:00, 00:00
    [InlineData(22, 4, 1, true)] // 22:00 ~ 02:00, 01:00
    [InlineData(23, 2, 0, true)] // 23:00 ~ 01:00, 00:00
    [InlineData(23, 2, 1.5, false)] // 23:00 ~ 01:00, 01:30
    [InlineData(23, 2, 1, false)] // 23:00 ~ 01:00, 01:00
    [InlineData(23, 1, 0, false)] // 23:00 ~ 00:00, 00:00 
    [InlineData(18, 12, 18, true)] // 18:00 ~ 06:00, 18:00
    [InlineData(18, 12, 6, false)] // 18:00 ~ 06:00, 06:00
    [InlineData(6, 12, 6, true)] // 06:00 ~ 18:00, 06:00
    [InlineData(6, 12, 18, false)] // 06:00 ~ 18:00, 18:00
    public void FishIsInAvailableTimeframe_EvaluatesCorrectly(int availableStartHour, int availableForHours, float currentStandardTime, bool expected)
    {
        float actualTime = currentStandardTime / 24f;
        bool actual = FishZoneController.FishIsInAvailableTimeframe(availableStartHour, availableForHours, actualTime);
        
        Assert.Equal(expected, actual);
    }
}