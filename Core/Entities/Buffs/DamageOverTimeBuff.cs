using SuspiciousGames.Saligia.Core.Entities.Components;
using UnityEngine;

namespace SuspiciousGames.Saligia.Core.Entities.Buffs
{
    public class DamageOverTimeBuff : Buff
    {
        private DamageData _tickDamageData;
        private int _numberOfTicks;
        private float _tickInterval;
        private float _lastTickTime;

        public DamageOverTimeBuff(BuffData buffData, Entity buffTarget, Entity buffSource) : base(buffData, buffTarget, buffSource)
        {
            DamaveOverTimeBuffData poisonBuffData = (DamaveOverTimeBuffData)BuffData;
            _numberOfTicks = poisonBuffData.NumberOfTicks;
            _tickDamageData = poisonBuffData.TickDamageData;
            _tickDamageData.damageSource = buffSource;
            _tickInterval = BuffData.Duration / _numberOfTicks;
        }

        public override void Tick(float delta)
        {
            base.Tick(delta);
            if (_lastTickTime - Duration >= _tickInterval)
                DoDamageTick();
        }

        protected override void ApplyEffect()
        {
            DoDamageTick();
        }

        protected override void UndoEffect()
        {

        }

        protected override void RefreshDuration()
        {
            var prevDuration = Duration;
            base.RefreshDuration();
            _lastTickTime += Duration - prevDuration;
        }

        private void DoDamageTick()
        {
            _lastTickTime = Duration;
            buffTarget.ApplyDamage(_tickDamageData);
        }
    }

}
