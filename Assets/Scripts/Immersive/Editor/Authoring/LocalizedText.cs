using WiDiD.ImmersiveAPI.Trainings.GameFlow.Building;
using WiDiD.Localization;

namespace ICOPE.Immersive.Editor.Authoring
{
    public static class LocalizedText
    {
        public static LanguageDictionary Of(string fr, string en)
        {
            LanguageDictionary dictionary = LocalizedTextFactory.Empty();
            LocalizedTextFactory.Set(dictionary, "fr", fr);
            LocalizedTextFactory.Set(dictionary, "en", en);
            return dictionary;
        }
    }
}
