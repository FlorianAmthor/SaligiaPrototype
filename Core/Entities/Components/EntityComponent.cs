using UnityEngine;

namespace SuspiciousGames.Saligia.Core.Entities.Components
{
    [RequireComponent(typeof(Entity))]
    public abstract class EntityComponent : MonoBehaviour
    {
        private Entity _owner;

        public Entity Owner
        {
            get
            {
                if (_owner == null)
                    _owner = GetComponent<Entity>();
                return _owner;
            }
        }
    }
}
