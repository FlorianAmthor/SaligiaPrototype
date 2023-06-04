using SuspiciousGames.Saligia.Core.Entities.Player;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using static UnityEngine.InputSystem.InputAction;

namespace SuspiciousGames.Saligia.UI
{
    public class MenuManager : MonoBehaviour
    {
        public static MenuManager Instance { get; private set; }

        [SerializeField] private TabManager _tabManager;
        private Dictionary<int, UIWindow> _layeredUIWindows;
        private Dictionary<int, Selectable> _layerLastSelectecSelectable;
        private int _currentLayerIndex;

        [HideInInspector] public UnityEvent<UIWindow> onWindowOpened;
        [HideInInspector] public UnityEvent<UIWindow> onWindowClosed;

        private GameObject _lastSelectedObject;

        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
            }
            else
            {
                Instance = this;
                _layeredUIWindows = new();
                _layerLastSelectecSelectable = new();
                _currentLayerIndex = 0;
            }
        }

        private void OnEnable()
        {
            onWindowOpened.AddListener(OpenWindow);
            onWindowClosed.AddListener(CloseWindow);
        }

        private void OnDisable()
        {
            onWindowOpened.RemoveListener(OpenWindow);
            onWindowClosed.RemoveListener(CloseWindow);
        }

        private void Update()
        {
            if (_lastSelectedObject != EventSystem.current.currentSelectedGameObject && EventSystem.current.currentSelectedGameObject != null)
            {
                _lastSelectedObject = EventSystem.current.currentSelectedGameObject;
                if (_layerLastSelectecSelectable.ContainsKey(_currentLayerIndex))
                    _layerLastSelectecSelectable[_currentLayerIndex] = _lastSelectedObject.GetComponent<Selectable>();
                else
                    _layerLastSelectecSelectable.Add(_currentLayerIndex, _lastSelectedObject.GetComponent<Selectable>());
            }
        }

        private void SetActiveUIWindowForLayerIndex(UIWindow uiWindow)
        {
            if (_layeredUIWindows.ContainsKey(uiWindow.WindowLayerIndex))
                _layeredUIWindows[uiWindow.WindowLayerIndex] = uiWindow;
            else
                _layeredUIWindows.Add(uiWindow.WindowLayerIndex, uiWindow);
        }

        private void RemoveActiveUIWindowForLayerIndex(UIWindow uiWindow)
        {
            if (_layeredUIWindows.ContainsKey(uiWindow.WindowLayerIndex) && _layeredUIWindows[uiWindow.WindowLayerIndex] == uiWindow)
                _layeredUIWindows.Remove(uiWindow.WindowLayerIndex);
        }

        public void OpenWindow(UIWindow uiWindow)
        {
            _currentLayerIndex = uiWindow.WindowLayerIndex;
            SetActiveUIWindowForLayerIndex(uiWindow);
            uiWindow.Open();
        }

        public void CloseWindow(UIWindow uiWindow)
        {
            RemoveActiveUIWindowForLayerIndex(uiWindow);
            if (uiWindow.WindowLayerIndex > 0)
            {
                _currentLayerIndex = uiWindow.WindowLayerIndex - 1;
                if (_layerLastSelectecSelectable.ContainsKey(_currentLayerIndex))
                    _layerLastSelectecSelectable[_currentLayerIndex].Select();
                else
                    _layeredUIWindows[_currentLayerIndex].SelectDefaultSelectable();
            }
            uiWindow.Close();
        }

        public void OnBackInputAction(CallbackContext context)
        {
            if (context.started && _layeredUIWindows?.Count > 0)
            {
                if (_currentLayerIndex == 0)
                {
                    CloseMenu();
                }
                else
                {
                    CloseWindow(_layeredUIWindows[_currentLayerIndex]);
                }
            }
        }

        public void OnPageNext(CallbackContext context)
        {
            if (context.started && _layeredUIWindows?.Count > 0 && _currentLayerIndex == 0)
                _tabManager.ActivateNextTab();
        }

        public void OnPagePrevious(CallbackContext context)
        {
            if (context.started && _layeredUIWindows?.Count > 0 && _currentLayerIndex == 0)
                _tabManager.ActivatePreviousTab();
        }

        public void OnOpenMenu(CallbackContext context)
        {
            if (context.canceled)
            {
                OpenMenu();
            }
        }

        public void OnCloseMenu(CallbackContext context)
        {
            if (context.canceled)
            {
                CloseMenu();
            }
        }

        public void EnableOpenCloseMenuActions(bool value)
        {
            var actions = PlayerEntity.Instance.GetComponent<PlayerInput>().currentActionMap.actions;
            foreach (var action in actions)
            {
                if (value && (action.name == "Menu_Main" || action.name == "Menu_Character"))
                    action.Enable();
                else if (action.name == "Menu_Main" || action.name == "Menu_Character")
                    action.Disable();
            }
        }

        public void OnControlsChanged(PlayerInput playerInput)
        {
            if (playerInput.currentControlScheme == "Keyboard and Mouse")
            {
                Cursor.lockState = CursorLockMode.Confined;
                Cursor.visible = true;
            }
            else
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }

            if (gameObject.activeInHierarchy)
                if (_layeredUIWindows.ContainsKey(_currentLayerIndex))
                    _layeredUIWindows[_currentLayerIndex].SelectDefaultSelectable();
        }

        public void OpenMenu()
        {
            AudioListener.pause = true;
            PlayerEntity.Instance.GetComponent<PlayerInput>().SwitchCurrentActionMap("Menu");
            Time.timeScale = 0.0f;
            gameObject.SetActive(true);
            _tabManager.ResetActiveTabIndex();
            _tabManager.ActivateDefaultTab();
        }

        public void CloseMenu()
        {
            AudioListener.pause = false;
            PlayerEntity.Instance.GetComponent<PlayerInput>().SwitchCurrentActionMap("Player");
            CloseOpenedUILayers();
            gameObject.SetActive(false);
            Time.timeScale = 1.0f;
        }

        public void CloseOpenedUILayers()
        {
            while (_currentLayerIndex > 0)
                CloseWindow(_layeredUIWindows[_currentLayerIndex]);
        }
    }
}