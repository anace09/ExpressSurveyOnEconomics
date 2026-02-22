namespace WebApp.Helpers;

public static class TimeFormatter
{
    public static string Format(int totalSeconds, string lang = "ru")
    {
        int minutes = totalSeconds / 60;
        int seconds = totalSeconds % 60;

        if (lang == "kk")
        {
            if (minutes == 0 && seconds == 0) return "0 сек";

            var parts = new List<string>();
            if (minutes > 0) parts.Add($"{minutes} минут");
            if (seconds > 0) parts.Add($"{seconds} секунд");

            return string.Join(" ", parts);
        }
        else // ru
        {
            if (minutes == 0 && seconds == 0) return "0 секунд";

            var parts = new List<string>();

            if (minutes > 0)
                parts.Add($"{minutes} {Pluralize(minutes, "минута", "минуты", "минут")}");

            if (seconds > 0)
                parts.Add($"{seconds} {Pluralize(seconds, "секунда", "секунды", "секунд")}");

            return string.Join(" ", parts);
        }
    }

    private static string Pluralize(int value, string one, string few, string many)
    {
        var n = Math.Abs(value) % 100;
        var n1 = n % 10;

        if (n > 10 && n < 20) return many;
        if (n1 > 1 && n1 < 5) return few;
        if (n1 == 1) return one;
        return many;
    }
}
