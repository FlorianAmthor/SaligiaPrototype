using SuspiciousGames.Saligia.Core.Entities.Components;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace SuspiciousGames.Saligia.Core.Entities
{
    public class BossEntity : EnemyEntity
    {
        [field: SerializeField] public GameObject MugObject { get; private set; }
        [field: SerializeField] public Transform ForkTransform { get; private set; }
        [field: SerializeField] public Transform BellyTransform { get; private set; }

        [Serializable]
        protected class PhaseThreshold
        {
            public BossPhase bossPhase;
            public float threshold;
        }

        [SerializeField] private List<PhaseThreshold> _phaseThresholds;
        private Dictionary<BossPhase, float> _phaseThresholdDict;

        public BossPhase CurrentPhase { get; private set; } = BossPhase.One;

        protected override void OnStart()
        {
            base.OnStart();
            _phaseThresholdDict = new();
            foreach (var phaseThreshold in _phaseThresholds)
                _phaseThresholdDict.TryAdd(phaseThreshold.bossPhase, phaseThreshold.threshold);
            if (behaviourTreeRunner)
                behaviourTreeRunner.tree.blackboard.currentPhase = CurrentPhase;
        }

        public override int ApplyDamage(DamageData damageData)
        {
            int actualDamage = base.ApplyDamage(damageData);
            if (!HealthComponent || HealthComponent.IsDead)
                return actualDamage;

            if (CurrentPhase == BossPhase.One)
            {
                if (_phaseThresholdDict[BossPhase.Two] >= HealthComponent.HealthPercentage)
                    ActivateNextPhase();
            }
            else if (CurrentPhase == BossPhase.Two)
            {
                if (_phaseThresholdDict[BossPhase.Three] >= HealthComponent.HealthPercentage)
                    ActivateNextPhase();
            }

            return actualDamage;
        }

        public void ForcePhase(BossPhase bossPhase)
        {
            CurrentPhase = bossPhase;
            behaviourTreeRunner.tree.blackboard.currentPhase = CurrentPhase;
        }

        private void ActivateNextPhase()
        {
            if (CurrentPhase == BossPhase.Three)
                return;
            ForcePhase(CurrentPhase + 1);
        }
    }
}
