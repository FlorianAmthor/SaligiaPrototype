using UnityEngine;

namespace SuspiciousGames.Saligia.Core.Entities.Buffs
{
    public abstract class BuffData : ScriptableObject
    {
        [SerializeField] protected string buffName;
        public string BuffName => buffName;

        [SerializeField] protected Sprite _buffIcon;
        public Sprite BuffIcon => _buffIcon;

        /**
         * Time duration of the buff in seconds.
         */
        public float Duration;

        /**
         * Effect value is renewed each time the buff is applied.
         */
        public bool IsDurationRenewed;

        public bool IsPositiveBuff;

        /**
         * Effect value is increased each time the buff is applied.
         */
        [Min(1)]
        public int MaxStacks = 1;

        [Tooltip("If this number is set to 0, all stacks will be removed when the buff ends.")]
        public int RemoveNumberOfStacksOnEnd = 1;


        public abstract Buff InitializeBuff(Entity buffTarget, Entity buffSource = null);
    }
}