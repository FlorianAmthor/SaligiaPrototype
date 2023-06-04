using UnityEngine;

namespace SuspiciousGames.Saligia.Core.Entities.Buffs
{
    [CreateAssetMenu(menuName = "Saligia/Buffs/Stun")]
    public class StunBuffData : BuffData
    {
        public override Buff InitializeBuff(Entity buffTarget, Entity source)
        {
            return new StunBuff(this, buffTarget, source);
        }
    }
}
