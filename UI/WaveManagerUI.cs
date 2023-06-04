using UnityEngine;
using UnityEngine.UIElements;

namespace SuspiciousGames.Saligia.UI
{
    public class WaveManagerUI : MonoBehaviour
    {
        [SerializeField] private UIDocument _waveCounter;

        private Label _waveLabel;
        private Label _maxWavesLabel;

        private void Start()
        {
            FetchComponents();
            _waveCounter.rootVisualElement.style.display = DisplayStyle.None;
        }

        private void FetchComponents()
        {
            _waveLabel = _waveCounter.rootVisualElement.Q<Label>("WaveCounter");
            _maxWavesLabel = _waveCounter.rootVisualElement.Q<Label>("MaxWavesLabel");
        }

        public void UpdateWaveCounter(int currentwave, int maxwave)
        {
            _waveCounter.rootVisualElement.style.display = DisplayStyle.Flex;
            _waveLabel.text = currentwave.ToString();
            _maxWavesLabel.text = maxwave.ToString();
        }

    }
}

