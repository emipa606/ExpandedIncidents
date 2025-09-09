using Mlie;
using UnityEngine;
using Verse;

namespace ExpandedIncidents.Settings;

internal class IE_Mod : Mod
{
    private static EI_ModSettings settings;
    private static string currentVersion;

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

    private static void resetSettings()
    {
        EI_ModSettings.QuarrelBaseChance = 0.3f;
        EI_ModSettings.CliqueBaseChance = 0.3f;
        EI_ModSettings.SabotageBaseChance = 0.5f;
        EI_ModSettings.ThiefBaseChance = 2.0f;
        settings.Write();
    }

    public override void DoSettingsWindowContents(Rect rect)
    {
        var listingStandard = new Listing_Standard();
        listingStandard.Begin(rect);

        var headerPosition = listingStandard.Label("EI_SettingHeader".Translate());
        if (Widgets.ButtonText(headerPosition.RightPart(0.2f), "EI_Reset".Translate()))
        {
            resetSettings();
        }

        listingStandard.Label("LetterLabelQuarrel".Translate());
        EI_ModSettings.QuarrelBaseChance = listingStandard.SliderLabeled(
            EI_ModSettings.QuarrelBaseChance.ToStringDecimalIfSmall(),
            EI_ModSettings.QuarrelBaseChance, 0f, 10f);
        listingStandard.Gap(10f);
        listingStandard.Label("LetterLabelCliques".Translate());
        EI_ModSettings.CliqueBaseChance = listingStandard.SliderLabeled(
            EI_ModSettings.CliqueBaseChance.ToStringDecimalIfSmall(),
            EI_ModSettings.CliqueBaseChance, 0f, 10f);
        listingStandard.Gap(10f);
        listingStandard.Label("LetterLabelSabotage".Translate());
        EI_ModSettings.SabotageBaseChance = listingStandard.SliderLabeled(
            EI_ModSettings.SabotageBaseChance.ToStringDecimalIfSmall(),
            EI_ModSettings.SabotageBaseChance, 0f, 10f);
        listingStandard.Gap(10f);
        listingStandard.Label("LetterLabelThief".Translate());
        EI_ModSettings.ThiefBaseChance = listingStandard.SliderLabeled(
            EI_ModSettings.ThiefBaseChance.ToStringDecimalIfSmall(),
            EI_ModSettings.ThiefBaseChance, 0f, 10f);
        listingStandard.Gap(10f);
        listingStandard.Label("LetterLabelHomesick".Translate());
        EI_ModSettings.HomesickBaseChance = listingStandard.SliderLabeled(
            EI_ModSettings.HomesickBaseChance.ToStringDecimalIfSmall(), EI_ModSettings.HomesickBaseChance, 0f, 10f);
        listingStandard.Gap(10f);
        listingStandard.Label("LetterLabelHomesickCured".Translate());
        EI_ModSettings.HomesickCuredBaseChance = listingStandard.SliderLabeled(
            EI_ModSettings.HomesickCuredBaseChance.ToStringDecimalIfSmall(), EI_ModSettings.HomesickCuredBaseChance, 0f,
            10f);
        if (currentVersion != null)
        {
            listingStandard.Gap(10f);
            GUI.contentColor = Color.gray;
            listingStandard.Label("EI_VersionInfo".Translate(currentVersion));
            GUI.contentColor = Color.white;
        }

        listingStandard.End();
    }

    public override void WriteSettings()
    {
        base.WriteSettings();
        EI_ModSettings.ChangeDef();
    }
}