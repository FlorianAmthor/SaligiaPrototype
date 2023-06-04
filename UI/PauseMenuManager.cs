using SuspiciousGames.Saligia.Audio;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

namespace SuspiciousGames.Saligia.UI
{
    public class PauseMenuManager : SystemMenu
    {
        [SerializeField] private UIDocument _document;

        [SerializeField] private UnityEvent OnResumeButton;
        [SerializeField] private UnityEvent OnResetWavesButton;
        [SerializeField] private UnityEvent OnExitButton;
        [SerializeField] private UnityEvent OnQuestionnaireButton;
        [SerializeField] private UnityEvent OnDiscordButton;
        [SerializeField] private UnityEvent OnCloseMenu;

        private SoundManager _soundManager;

        private Button _resumeButton;
        private Button _resteWavesButton;
        private Button _exitButton;
        private Button _questionnaireButton;
        private Button _discordButton;
        private SliderInt _volumeSlider;

        private bool _active = false;

        private void Awake()
        {
            _document.rootVisualElement.style.display = DisplayStyle.None;
        }

        // Start is called before the first frame update
        public void Init()
        {
            FetchComponents();
            _soundManager = FindObjectOfType<SoundManager>();
            if (_soundManager)
                _volumeSlider.value = _soundManager.CurrentVolume;
            RegisterCallbacks();
        }


        private void FetchComponents()
        {
            _resumeButton = _document.rootVisualElement.Q<Button>("ButtonResume");
            _resteWavesButton = _document.rootVisualElement.Q<Button>("ButtonResetWaves");
            _exitButton = _document.rootVisualElement.Q<Button>("ButtonQuit");
            _questionnaireButton = _document.rootVisualElement.Q<Button>("ButtonGoogleForm");
            _discordButton = _document.rootVisualElement.Q<Button>("ButtonDiscord");
            _volumeSlider = _document.rootVisualElement.Q<SliderInt>("SliderVolume");
        }

        private void RegisterCallbacks()
        {
            _resumeButton.clicked += () => OnResumeButton.Invoke();
            _resteWavesButton.clicked += () => OnResetWavesButton.Invoke();
            _exitButton.clicked += () => OnExitButton.Invoke();
            _questionnaireButton.clicked += () => OnQuestionnaireButton.Invoke();
            _discordButton.clicked += () => OnDiscordButton.Invoke();
            _volumeSlider.RegisterCallback<NavigationMoveEvent>(UpdateSlider);
            if (_soundManager)
                _volumeSlider.RegisterValueChangedCallback(e => _soundManager.SetMasterVolume(e.newValue));
        }

        public void OpenPauseMenu()
        {
            if (!_active)
            {
                Time.timeScale = 0;
                AudioListener.pause = true;
                if (_soundManager)
                    _volumeSlider.value = _soundManager.CurrentVolume;
                _document.rootVisualElement.style.display = DisplayStyle.Flex;
                _resumeButton.Focus();
                _active = true;
            }
        }

        public void ClosePauseMenu()
        {
            if (_active)
            {
                _document.rootVisualElement.style.display = DisplayStyle.None;
                OnCloseMenu.Invoke();
                _active = false;
                AudioListener.pause = false;
                Time.timeScale = 1;
            }
        }

        private void UpdateSlider(NavigationMoveEvent e)
        {
            switch (e.direction)
            {
                case NavigationMoveEvent.Direction.Left:
                    _volumeSlider.value--; break;
                case NavigationMoveEvent.Direction.Right:
                    _volumeSlider.value++; break;
                default:
                    break;
            }
        }

        private float RemapSliderValues(float sliderValue)
        {
            float min = _volumeSlider.lowValue;
            float max = _volumeSlider.highValue;
            float mix = Mathf.InverseLerp(min, max, sliderValue);
            return Mathf.Clamp(mix, 0.0001f, 1);
        }
    }

}

