using UnityEngine;

namespace SuspiciousGames.Saligia.Core.Entities.Buffs
{
    [CreateAssetMenu(menuName = "Saligia/Buffs/BaseMpRegenMultiplierBuff")]
    public class BaseMpRegenMultiplierBuffData : BuffData
    {
        [SerializeField] private float _mpRegenMultiplier;
        public float MpRegenMultiplier => _mpRegenMultiplier;

        public override Buff InitializeBuff(Entity buffTarget, Entity buffSource)
        {
            return new BaseMpRegenMultiplierBuff(this, buffTarget, buffSource);
        }
    }
}
