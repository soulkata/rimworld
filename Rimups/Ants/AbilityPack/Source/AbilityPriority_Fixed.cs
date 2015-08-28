using Verse;

namespace AbilityPack
{
    /// <summary>
    /// This class gives a fixed priority to the ability
    /// </summary>
    public class AbilityPriority_Fixed : AbilityPriority
    {
        public int value;

        public override int GetPriority(AbilityDef ability, Pawn pawn) { return this.value; }
    }
}
