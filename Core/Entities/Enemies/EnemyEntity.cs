using Gamekit3D;
using Gamekit3Dk;
using SuspiciousGames.Saligia.Core.Entities.Components;
using SuspiciousGames.Saligia.Core.Entities.Player;
using System;
using System.Collections;
using System.Collections.Generic;
using TheKiwiCoder;
using UnityEngine;
using UnityEngine.Events;

namespace SuspiciousGames.Saligia.Core.Entities
{
    public class EnemyEntity : Entity
    {
        #region General Animation Hashes
        [HideInInspector] public int runTriggerHash = Animator.StringToHash("RunTrigger");
        [HideInInspector] public int customIdleTriggerHash = Animator.StringToHash("CustomIdleTrigger");
        [HideInInspector] public int idleTriggerHash = Animator.StringToHash("IdleTrigger");
        #endregion

        public PlayerScanner PlayerScanner { get; private set; }

        protected bool isResetting = false;
        protected List<MonoBehaviour> monoBehavioursToDestroyOnDeath;

        protected BehaviourTreeRunner behaviourTreeRunner;
        public BehaviourTreeRunner BehaviourTreeRunner => behaviourTreeRunner;

        [field: SerializeField] public int PotionChargesOnDeath { get; protected set; } = 5;

        [SerializeField] protected ParticleSystem onDeathEffectParticleSystem;
        [SerializeField] protected Renderer modelRenderer;
        [SerializeField] protected Color receiveDamageColor;
        [SerializeField] protected float _blinkOnDamageDuration = 0.1f;
        protected Color originalMaterialColor = Color.white;

        public UnityEvent<bool> onIsAggroed;

        protected override void OnStart()
        {
            PlayerScanner = GetComponent<PlayerScanner>();
            monoBehavioursToDestroyOnDeath = new List<MonoBehaviour>();
            behaviourTreeRunner = GetComponent<BehaviourTreeRunner>();

            if (HealthComponent != null)
                monoBehavioursToDestroyOnDeath.Add(HealthComponent);
            if (MovementComponent != null)
                monoBehavioursToDestroyOnDeath.Add(MovementComponent);
            if (WeaponComponent != null)
                monoBehavioursToDestroyOnDeath.Add(WeaponComponent);
            if (CastComponent != null)
                monoBehavioursToDestroyOnDeath.Add(CastComponent);
            if (BuffComponent != null)
                monoBehavioursToDestroyOnDeath.Add(BuffComponent);
            if (PlayerScanner != null)
                monoBehavioursToDestroyOnDeath.Add(PlayerScanner);
            if (behaviourTreeRunner != null)
                monoBehavioursToDestroyOnDeath.Add(behaviourTreeRunner);
            if (TargetingComponent != null)
                monoBehavioursToDestroyOnDeath.Add(TargetingComponent);

            PlayerEntity.Instance.onDeath += OnPlayerDeath;

            if (modelRenderer.material.name.StartsWith("M_RT"))
                originalMaterialColor = modelRenderer.material.GetColor("_MainColor");
            else
                originalMaterialColor = modelRenderer.material.color;

            if (originalMaterialColor == receiveDamageColor)
                originalMaterialColor = Color.white;

            if (EnemyManager.Instance)
                EnemyManager.Instance.AddEnemy(this);

            behaviourTreeRunner.OnInitialized.AddListener(OnBehaviourTreeRunnerInitialized);
        }

        private void OnBehaviourTreeRunnerInitialized()
        {
            behaviourTreeRunner.tree.blackboard.OnIsAggroedChanged.AddListener(onIsAggroed.Invoke);
            behaviourTreeRunner.OnInitialized.RemoveListener(OnBehaviourTreeRunnerInitialized);
        }

        private void OnPlayerDeath(Entity playerEntity)
        {
            PlayerEntity player = playerEntity as PlayerEntity;
            if (!player)
                return;
            behaviourTreeRunner.tree.blackboard.shouldReset = true;
            player.HealthComponent.OnDeath.RemoveListener(OnPlayerDeath);
            PlayerEntity.Instance.onDeath -= OnPlayerDeath;
        }

        public void ForceTarget(Entity entity)
        {
            behaviourTreeRunner.tree.blackboard.target = entity;
            behaviourTreeRunner.tree.blackboard.isAggroed = true;

            PlayerScanner.CanReset = false;

            if (behaviourTreeRunner.tree.blackboard.distributor != null && behaviourTreeRunner.tree.blackboard.followerData != null)
            {
                behaviourTreeRunner.tree.blackboard.distributor.UnregisterFollower(behaviourTreeRunner.tree.blackboard.followerData);
                behaviourTreeRunner.tree.blackboard.followerData = null;
            }
            behaviourTreeRunner.tree.blackboard.distributor = behaviourTreeRunner.tree.blackboard.target.GetComponent<TargetDistributor>();
            if (behaviourTreeRunner.tree.blackboard.followerData == null)
                behaviourTreeRunner.tree.blackboard.followerData = behaviourTreeRunner.tree.blackboard.distributor.RegisterNewFollower();
        }

        public override int ApplyDamage(DamageData damageData)
        {
            int actualDamage = base.ApplyDamage(damageData);
            if (!HealthComponent || HealthComponent.IsDead)
                return actualDamage;

            if (actualDamage != 0)
                StartCoroutine(AnimatHitCoroutine(modelRenderer, _blinkOnDamageDuration));

            if (damageData.damageSource && !behaviourTreeRunner.tree.blackboard.target)
            {
                if (damageData.damageSource.TryGetComponent(out HealthComponent healthComponent) && damageData.damageSource.gameObject != gameObject)
                {
                    behaviourTreeRunner.tree.blackboard.target = healthComponent.Owner;
                    behaviourTreeRunner.tree.blackboard.isAggroed = true;
                    behaviourTreeRunner.tree.blackboard.shouldReset = false;
                }
            }

            return actualDamage;
        }

        private IEnumerator AnimatHitCoroutine(Renderer renderer, float seconds)
        {
            //float timeSoFar = 0f;

            //while (timeSoFar < seconds)
            //{
            //    timeSoFar += Time.deltaTime;
            //    yield return new WaitForEndOfFrame();
            //}
            if (renderer.material.name.StartsWith("M_RT"))
            {
                renderer.material.SetColor("_MainColor", receiveDamageColor);
                yield return new WaitForSeconds(seconds);
                renderer.material.SetColor("_MainColor", originalMaterialColor);
            }
            else
            {
                renderer.material.color = receiveDamageColor;
                yield return new WaitForSeconds(seconds);
                renderer.material.color = originalMaterialColor;
            }
        }

        public override void OnDeath()
        {
            if (EnemyManager.Instance)
                EnemyManager.Instance.OnEnemyDeath(this);

            behaviourTreeRunner.tree.blackboard.OnIsAggroedChanged.RemoveListener(onIsAggroed.Invoke);

            foreach (var mb in monoBehavioursToDestroyOnDeath)
                Destroy(mb);
            base.OnDeath();
            PlayerEntity.Instance.onDeath -= OnPlayerDeath;

            if (TryGetComponent(out Collider col))
                Destroy(col);
            gameObject.layer = 0;//Default layer
        }

        private void OnDeathAnimationEnd()
        {
            modelRenderer.enabled = false;
            if (DeathVFXAudioPlayer)
                DeathVFXAudioPlayer.PlayRandomClip();
            if (onDeathEffectParticleSystem)
                onDeathEffectParticleSystem.Play();
            Destroy(gameObject, 1);
        }

        public void IsResetting(bool value)
        {
            isResetting = value;
        }

        public void StartReset()
        {
            StartCoroutine(ResetCoroutine());
        }

        protected override void OnFootStepAnimationEvent()
        {
            if (!FootStepAudioPlayer || !MovementComponent || !MovementComponent.CanMove)
                return;
            FootStepAudioPlayer.PlayRandomClip();
        }

        protected IEnumerator ResetCoroutine()
        {
            while (isResetting)
                yield return new WaitForEndOfFrame();
            transform.rotation = SpawnRotation;
        }

        public void SetIsInCastAnimation(bool value)
        {
            behaviourTreeRunner.tree.blackboard.isInCastAnimation = value;
        }

        public void SetIsInSkillAnimation(bool value)
        {
            behaviourTreeRunner.tree.blackboard.isInSkillAnimation = value;
        }
    }
}
