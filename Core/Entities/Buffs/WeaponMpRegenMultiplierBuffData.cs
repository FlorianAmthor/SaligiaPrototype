using UnityEngine;

namespace SuspiciousGames.Saligia.Core.Entities.Buffs
{
    [CreateAssetMenu(menuName = "Saligia/Buffs/WeaponMpRegenMultiplierBuff")]
    public class WeaponMpRegenMultiplierBuffData : BuffData
    {
        [SerializeField] private float _mpRegenMultiplier;
        public float MpRegenMultiplier => _mpRegenMultiplier;

        public override Buff InitializeBuff(Entity buffTarget, Entity buffSource)
        {
            return new WeaponMpRegenMultiplierBuff(this, buffTarget, buffSource);
        }
    }
}
