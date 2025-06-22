using System.Linq;
using RimWorld;
using Verse;

namespace ExpandedIncidents.Settings;

internal class EI_ModSettings : ModSettings
{
    internal static float QuarrelBaseChance = 0.3f;
    private static float homesickBaseChance = 0.1f;
    private static float homesickCuredBaseChance = 0.05f;
    internal static float CliqueBaseChance = 0.3f;
    internal static float SabotageBaseChance = 0.5f;
    internal static float ThiefBaseChance = 2.0f;

    public static void ChangeDef()
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
                    incidentDef.baseChance = homesickBaseChance;
                    break;
                case "HomesickCured":
                    incidentDef.baseChance = homesickCuredBaseChance;
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
                    incidentDef.baseChance = homesickBaseChance;
                    break;
                case "HomesickCured":
                    incidentDef.baseChance = homesickCuredBaseChance;
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

    public override void ExposeData()
    {
        base.ExposeData();
        Scribe_Values.Look(ref QuarrelBaseChance, "QuarrelBaseChance", 0.3f);
        Scribe_Values.Look(ref homesickBaseChance, "HomesickBaseChance", 0.1f);
        Scribe_Values.Look(ref homesickCuredBaseChance, "HomesickCuredBaseChance", 0.05f);
        Scribe_Values.Look(ref CliqueBaseChance, "CliqueBaseChance", 0.3f);
        Scribe_Values.Look(ref SabotageBaseChance, "SabotageBaseChance", 0.5f);
        Scribe_Values.Look(ref ThiefBaseChance, "ThiefBaseChance", 2.0f);
    }
}