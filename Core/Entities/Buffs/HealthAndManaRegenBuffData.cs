using UnityEngine;

namespace SuspiciousGames.Saligia.Core.Entities.Buffs
{
    [CreateAssetMenu(menuName = "Saligia/Buffs/Health and Mana Regeneration Buff")]
    public class HealthAndManaRegenBuffData : BuffData
    {
        [SerializeField, Range(0.0f, 1.0f)] private float _percentualManaRegenerationValue;
        [SerializeField, Range(0.0f, 1.0f)] private float _percentualHealthRegenerationValue;
        public float PercentualManaRegenerationValue => _percentualManaRegenerationValue;
        public float PercentualHealthRegenerationValue => _percentualHealthRegenerationValue;

        public override Buff InitializeBuff(Entity buffTarget, Entity buffSource)
        {
            return new HealthAndManaRegenBuff(this, buffTarget, buffSource);
        }
    }
}
