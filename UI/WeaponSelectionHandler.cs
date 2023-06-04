using SuspiciousGames.Saligia.Core.Player;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace SuspiciousGames.Saligia.UI
{
    public class WeaponSelectionHandler : MonoBehaviour, ISelectHandler
    {
        [SerializeField] private Selectable _selectableToCheckAgainst;
        [SerializeField] private PlayerWeaponType _playerWeaponType;
        public UnityEvent<PlayerWeaponType> onWeaponSelectionChange;

        public void OnSelect(BaseEventData eventData)
        {
            if (eventData.selectedObject == _selectableToCheckAgainst.gameObject)
                onWeaponSelectionChange.Invoke(_playerWeaponType);
        }
    }
}