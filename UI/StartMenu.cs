using SuspiciousGames.Saligia.Audio;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

namespace SuspiciousGames.Saligia.UI
{
    public class StartMenu : SystemMenu
    {
        [SerializeField] private UIDocument _document;

        [SerializeField] private UnityEvent OnPlayButton;
        [SerializeField] private UnityEvent OnExitButton;
        [SerializeField] private UnityEvent OnQuestionnaireButton;
        [SerializeField] private UnityEvent OnDiscordButton;

        private SoundManager _soundManager;

        private Button _playButton;
        private Button _exitButton;
        private Button _questionnaireButton;
        private Button _discordButton;
        private SliderInt _volumeSlider;

        // Start is called before the first frame update
        void Start()
        {
            FetchComponents();
            _soundManager = FindObjectOfType<SoundManager>();
            _volumeSlider.value = _soundManager.CurrentVolume;
            _playButton.Focus();
            RegisterCallbacks();
        }

        private void FetchComponents()
        {
            _playButton = _document.rootVisualElement.Q<Button>("ButtonPlay");
            _exitButton = _document.rootVisualElement.Q<Button>("ButtonQuit");
            _questionnaireButton = _document.rootVisualElement.Q<Button>("ButtonGoogleForm");
            _discordButton = _document.rootVisualElement.Q<Button>("ButtonDiscord");
            _volumeSlider = _document.rootVisualElement.Q<SliderInt>("SliderVolume");
        }

        private void RegisterCallbacks()
        {
            _playButton.clicked += () => OnPlayButton.Invoke();
            _exitButton.clicked += () => OnExitButton.Invoke();
            _questionnaireButton.clicked += () => OnQuestionnaireButton.Invoke();
            _discordButton.clicked += () => OnDiscordButton.Invoke();
            _volumeSlider.RegisterCallback<NavigationMoveEvent>(UpdateSlider);
            _volumeSlider.RegisterValueChangedCallback(e => _soundManager.SetMasterVolume(e.newValue));
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

        
    }
}

