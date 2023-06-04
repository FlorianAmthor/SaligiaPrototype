using SuspiciousGames.Saligia.Core.Skills;
using System.Collections.Generic;
using TheKiwiCoder;
using UnityEngine;

namespace SuspiciousGames.Saligia.Core.BehaviourTreeNodes.Actions
{
    public class InitializeSkillCooldowns : ActionNode
    {
        [System.Serializable]
        public class BaseSkillCooldowns
        {
            [Tooltip("If true the cooldown will be a random number between 0 and the skills cooldown.\n If false the cooldown of the skill will be used.")]
            public bool randomizeCooldown;
            public BaseSkill baseSkill;
        }

        [SerializeField] private List<BaseSkillCooldowns> _initialSkillCooldowns;
        protected override void OnStart()
        {
            foreach (var baseSkillCooldown in _initialSkillCooldowns)
                if (context.owner.CastComponent)
                {
                    if (baseSkillCooldown.randomizeCooldown)
                        context.owner.CastComponent.SetInitialCooldownForSkill(baseSkillCooldown.baseSkill, Random.value * baseSkillCooldown.baseSkill.Cooldown);
                    else
                        context.owner.CastComponent.SetInitialCooldownForSkill(baseSkillCooldown.baseSkill, baseSkillCooldown.baseSkill.Cooldown);
                }
        }

        protected override void OnStop()
        {
        }

        protected override State OnUpdate()
        {
            return State.Success;
        }
    }
}