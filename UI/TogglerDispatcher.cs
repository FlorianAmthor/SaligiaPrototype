using UnityEngine;
using UnityEngine.Events;

namespace SuspiciousGames.Saligia.UI
{
    public class TogglerDispatcher : MonoBehaviour
    {
        public UnityEvent onToggleOn;
        public UnityEvent onToggleOff;

        public void OnToggleChanged(bool value)
        {
            if (value)
                onToggleOn.Invoke();
            else
                onToggleOff.Invoke();
        }
    }
}