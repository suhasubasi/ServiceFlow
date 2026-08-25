using System.Globalization;

namespace ServiceFlow.Core.ValueObjects;

public readonly record struct Money(decimal Amount, string Currency = "SEK")
{
    public override string ToString() =>
        $"{Amount.ToString("0.00", CultureInfo.InvariantCulture)} {Currency}";
}
