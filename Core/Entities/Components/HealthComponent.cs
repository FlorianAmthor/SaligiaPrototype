using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

namespace SuspiciousGames.Saligia.Core.Entities.Components
{
    //[RequireComponent(typeof(UIDocument))]
    public class HealthComponent : EntityComponent
    {
        [SerializeField] private int _maxHitPoints;
        [SerializeField] private bool _isInvulnerable;
        [SerializeField] private bool _showFloatingCombatText;
        [SerializeField] private GameObject _combatTextPrefab;
        [SerializeField] private bool _showFloatingHealthBar;
        [SerializeField] private Vector3 _floatingHealthBarOffset;

        public int MaxHitPoints => _maxHitPoints;
        public int CurrentHitPoints { get; private set; }
        public float HealthPercentage => CurrentHitPoints / (float)MaxHitPoints;
        public bool IsInvulnerable => _isInvulnerable;
        public bool IsDead => CurrentHitPoints <= 0;
        public bool IsFullLife => CurrentHitPoints == _maxHitPoints;
        [field: SerializeField] public Multiplier DamageTakenMultiplier { get; private set; }

        public UnityEvent<Entity> OnDeath;
        public UnityEvent<DamageData> OnReceiveDamage;
        public UnityEvent<int> OnReceiveHealth;
        public UnityEvent OnHitWhileInvulnerable;
        public UnityEvent<float> OnHealthChanged;

        private VisualElement _floatingHealthBar;
        private Camera _camera;

        private void Start()
        {
            _camera = Camera.main;
        }

        private void Awake()
        {
            DamageTakenMultiplier.ResetMultiplier();
            ResetHealth();
            _floatingHealthBar = GetComponent<UIDocument>().rootVisualElement.Q("FloatingHealthBar");
            _floatingHealthBar.visible = false;
            OnReceiveDamage.AddListener(e => OnHealthChanged.Invoke(HealthPercentage));
            OnReceiveHealth.AddListener(e => OnHealthChanged.Invoke(HealthPercentage));
        }

        private void OnDestroy()
        {
            if (TryGetComponent(out CastCostComponent castCostComponent))
                Destroy(castCostComponent);
            if (Owner.BuffComponent)
                Destroy(Owner.BuffComponent);
            if (TryGetComponent(out UIDocument uIDocument))
                Destroy(uIDocument);
        }
        private void FixedUpdate()
        {
            SetHealthBarPosition();
        }

        public void ResetHealth()
        {
            CurrentHitPoints = _maxHitPoints;
        }

        internal int ApplyDamage(DamageData damageData)
        {
            if (IsDead)
                return 0;

            if (IsInvulnerable)
            {
                OnHitWhileInvulnerable?.Invoke();
                return 0;
            }

            if (damageData.damageAmount <= 0)
                return 0;

            int actualDamage = damageData.damageAmount;

            if (!damageData.isTrueDamage)
                actualDamage = Mathf.RoundToInt(damageData.damageAmount * DamageTakenMultiplier.Value);

            //CurrentHitPoints -= actualDamage;
            CurrentHitPoints = Mathf.Clamp(CurrentHitPoints - actualDamage, 0, _maxHitPoints);

            if (_showFloatingHealthBar && CurrentHitPoints < _maxHitPoints)
            {
                float percentage = (float)CurrentHitPoints / _maxHitPoints * 100;
                _floatingHealthBar.Q("HealthBar").style.width = Length.Percent(percentage);
                _floatingHealthBar.visible = true;
            }

            //TODO this should later be set by the game options in general and locally so for example the player can still ignore this combat text over his head
            if (_showFloatingCombatText)
            {
                DamagePopup.Create(_combatTextPrefab, transform.position + Vector3.up, actualDamage);
            }

            damageData.actualDamageTakenByEntity = actualDamage;
            OnReceiveDamage.Invoke(damageData);

            if (CurrentHitPoints <= 0)
                OnDeath.Invoke(Owner);

            return actualDamage;
        }

        public void ApplyHealing(int amount, MonoBehaviour source = null)
        {
            if (IsDead)
                return;

            CurrentHitPoints = Mathf.Clamp(CurrentHitPoints + amount, 0, _maxHitPoints);
            if (_showFloatingHealthBar && CurrentHitPoints == _maxHitPoints)
                _floatingHealthBar.visible = false;

            OnReceiveHealth.Invoke(amount);
        }

        public void SetCurrentHitPoints(int value)
        {
            CurrentHitPoints = value;
            OnHealthChanged.Invoke(HealthPercentage);
        }
        public void SetCurrentHitPoints(float value)
        {
            CurrentHitPoints = (int)value;
            OnHealthChanged.Invoke(HealthPercentage);
        }

        public void StartRegenerateUntilFull(float percentPerSecond)
        {
            StartCoroutine(RegenerateUntilFull(percentPerSecond));
        }

        private IEnumerator RegenerateUntilFull(float percentPerSecond)
        {
            while (CurrentHitPoints < _maxHitPoints)
            {
                ApplyHealing((int)(_maxHitPoints * percentPerSecond));
                yield return new WaitForSeconds(1);
            }
        }

        private void SetHealthBarPosition()
        {
            if (Owner == null || Owner.MovementComponent == null || Owner.MovementComponent.Agent == null)
                return;
            
            Vector3 followPos = transform.position + _floatingHealthBarOffset;
            followPos.y += Owner.MovementComponent.Agent.height;
            Vector2 newPosition = RuntimePanelUtils.CameraTransformWorldToPanel(
                _floatingHealthBar.panel, followPos, _camera);
            newPosition.x -= _floatingHealthBar.layout.width / 2;
            _floatingHealthBar.transform.position = newPosition;
        }
    }
}
