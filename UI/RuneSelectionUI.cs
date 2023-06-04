using SuspiciousGames.Saligia.Core.Entities.Player;
using SuspiciousGames.Saligia.Core.Skills;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.UI;
using static SuspiciousGames.Saligia.UI.RuneSelectionWindow;

namespace SuspiciousGames.Saligia.UI
{
    public class RuneSelectionUI : MonoBehaviour
    {
        [SerializeField] private Image _runeImage;
        [SerializeField] private TextMeshProUGUI _runeNameText;
        [SerializeField] private TextMeshProUGUI _runeDescriptionText;
        [SerializeField] private TextMeshProUGUI _runeInUseText;

        public Rune Rune { get; private set; }
        private RuneAction _runeAction;
        private BaseSkill _currentSkill;
        private BaseSkill _equippedInSkill;
        private Action<Rune, BaseSkill, RuneAction> _confirmAction;

        private LocalizedString _localizedRuneName = new("GreatRunes", "");
        private LocalizedString _localizedRuneHint = new("GreatRunes", "runeHint");

        public void SetUIActions(Action<Rune, BaseSkill, RuneAction> runeConfirmAction)
        {
            _confirmAction = runeConfirmAction;
        }

        public void SetRune(Rune rune)
        {
            Rune = rune;
        }

        public void UpdateUI(BaseSkill currentSkill, RuneAction runeAction)
        {
            _currentSkill = currentSkill;
            _runeAction = runeAction;

            if (_localizedRuneName == null)
                _localizedRuneName = new LocalizedString();

            if (_runeAction == RuneAction.Remove)
            {
                _localizedRuneName.TableEntryReference = "remove";
                _runeNameText.text = "";
                _runeDescriptionText.text = _localizedRuneName.GetLocalizedString();
                _runeInUseText.gameObject.SetActive(false);
            }
            else
            {
                _localizedRuneName.TableEntryReference = Rune.ToString();
                _runeNameText.text = _localizedRuneName.GetLocalizedString();
                switch (Rune)
                {
                    case Rune.Envy:
                        _runeDescriptionText.text = currentSkill.SkillDescriptionEnvy;
                        break;
                    case Rune.Gluttony:
                        _runeDescriptionText.text = currentSkill.SkillDescriptionGluttony;
                        break;
                    default:
                        _runeDescriptionText.text = "";
                        break;
                }

                var playerInventory = PlayerEntity.Instance.PlayerInventory;
                _equippedInSkill = playerInventory.GetSpellForEquippedRune(Rune);
                if (_equippedInSkill != null && _equippedInSkill != currentSkill)
                {
                    _runeInUseText.text = _localizedRuneHint.GetLocalizedString(_equippedInSkill.SkillName);
                    _runeInUseText.gameObject.SetActive(true);

                }
                else
                {
                    _runeInUseText.gameObject.SetActive(false);
                }
            }
        }

        public void SetVerticalDownSelectable(Selectable selectableOnDown)
        {
            var selectable = GetComponent<Selectable>();
            var navigation = selectable.navigation;
            navigation.mode = Navigation.Mode.Explicit;
            navigation.selectOnDown = selectableOnDown;
            navigation.wrapAround = false;
            selectable.navigation = navigation;
        }

        public void SetVerticalUpSelectable(Selectable selectableOnUp)
        {
            var selectable = GetComponent<Selectable>();
            var navigation = selectable.navigation;
            navigation.mode = Navigation.Mode.Explicit;
            navigation.selectOnUp = selectableOnUp;
            navigation.wrapAround = false;
            selectable.navigation = navigation;
        }

        public void OnConfirm()
        {
            _confirmAction?.Invoke(Rune, _currentSkill, _runeAction);
        }
    }
}