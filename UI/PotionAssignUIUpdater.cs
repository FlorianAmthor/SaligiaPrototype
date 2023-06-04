using SuspiciousGames.Saligia.Core.Entities.Player;
using UnityEngine;
using UnityEngine.UI;

namespace SuspiciousGames.Saligia.UI
{
    public class PotionAssignUIUpdater : MonoBehaviour
    {
        [SerializeField] private Image _northPotionImage;
        [SerializeField] private Image _eastPotionImage;
        [SerializeField] private Image _southPotionImage;
        [SerializeField] private Image _westPotionImage;

        private PlayerInventory _playerInventory;

        private void OnEnable()
        {
            UpdatePotionImages();
        }

        public void UpdatePotionImages()
        {
            if (_playerInventory == null)
                _playerInventory = PlayerEntity.Instance.PlayerInventory;
            _northPotionImage.sprite = _playerInventory.GetPotion(0).PotionIcon;
            _eastPotionImage.sprite = _playerInventory.GetPotion(1).PotionIcon;
            _southPotionImage.sprite = _playerInventory.GetPotion(2).PotionIcon;
            _westPotionImage.sprite = _playerInventory.GetPotion(3).PotionIcon;
        }
    }
}