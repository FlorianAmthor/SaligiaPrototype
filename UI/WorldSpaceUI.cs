using UnityEngine;
using UnityEngine.UIElements;

namespace SuspiciousGames.Saligia.UI
{
    public class WorldSpaceUI : MonoBehaviour
    {
        [SerializeField] private UIDocument _document;
        [SerializeField] private Vector3 _offset;
        [SerializeField] private bool _update = true;

        VisualElement _container;

        private void Start()
        {
            _container = _document.rootVisualElement.Q("Container");
            //_root.style.flexGrow = 1;
            SetPosition();
        }

        private void LateUpdate()
        {
            if(_update)
                SetPosition();
        }


        private void SetPosition()
        {
            Vector2 pos = RuntimePanelUtils.CameraTransformWorldToPanel(_container.panel, transform.position + _offset, Camera.main);
            pos.x -= _container.layout.width/2;
            _container.transform.position = pos;
        }
    }
}

