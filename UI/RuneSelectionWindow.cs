using SuspiciousGames.Saligia.Core.Entities.Player;
using SuspiciousGames.Saligia.Core.Skills;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SuspiciousGames.Saligia.UI
{
    public class RuneSelectionWindow : UIWindow
    {
        [Header("UI Prefabs")]
        [SerializeField] private RuneSelectionUI _runeSelectionUIPrefab;

        [Space(10), Header("Gameobject References")]
        [SerializeField] private GameObject _selectionWindowContentGameObject;

        private PlayerInventory _playerInventory => PlayerEntity.Instance.PlayerInventory;
        private List<RuneSelectionUI> _runeSelectionUIs = new();

        private UISpellWindow _spellWindow;
        private int _currentRuneSlotIndex;

        public void UpdateRuneSelection(int runeSlotIndex)
        {
            if (!_spellWindow)
                _spellWindow = FindObjectOfType<UISpellWindow>();

            var currentSkill = _spellWindow.CurrentSkill;
            _currentRuneSlotIndex = runeSlotIndex;

            if (_runeSelectionUIs == null)
                _runeSelectionUIs = new();

            if (_runeSelectionUIs.Count == 0)
            {
                foreach (Rune rune in Enum.GetValues(typeof(Rune)))
                {
                    var runeSelectionUI = Instantiate(_runeSelectionUIPrefab, _selectionWindowContentGameObject.transform);
                    runeSelectionUI.SetUIActions(OnRuneSelect);
                    runeSelectionUI.SetRune(rune);
                    _runeSelectionUIs.Add(runeSelectionUI);
                }
            }

            LinkedList<RuneSelectionUI> activeUIs = new LinkedList<RuneSelectionUI>();

            foreach (var runeSelectionUI in _runeSelectionUIs)
                runeSelectionUI.gameObject.SetActive(false);

            HashSet<Rune> runesToActivate = new() { Rune.Base, Rune.Envy, Rune.Gluttony };

            for (int i = 0; i < _runeSelectionUIs.Count; i++)
            {
                if (!runesToActivate.Contains(_runeSelectionUIs[i].Rune))
                    continue;
                bool isActive = false;

                if (_runeSelectionUIs[i].Rune == Rune.Base)
                {
                    if (runeSlotIndex == 0)
                    {
                        _runeSelectionUIs[i].SetRune(_spellWindow.RuneInFirstSlot);
                        isActive = _spellWindow.RuneInFirstSlot != Rune.Base;
                    }
                    else
                    {
                        _runeSelectionUIs[i].SetRune(_spellWindow.RuneInSecondSlot);
                        isActive = _spellWindow.RuneInSecondSlot != Rune.Base;
                    }

                    _runeSelectionUIs[i].gameObject.SetActive(isActive);
                    _runeSelectionUIs[i].UpdateUI(currentSkill, RuneAction.Remove);
                    if (isActive)
                        activeUIs.AddLast(new LinkedListNode<RuneSelectionUI>(_runeSelectionUIs[i]));
                }
                else if (_runeSelectionUIs[i].Rune == Rune.Envy)
                {
                    isActive = _spellWindow.RuneInFirstSlot != Rune.Envy & _spellWindow.RuneInSecondSlot != Rune.Envy & _playerInventory.PlayerInventoryData.UnlockedTier2Runes.HasFlag(PlayerInventoryData.RuneFlags.Envy);
                    _runeSelectionUIs[i].gameObject.SetActive(isActive);
                    if (isActive)
                    {
                        _runeSelectionUIs[i].UpdateUI(currentSkill, RuneAction.AddOrSwap);
                        activeUIs.AddLast(new LinkedListNode<RuneSelectionUI>(_runeSelectionUIs[i]));
                    }
                }
                else if (_runeSelectionUIs[i].Rune == Rune.Gluttony)
                {
                    isActive = _spellWindow.RuneInFirstSlot != Rune.Gluttony & _spellWindow.RuneInSecondSlot != Rune.Gluttony & _playerInventory.PlayerInventoryData.UnlockedTier2Runes.HasFlag(PlayerInventoryData.RuneFlags.Gluttony);
                    _runeSelectionUIs[i].gameObject.SetActive(isActive);
                    if (isActive)
                    {
                        _runeSelectionUIs[i].UpdateUI(currentSkill, RuneAction.AddOrSwap);
                        activeUIs.AddLast(new LinkedListNode<RuneSelectionUI>(_runeSelectionUIs[i]));
                    }
                }
            }

            var currentSkillUINode = activeUIs.First;

            while (currentSkillUINode != null)
            {
                currentSkillUINode.Value.SetVerticalUpSelectable(currentSkillUINode.Previous?.Value.GetComponent<Button>());
                currentSkillUINode.Value.SetVerticalDownSelectable(currentSkillUINode.Next?.Value.GetComponent<Button>());
                currentSkillUINode = currentSkillUINode.Next;
            }

            if (activeUIs.Count > 0)
                activeUIs.First.Value.GetComponent<Selectable>().Select();
        }

        public enum RuneAction
        {
            AddOrSwap,
            Remove
        }

        private void OnRuneSelect(Rune rune, BaseSkill baseSkill, RuneAction runeAction)
        {
            if (runeAction == RuneAction.Remove)
            {
                _playerInventory.PlayerInventoryData.RemoveRuneFromSkill(rune);
            }
            else
            {
                _playerInventory.PlayerInventoryData.AddRuneToSkill(rune, baseSkill);
            }

            MenuManager.Instance.CloseWindow(this);
        }

        public override void Close()
        {
            base.Close();
            if (_runeSelectionUIs.Count > 0)
                _runeSelectionUIs[0].SetRune(Rune.Base);
            _currentRuneSlotIndex = 0;
        }
    }
}