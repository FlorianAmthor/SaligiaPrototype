using UnityEngine;

namespace SuspiciousGames.Saligia.Core.Entities.Buffs
{
    [CreateAssetMenu(menuName = "Saligia/Buffs/GlobalDamageBuff")]
    public class GlobalDamageBuffData : BuffData
    {
        [SerializeField] private float _damageMultiplier;
        public float DamageMultiplier => _damageMultiplier;

        public override Buff InitializeBuff(Entity buffTarget, Entity buffSource)
        {
            return new GlobalDamageBuff(this, buffTarget, buffSource);
        }
    }

}
