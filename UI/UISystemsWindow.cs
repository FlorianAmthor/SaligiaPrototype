using SuspiciousGames.Saligia.Core;
using System.Collections.Generic;
using PixelCrushers.DialogueSystem;
using PixelCrushers.Wrappers;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.Localization.Components;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace SuspiciousGames.Saligia.UI
{
    public class UISystemsWindow : UIWindow
    {
        [Space(10), Header("Window Settings Components")]
        [SerializeField] private LocalizeStringEvent _windowModeLocalizedStringEvent;
        [SerializeField] private TextMeshProUGUI _resolutionText;

        [Space(10), Header("Sliders")]
        [SerializeField] private Slider _masterVolumeSlider;
        [SerializeField] private Slider _musicVolumeSlider;
        [SerializeField] private Slider _sfxVolumeSlider;
        [SerializeField] private Slider _dialogueVolumeSlider;
        [SerializeField] private Slider _ambienceVolumeSlider;

        [Space(10), Header("Slider Texts")]
        [SerializeField] private TextMeshProUGUI _masterVolumeText;
        [SerializeField] private TextMeshProUGUI _musicVolumeText;
        [SerializeField] private TextMeshProUGUI _sfxVolumeText;
        [SerializeField] private TextMeshProUGUI _dialogueVolumeText;
        [SerializeField] private TextMeshProUGUI _ambienceVolumeText;

        private List<Resolution> _supportedResolutions;
        private Resolution _currentResolution;
        private FullScreenMode _tempFullScreenMode;

        public override void Close()
        {
            base.Close();
            SettingsManager.Instance.SaveSettings();
        }

        public void LoadAndApplySettings()
        {
            _supportedResolutions = new(Screen.resolutions);


            if (SettingsManager.Instance.ResolutionWidth == 0)
            {
                _currentResolution = Screen.currentResolution;
                SettingsManager.Instance.SetFullScreenMode(Screen.fullScreenMode);
                SettingsManager.Instance.SetResolution(_currentResolution.width, _currentResolution.height, _currentResolution.refreshRate);
            }
            else
            {
                _tempFullScreenMode = SettingsManager.Instance.FullScreenMode;

                _currentResolution = _supportedResolutions.Find(resolution =>
                    resolution.width == SettingsManager.Instance.ResolutionWidth &&
                    resolution.height == SettingsManager.Instance.ResolutionHeight &&
                    resolution.refreshRate == SettingsManager.Instance.RefreshRate);

                if (_currentResolution.Equals(new Resolution()))
                    _currentResolution = Screen.currentResolution;
#if !UNITY_EDITOR
                Screen.SetResolution(_currentResolution.width, _currentResolution.height, _tempFullScreenMode, _currentResolution.refreshRate);
#endif
            }

            SetResolutionText();
            _masterVolumeSlider.value = SettingsManager.Instance.MasterVolume;
            _masterVolumeSlider.onValueChanged.Invoke(_masterVolumeSlider.value);

            _musicVolumeSlider.value = SettingsManager.Instance.MusicVolume;
            _musicVolumeSlider.onValueChanged.Invoke(_musicVolumeSlider.value);

            _sfxVolumeSlider.value = SettingsManager.Instance.SFXVolume;
            _sfxVolumeSlider.onValueChanged.Invoke(_sfxVolumeSlider.value);

            _dialogueVolumeSlider.value = SettingsManager.Instance.DialogueVolume;
            _dialogueVolumeSlider.onValueChanged.Invoke(_dialogueVolumeSlider.value);

            _ambienceVolumeSlider.value = SettingsManager.Instance.AmbienceVolume;
            _ambienceVolumeSlider.onValueChanged.Invoke(_ambienceVolumeSlider.value);
        }

        public void OnWindowModeLeftButton()
        {
            if (_tempFullScreenMode == FullScreenMode.ExclusiveFullScreen)
                _tempFullScreenMode = FullScreenMode.Windowed;
            else
                _tempFullScreenMode--;
            if (_tempFullScreenMode == FullScreenMode.MaximizedWindow)
                _tempFullScreenMode--;
            _windowModeLocalizedStringEvent.StringReference.TableEntryReference = _tempFullScreenMode.ToString();
        }

        public void OnWindowModeRightButton()
        {
            if (_tempFullScreenMode == FullScreenMode.Windowed)
                _tempFullScreenMode = FullScreenMode.ExclusiveFullScreen;
            else
                _tempFullScreenMode++;
            if (_tempFullScreenMode == FullScreenMode.MaximizedWindow)
                _tempFullScreenMode++;
            _windowModeLocalizedStringEvent.StringReference.TableEntryReference = _tempFullScreenMode.ToString();
        }

        public void OnResolutionLeftButton()
        {
            int resolutionIndex = _supportedResolutions.IndexOf(_currentResolution);
            if (resolutionIndex == 0)
                resolutionIndex = _supportedResolutions.Count - 1;
            else
                resolutionIndex--;
            _currentResolution = _supportedResolutions[resolutionIndex];
            SetResolutionText();
        }

        public void OnResolutionRightButton()
        {
            int resolutionIndex = _supportedResolutions.IndexOf(_currentResolution);
            if (resolutionIndex == _supportedResolutions.Count - 1)
                resolutionIndex = 0;
            else
                resolutionIndex++;
            _currentResolution = _supportedResolutions[resolutionIndex];
            SetResolutionText();
        }

        private void SetResolutionText()
        {
            _resolutionText.text = $"{_currentResolution.width}x{_currentResolution.height}({_currentResolution.refreshRate}Hz)";
        }

        public void OnSubmitWindowSettingsButton()
        {
            SettingsManager.Instance.SetFullScreenMode(_tempFullScreenMode);
            SettingsManager.Instance.SetResolution(_currentResolution.width, _currentResolution.height, _currentResolution.refreshRate);
            SettingsManager.Instance.SaveSettings();
#if !UNITY_EDITOR
            Screen.SetResolution(SettingsManager.Instance.ResolutionWidth, SettingsManager.Instance.ResolutionHeight, SettingsManager.Instance.FullScreenMode, SettingsManager.Instance.RefreshRate);
#else
            Screen.SetResolution(_currentResolution.width, _currentResolution.height, _tempFullScreenMode, _currentResolution.refreshRate);
#endif
        }

        public void OnCancelWindowSettingsButton()
        {
            _currentResolution = _supportedResolutions.Find(resolution =>
                resolution.width == SettingsManager.Instance.ResolutionWidth &&
                resolution.height == SettingsManager.Instance.ResolutionWidth &&
                resolution.refreshRate == SettingsManager.Instance.RefreshRate);

            if (_currentResolution.Equals(new Resolution()))
                _currentResolution = Screen.currentResolution;
        }

        public void OnMasterVolumeChange(float value)
        {
            _masterVolumeText.text = ((int)value).ToString();
            SettingsManager.Instance.SetMasterVolume(value, _masterVolumeSlider.maxValue);
        }

        public void OnMusicVolumeChange(float value)
        {
            _musicVolumeText.text = ((int)value).ToString();
            SettingsManager.Instance.SetMusicVolume(value, _musicVolumeSlider.maxValue);
        }

        public void OnSFXVolumeChange(float value)
        {
            _sfxVolumeText.text = ((int)value).ToString();
            SettingsManager.Instance.SetSFXVolume(value, _sfxVolumeSlider.maxValue);
        }

        public void OnDialogueVolumeChange(float value)
        {
            _dialogueVolumeText.text = ((int)value).ToString();
            SettingsManager.Instance.SetDialogueVolume(value, _dialogueVolumeSlider.maxValue);
        }

        public void OnAmbienceVolumeChange(float value)
        {
            _ambienceVolumeText.text = ((int)value).ToString();
            SettingsManager.Instance.SetAmbienceVolume(value, _ambienceVolumeSlider.maxValue);
        }

        public void OnSocialButton1()
        {

        }

        public void OnSocialButton2()
        {

        }

        public void OnResetSaveGame()
        {
            MenuManager.Instance.CloseMenu();
            SaveSystem.ResetGameState();
            SaveSystem.storer.DeleteSavedGameData(1);
            DialogueManager.StopAllConversations();
            DialogueManager.ResetDatabase();
            SceneManager.LoadScene("Demo");
            SaveSystem.SaveToSlot(1);
        }

        public void OnQuitGame()
        {
#if UNITY_EDITOR
            EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
    }
}