using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace SuspiciousGames.Saligia.UI
{
    public class QuestGroupSelectionHandler : MonoBehaviour, ISelectHandler
    {
        [SerializeField] private Selectable _selectableToCheckAgainst;

        public UnityEvent<Selectable> onQuestGroupSelectionChange;

        public void OnSelect(BaseEventData eventData)
        {
            if (eventData.selectedObject == _selectableToCheckAgainst.gameObject)
                onQuestGroupSelectionChange.Invoke(_selectableToCheckAgainst);
        }
    }
}