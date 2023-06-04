using UnityEngine;
using UnityEngine.AI;

namespace SuspiciousGames.Saligia.Core.Entities.Components
{
    [RequireComponent(typeof(NavMeshAgent))]
    public class MovementComponent : EntityComponent
    {
        private float _baseMovementSpeed;
        public float BaseMovementSpeed => _baseMovementSpeed;
        public float CurrentMoveSpeed => Agent != null ? Agent.speed : 0f;
        private int _moveBlockStacks = 0;
        private int _rotationBlockStacks = 0;
        public bool CanMove => _moveBlockStacks == 0;
        public bool CanRotate => _rotationBlockStacks == 0;
        [field: SerializeField] public Multiplier MovementSpeedMultiplier { get; private set; }
        public NavMeshAgent Agent { get; private set; }

        private void Start()
        {
            Agent = GetComponent<NavMeshAgent>();
            _baseMovementSpeed = Agent.speed;
            MovementSpeedMultiplier.ResetMultiplier();
            MovementSpeedMultiplier.onMultiplierchange += OnMovementSpeedMultiplierChange;
        }

        private void OnDisable()
        {
            MovementSpeedMultiplier.onMultiplierchange -= OnMovementSpeedMultiplierChange;
        }

        private void OnDestroy()
        {
            if (Agent != null)
                Destroy(Agent);
        }

        private void OnMovementSpeedMultiplierChange(float multiplier)
        {
            Agent.speed = _baseMovementSpeed * multiplier;
        }

        public void BlockRotation(bool value)
        {
            _rotationBlockStacks += value ? 1 : -1;
            if (_rotationBlockStacks <= 0)
                _rotationBlockStacks = 0;
        }

        public void BlockMovement(bool value)
        {
            if (Agent == null)
                return;
            _moveBlockStacks += value ? 1 : -1;
            if (_moveBlockStacks <= 0)
                _moveBlockStacks = 0;
            //Agent.isStopped = !CanMove;
        }

        public bool Warp(Vector3 position)
        {
            if (CanMove)
            {
                NavMesh.Raycast(transform.position, position, out var hit, NavMesh.AllAreas);
                return Agent.Warp(hit.position);
            }
            return false;
        }

        public void Move(Vector3 movement, bool ignoreAgentSpeed = false)
        {
            if (!CanMove)
            {
                //TODO this is an option to take if the player shouldn't be move by other objects, this also makes every other form of movement ignore the navmesh when Agent.enabled is set fo false
                Agent.enabled = false;
                return;
            }
            Agent.enabled = true;
            if (ignoreAgentSpeed)
                Agent.Move(movement);
            else
                Agent.Move(movement * Agent.speed);
        }

        public void ForceMove(Vector3 movement, bool ignoreAgentSpeed = false)
        {
            Agent.enabled = true;
            if (ignoreAgentSpeed)
                Agent.Move(movement);
            else
                Agent.Move(movement * Agent.speed);
        }

        public void SetDestination(Vector3 position)
        {
            if (CanMove)
                Agent.SetDestination(position);
        }

        public void SetDestination(GameObject movementTarget)
        {
            if (CanMove)
                Agent.SetDestination(movementTarget.transform.position);
        }

        public void LookAt(GameObject objectToLookAt)
        {
            LookAt(objectToLookAt.transform);
        }

        public void LookAt(Transform transformToLookAt)
        {
            if (CanRotate)
                transform.LookAt(transformToLookAt);
        }

        public void SetRotation(Quaternion rotation)
        {
            if (CanRotate)
                transform.rotation = rotation;
        }
    }
}
