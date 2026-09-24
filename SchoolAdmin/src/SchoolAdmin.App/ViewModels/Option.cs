namespace SchoolAdmin.App.ViewModels;

/// <summary>An item for a drop-down: the stored value and the text shown to the user.</summary>
public record Option<T>(T Value, string Label);

public static class DateConversions
{
    public static DateTime? ToDateTime(this DateOnly? date) => date?.ToDateTime(TimeOnly.MinValue);

    public static DateTime ToDateTime(this DateOnly date) => date.ToDateTime(TimeOnly.MinValue);

    public static DateOnly? ToDateOnly(this DateTime? date) => date is null ? null : DateOnly.FromDateTime(date.Value);
}
