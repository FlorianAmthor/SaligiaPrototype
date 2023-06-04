using SuspiciousGames.Saligia.Core.Entities.Player;
using SuspiciousGames.Saligia.Core.Skills;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SuspiciousGames.Saligia.UI
{
    public class SpellSelectionWindow : UIWindow
    {
        [Header("UI Prefabs")]
        [SerializeField] private SkillSelectionUI _skillSelectionUIPrefab;

        [Space(10), Header("Gameobject References")]
        [SerializeField] private GameObject _selectionWindowContentGameObject;

        private List<SkillSelectionUI> _primarySkillSelectionUIs = new();
        private List<SkillSelectionUI> _secondarySkillSelectionUIs = new();

        private PlayerInventory _playerInventory => PlayerEntity.Instance.PlayerInventory;
        private int _currentSelectionSkillIndex = 0;

        public void UpdateSkillSelection(int selectedSkillSlot)
        {
            _currentSelectionSkillIndex = selectedSkillSlot;

            if (_primarySkillSelectionUIs == null)
                _primarySkillSelectionUIs = new();

            if (_secondarySkillSelectionUIs == null)
                _secondarySkillSelectionUIs = new();

            if (_primarySkillSelectionUIs.Count == 0)
            {
                var skillUi = Instantiate(_skillSelectionUIPrefab, _selectionWindowContentGameObject.transform);
                skillUi.SetUIActions(OnSkillSelect);
                _primarySkillSelectionUIs.Add(skillUi);

                skillUi = Instantiate(_skillSelectionUIPrefab, _selectionWindowContentGameObject.transform);
                skillUi.SetUIActions(OnSkillSelect);
                _primarySkillSelectionUIs.Add(skillUi);
            }

            if (_secondarySkillSelectionUIs.Count == 0)
            {
                for (int i = 0; i < _playerInventory.PlayerInventoryData.SecondarySkills.Count; i++)
                {
                    var skillUi = Instantiate(_skillSelectionUIPrefab, _selectionWindowContentGameObject.transform);
                    skillUi.SetUIActions(OnSkillSelect);
                    _secondarySkillSelectionUIs.Add(skillUi);
                }
            }

            PlayerEntity playerEntity = PlayerEntity.Instance;
            LinkedList<SkillSelectionUI> activeUIs = new LinkedList<SkillSelectionUI>();

            if (selectedSkillSlot == (int)SkillSlot.Primary)
            {
                foreach (var primarySkillUI in _primarySkillSelectionUIs)
                    primarySkillUI.gameObject.SetActive(false);
                foreach (var secondarySkillUI in _secondarySkillSelectionUIs)
                    secondarySkillUI.gameObject.SetActive(false);

                BaseSkill skill = _playerInventory.PlayerInventoryData.GrimoireSkill;
                _primarySkillSelectionUIs[0].UpdateUI(playerEntity, skill);
                bool isActive = (playerEntity.GetAbilityBySlot((SkillSlot)selectedSkillSlot) != skill);
                _primarySkillSelectionUIs[0].gameObject.SetActive(isActive);
                if (isActive)
                    activeUIs.AddLast(new LinkedListNode<SkillSelectionUI>(_primarySkillSelectionUIs[0]));

                skill = _playerInventory.PlayerInventoryData.ScytheSkill;
                _primarySkillSelectionUIs[1].UpdateUI(playerEntity, skill);
                isActive = (playerEntity.GetAbilityBySlot((SkillSlot)selectedSkillSlot) != skill);
                _primarySkillSelectionUIs[1].gameObject.SetActive(isActive);
                if (isActive)
                    activeUIs.AddLast(new LinkedListNode<SkillSelectionUI>(_primarySkillSelectionUIs[1]));
            }
            else
            {
                foreach (var primarySkillUI in _primarySkillSelectionUIs)
                    primarySkillUI.gameObject.SetActive(false);
                foreach (var secondarySkillUI in _secondarySkillSelectionUIs)
                    secondarySkillUI.gameObject.SetActive(false);

                for (int i = 0; i < _playerInventory.PlayerInventoryData.SecondarySkills.Count; i++)
                {
                    BaseSkill skill = _playerInventory.PlayerInventoryData.SecondarySkills[i];
                    _secondarySkillSelectionUIs[i].UpdateUI(playerEntity, skill);
                    bool isActive = (playerEntity.GetAbilityBySlot((SkillSlot)selectedSkillSlot) != skill);
                    _secondarySkillSelectionUIs[i].gameObject.SetActive(isActive);
                    if (isActive)
                        activeUIs.AddLast(new LinkedListNode<SkillSelectionUI>(_secondarySkillSelectionUIs[i]));
                }
            }

            var currentSkillUINode = activeUIs.First;

            while (currentSkillUINode != null)
            {
                currentSkillUINode.Value.SetVerticalUpSelectable(currentSkillUINode.Previous?.Value.GetComponent<Selectable>());
                currentSkillUINode.Value.SetVerticalDownSelectable(currentSkillUINode.Next?.Value.GetComponent<Selectable>());
                currentSkillUINode = currentSkillUINode.Next;
            }

            if (activeUIs.Count > 0)
                activeUIs.First.Value.GetComponent<Selectable>().Select();
        }

        private void OnSkillSelect(BaseSkill skill)
        {
            if ((SkillSlot)_currentSelectionSkillIndex == SkillSlot.Primary)
            {
                if (PlayerEntity.Instance.CurrentPlayerWeaponType == Core.Player.PlayerWeaponType.Scythe)
                    _playerInventory.PlayerInventoryData.ChangeActiveWeapon(Core.Player.PlayerWeaponType.Grimoire);
                else
                    _playerInventory.PlayerInventoryData.ChangeActiveWeapon(Core.Player.PlayerWeaponType.Scythe);
            }
            else
                _playerInventory.PlayerInventoryData.ChangeActiveSkillBySkillSlot(skill, (SkillSlot)_currentSelectionSkillIndex);

            //PlayerEntity.Instance.ChangeSecondaryAbility(skill, (SkillSlot)_currentSelectionSkillIndex);
            MenuManager.Instance.CloseWindow(this);
        }

        public override void Close()
        {
            base.Close();
            _currentSelectionSkillIndex = 0;
        }
    }
}