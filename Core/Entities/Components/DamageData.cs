using UnityEngine;

namespace SuspiciousGames.Saligia.Core.Entities.Components
{
    [System.Serializable]
    public class DamageData
    {
        [Min(0)] public int damageAmount;
        public bool isTrueDamage;
        public bool canStagger;
        public bool forceStagger;
        [Min(0)] public float pushBackForce = 0.0f;

        [HideInInspector] public int actualDamageTakenByEntity;
        [HideInInspector] public Entity damageSource;

        public DamageData()
        {
            damageAmount = 0;
            isTrueDamage = false;
            forceStagger = false;
            canStagger = false;
            actualDamageTakenByEntity = damageAmount;
            damageSource = null;
            pushBackForce = 0;
        }

        public DamageData(DamageData damageData)
        {
            damageAmount = damageData.damageAmount;
            isTrueDamage = damageData.isTrueDamage;
            forceStagger = damageData.forceStagger;
            canStagger = damageData.canStagger;
            actualDamageTakenByEntity = damageData.actualDamageTakenByEntity;
            damageSource = damageData.damageSource;
            pushBackForce = damageData.pushBackForce;
        }

        /// <summary>
        /// This will create the damage data. The only value that needs to be set is <paramref name="damageAmount"/>
        /// </summary>
        /// <param name="damageAmount"></param>
        /// <param name="canStagger"></param>
        /// <param name="isTrueDamage"></param>
        /// <param name="isTrueStagger">If this is true it will also set <paramref name="canStagger"/> to true</param>
        /// <param name="damageSource"></param>
        /// <param name="pushBackForce"></param>
        public DamageData(int damageAmount, bool canStagger = false, bool isTrueDamage = false, bool isTrueStagger = false, Entity damageSource = null, float pushBackForce = 0)
        {
            this.damageAmount = damageAmount;
            actualDamageTakenByEntity = damageAmount;
            this.canStagger = canStagger;
            this.isTrueDamage = isTrueDamage;
            this.forceStagger = isTrueStagger;
            this.damageSource = damageSource;
            this.pushBackForce = pushBackForce;

            if (isTrueStagger)
                canStagger = true;
        }
    }
}
