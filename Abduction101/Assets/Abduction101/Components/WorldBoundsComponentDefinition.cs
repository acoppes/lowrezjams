using Gemserk.Leopotam.Ecs;
using UnityEngine;

namespace Abduction101.Components
{
    public struct WorldBoundsComponent : IEntityComponent
    {
        public Bounds bounds;
    }
    
    public class WorldBoundsComponentDefinition : ComponentDefinitionBase
    {
        public Vector3 size;
        
        public override string GetComponentName()
        {
            return nameof(WorldBoundsComponent);
        }

        public override void Apply(World world, Entity entity)
        {
            world.AddComponent(entity, new WorldBoundsComponent()
            {
                bounds = new Bounds(Vector3.zero, size)
            });
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.DrawWireCube(transform.position, size);
        }
    }
}