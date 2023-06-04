using SuspiciousGames.Saligia.Core.Skills;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering.UI;
using UnityEngine.UI;

namespace SuspiciousGames.Saligia.UI
{
    public class UISpellWindow : UIWindow
    {
        [Header("UI Components")]
        [SerializeField] private TextMeshProUGUI _spellNameText;
        [SerializeField] private TextMeshProUGUI _spellDescriptionText;
        [SerializeField] private Image _spellRune1Image;
        [SerializeField] private TextMeshProUGUI _spellRune1DescriptionText;
        [SerializeField] private Image _spellRune2Image;
        [SerializeField] private TextMeshProUGUI _spellRune2DescriptionText;

        public Rune RuneInFirstSlot { get; private set; }
        public Rune RuneInSecondSlot { get; private set; }

        public BaseSkill CurrentSkill { get; private set; }

        public void UpdateWindowWithCurrentSkill()
        {
            UpdateUIWithSpell(CurrentSkill);
        }

        public void OnSpellSelectionChange(BaseSkill baseSkill, Selectable spellSelectionSelectable)
        {
            var navigation = _spellRune1Image.GetComponent<Selectable>().navigation;
            navigation.selectOnLeft = spellSelectionSelectable;
            _spellRune1Image.GetComponent<Selectable>().navigation = navigation;
            UpdateUIWithSpell(baseSkill);
        }

        private void UpdateUIWithSpell(BaseSkill baseSkill)
        {
            if (!baseSkill)
                return;
            CurrentSkill = baseSkill;
            _spellNameText.text = baseSkill.SkillName;
            _spellDescriptionText.text = baseSkill.SkillDescription;

            switch (baseSkill.Runes.Count)
            {
                case 2:
                    _spellRune1Image.enabled = true;
                    _spellRune2Image.enabled = false;
                    if (baseSkill.Runes.Contains(Rune.Envy))
                    {
                        //_spellRune1Image.sprite = envySprite;
                        _spellRune1DescriptionText.text = baseSkill.SkillDescriptionEnvy;
                        _spellRune2DescriptionText.text = "";
                        RuneInFirstSlot = Rune.Envy;
                        RuneInSecondSlot = Rune.Base;
                    }
                    else if (baseSkill.Runes.Contains(Rune.Gluttony))
                    {
                        //_spellRune1Image.sprite = gluttonySprite;
                        _spellRune1DescriptionText.text = baseSkill.SkillDescriptionGluttony;
                        _spellRune2DescriptionText.text = "";
                        RuneInFirstSlot = Rune.Gluttony;
                        RuneInSecondSlot = Rune.Base;
                    }
                    break;
                case 3:
                    _spellRune1Image.enabled = true;
                    _spellRune2Image.enabled = true;
                    _spellRune1DescriptionText.text = baseSkill.SkillDescriptionEnvy;
                    _spellRune2DescriptionText.text = baseSkill.SkillDescriptionGluttony;
                    RuneInFirstSlot = Rune.Envy;
                    RuneInSecondSlot = Rune.Gluttony;
                    break;
                default:
                    _spellRune1Image.enabled = false;
                    _spellRune2Image.enabled = false;
                    _spellRune1DescriptionText.text = "";
                    _spellRune2DescriptionText.text = "";
                    RuneInFirstSlot = Rune.Base;
                    RuneInSecondSlot = Rune.Base;
                    break;
            }
        }
    }
}