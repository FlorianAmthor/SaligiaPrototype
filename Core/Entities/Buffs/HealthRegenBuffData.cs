using UnityEngine;

namespace SuspiciousGames.Saligia.Core.Entities.Buffs
{
    [CreateAssetMenu(menuName = "Saligia/Buffs/Health Regeneration Buff")]
    public class HealthRegenBuffData : BuffData
    {
        [SerializeField, Range(0.0f, 1.0f)] private float _percentualHealthRegenerationValue;
        public float PercentualHealthRegenerationValue => _percentualHealthRegenerationValue;

        public override Buff InitializeBuff(Entity buffTarget, Entity buffSource)
        {
            return new HealthRegenBuff(this, buffTarget, buffSource);
        }
    }
}
