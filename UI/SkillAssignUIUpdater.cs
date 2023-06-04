using SuspiciousGames.Saligia.Core.Entities.Player;
using UnityEngine;
using UnityEngine.UI;

namespace SuspiciousGames.Saligia.UI
{
    public class SkillAssignUIUpdater : MonoBehaviour
    {
        [SerializeField] private Image _northSkillImage;
        [SerializeField] private Image _eastSkillImage;
        [SerializeField] private Image _southSkillImage;
        [SerializeField] private Image _westSkillImage;

        private PlayerEntity _playerEntity;

        private void OnEnable()
        {
            UpdateSkillImages();
        }

        public void UpdateSkillImages()
        {
            if (_playerEntity == null)
                _playerEntity = PlayerEntity.Instance;
            _westSkillImage.sprite = _playerEntity.WeaponComponent.GetActiveAttackSkill().Sprite;
            _northSkillImage.sprite = _playerEntity.SecondaryAbilityOne.Sprite;
            _eastSkillImage.sprite = _playerEntity.SecondaryAbilityTwo.Sprite;
            _southSkillImage.sprite = _playerEntity.SecondaryAbilityThree.Sprite;
        }
    }
}