using SuspiciousGames.Saligia.Core.Entities.Buffs;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace SuspiciousGames.Saligia.UI
{
    public class BuffUI : MonoBehaviour
    {
        [SerializeField] private Image _buffBackgroundImage;
        [SerializeField] private Image _buffDurationFillImage;

        private Coroutine _durationCoroutine;

        public void Init(BuffData buffData)
        {
            _buffBackgroundImage.sprite = buffData.BuffIcon;
            _buffDurationFillImage.sprite = buffData.BuffIcon;
            _buffDurationFillImage.fillAmount = 1;
            if (_durationCoroutine != null)
                StopCoroutine(_durationCoroutine);
            _durationCoroutine = StartCoroutine(StartBuffDuration(buffData.Duration));
        }

        private IEnumerator StartBuffDuration(float duration)
        {
            float remainingDuration = duration;
            while (duration > 0)
            {
                yield return new WaitForEndOfFrame();
                remainingDuration -= Time.deltaTime;
                _buffDurationFillImage.fillAmount = remainingDuration / duration;
            }
        }
    }
}