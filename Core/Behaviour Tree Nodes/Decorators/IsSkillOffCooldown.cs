using SuspiciousGames.Saligia.Core.Skills;
using TheKiwiCoder;
using UnityEngine;

namespace SuspiciousGames.Saligia.Core.BehaviourTreeNodes.Decorators
{
    public class IsSkillOffCooldown : DecoratorNode
    {
        [SerializeField] private BaseSkill _skillToUse;
        protected override void OnStart() { }

        protected override void OnStop() { }

        protected override State OnUpdate()
        {
            if (!_skillToUse)
            {
                Debug.LogError($"SkillToUse is null! \nSource: \"{name}\" ");
                return State.Failure;
            }
            if (!context.owner.CastComponent)
            {
                Debug.LogError($"{context.owner.name} has no CastComponent.\nSource: \"{name}\" ");
                return State.Failure;
            }
            if (context.owner.CastComponent.IsSkillOnCooldown(_skillToUse))
                return State.Failure;
            else
                return child.Update();
        }
    }
}