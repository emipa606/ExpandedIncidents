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
            var list = DefDatabase<IncidentDef>.AllDefs.ToList();
            foreach (var incidentDef in list)
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
            var list = DefDatabase<IncidentDef>.AllDefs.ToList();
            foreach (var incidentDef in list)
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
            Scribe_Values.Look(ref QuarrelBaseChance, "QuarrelBaseChance", 0.3f);
            Scribe_Values.Look(ref HomesickBaseChance, "HomesickBaseChance", 0.1f);
            Scribe_Values.Look(ref HomesickCuredBaseChance, "HomesickCuredBaseChance", 0.05f);
            Scribe_Values.Look(ref CliqueBaseChance, "CliqueBaseChance", 0.3f);
            Scribe_Values.Look(ref SabotageBaseChance, "SabotageBaseChance", 0.5f);
            Scribe_Values.Look(ref ThiefBaseChance, "ThiefBaseChance", 2.0f);
        }
    }
}