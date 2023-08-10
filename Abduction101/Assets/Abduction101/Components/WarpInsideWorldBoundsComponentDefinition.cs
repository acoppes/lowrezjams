using Gemserk.Leopotam.Ecs;
using UnityEngine;

namespace Abduction101.Components
{
    public struct WarpInsideWorldBoundsComponent : IEntityComponent
    {
        public Bounds bounds;
    }
    
    public class WarpInsideWorldBoundsComponentDefinition : ComponentDefinitionBase
    {
        public override string GetComponentName()
        {
            return nameof(WarpInsideWorldBoundsComponent);
        }

        public override void Apply(World world, Entity entity)
        {
            world.AddComponent(entity, new WarpInsideWorldBoundsComponent());
        }
    }
}