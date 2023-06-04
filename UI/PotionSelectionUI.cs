using SuspiciousGames.Saligia.Core.Entities.Player;
using SuspiciousGames.Saligia.Core.Potions;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SuspiciousGames.Saligia.UI
{
    public class PotionSelectionUI : MonoBehaviour
    {
        [SerializeField] private Image _potionImage;
        [SerializeField] private TextMeshProUGUI _potionNameText;
        [SerializeField] private TextMeshProUGUI _potionEffectText;

        private Action<PotionSlot> _confirmAction;

        public PlayerInventory PlayerInventory { get; private set; }
        private PotionSlot _slot;

        public void SetUIActions(Action<PotionSlot> potionConfirmAction)
        {
            _confirmAction = potionConfirmAction;
        }

        public void UpdateUI(PlayerInventory playerInventory, PotionSlot potionSlot)
        {
            var potion = playerInventory.GetPotion(potionSlot);
            _slot = potionSlot;
            _potionImage.sprite = potion.PotionIcon;
            _potionNameText.text = potion.GetPotionName();
            _potionEffectText.text = potion.GetPotionDescription();
        }

        public void SetVerticalSelectables(Selectable selectableOnUp, Selectable selectableOnDown)
        {
            var selectable = GetComponent<Selectable>();
            var navigation = new Navigation() { mode = Navigation.Mode.Explicit, selectOnUp = selectableOnUp, selectOnDown = selectableOnDown, wrapAround = false };
            selectable.navigation = navigation;
        }

        public void OnConfirm()
        {
            _confirmAction?.Invoke(_slot);
        }
    }
}