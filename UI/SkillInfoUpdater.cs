using SuspiciousGames.Saligia.Core.Entities.Player;
using SuspiciousGames.Saligia.Core.Skills;
using UnityEngine;

namespace SuspiciousGames.Saligia.UI
{
    public class SkillInfoUpdater : MonoBehaviour
    {
        private PlayerEntity _playerEntity;

        private BaseSkill _primarySkill;
        [Header("First Skill")]
        [SerializeField] private LoadoutInfoDisplay _loadoutInfoDisplay1;
        private BaseSkill _secondarySkillOne;
        [Header("Second Skill"), Space(10)]
        [SerializeField] private LoadoutInfoDisplay _loadoutInfoDisplay2;
        private BaseSkill _secondarySkillTwo;
        [Header("Third Skill"), Space(10)]
        [SerializeField] private LoadoutInfoDisplay _loadoutInfoDisplay3;
        private BaseSkill _secondarySkillThree;
        [Header("Fourth Skill"), Space(10)]
        [SerializeField] private LoadoutInfoDisplay _loadoutInfoDisplay4;

        private void OnEnable()
        {
            UpdateInfoDisplays();
        }

        public void UpdateInfoDisplays()
        {
            if (_playerEntity == null)
                _playerEntity = PlayerEntity.Instance;

            _primarySkill = _playerEntity.WeaponComponent.GetActiveAttackSkill();
            _secondarySkillOne = _playerEntity.SecondaryAbilityOne;
            _secondarySkillTwo = _playerEntity.SecondaryAbilityTwo;
            _secondarySkillThree = _playerEntity.SecondaryAbilityThree;

            _loadoutInfoDisplay1.UpdateDisplay(_primarySkill.Sprite, _primarySkill.SkillName);
            _loadoutInfoDisplay2.UpdateDisplay(_secondarySkillOne.Sprite, _secondarySkillOne.SkillName);
            _loadoutInfoDisplay3.UpdateDisplay(_secondarySkillTwo.Sprite, _secondarySkillTwo.SkillName);
            _loadoutInfoDisplay4.UpdateDisplay(_secondarySkillThree.Sprite, _secondarySkillThree.SkillName);
        }
    }
}