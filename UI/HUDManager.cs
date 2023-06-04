using SuspiciousGames.Saligia.Core.Entities.Player;
using SuspiciousGames.Saligia.Core.Potions;
using SuspiciousGames.Saligia.Core.Skills;
using UnityEngine;

namespace SuspiciousGames.Saligia.UI
{
    public class HUDManager : MonoBehaviour
    {
        [SerializeField] private PlayerEntity _playerEntity;
        [SerializeField] private PlayerInventory _playerInventory;

        [Space(10), Header("Resource Bars")]
        [SerializeField] private UIBarUpdater _healthBarUpdater;
        [SerializeField] private UIBarUpdater _mindPowerBarUpdater;

        [Space(10), Header("Skill HUD")]
        [SerializeField] private DiamondUI _northSkillDiamondUI;
        [SerializeField] private DiamondUI _eastSkillDiamondUI;
        [SerializeField] private DiamondUI _southSkillDiamondUI;
        [SerializeField] private DiamondUI _westSkillDiamondUI;
        [SerializeField] private DiamondUI _northEastSkillDiamondUI;
        [SerializeField] private Color _canCastColor;
        [SerializeField] private Color _canNotCastColor;

        [Space(10), Header("Potion HUD")]
        [SerializeField] private DiamondUI _northPotionDiamondUI;
        [SerializeField] private DiamondUI _eastPotionDiamondUI;
        [SerializeField] private DiamondUI _southPotionDiamondUI;
        [SerializeField] private DiamondUI _westPotionDiamondUI;

        private BaseSkill _primarySkill;
        private BaseSkill _movementSkill;
        private BaseSkill _secondarySkillOne;
        private BaseSkill _secondarySkillTwo;
        private BaseSkill _secondarySkillThree;

        #region Ressource Bar Callbacks
        public void OnHealthChange(float healthPercentage)
        {
            _healthBarUpdater.UpdateBar(healthPercentage);
            UpdateSkillUI();
        }

        public void OnMindPowerChange(float mindPowerPercentage)
        {
            _mindPowerBarUpdater.UpdateBar(mindPowerPercentage);
            UpdateSkillUI();
        }
        #endregion

        #region Skill Callbacks
        public void OnSkillCooldownStart(string skillName, float cooldown)
        {
            if (_primarySkill.SkillName == skillName)
                _westSkillDiamondUI.UpdateFillAmount(cooldown / _primarySkill.Cooldown);
            else if (_secondarySkillOne.SkillName == skillName)
                _northSkillDiamondUI.UpdateFillAmount(cooldown / _secondarySkillOne.Cooldown);
            else if (_secondarySkillTwo.SkillName == skillName)
                _eastSkillDiamondUI.UpdateFillAmount(cooldown / _secondarySkillTwo.Cooldown);
            else if (_secondarySkillThree.SkillName == skillName)
                _southSkillDiamondUI.UpdateFillAmount(cooldown / _secondarySkillThree.Cooldown);
            else if (_movementSkill.SkillName == skillName)
                _northEastSkillDiamondUI.UpdateFillAmount(cooldown / _movementSkill.Cooldown);
        }

        public void OnSkillCooldownPassed(string skillName, float cooldown)
        {
            if (_primarySkill.SkillName == skillName)
                _westSkillDiamondUI.UpdateFillAmount(cooldown / _primarySkill.Cooldown);
            else if (_secondarySkillOne.SkillName == skillName)
                _northSkillDiamondUI.UpdateFillAmount(cooldown / _secondarySkillOne.Cooldown);
            else if (_secondarySkillTwo.SkillName == skillName)
                _eastSkillDiamondUI.UpdateFillAmount(cooldown / _secondarySkillTwo.Cooldown);
            else if (_secondarySkillThree.SkillName == skillName)
                _southSkillDiamondUI.UpdateFillAmount(cooldown / _secondarySkillThree.Cooldown);
            else if (_movementSkill.SkillName == skillName)
                _northEastSkillDiamondUI.UpdateFillAmount(cooldown / _movementSkill.Cooldown);
        }

        public void OnSkillCooldownEnd(string skillName)
        {
            if (_primarySkill.SkillName == skillName)
                _westSkillDiamondUI.UpdateFillAmount(0);
            else if (_secondarySkillOne.SkillName == skillName)
                _northSkillDiamondUI.UpdateFillAmount(0);
            else if (_secondarySkillTwo.SkillName == skillName)
                _eastSkillDiamondUI.UpdateFillAmount(0);
            else if (_secondarySkillThree.SkillName == skillName)
                _southSkillDiamondUI.UpdateFillAmount(0);
            else if (_movementSkill.SkillName == skillName)
                _northEastSkillDiamondUI.UpdateFillAmount(0);
        }

        public void OnActiveSkillChanged(SkillSlot skillSlot, BaseSkill skill)
        {
            switch (skillSlot)
            {
                case SkillSlot.Primary:
                    _primarySkill = skill;
                    _westSkillDiamondUI.SetImage(skill.Sprite);
                    _westSkillDiamondUI.UpdateFillAmount(0);
                    break;
                case SkillSlot.SecondaryOne:
                    _secondarySkillOne = skill;
                    _northSkillDiamondUI.SetImage(skill.Sprite);
                    _northSkillDiamondUI.UpdateFillAmount(0);
                    break;
                case SkillSlot.SecondaryTwo:
                    _secondarySkillTwo = skill;
                    _eastSkillDiamondUI.SetImage(skill.Sprite);
                    _eastSkillDiamondUI.UpdateFillAmount(0);
                    break;
                case SkillSlot.SecondaryThree:
                    _secondarySkillThree = skill;
                    _southSkillDiamondUI.SetImage(skill.Sprite);
                    _southSkillDiamondUI.UpdateFillAmount(0);
                    break;
                case SkillSlot.Movement:
                    _movementSkill = skill;
                    _northEastSkillDiamondUI.SetImage(skill.Sprite);
                    _northEastSkillDiamondUI.UpdateFillAmount(0);
                    break;
                default:
                    break;
            }
        }
        #endregion

        #region Potion Callbacks
        public void OnPotionUpgraded(PotionSlot potionSlot)
        {
            UpdateSinglePotionUI(potionSlot);
        }

        public void OnPotionUsed(PotionSlot potionSlot)
        {
            UpdateSinglePotionUI(potionSlot);
        }

        public void OnPotionChargesGained()
        {
            UpdatePotions();
        }

        public void OnPotionSlotsChanged()
        {
            UpdatePotions();
        }

        private void UpdatePotions()
        {
            UpdatePotionUI(_northPotionDiamondUI, _playerInventory.GetPotion(0));
            UpdatePotionUI(_eastPotionDiamondUI, _playerInventory.GetPotion(1));
            UpdatePotionUI(_southPotionDiamondUI, _playerInventory.GetPotion(2));
            UpdatePotionUI(_westPotionDiamondUI, _playerInventory.GetPotion(3));
        }

        private void UpdateSinglePotionUI(PotionSlot potionSlot)
        {
            switch (potionSlot)
            {
                case PotionSlot.North:
                    UpdatePotionUI(_northPotionDiamondUI, _playerEntity.PlayerInventory.GetPotion(0));
                    break;
                case PotionSlot.East:
                    UpdatePotionUI(_eastPotionDiamondUI, _playerEntity.PlayerInventory.GetPotion(1));
                    break;
                case PotionSlot.South:
                    UpdatePotionUI(_southPotionDiamondUI, _playerEntity.PlayerInventory.GetPotion(2));
                    break;
                case PotionSlot.West:
                    UpdatePotionUI(_westPotionDiamondUI, _playerEntity.PlayerInventory.GetPotion(3));
                    break;
                default:
                    break;
            }
        }

        private void UpdatePotionUI(DiamondUI potionDiamondUI, Potion potion)
        {
            if (potion != null)
            {
                if (potion.CurrentNumberOfCharges >= potion.ChargesConsumedOnUse)
                    potionDiamondUI.SetImage(potion.PotionIcon);
                else
                    potionDiamondUI.SetImage(potion.GetEmptySprite());
                potionDiamondUI.SetText("x" + (potion.CurrentNumberOfCharges / potion.ChargesConsumedOnUse).ToString());
            }
            else
            {
                potionDiamondUI.Image.sprite = null;
                potionDiamondUI.SetText(string.Empty);
            }
        }
        #endregion

        private void UpdateSkillUI()
        {
            if (_playerEntity.CastCostComponent.CanPaySkillCost(_primarySkill.SkillCost))
                _westSkillDiamondUI.Image.color = _canCastColor;
            else
                _westSkillDiamondUI.Image.color = _canNotCastColor;

            if (_playerEntity.CastCostComponent.CanPaySkillCost(_secondarySkillOne.SkillCost))
                _northSkillDiamondUI.Image.color = _canCastColor;
            else
                _northSkillDiamondUI.Image.color = _canNotCastColor;

            if (_playerEntity.CastCostComponent.CanPaySkillCost(_secondarySkillTwo.SkillCost))
                _eastSkillDiamondUI.Image.color = _canCastColor;
            else
                _eastSkillDiamondUI.Image.color = _canNotCastColor;

            if (_playerEntity.CastCostComponent.CanPaySkillCost(_secondarySkillThree.SkillCost))
                _southSkillDiamondUI.Image.color = _canCastColor;
            else
                _southSkillDiamondUI.Image.color = _canNotCastColor;
        }
    }
}