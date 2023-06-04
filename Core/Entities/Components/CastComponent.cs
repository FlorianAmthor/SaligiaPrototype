using SuspiciousGames.Saligia.Core.Entities.Player;
using SuspiciousGames.Saligia.Core.Skills;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace SuspiciousGames.Saligia.Core.Entities.Components
{
    public class CastComponent : EntityComponent
    {
        [SerializeField] private bool _isStaggerImmuneWhileCasting;
        [field: SerializeField] public Multiplier DamageMultiplier { get; private set; }

        private EnemyEntity _enemyOwner;

        private Dictionary<string, float> _skillsOnCooldownDictionary;

        public BaseSkill ActiveSkill { get; private set; }

        public bool HasActiveSkill => ActiveSkill != null;

        public TargetData TargetData;

        public UnityEvent<string, float> onSkillCoolDownStart;
        public UnityEvent<string, float> onSkillCoolDownPassed;
        public UnityEvent<string> onSkillCoolDownEnd;

        private BaseSkill _cancelledSkill;

        private void Start()
        {
            _skillsOnCooldownDictionary = new();
            _enemyOwner = Owner as EnemyEntity;
            TargetData = null;
        }

        private void OnDestroy()
        {
            if (Owner.CastComponent)
                Destroy(Owner.CastComponent);
            if (Owner.WeaponComponent != null)
                Destroy(Owner.WeaponComponent);
        }

        private void Update()
        {
            if (Owner.HealthComponent && Owner.HealthComponent.IsDead)
                return;
            foreach (var skillName in _skillsOnCooldownDictionary.Keys.ToList())
            {
                _skillsOnCooldownDictionary[skillName] -= Time.deltaTime;
                onSkillCoolDownPassed.Invoke(skillName, _skillsOnCooldownDictionary[skillName]);
                if (_skillsOnCooldownDictionary[skillName] <= 0.0f)
                {
                    onSkillCoolDownEnd.Invoke(skillName);
                    _skillsOnCooldownDictionary.Remove(skillName);
                }
            }
        }

        public void SetInitialCooldownForSkill(BaseSkill baseSkill, float cooldown)
        {
            onSkillCoolDownStart.Invoke(baseSkill.SkillName, cooldown);
            _skillsOnCooldownDictionary.Add(baseSkill.SkillName, cooldown);
        }

        public void CancelActiveSkill()
        {
            if (!ActiveSkill)
                return;
            ActiveSkill.CleanUp();
            Destroy(ActiveSkill);
            ActiveSkill = null;
        }

        public bool Cast(BaseSkill skill)
        {
            if (skill == null)
            {
                return false;
            }

            if (!CanCast(skill))
                return false;

            if (ActiveSkill != null)
            {
                if (skill.Priority > ActiveSkill.Priority)
                {
                    if (_enemyOwner != null)
                    {
                        _enemyOwner.SetIsInCastAnimation(false);
                        _enemyOwner.SetIsInSkillAnimation(false);
                    }

                    if (Owner.AimingComponent && Owner.AimingComponent.IsAiming)
                    {
                        Owner.AimingComponent.AbortAiming();
                    }
                    else
                    {
                        if (skill)
                            _cancelledSkill = ActiveSkill;
                        _cancelledSkill.CleanUp();
                        Destroy(_cancelledSkill);
                        ActiveSkill = null;
                    }
                }
                else if (skill.Priority == SkillPriority.Medium && skill.Priority == ActiveSkill.Priority)
                {
                    ActiveSkill.CleanUp();
                    Destroy(ActiveSkill);
                    ActiveSkill = null;
                }
                else
                {
                    return false;
                }
            }

            ActiveSkill = Instantiate(skill);

            if (skill.HasAiming && !skill.NeedsTarget)
            {
                if (Owner.TargetingComponent.HasTarget)
                {
                    TargetData = new TargetData(Owner.TargetingComponent.Target.gameObject);
                    // execute skill casting ???
                }
                else
                {
                    Owner.AimingComponent.StartAiming(ActiveSkill.Aiming);
                    return true;
                }
            }
            else if (skill.NeedsTarget)
            {
                if (Owner.TargetingComponent.HasTarget)
                {
                    TargetData = new TargetData(Owner.TargetingComponent.Target.gameObject);
                    // execute skill casting ???
                }
                else
                {
                    // ActiveSkill.Aiming.GetType() != typeof(ObjectLockedSO) && 
                    if (ActiveSkill.HasAiming && Owner.AimingComponent && Owner.TargetingComponent.FindTarget(((PlayerEntity)Owner).MaxActionRange))
                    {
                        Owner.AimingComponent.StartAiming(ActiveSkill.Aiming);
                        return true;
                    }
                    else
                    {
                        if (Owner is PlayerEntity)
                        {
                            if (Owner.TargetingComponent.FindTarget(((PlayerEntity)Owner).MaxActionRange))
                            {
                                TargetData = new TargetData(Owner.TargetingComponent.Target.gameObject);
                                Owner.TargetingComponent.ResetTarget();
                            }
                            else
                            {
                                ActiveSkill = null;
                                return false;
                            }
                        }
                    }
                }
            }

            if (ActiveSkill.HasCastAnimation)
            {
                if (_enemyOwner)
                    _enemyOwner.SetIsInCastAnimation(true);
                Owner.BlockStaggering(true);
                ActiveSkill.CastActivate(Owner);
                //Owner.Animator.SetTrigger(ActiveSkill.CastAnimationTriggerHash);
            }
            else
            {
                if (_enemyOwner)
                    _enemyOwner.SetIsInCastAnimation(false);
                ExecuteActiveSkill();
            }

            return true;
        }

        private void ExecuteActiveSkill()
        {
            if (_enemyOwner)
                _enemyOwner.SetIsInSkillAnimation(true);

            Owner.BlockStaggering(true);

            if (Owner.CastCostComponent && !ActiveSkill.SkillCost.IsPrerequisitOnly)
                Owner.CastCostComponent.Pay(ActiveSkill.SkillCost);
            if (ActiveSkill.Cooldown > 0)
            {
                onSkillCoolDownStart.Invoke(ActiveSkill.SkillName, ActiveSkill.Cooldown);
                _skillsOnCooldownDictionary.Add(ActiveSkill.SkillName, ActiveSkill.Cooldown);
            }

            ActiveSkill.Activate(Owner, TargetData);
            TargetData = null;
        }

        private void OnSkillAnimationEnd()
        {
            if (!ActiveSkill)
                return;

            if (_enemyOwner)
                _enemyOwner.SetIsInSkillAnimation(false);

            if (ActiveSkill)
            {
                ActiveSkill.CleanUp();
                Owner.BlockStaggering(false);
                Destroy(ActiveSkill);
            }

            ActiveSkill = null;
        }

        private void OnSkillAnimationEnd(AnimationEvent animationEvent)
        {
            if (animationEvent.objectReferenceParameter != null)
            {
                BaseSkill skill = (BaseSkill)animationEvent.objectReferenceParameter;

                if (ActiveSkill && skill.SkillName != ActiveSkill.SkillName)
                {
                }
                else
                {
                    if (_cancelledSkill && skill.SkillName == _cancelledSkill.SkillName)
                    {
                        //_lastSkillWasCancelled = false;
                        _cancelledSkill = null;
                        Destroy(_cancelledSkill);
                        return;
                    }
                }
            }

            if (!ActiveSkill)
                return;

            if (_enemyOwner)
                _enemyOwner.SetIsInSkillAnimation(false);

            if (ActiveSkill)
            {
                ActiveSkill.CleanUp();
                Owner.BlockStaggering(false);
                Destroy(ActiveSkill);
            }

            ActiveSkill = null;
        }

        public void OnAimingEnd()
        {
            if (ActiveSkill.HasCastAnimation)
            {
                ActiveSkill.CastActivate(Owner);
                //Owner.Animator.SetTrigger(ActiveSkill.CastAnimationTriggerHash);
            }
            else
            {
                ExecuteActiveSkill();
            }
        }

        private void ExecuteAdditionalSkillLogic()
        {
            if (ActiveSkill)
                ActiveSkill.AnimationTriggeredLogic();
        }

        public void OnCastExecutionPointReached(AnimationEvent animationEvent)
        {
            if (animationEvent.objectReferenceParameter != null)
            {
                BaseSkill skill = (BaseSkill)animationEvent.objectReferenceParameter;
                if (skill.SkillName != ActiveSkill.SkillName)
                    return;
            }

            //TODO this is for the errow when a spell gets canceled but its animation event is still fired, this needs another fix
            if (_skillsOnCooldownDictionary.ContainsKey(ActiveSkill.SkillName))
                return;
            if (_enemyOwner)
                _enemyOwner.SetIsInCastAnimation(false);
            ExecuteActiveSkill();
        }

        public bool IsSkillOnCooldown(BaseSkill skill)
        {
            return _skillsOnCooldownDictionary.ContainsKey(skill.SkillName);
        }

        private bool CanCast(BaseSkill skill)
        {
            //TODO cc???
            if (IsSkillOnCooldown(skill) || Owner.IsStunned)
                return false;

            if (Owner.GetType() == typeof(PlayerEntity))
            {
                if (Owner.WeaponComponent.IsUnarmed)
                    return false;
            }

            if (!Owner.CastCostComponent)
                return true;

            return Owner.CastCostComponent.CanPaySkillCost(skill.SkillCost);
        }

        public float CalculateMpCost(SkillCost cost)
        {
            if (!Owner.CastCostComponent)
                return 0;
            return Owner.CastCostComponent.CalculateMpCost(cost);
        }

        public int CalculateLifeCost(SkillCost cost)
        {
            if (!Owner.CastCostComponent)
                return 0;
            return Owner.CastCostComponent.CalculateLifeCost(cost);
        }
    }
}