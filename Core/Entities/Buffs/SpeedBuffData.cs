using UnityEngine;

namespace SuspiciousGames.Saligia.Core.Entities.Buffs
{
    [CreateAssetMenu(menuName = "Saligia/Buffs/SpeedBuff")]
    public class SpeedBuffData : BuffData
    {
        [SerializeField] private float _speedMultiplier;
        public float SpeedMultiplier => _speedMultiplier;

        public override Buff InitializeBuff(Entity buffTarget, Entity buffSource)
        {
            return new SpeedBuff(this, buffTarget, buffSource);
        }
    }
}
