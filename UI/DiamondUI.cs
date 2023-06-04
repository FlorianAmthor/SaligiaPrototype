using DuloGames.UI;
using UnityEngine;
using UnityEngine.UI;

namespace SuspiciousGames.Saligia.UI
{
    public class DiamondUI : MonoBehaviour
    {
        [SerializeField] private Image _maskImage;
        [SerializeField] private Image _image;
        [SerializeField] private UIProgressBar _uiProgressBar;
        [SerializeField] private TMPro.TextMeshProUGUI _text;

        public Image Image => _image;

        public void SetText(string s)
        {
            _text.text = s;
        }

        public void SetImage(Sprite sprite)
        {
            _image.sprite = sprite;
        }

        public void UpdateFillAmount(float value)
        {
            if (_uiProgressBar)
                _uiProgressBar.fillAmount = value;
        }

        private void Start()
        {
            UpdateFillAmount(0);
        }
    }
}