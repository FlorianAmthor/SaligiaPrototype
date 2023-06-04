using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

namespace SuspiciousGames.Saligia.UI
{
    public class FeedBackMenu : SystemMenu
    {
        [SerializeField] private UIDocument _document;
        [SerializeField] private UnityEvent _onButtonclciked;
        [SerializeField] private UnityEvent _onStart;

        private Button _button;

        private void Start()
        {
            _document.rootVisualElement.style.display = DisplayStyle.None;
            _button = _document.rootVisualElement.Q<Button>("Button");
            _button.clicked += () => _onButtonclciked.Invoke();
            _onStart.Invoke();
        }

        public void Activate()
        {
            _document.rootVisualElement.style.display = DisplayStyle.Flex;
            _button.Focus();
        }

        public void DeActivate()
        {
            _document.rootVisualElement.style.display = DisplayStyle.None;
        }


    }
}

