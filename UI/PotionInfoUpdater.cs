using SuspiciousGames.Saligia.Core.Entities.Player;
using SuspiciousGames.Saligia.Core.Potions;
using UnityEngine;

namespace SuspiciousGames.Saligia.UI
{
    public class PotionInfoUpdater : MonoBehaviour
    {
        private PlayerInventory _playerInventory;

        private Potion _potion1;
        [Header("First Potion")]
        [SerializeField] private PotionSettings _potionSetting1;
        [SerializeField] private LoadoutInfoDisplay _potionLoadoutInfoDisplay1;
        private Potion _potion2;
        [Header("Second Potion"), Space(10)]
        [SerializeField] private PotionSettings _potionSetting2;
        [SerializeField] private LoadoutInfoDisplay _potionLoadoutInfoDisplay2;
        private Potion _potion3;
        [Header("Third Potion"), Space(10)]
        [SerializeField] private PotionSettings _potionSetting3;
        [SerializeField] private LoadoutInfoDisplay _potionLoadoutInfoDisplay3;
        private Potion _potion4;
        [Header("Fourth Potion"), Space(10)]
        [SerializeField] private PotionSettings _potionSetting4;
        [SerializeField] private LoadoutInfoDisplay _potionLoadoutInfoDisplay4;

        private void OnEnable()
        {
            UpdateInfoDisplays();
        }

        public void UpdateInfoDisplays()
        {
            if (_playerInventory == null)
                _playerInventory = PlayerEntity.Instance.GetComponent<PlayerInventory>();

            _potion1 = _playerInventory.GetPotion(_potionSetting1);
            _potion2 = _playerInventory.GetPotion(_potionSetting2);
            _potion3 = _playerInventory.GetPotion(_potionSetting3);
            _potion4 = _playerInventory.GetPotion(_potionSetting4);

            _potionLoadoutInfoDisplay1.UpdateDisplay(_potion1.PotionIcon, _potion1.GetPotionDescription());
            _potionLoadoutInfoDisplay2.UpdateDisplay(_potion2.PotionIcon, _potion2.GetPotionDescription());
            _potionLoadoutInfoDisplay3.UpdateDisplay(_potion3.PotionIcon, _potion3.GetPotionDescription());
            _potionLoadoutInfoDisplay4.UpdateDisplay(_potion4.PotionIcon, _potion4.GetPotionDescription());
        }

        private void GetPlayerInventory()
        {
            _playerInventory = PlayerEntity.Instance.GetComponent<PlayerInventory>();
        }
    }
}