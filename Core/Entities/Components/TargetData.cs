using UnityEngine;

namespace SuspiciousGames.Saligia.Core.Entities.Components
{
    public class TargetData
    {

        private Vector3 _targetPosition;
        private GameObject _targetObject;

        public TargetData(TargetData targetData)
        {
            _targetObject = targetData.GetTargetObject();
            _targetPosition = targetData.GetTargetPosition();
        }

        public TargetData(Vector3 targetPosition)
        {
            _targetPosition = targetPosition;
            _targetObject = null;
        }

        public TargetData(GameObject targetObject)
        {
            if (targetObject)
            {
                _targetObject = targetObject;
                _targetPosition = targetObject.transform.position;
            }          
        }

        public Vector3 GetTargetPosition()
        {
            return _targetPosition;
        }

        public Vector3 GetTargetDirection(Vector3 caster)
        {
            return _targetPosition - caster;
        }

        public GameObject GetTargetObject()
        {
            return _targetObject;
        }
    }
}
