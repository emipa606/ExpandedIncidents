using System.Linq;
using RimWorld;
using Verse;

namespace ExpandedIncidents
{
    public class Alert_Homesick : Alert_Thought
    {
        public Alert_Homesick()
        {
            explanationKey = "HomesickDesc".Translate();
        }

        protected override ThoughtDef Thought => ThoughtDefOfIncidents.Homesickness;

        public override string GetLabel()
        {
            if (GetReport().AllCulprits.Count() == 1)
            {
                return "ColonistHomesickAlert".Translate();
            }

            return "ColonistsHomesickAlert".Translate();
        }
    }
}