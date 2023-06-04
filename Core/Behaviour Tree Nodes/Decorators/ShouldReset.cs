using TheKiwiCoder;

public class ShouldReset : DecoratorNode
{
    protected override void OnStart()
    {
        if (blackboard.shouldReset && !blackboard.isInCastAnimation && !blackboard.isInSkillAnimation)
        {
            blackboard.isAggroed = false;
            blackboard.target = null;
            if (context.owner)
            {
                context.owner.IsResetting(blackboard.shouldReset);
                context.owner.StartReset();
                context.owner.MovementComponent.Agent.ResetPath();
            }
        }
        else
        {
            if (context.owner)
                context.owner.IsResetting(false);
        }
    }

    protected override void OnStop()
    {
        context.owner.IsResetting(blackboard.shouldReset);
    }

    protected override State OnUpdate()
    {
        if (blackboard.shouldReset && !blackboard.isInCastAnimation && !blackboard.isInSkillAnimation)
        {
            var result = child.Update();
            if (result == State.Success)
                blackboard.shouldReset = false;
            return result;
        }
        return State.Failure;
    }
}
