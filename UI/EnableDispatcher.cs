using UnityEngine;
using UnityEngine.Events;

namespace SuspiciousGames.Saligia.UI
{
    public class EnableDispatcher : MonoBehaviour
    {
        [SerializeField] private bool _dispatchOnEnable;
        [SerializeField] private bool _dispatchOnDisable;

        public UnityEvent<bool> onEnable;

        private void OnEnable()
        {
            onEnable.Invoke(true);
        }

        private void OnDisable()
        {
            onEnable.Invoke(false);
        }
    }
}