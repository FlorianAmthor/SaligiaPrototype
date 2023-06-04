using SuspiciousGames.Saligia.Core.Entities.Components;
using UnityEngine;

namespace SuspiciousGames.Saligia.Core.Entities.Buffs
{
    [CreateAssetMenu(menuName = "Saligia/Buffs/DamageOverTimeBuff")]
    public class DamaveOverTimeBuffData : BuffData
    {
        [SerializeField] private DamageData _tickDamageData;
        public DamageData TickDamageData => _tickDamageData;

        [SerializeField] private int _numberOfTicks;
        public int NumberOfTicks => _numberOfTicks;

        public override Buff InitializeBuff(Entity buffTarget, Entity buffSource)
        {
            return new DamageOverTimeBuff(this, buffTarget, buffSource);
        }
    }
}
