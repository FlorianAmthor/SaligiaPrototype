using DuloGames.UI;
using UnityEngine;
using UnityEngine.Events;

namespace SuspiciousGames.Saligia.UI
{
    public class UIBarUpdater : MonoBehaviour
    {
        [SerializeField] private UIProgressBar _fillImage;
        [SerializeField] private string _stringSuffix;

        public UnityEvent<string> onUpdateBarValue;

        public void UpdateBar(float value)
        {
            _fillImage.fillAmount = value;
            onUpdateBarValue.Invoke(Mathf.CeilToInt(100 * value) + _stringSuffix);
        }
    }
}