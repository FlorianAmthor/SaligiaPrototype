using SuspiciousGames.Saligia.Core.Components.Weapons;
using SuspiciousGames.Saligia.Core.Skills;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace SuspiciousGames.Saligia.Core.Entities.Components.Weapons
{
    [Serializable]
    public class WeaponContainer
    {
        private int _attackSkillIndex = 0;
        [SerializeField] private float _attackResetTime = 0;
        private float _lastTimeUsed = float.MinValue;
        [field: SerializeField] public WeaponIdentifier Identifier { get; private set; }
        [field: SerializeField] public List<BaseAttackSkill> AttackSkills { get; private set; }
        [field: SerializeField] public BaseSkill MovementSkill { get; private set; }
        [field: SerializeField] public Weapon Weapon { get; private set; }

        public int AttackSkillIndex => _attackSkillIndex;

        public void Reset()
        {
            _attackSkillIndex = 0;
            _lastTimeUsed = float.MinValue;
        }

        public bool CheckForReset()
        {
            if (AttackSkills.Count <= 1)
            {
                _attackSkillIndex = 0;
                return false;
            }
            else
            {
                if (Time.time - _lastTimeUsed >= _attackResetTime)
                {
                    _lastTimeUsed = Time.time;
                    _attackSkillIndex = 0;
                    return true;
                }
            }

            return false;
        }

        public void IncreaseAttackIndex()
        {
            _lastTimeUsed = Time.time;
            _attackSkillIndex = (_attackSkillIndex + 1) % AttackSkills.Count;
        }

        public BaseAttackSkill GetActiveAttackSkill()
        {
            if (AttackSkills.Count == 0)
            {
                Debug.LogWarning($"The {Identifier.weaponType} Weapon {Identifier.weaponName} has no active attack skill!");
                return null;
            }
            return AttackSkills[_attackSkillIndex];
        }

        public BaseAttackSkill GetNextAttackSkill()
        {
            if (AttackSkills.Count == 0)
            {
                Debug.LogWarning($"The {Identifier.weaponType} Weapon {Identifier.weaponName} has no active attack skill!");
                return null;
            }

            var newAttackIndex = (_attackSkillIndex + 1) % AttackSkills.Count;

            return AttackSkills[newAttackIndex];
        }

        public BaseSkill GetActiveMovementSkill()
        {
            return MovementSkill;
        }
    }
}
