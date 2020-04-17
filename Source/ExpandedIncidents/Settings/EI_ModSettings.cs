using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;

namespace ExpandedIncidents.Settings
{
    // Token: 0x0200000D RID: 13
    internal class EI_ModSettings : ModSettings
    {
        internal static float QuarrelBaseChance = 0.3f;
        internal static float HomesickBaseChance = 0.1f;
        internal static float HomesickCuredBaseChance = 0.05f;
        internal static float CliqueBaseChance = 0.3f;
        internal static float SabotageBaseChance = 0.5f;
        internal static float ThiefBaseChance = 2.0f;

        public void ChangeDef()
        {
            List<IncidentDef> list = DefDatabase<IncidentDef>.AllDefs.ToList<IncidentDef>();
            foreach (IncidentDef incidentDef in list)
            {
                switch (incidentDef.defName)
                {
                    case "Quarrel":
                        incidentDef.baseChance = QuarrelBaseChance;
                        break;
                    case "Homesick":
                        incidentDef.baseChance = HomesickBaseChance;
                        break;
                    case "HomesickCured":
                        incidentDef.baseChance = HomesickCuredBaseChance;
                        break;
                    case "Clique":
                        incidentDef.baseChance = CliqueBaseChance;
                        break;
                    case "Sabotage":
                        incidentDef.baseChance = SabotageBaseChance;
                        break;
                    case "Thief":
                        incidentDef.baseChance = ThiefBaseChance;
                        break;
                }
            }
        }

        public static void ChangeDefPost()
        {
            List<IncidentDef> list = DefDatabase<IncidentDef>.AllDefs.ToList<IncidentDef>();
            foreach (IncidentDef incidentDef in list)
            {
                switch (incidentDef.defName)
                {
                    case "Quarrel":
                        incidentDef.baseChance = QuarrelBaseChance;
                        break;
                    case "Homesick":
                        incidentDef.baseChance = HomesickBaseChance;
                        break;
                    case "HomesickCured":
                        incidentDef.baseChance = HomesickCuredBaseChance;
                        break;
                    case "Clique":
                        incidentDef.baseChance = CliqueBaseChance;
                        break;
                    case "Sabotage":
                        incidentDef.baseChance = SabotageBaseChance;
                        break;
                    case "Thief":
                        incidentDef.baseChance = ThiefBaseChance;
                        break;
                }
            }
        }

        // Token: 0x06000024 RID: 36 RVA: 0x0000345C File Offset: 0x0000165C
        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look<float>(ref EI_ModSettings.QuarrelBaseChance, "QuarrelBaseChance", 0.3f, false);
            Scribe_Values.Look<float>(ref EI_ModSettings.HomesickBaseChance, "HomesickBaseChance", 0.1f, false);
            Scribe_Values.Look<float>(ref EI_ModSettings.HomesickCuredBaseChance, "HomesickCuredBaseChance", 0.05f, false);
            Scribe_Values.Look<float>(ref EI_ModSettings.CliqueBaseChance, "CliqueBaseChance", 0.3f, false);
            Scribe_Values.Look<float>(ref EI_ModSettings.SabotageBaseChance, "SabotageBaseChance", 0.5f, false);
            Scribe_Values.Look<float>(ref EI_ModSettings.ThiefBaseChance, "ThiefBaseChance", 2.0f, false);
        }


    }
}
