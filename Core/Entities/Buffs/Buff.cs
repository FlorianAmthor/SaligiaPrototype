using UnityEngine;

namespace SuspiciousGames.Saligia.Core.Entities.Buffs
{
    public abstract class Buff
    {
        protected float Duration;
        protected int EffectStacks;
        public BuffData BuffData { get; }
        protected readonly Entity buffTarget;
        public bool IsFinished;
        public Entity Source { get; private set; }

        public Buff(BuffData buffData, Entity buffTarget, Entity source)
        {
            BuffData = buffData;
            this.buffTarget = buffTarget;
            Source = source;
        }

        public virtual void Tick(float delta)
        {
            Duration -= delta;
            if (Duration <= 0)
            {
                End();
            }
        }

        /**
         * Activates buff or extends duration if ScriptableBuff has IsDurationStacked or IsEffectStacked set to true.
         */
        public void Activate(float diminishingReturnMultiplier = 1.0f)
        {
            if (BuffData.MaxStacks > EffectStacks || Duration <= 0)
            {
                Apply();
            }
            if (BuffData.IsDurationRenewed || Duration <= 0)
            {
                RefreshDuration();
            }
        }
        public void End()
        {
            if (BuffData.RemoveNumberOfStacksOnEnd == 0)
            {
                for (int i = 0; i < EffectStacks; i++)
                    Undo();
            }
            else
            {
                int counter = Mathf.Min(BuffData.RemoveNumberOfStacksOnEnd, EffectStacks);
                for (int i = 0; i < counter; i++)
                    Undo();
            }

            if (EffectStacks == 0)
            {
                IsFinished = true;
                return;
            }
            else
            {
                Duration = BuffData.Duration;
            }
        }

        protected virtual void RefreshDuration()
        {
            Duration = BuffData.Duration;
        }

        private void Apply()
        {
            EffectStacks++;
            ApplyEffect();
        }

        private void Undo()
        {
            EffectStacks--;
            UndoEffect();
        }

        protected abstract void ApplyEffect();
        protected abstract void UndoEffect();
    }
}
