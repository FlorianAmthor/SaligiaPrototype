using UnityEngine;
using UnityEngine.UI;

namespace SuspiciousGames.Saligia.UI
{
    public class LoadoutInfoDisplay : MonoBehaviour
    {
        [SerializeField] private Image _image;
        [SerializeField] private TMPro.TextMeshProUGUI _text;

        public void UpdateDisplay(Sprite sprite, string text)
        {
            _image.sprite = sprite;
            _text.text = text;
        }
    }
}