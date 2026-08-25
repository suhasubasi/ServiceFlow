using ServiceFlow.Core.ValueObjects;

namespace ServiceFlow.Core.Tests;

public class MoneyTests
{
    [Fact]
    public void TwoMoneyObjects_WithSameAmountAndCurrency_AreEqual()
    {
        // Arrange
        var m1 = new Money(1500m, "SEK");
        var m2 = new Money(1500m, "SEK");

        // Assert (record structs compare values, not memory addresses)
        Assert.Equal(m1, m2);
    }

    [Fact]
    public void ToString_ShouldFormatAmountAndCurrency()
    {
        // Arrange
        var money = new Money(250.5m, "SEK");

        // Act
        var result = money.ToString();

        // Assert
        Assert.Contains("250.50", result);
        Assert.Contains("SEK", result);
    }
}
