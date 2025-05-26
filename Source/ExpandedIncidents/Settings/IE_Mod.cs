using Mlie;
using UnityEngine;
using Verse;

namespace ExpandedIncidents.Settings;

internal class IE_Mod : Mod
{
    public static EI_ModSettings settings;
    private static string currentVersion;

    private Vector2 scrollPosition = Vector2.zero;

    public IE_Mod(ModContentPack content) : base(content)
    {
        settings = GetSettings<EI_ModSettings>();
        currentVersion =
            VersionFromManifest.GetVersionFromModMetaData(content.ModMetaData);
    }

    public override string SettingsCategory()
    {
        return "Expanded Incidents";
    }

    public void ResetSettings()
    {
        EI_ModSettings.QuarrelBaseChance = 0.3f;
        EI_ModSettings.CliqueBaseChance = 0.3f;
        EI_ModSettings.SabotageBaseChance = 0.5f;
        EI_ModSettings.ThiefBaseChance = 2.0f;
        settings.Write();
        EI_ModSettings.ChangeDef();
    }

    public override void DoSettingsWindowContents(Rect rect)
    {
        EI_ModSettings.ChangeDef();
        var rect2 = new Rect(rect.x, rect.y, rect.width - 30f, rect.height - 10f);
        var listing_Standard = new Listing_Standard();
        Widgets.BeginScrollView(rect, ref scrollPosition, rect2);
        listing_Standard.Begin(rect2);
        listing_Standard.Gap(10f);
        var rect3 = listing_Standard.GetRect(Text.LineHeight);
        if (Widgets.ButtonText(rect3, "Reset Settings"))
        {
            ResetSettings();
        }

        listing_Standard.Gap(10f);

        var rect7 = listing_Standard.GetRect(Text.LineHeight);
        Widgets.Label(rect7, "EI_SettingHeader".Translate());
        listing_Standard.Gap(10f);
        var rect8 = listing_Standard.GetRect(Text.LineHeight);
        var rect9 = rect8.LeftHalf().Rounded();
        var rect10 = rect8.RightHalf().Rounded();
        var rect11 = rect9.LeftHalf().Rounded();
        var rect12 = rect9.RightHalf().Rounded();
        _ = rect11.Overlaps(rect12);
        var rect13 = rect12.RightHalf().Rounded();
        Widgets.Label(rect11, "LetterLabelQuarrel".Translate());
        Widgets.Label(rect13, EI_ModSettings.QuarrelBaseChance.ToString());
        EI_ModSettings.QuarrelBaseChance = Widgets.HorizontalSlider(
            new Rect(rect10.xMin + rect10.height + 10f, rect10.y, rect10.width - ((rect10.height * 2f) + 20f),
                rect10.height), EI_ModSettings.QuarrelBaseChance, 0f, 10f, true);
        listing_Standard.Gap(10f);
        var rect14 = listing_Standard.GetRect(Text.LineHeight);
        var rect15 = rect14.LeftHalf().Rounded();
        var rect16 = rect14.RightHalf().Rounded();
        var rect17 = rect15.LeftHalf().Rounded();
        var rect18 = rect15.RightHalf().Rounded();
        _ = rect17.Overlaps(rect18);
        var rect19 = rect18.RightHalf().Rounded();
        Widgets.Label(rect17, "LetterLabelCliques".Translate());
        Widgets.Label(rect19, EI_ModSettings.CliqueBaseChance.ToString());
        EI_ModSettings.CliqueBaseChance = Widgets.HorizontalSlider(
            new Rect(rect16.xMin + rect16.height + 10f, rect16.y, rect16.width - ((rect16.height * 2f) + 20f),
                rect16.height), EI_ModSettings.CliqueBaseChance, 0f, 10f, true);
        listing_Standard.Gap(10f);
        var rect32 = listing_Standard.GetRect(Text.LineHeight);
        var rect33 = rect32.LeftHalf().Rounded();
        var rect34 = rect32.RightHalf().Rounded();
        var rect35 = rect33.LeftHalf().Rounded();
        var rect36 = rect33.RightHalf().Rounded();
        _ = rect35.Overlaps(rect36);
        var rect37 = rect36.RightHalf().Rounded();
        Widgets.Label(rect35, "LetterLabelSabotage".Translate());
        Widgets.Label(rect37, EI_ModSettings.SabotageBaseChance.ToString());
        EI_ModSettings.SabotageBaseChance = Widgets.HorizontalSlider(
            new Rect(rect34.xMin + rect34.height + 10f, rect34.y, rect34.width - ((rect34.height * 2f) + 20f),
                rect34.height), EI_ModSettings.SabotageBaseChance, 0f, 10f, true);
        listing_Standard.Gap(10f);
        var rect38 = listing_Standard.GetRect(Text.LineHeight);
        var rect39 = rect38.LeftHalf().Rounded();
        var rect40 = rect38.RightHalf().Rounded();
        var rect41 = rect39.LeftHalf().Rounded();
        var rect42 = rect39.RightHalf().Rounded();
        _ = rect41.Overlaps(rect42);
        var rect43 = rect42.RightHalf().Rounded();
        Widgets.Label(rect41, "LetterLabelThief".Translate());
        Widgets.Label(rect43, EI_ModSettings.ThiefBaseChance.ToString());
        EI_ModSettings.ThiefBaseChance = Widgets.HorizontalSlider(
            new Rect(rect40.xMin + rect40.height + 10f, rect40.y, rect40.width - ((rect40.height * 2f) + 20f),
                rect40.height), EI_ModSettings.ThiefBaseChance, 0f, 10f, true);
        if (currentVersion != null)
        {
            listing_Standard.Gap(10f);
            GUI.contentColor = Color.gray;
            listing_Standard.Label("EI_VersionInfo".Translate(currentVersion));
            GUI.contentColor = Color.white;
        }

        listing_Standard.End();
        Widgets.EndScrollView();
        settings.Write();
        EI_ModSettings.ChangeDef();
    }
}