using SuspiciousGames.Saligia.Audio;
using SuspiciousGames.Saligia.Core.Entities.Components;
using SuspiciousGames.Saligia.Core.Entities.Player;
using SuspiciousGames.Saligia.Core.Targeting;
using UnityEngine;

namespace SuspiciousGames.Saligia.Core.Entities
{
    public abstract class Entity : MonoBehaviour
    {
        #region Animation Hashes
        protected static int hitTriggerHash = Animator.StringToHash("HitTrigger");
        protected static int isDeadHash = Animator.StringToHash("IsDead");
        #endregion

        [field: SerializeField, Header("Audio")] public AudioPlayer GruntAudioPlayer { get; private set; }
        [field: SerializeField] public AudioPlayer DeathAudioPlayer { get; private set; }
        [field: SerializeField] public AudioPlayer DeathVFXAudioPlayer { get; private set; }
        [field: SerializeField] public AudioPlayer SpellAudioPlayer { get; private set; }
        [field: SerializeField] public AudioPlayer FootStepAudioPlayer { get; private set; }

        [Space(10), SerializeField] protected float staggerCooldown;
        protected float lastTimeStaggered = float.MinValue;

        public bool IsStunned => _stunnedStacks > 0;
        private int _stunnedStacks = 0;

        public Animator Animator { get; private set; }
        public HealthComponent HealthComponent { get; private set; }
        public MovementComponent MovementComponent { get; private set; }
        public CastComponent CastComponent { get; private set; }
        public CastCostComponent CastCostComponent { get; private set; }
        public BuffComponent BuffComponent { get; private set; }
        public AimingComponent AimingComponent { get; private set; }
        public WeaponComponent WeaponComponent { get; private set; }
        public TargetingComponent TargetingComponent { get; private set; }
        public Vector3 SpawnPosition { get; private set; }
        public Quaternion SpawnRotation { get; private set; }


        private void Start()
        {
            Animator = GetComponent<Animator>();
            HealthComponent = GetComponent<HealthComponent>();
            MovementComponent = GetComponent<MovementComponent>();
            CastComponent = GetComponent<CastComponent>();
            CastCostComponent = GetComponent<CastCostComponent>();
            BuffComponent = GetComponent<BuffComponent>();
            AimingComponent = GetComponent<AimingComponent>();
            WeaponComponent = GetComponent<WeaponComponent>();
            TargetingComponent = GetComponent<TargetingComponent>();

            SpawnPosition = transform.position;
            SpawnRotation = transform.rotation;

            OnStart();
        }

        protected abstract void OnStart();
        public virtual void OnDeath()
        {
            Animator.SetBool(isDeadHash, true);
        }

        //TODO move this into a buff for buffcomponent or smth similar??? also see canMove of MovementComponent to handle this correctly
        public bool IsStaggerImmune { get; private set; }
        [SerializeField] private bool _canBeStaggered;
        [SerializeField] private bool _canBePushed = false;

        public void BlockStaggering(bool value)
        {
            IsStaggerImmune = value;
        }

        public void SetStunned(bool value)
        {
            _stunnedStacks += value ? 1 : -1;
            if (IsStunned)
            {
                if (AimingComponent && AimingComponent.IsAiming)
                    AimingComponent.AbortAiming();
                else if (CastComponent && CastComponent.ActiveSkill != null)
                    CastComponent.SendMessage("OnSkillAnimationEnd");
            }
        }

        public virtual int ApplyDamage(DamageData damageData)
        {
            if (!HealthComponent || HealthComponent.IsDead)
                return 0;

            damageData.actualDamageTakenByEntity = HealthComponent.ApplyDamage(damageData);

            if (_canBeStaggered)
            {
                if (Animator && !HealthComponent.IsDead && damageData.actualDamageTakenByEntity > 0)
                {
                    if (damageData.forceStagger || !IsStaggerImmune && (damageData.canStagger && Time.time - lastTimeStaggered > staggerCooldown))
                    {
                        Animator.SetTrigger(hitTriggerHash);
                        lastTimeStaggered = Time.time;
                        //TODO pushbackImmunity?

                    }
                }
            }
            if (_canBePushed)
            {
                if (MovementComponent && damageData.damageSource && damageData.pushBackForce > 0)
                {
                    MovementComponent.Agent.updateRotation = false;
                    MovementComponent.ForceMove((transform.position - damageData.damageSource.transform.position).normalized * damageData.pushBackForce, true);
                    MovementComponent.Agent.updateRotation = true;
                }
            }
            return damageData.actualDamageTakenByEntity;
        }

        protected abstract void OnFootStepAnimationEvent();

        protected virtual void OnPlayAudioOneShotEvent(AnimationEvent animationEvent)
        {
            AudioPlayer audioPlayerToUse = null;
            switch (animationEvent.stringParameter)
            {
                case "death":
                    audioPlayerToUse = DeathAudioPlayer;
                    break;
                case "grunt":
                    audioPlayerToUse = GruntAudioPlayer;
                    break;
                case "skill":
                    audioPlayerToUse = SpellAudioPlayer;
                    break;
                default:
                    break;
            }
            if (audioPlayerToUse == null)
                return;
            if (animationEvent.floatParameter == 0)
                audioPlayerToUse.PlayClipOneShot((AudioClip)animationEvent.objectReferenceParameter, 1.0f, true);
            else
                audioPlayerToUse.PlayClipOneShot((AudioClip)animationEvent.objectReferenceParameter, animationEvent.floatParameter, true);
        }

        protected virtual void OnPlayAudioEvent(AnimationEvent animationEvent)
        {
            AudioPlayer audioPlayerToUse = null;
            switch (animationEvent.stringParameter)
            {
                case "death":
                    audioPlayerToUse = DeathAudioPlayer;
                    break;
                case "grunt":
                    audioPlayerToUse = GruntAudioPlayer;
                    break;
                case "skill":
                    audioPlayerToUse = SpellAudioPlayer;
                    break;
                default:
                    break;
            }
            if (audioPlayerToUse == null)
                return;
            if (audioPlayerToUse.Playing)
                audioPlayerToUse.AudioSource.Stop();

            audioPlayerToUse.PlayClip((AudioClip)animationEvent.objectReferenceParameter, true);
        }

        protected virtual void OnStopAudioEvent(AnimationEvent animationEvent)
        {
            AudioPlayer audioPlayerToUse = null;
            switch (animationEvent.stringParameter)
            {
                case "death":
                    audioPlayerToUse = DeathAudioPlayer;
                    break;
                case "grunt":
                    audioPlayerToUse = GruntAudioPlayer;
                    break;
                case "skill":
                    audioPlayerToUse = SpellAudioPlayer;
                    break;
                default:
                    break;
            }
            if (audioPlayerToUse == null)
                return;

            if (audioPlayerToUse.Playing)
                audioPlayerToUse.AudioSource.Stop();
        }
    }
}
