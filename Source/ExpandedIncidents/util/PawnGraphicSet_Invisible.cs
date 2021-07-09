using Verse;

namespace ExpandedIncidents
{
    public class PawnGraphicSet_Invisible : PawnGraphicSet
    {
        public PawnGraphicSet_Invisible(Pawn pawn) : base(pawn)
        {
            this.pawn = pawn;
            ResolveAllGraphics();
        }

        public new void ResolveAllGraphics()
        {
            ClearCache();
            if (pawn.RaceProps.Humanlike)
            {
                nakedGraphic = new Graphic_Invisible();
                rottingGraphic = new Graphic_Invisible();
                dessicatedGraphic = new Graphic_Invisible();
                headGraphic = null;
                desiccatedHeadGraphic = new Graphic_Invisible();
                skullGraphic = new Graphic_Invisible();
                hairGraphic = new Graphic_Invisible();
                ResolveApparelGraphics();
            }
            else
            {
                var curKindLifeStage = pawn.ageTracker.CurKindLifeStage;
                nakedGraphic = new Graphic_Invisible();
                rottingGraphic = new Graphic_Invisible();
                if (pawn.RaceProps.packAnimal)
                {
                    packGraphic = new Graphic_Invisible();
                }

                if (curKindLifeStage.dessicatedBodyGraphicData != null)
                {
                    dessicatedGraphic = new Graphic_Invisible();
                }
            }
        }

        public new void ResolveApparelGraphics()
        {
            ClearCache();
            apparelGraphics.Clear();
        }
    }
}