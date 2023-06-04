using UnityEngine;

namespace SuspiciousGames.Saligia.Core.Entities.Buffs
{
    //TODO implement diminishing returns later
    public abstract class CrowdControlBuffData : BuffData
    {
        public enum CrowdControlType
        {
            Charm,
            Fear,
            Root,
            Slow,
            Stun
        }

        [SerializeField] protected CrowdControlType type;
        public CrowdControlType Type => type;
    }
}