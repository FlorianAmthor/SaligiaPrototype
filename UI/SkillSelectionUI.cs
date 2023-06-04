using SuspiciousGames.Saligia.Core.Entities.Player;
using SuspiciousGames.Saligia.Core.Skills;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SuspiciousGames.Saligia.UI
{
    public class SkillSelectionUI : MonoBehaviour
    {
        [SerializeField] private Image _skillImage;
        [SerializeField] private TextMeshProUGUI _skillNameText;
        [SerializeField] private TextMeshProUGUI _skillDescriptionText;

        private Action<BaseSkill> _confirmAction;

        private BaseSkill _skill;

        public void SetUIActions(Action<BaseSkill> skillConfirmAction)
        {
            _confirmAction = skillConfirmAction;
        }

        public void UpdateUI(PlayerEntity playerEntity, BaseSkill skill)
        {
            _skill = skill;
            _skillImage.sprite = _skill.Sprite;
            _skillNameText.text = _skill.SkillName;
            _skillDescriptionText.text = _skill.SkillDescription;
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
            _confirmAction?.Invoke(_skill);
        }
    }
}