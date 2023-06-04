using SuspiciousGames.Saligia.Core.Entities.Player;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization.Components;
using UnityEngine.UI;

namespace SuspiciousGames.Saligia.UI
{
    public class PotionSelectionWindow : UIWindow
    {
        [Header("UI Prefabs")]
        [SerializeField] private PotionSelectionUI _potionSelectionUIPrefab;

        [Space(10), Header("Gameobject References")]
        [SerializeField] private GameObject _selectionWindowContentGameObject;

        private PlayerInventory _playerInventory => GetPlayerInventory();
        private List<PotionSelectionUI> _potionSelectionUIs = new();

        private int _currentSelectionPotionIndex = 0;

        public void UpdatePotionSelection(int selectedPotionSlot)
        {
            _currentSelectionPotionIndex = selectedPotionSlot;

            if (_potionSelectionUIs == null)
                _potionSelectionUIs = new();

            if (_potionSelectionUIs.Count == 0)
            {
                for (int i = 0; i < _playerInventory.PlayerInventoryData.Potions.Count; i++)
                {
                    var potionUI = Instantiate(_potionSelectionUIPrefab, _selectionWindowContentGameObject.transform);
                    potionUI.SetUIActions(OnPotionSelect);
                    _potionSelectionUIs.Add(potionUI);
                }
            }

            List<PotionSelectionUI> activeUIs = new List<PotionSelectionUI>();

            for (int i = 0; i < _potionSelectionUIs.Count; i++)
            {
                _potionSelectionUIs[i].UpdateUI(_playerInventory, (PotionSlot)i);
                bool isActive = (selectedPotionSlot != i);
                _potionSelectionUIs[i].gameObject.SetActive(isActive);
                if (isActive)
                    activeUIs.Add(_potionSelectionUIs[i]);
            }


            for (int i = 0; i < activeUIs.Count; i++)
            {
                if (i == 0)
                    activeUIs[i].SetVerticalSelectables(null, activeUIs[i + 1].GetComponent<Button>());
                else if (i == activeUIs.Count - 1)
                    activeUIs[i].SetVerticalSelectables(activeUIs[i - 1].GetComponent<Button>(), null);
                else
                    activeUIs[i].SetVerticalSelectables(activeUIs[i - 1].GetComponent<Button>(), activeUIs[i + 1].GetComponent<Button>());

            }

            _potionSelectionUIs.Find(potionUI => potionUI.isActiveAndEnabled).GetComponent<Selectable>().Select();
        }

        private void OnPotionSelect(PotionSlot potionSlot)
        {
            _playerInventory.SwapPotionSlots(_currentSelectionPotionIndex, (int)potionSlot);
            MenuManager.Instance.CloseWindow(this);
        }

        public override void Close()
        {
            base.Close();
            _currentSelectionPotionIndex = 0;
        }

        private PlayerInventory GetPlayerInventory()
        {
            return PlayerEntity.Instance.PlayerInventory;
        }
    }
}