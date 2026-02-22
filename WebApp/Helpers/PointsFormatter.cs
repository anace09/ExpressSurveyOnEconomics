namespace WebApp.Helpers;

public static class PointsFormatter
{
    public static string Format(int points, string lang = "ru")
    {
        if (lang == "kk")
        {
            return $"{points} балл";
        }

        return $"{points} {Pluralize(points, "балл", "балла", "баллов")}";
    }

    public static string FormatRange(int minPoints, int maxPoints, string lang = "ru")
    {
        if (minPoints == maxPoints)
            return Format(minPoints, lang);

        if (lang == "kk")
            return $"{minPoints}-{maxPoints} балл";

        return $"{minPoints}-{maxPoints} {Pluralize(maxPoints, "балл", "балла", "баллов")}";
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
