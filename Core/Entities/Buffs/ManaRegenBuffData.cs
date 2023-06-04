using UnityEngine;

namespace SuspiciousGames.Saligia.Core.Entities.Buffs
{
    [CreateAssetMenu(menuName = "Saligia/Buffs/Mana Regeneration Buff")]
    public class ManaRegenBuffData : BuffData
    {
        [SerializeField, Range(0.0f, 1.0f)] private float _percentualManaRegenerationValue;
        public float PercentualManaRegenerationValue => _percentualManaRegenerationValue;

        public override Buff InitializeBuff(Entity buffTarget, Entity buffSource)
        {
            return new ManaRegenBuff(this, buffTarget, buffSource);
        }
    }
}
