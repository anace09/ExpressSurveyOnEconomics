using System.Globalization;

namespace WebApp.Helpers
{
    public static class LocalizationHelper
    {

        public static string Pick(string kk, string ru) => CultureInfo.CurrentUICulture.TwoLetterISOLanguageName == "kk" ? kk : ru;

        public static List<T> Pick<T>(List<T> kk, List<T> ru) => CultureInfo.CurrentUICulture.Name == "kk" ? kk : ru;

    }
}
