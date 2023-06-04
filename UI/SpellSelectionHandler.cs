using SuspiciousGames.Saligia.Core.Skills;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace SuspiciousGames.Saligia.UI
{
    public class SpellSelectionHandler : MonoBehaviour, ISelectHandler
    {
        [SerializeField] private Selectable _selectableToCheckAgainst;
        [SerializeField] private BaseSkill _baseSkill;
        [SerializeField] private Image _spellSelectionImage;

        public UnityEvent<BaseSkill, Selectable> onSkillSelectionChange;

        private void OnEnable()
        {
            _spellSelectionImage.sprite = _baseSkill.Sprite;
        }

        public void OnSelect(BaseEventData eventData)
        {
            if (eventData.selectedObject == _selectableToCheckAgainst.gameObject)
                onSkillSelectionChange.Invoke(_baseSkill, _selectableToCheckAgainst);
        }
    }
}