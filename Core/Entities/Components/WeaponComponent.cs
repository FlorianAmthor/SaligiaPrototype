using SuspiciousGames.Saligia.Core.Components.Weapons;
using SuspiciousGames.Saligia.Core.Entities.Components.Weapons;
using SuspiciousGames.Saligia.Core.Skills;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace SuspiciousGames.Saligia.Core.Entities.Components
{
    [RequireComponent(typeof(CastComponent))]
    public class WeaponComponent : EntityComponent
    {
        [SerializeField] private List<WeaponContainer> _weapons;
        [SerializeField] private bool _activateFirstWeapon;
        [SerializeField] private bool _canUnequipWeapon;

        private WeaponContainer _activeWeapon;
        public int AttackSkillIndex => _activeWeapon.AttackSkillIndex;

        public UnityEvent<WeaponContainer> onWeaponChange;

        public bool IsUnarmed => _activeWeapon == null;

        public bool IsInAttack
        {
            get
            {
                if (IsUnarmed)
                    return false;
                return _activeWeapon.Weapon.IsInAttack;
            }
        }

        private void Start()
        {
            if (_weapons.Count > 0)
                foreach (var weaponContainer in _weapons)
                    weaponContainer.Weapon.SetOwner(Owner);

            InitDefaultWeapon();
        }

        private bool _canQueue;

        private void StartWeaponLogic()
        {
            _activeWeapon.Weapon?.BeginAttack();
            if (_activeWeapon.AttackSkills.Count > 1)
                _activeWeapon.IncreaseAttackIndex();
        }

        private void EndWeaponLogic()
        {
            _activeWeapon.Weapon?.EndAttack();
        }

        /// <summary>
        /// This function should be called when cleaning up a weapon attack or when canceling it prematurely
        /// </summary>
        public void CleanUp()
        {
            if (_activeWeapon == null)
                return;
            _activeWeapon.Weapon?.EndAttack();
            _canQueue = false;
            if (_queueAttackSkill)
            {
                _queueAttackSkill = false;
                AttackWithActiveWeapon();
            }
        }

        private void ResetAlreadyHit()
        {
            _activeWeapon.Weapon?.ResetAlreadyHit();
        }

        public bool _queueAttackSkill;

        private void OnNextSpellQueueable(AnimationEvent animationEvent)
        {
            _canQueue = animationEvent.intParameter == 1 ? true : false;
        }

        public bool AttackWithActiveWeapon()
        {
            if (Owner.CastComponent.HasActiveSkill)
            {
                if (Owner.CastComponent.ActiveSkill.Priority > _activeWeapon.GetActiveAttackSkill().Priority)
                {
                    _canQueue = false;
                    _queueAttackSkill = false;
                    return false;
                }

                if (Owner.CastComponent.ActiveSkill.Priority == _activeWeapon.GetActiveAttackSkill().Priority)
                {
                    if (_canQueue)
                    {
                        if (_activeWeapon.AttackSkillIndex == 2)
                        {
                            _canQueue = false;
                            return false;
                        }
                        Owner.CastComponent.CancelActiveSkill();
                        //_queueAttackSkill = true;
                        //return false;
                    }
                    else
                    {
                        _queueAttackSkill = false;
                        return false;
                    }
                }
            }

            if (_activeWeapon?.AttackSkills == null)
                return false;


            _activeWeapon.CheckForReset();
            var activeSkill = _activeWeapon.GetActiveAttackSkill();

            _canQueue = false;
            return Owner.CastComponent.Cast(activeSkill);
        }

        public bool CastMovementSkill()
        {
            if (_activeWeapon?.MovementSkill == null)
                return false;
            _queueAttackSkill = false;
            _canQueue = false;
            _activeWeapon.Reset();
            Owner.CastComponent.Cast(_activeWeapon.MovementSkill);
            return true;
        }

        public BaseAttackSkill GetActiveAttackSkill()
        {
            if (_activeWeapon == null)
                return null;
            return _activeWeapon.GetActiveAttackSkill();
        }
        public BaseSkill GetActiveMovementSkill()
        {
            if (_activeWeapon == null)
                return null;
            return _activeWeapon.GetActiveMovementSkill();
        }

        private void InitDefaultWeapon()
        {
            if (_activeWeapon != null)
                return;

            if (_canUnequipWeapon && !_activateFirstWeapon)
            {
                _activeWeapon = null;
                return;
            }

            if (_activateFirstWeapon && _weapons.Count > 0)
            {
                _activeWeapon = _weapons[0];

                _activeWeapon?.Weapon.gameObject.SetActive(true);

                _activeWeapon.Reset();
                _activeWeapon.Weapon.Init(OnEnemyHit);
                return;
            }
            Debug.LogWarning("Entity needs a default weapon, but there are no weapons set.");
        }

        private void OnEnemyHit(Entity entity)
        {
            if ((BaseAttackSkill)Owner.CastComponent.ActiveSkill != null)
                ((BaseAttackSkill)Owner.CastComponent.ActiveSkill).OnEnemyHit(entity);
        }

        public bool IsWeaponEquipped(WeaponType weaponType, string weaponName)
        {
            if (_activeWeapon == null)
                return false;
            return _activeWeapon.Identifier.EqualsValues(new WeaponIdentifier(weaponType, weaponName));
        }

        public bool IsWeaponEquipped(WeaponIdentifier weaponIdentifier)
        {
            if (_activeWeapon == null)
                return false;
            return _activeWeapon.Identifier.EqualsValues(weaponIdentifier);
        }

        public bool GetActiveWeapon(out Weapon activeWeapon)
        {
            if (_activeWeapon == null)
            {
                activeWeapon = null;
                return false;
            }

            activeWeapon = _activeWeapon.Weapon;
            return true;
        }

        /// <summary>
        /// Looks for a weapon depending on <paramref name="weaponType"/> and <paramref name="weaponName"/> and switches to it if possible
        /// </summary>
        /// <param name="weaponType"></param>
        /// <param name="weaponName"></param>
        /// <returns>True if the weapon is already equipped or the weapon is switched to</returns>
        public bool SwitchToWeapon(WeaponType weaponType, string weaponName = "")
        {
            if (Owner.CastComponent.HasActiveSkill)
                return false;

            var weaponIdentifier = new WeaponIdentifier(weaponType, weaponName);
            WeaponContainer weaponToChangeTo = _weapons.Find(weaponContainer => weaponContainer.Identifier.EqualsValues(weaponIdentifier));

            if (weaponToChangeTo == null)
                return false;

            if (_activeWeapon == weaponToChangeTo)
                return false;

            if (_activeWeapon != null)
            {
                _activeWeapon.Weapon.gameObject.SetActive(false);
            }
            _activeWeapon = weaponToChangeTo;

            _activeWeapon?.Weapon.gameObject.SetActive(true);

            _activeWeapon.Reset();
            _activeWeapon.Weapon.Init(OnEnemyHit);

            onWeaponChange.Invoke(_activeWeapon);

            return true;
        }

        public bool UnequipWeapon()
        {
            if (!_canUnequipWeapon || Owner.CastComponent.HasActiveSkill || _activeWeapon == null)
                return false;
            _activeWeapon?.Weapon.gameObject.SetActive(false);
            _activeWeapon = null;
            onWeaponChange.Invoke(_activeWeapon);
            return true;
        }

        public void ForceUnequip()
        {
            if (!_canUnequipWeapon || _activeWeapon == null)
                return;
            _activeWeapon?.Weapon.gameObject.SetActive(false);
            _activeWeapon = null;
            onWeaponChange.Invoke(_activeWeapon);
        }
    }
}
