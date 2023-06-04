using SuspiciousGames.Saligia.Core.Entities.Buffs;
using UnityEngine;

namespace SuspiciousGames.Saligia.Core.Potions
{
    [System.Serializable]
    public abstract class PotionTieredEffect
    {
        public PotionTier potionTier;
    }

    [System.Serializable]
    public class MindPowerPotionTieredEffect : PotionTieredEffect
    {
        [Range(0.0f, 1.0f)] public float percentualMindPowerRegen;
    }

    [System.Serializable]
    public class BuffPotionTieredEffect<T> : PotionTieredEffect where T : BuffData
    {
        public T buffData;
    }
}