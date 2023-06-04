using UnityEngine;
using UnityEngine.UI;

namespace SuspiciousGames.Saligia.UI
{
    public class OnEnableSelector : MonoBehaviour
    {
        [SerializeField] private Selectable _selectedObjectOnEnable;

        private void OnEnable()
        {
            if (_selectedObjectOnEnable)
                _selectedObjectOnEnable.Select();
        }
    }
}