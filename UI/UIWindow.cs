using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace SuspiciousGames.Saligia.UI
{
    public class UIWindow : MonoBehaviour
    {
        [System.Serializable]
        public class Events
        {
            public UnityEvent onOpen;
            public UnityEvent onClose;
        }

        [field: SerializeField] public int WindowLayerIndex { get; private set; } = 0;
        [SerializeField] private Selectable _defaultSelectable;
        public Events windowEvents;

        public bool IsOpen => gameObject.activeInHierarchy;

        public virtual void Open()
        {
            gameObject.SetActive(true);
            SelectDefaultSelectable();
            windowEvents.onOpen.Invoke();
        }

        public virtual void Close()
        {
            gameObject.SetActive(false);
            windowEvents.onClose.Invoke();
        }

        public void SelectDefaultSelectable()
        {
            if (_defaultSelectable)
                _defaultSelectable.Select();
        }
    }
}