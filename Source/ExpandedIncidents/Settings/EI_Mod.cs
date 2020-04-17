using System;
using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;

namespace ExpandedIncidents.Settings
{
    // Token: 0x0200000E RID: 14
    internal class IE_Mod : Mod
    {
        // Token: 0x06000027 RID: 39 RVA: 0x00003737 File Offset: 0x00001937
        public IE_Mod(ModContentPack content) : base(content)
        {
            settings = GetSettings<EI_ModSettings>();
        }

        // Token: 0x06000028 RID: 40 RVA: 0x00003758 File Offset: 0x00001958
        public override string SettingsCategory()
        {
            return "Expanded Incidents";
        }

        // Token: 0x06000029 RID: 41 RVA: 0x0000376C File Offset: 0x0000196C
        public void ResetSettings()
        {
            EI_ModSettings.QuarrelBaseChance = 0.3f;
            EI_ModSettings.HomesickBaseChance = 0.1f;
            EI_ModSettings.HomesickCuredBaseChance = 0.05f;
            EI_ModSettings.CliqueBaseChance = 0.3f;
            EI_ModSettings.SabotageBaseChance = 0.5f;
            EI_ModSettings.ThiefBaseChance = 2.0f;
            settings.Write();
            settings.ChangeDef();
        }

        // Token: 0x0600002C RID: 44 RVA: 0x000039CC File Offset: 0x00001BCC
        public override void DoSettingsWindowContents(Rect rect)
        {
            settings.ChangeDef();
            Rect rect2 = new Rect(rect.x, rect.y, rect.width - 30f, rect.height - 10f);
            Listing_Standard listing_Standard = new Listing_Standard();
            Widgets.BeginScrollView(rect, ref this.scrollPosition, rect2, true);
            listing_Standard.Begin(rect2);
            listing_Standard.Gap(10f);
            Rect rect3 = listing_Standard.GetRect(Text.LineHeight);
            bool flag = Widgets.ButtonText(rect3, "Reset Settings", true, true, true);
            if (flag)
            {
                ResetSettings();
            }
            listing_Standard.Gap(10f);

            Rect rect7 = listing_Standard.GetRect(Text.LineHeight);
            Widgets.Label(rect7, "EI_SettingHeader".Translate());
            listing_Standard.Gap(10f);
            Rect rect8 = listing_Standard.GetRect(Text.LineHeight);
            Rect rect9 = rect8.LeftHalf().Rounded();
            Rect rect10 = rect8.RightHalf().Rounded();
            Rect rect11 = rect9.LeftHalf().Rounded();
            Rect rect12 = rect9.RightHalf().Rounded();
            rect11.Overlaps(rect12);
            Rect rect13 = rect12.RightHalf().Rounded();
            Widgets.Label(rect11, "LetterLabelQuarrel".Translate());
            Widgets.Label(rect13, EI_ModSettings.QuarrelBaseChance.ToString());
            EI_ModSettings.QuarrelBaseChance = Widgets.HorizontalSlider(new Rect(rect10.xMin + rect10.height + 10f, rect10.y, rect10.width - (rect10.height * 2f + 20f), rect10.height), EI_ModSettings.QuarrelBaseChance, 0f, 10f, true, null, null, null, -1f);
            listing_Standard.Gap(10f);
            Rect rect14 = listing_Standard.GetRect(Text.LineHeight);
            Rect rect15 = rect14.LeftHalf().Rounded();
            Rect rect16 = rect14.RightHalf().Rounded();
            Rect rect17 = rect15.LeftHalf().Rounded();
            Rect rect18 = rect15.RightHalf().Rounded();
            rect17.Overlaps(rect18);
            Rect rect19 = rect18.RightHalf().Rounded();
            Widgets.Label(rect17, "LetterLabelHomesick".Translate());
            Widgets.Label(rect19, EI_ModSettings.HomesickBaseChance.ToString());
            EI_ModSettings.HomesickBaseChance = Widgets.HorizontalSlider(new Rect(rect16.xMin + rect16.height + 10f, rect16.y, rect16.width - (rect16.height * 2f + 20f), rect16.height), EI_ModSettings.HomesickBaseChance, 0f, 10f, true, null, null, null, -1f);
            listing_Standard.Gap(10f);
            Rect rect20 = listing_Standard.GetRect(Text.LineHeight);
            Rect rect21 = rect20.LeftHalf().Rounded();
            Rect rect22 = rect20.RightHalf().Rounded();
            Rect rect23 = rect21.LeftHalf().Rounded();
            Rect rect24 = rect21.RightHalf().Rounded();
            rect23.Overlaps(rect24);
            Rect rect25 = rect24.RightHalf().Rounded();
            Widgets.Label(rect23, "LetterLabelHomesickCured".Translate());
            Widgets.Label(rect25, EI_ModSettings.HomesickCuredBaseChance.ToString());
            EI_ModSettings.HomesickCuredBaseChance = Widgets.HorizontalSlider(new Rect(rect22.xMin + rect22.height + 10f, rect22.y, rect22.width - (rect22.height * 2f + 20f), rect22.height), EI_ModSettings.HomesickCuredBaseChance, 0f, 10f, true, null, null, null, -1f);
            listing_Standard.Gap(10f);
            Rect rect26 = listing_Standard.GetRect(Text.LineHeight);
            Rect rect27 = rect26.LeftHalf().Rounded();
            Rect rect28 = rect26.RightHalf().Rounded();
            Rect rect29 = rect27.LeftHalf().Rounded();
            Rect rect30 = rect27.RightHalf().Rounded();
            rect29.Overlaps(rect30);
            Rect rect31 = rect30.RightHalf().Rounded();
            Widgets.Label(rect29, "LetterLabelCliques".Translate());
            Widgets.Label(rect31, EI_ModSettings.CliqueBaseChance.ToString());
            EI_ModSettings.CliqueBaseChance = Widgets.HorizontalSlider(new Rect(rect28.xMin + rect28.height + 10f, rect28.y, rect28.width - (rect28.height * 2f + 20f), rect28.height), EI_ModSettings.CliqueBaseChance, 0f, 10f, true, null, null, null, -1f);
            listing_Standard.Gap(10f);
            Rect rect32 = listing_Standard.GetRect(Text.LineHeight);
            Rect rect33 = rect32.LeftHalf().Rounded();
            Rect rect34 = rect32.RightHalf().Rounded();
            Rect rect35 = rect33.LeftHalf().Rounded();
            Rect rect36 = rect33.RightHalf().Rounded();
            rect35.Overlaps(rect36);
            Rect rect37 = rect36.RightHalf().Rounded();
            Widgets.Label(rect35, "LetterLabelSabotage".Translate());
            Widgets.Label(rect37, EI_ModSettings.SabotageBaseChance.ToString());
            EI_ModSettings.SabotageBaseChance = Widgets.HorizontalSlider(new Rect(rect34.xMin + rect34.height + 10f, rect34.y, rect34.width - (rect34.height * 2f + 20f), rect34.height), EI_ModSettings.SabotageBaseChance, 0f, 10f, true, null, null, null, -1f);
            listing_Standard.Gap(10f);
            Rect rect38 = listing_Standard.GetRect(Text.LineHeight);
            Rect rect39 = rect38.LeftHalf().Rounded();
            Rect rect40 = rect38.RightHalf().Rounded();
            Rect rect41 = rect39.LeftHalf().Rounded();
            Rect rect42 = rect39.RightHalf().Rounded();
            rect41.Overlaps(rect42);
            Rect rect43 = rect42.RightHalf().Rounded();
            Widgets.Label(rect41, "LetterLabelThief".Translate());
            Widgets.Label(rect43, EI_ModSettings.ThiefBaseChance.ToString());
            EI_ModSettings.ThiefBaseChance = Widgets.HorizontalSlider(new Rect(rect40.xMin + rect40.height + 10f, rect40.y, rect40.width - (rect40.height * 2f + 20f), rect40.height), EI_ModSettings.ThiefBaseChance, 0f, 10f, true, null, null, null, -1f);
            listing_Standard.End();
            Widgets.EndScrollView();
            settings.Write();
            settings.ChangeDef();
        }

        // Token: 0x0400003D RID: 61
        public static EI_ModSettings settings;

        // Token: 0x0400003E RID: 62
        private Vector2 scrollPosition = Vector2.zero;
    }
}
