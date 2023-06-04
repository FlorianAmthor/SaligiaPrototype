using SuspiciousGames.Saligia.Core.Entities.Buffs;
using SuspiciousGames.Saligia.Core.Entities.Player;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace SuspiciousGames.Saligia.UI
{
    public class UIElementsManager : MonoBehaviour
    {
        [SerializeField] private PlayerEntity _playerEntity;
        [SerializeField] private UIDocument _hud;
        [SerializeField] private Sprite _noWeaponTexture;

        #region HUD
        private VisualElement _healthBar;
        private VisualElement _mpBar;
        private VisualElement _skillIconWest;
        private VisualElement _skillIconNorth;
        private VisualElement _skillIconEast;
        private VisualElement _skillIconSouth;
        private VisualElement _skillIconMove;
        private VisualElement _buffContainer;
        private Label _movementCooldown;
        private Dictionary<BuffData, VisualElement> _buffIcons;
        #endregion

        public void Init()
        {
            FetchAndInitHUDComponents();
            RegisterHUDCallbacks();
        }

        private void FetchAndInitHUDComponents()
        {
            _healthBar = _hud.rootVisualElement.Q("HealthFill");
            UpdateHUDHealthBar(_playerEntity.HealthComponent.HealthPercentage);
            _mpBar = _hud.rootVisualElement.Q("ManaFill");
            UpdateHUDMindPowerBar(_playerEntity.CastCostComponent.MindPowerPercentage);
            _skillIconWest = _hud.rootVisualElement.Q("EquippedAbilityOne");
            _skillIconSouth = _hud.rootVisualElement.Q("EquippedAbilityTwo");
            _skillIconEast = _hud.rootVisualElement.Q("EquippedAbilityThree");
            _skillIconNorth = _hud.rootVisualElement.Q("EquippedAbilityFour");
            _skillIconMove = _hud.rootVisualElement.Q("EquippedMovementAbility");
            UpdateHUDSkillIcons();

            _movementCooldown = _hud.rootVisualElement.Q<Label>("MovementAbilityCooldown");

            _buffContainer = _hud.rootVisualElement.Q("BuffContainer");
            _buffIcons = new Dictionary<BuffData, VisualElement>();

        }

        private void RegisterHUDCallbacks()
        {
            _playerEntity.HealthComponent.OnHealthChanged.AddListener(UpdateHUDHealthBar);
            _playerEntity.CastCostComponent.OnMpChanged.AddListener(UpdateHUDMindPowerBar);
            _playerEntity.CastCostComponent.OnMpChanged.AddListener((_) => UpdateHUDSkillIcons());

            _playerEntity.BuffComponent.BuffAdded.AddListener(AddHUDBuffIcon);
            _playerEntity.BuffComponent.BuffRemoved.AddListener(RemoveHUDBuffIcon);
        }

        private void UpdateHUDHealthBar(float healthPercentage)
        {
            _healthBar.style.width = Length.Percent(healthPercentage * 100);
        }

        private void UpdateHUDMindPowerBar(float mindPowerPercentage)
        {
            _mpBar.style.width = Length.Percent(mindPowerPercentage * 100);
        }

        public void UpdateHUDSkillIcons()
        {
            StyleFloat opacity;

            if (_playerEntity.WeaponComponent.GetActiveAttackSkill())
            {
                opacity = new StyleFloat(1f);
                _skillIconWest.style.backgroundImage = new StyleBackground(_playerEntity.WeaponComponent.GetActiveAttackSkill().Sprite.texture);
                _skillIconMove.style.backgroundImage = new StyleBackground(_playerEntity.WeaponComponent.GetActiveMovementSkill().Sprite.texture);
            }
            else
            {
                opacity = new StyleFloat(0.3f);
                _skillIconWest.style.backgroundImage = new StyleBackground(_noWeaponTexture);
                _skillIconMove.style.backgroundImage = new StyleBackground(_noWeaponTexture);
            }
            _skillIconWest.style.opacity = opacity;
            _skillIconMove.style.opacity = opacity;


            _skillIconSouth.style.backgroundImage = new StyleBackground(_playerEntity.SecondaryAbilityOne.Sprite.texture);
            _skillIconSouth.style.opacity = opacity;

            _skillIconEast.style.backgroundImage = new StyleBackground(_playerEntity.SecondaryAbilityTwo.Sprite.texture);
            _skillIconEast.style.opacity = opacity;

            _skillIconNorth.style.backgroundImage = new StyleBackground(_playerEntity.SecondaryAbilityThree.Sprite.texture);
            _skillIconNorth.style.opacity = opacity;

            if (!_playerEntity.CastCostComponent.CanPaySkillCost(_playerEntity.SecondaryAbilityOne.SkillCost))
                _skillIconSouth.style.opacity = new StyleFloat(0.3f);
            if (!_playerEntity.CastCostComponent.CanPaySkillCost(_playerEntity.SecondaryAbilityTwo.SkillCost))
                _skillIconEast.style.opacity = new StyleFloat(0.3f);
            if (!_playerEntity.CastCostComponent.CanPaySkillCost(_playerEntity.SecondaryAbilityThree.SkillCost))
                _skillIconNorth.style.opacity = new StyleFloat(0.3f);
        }

        private void AddHUDBuffIcon(BuffData buffdata)
        {
            if (_buffIcons.ContainsKey(buffdata))
                return;

            var icon = new VisualElement();
            icon.style.backgroundImage = buffdata.BuffIcon.texture;
            icon.AddToClassList("buffIcon");
            _buffContainer.Add(icon);
            _buffIcons.Add(buffdata, icon);
        }
        private void RemoveHUDBuffIcon(BuffData buffdata)
        {
            if (!_buffIcons.TryGetValue(buffdata, out var icon))
                return;
            _buffContainer.Remove(icon);
            _buffIcons.Remove(buffdata);
        }

        public void BeginMovementCooldown(string skillName, float time)
        {
            if (skillName != _playerEntity.WeaponComponent.GetActiveMovementSkill().SkillName)
                return;
            _movementCooldown.style.display = DisplayStyle.Flex;
            _movementCooldown.text = ((int)(time * 10) / 10).ToString();
        }
        public void UpdateMovementCooldown(string skillName, float time)
        {
            if (skillName != _playerEntity.WeaponComponent.GetActiveMovementSkill().SkillName)
                return;
            _movementCooldown.text = time.ToString("0.0");
        }
        public void EndMovementCooldown(string skillName)
        {
            if (skillName != _playerEntity.WeaponComponent.GetActiveMovementSkill().SkillName)
                return;
            _movementCooldown.style.display = DisplayStyle.None;
        }
    }

}

