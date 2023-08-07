using Gemserk.Leopotam.Ecs;
using UnityEngine;

namespace Abduction101.Components
{
    public struct VelocityComponent : IEntityComponent
    {
        public Vector3 value;
    }
    
    public class VelocityComponentDefinition : ComponentDefinitionBase
    {
        public override string GetComponentName()
        {
            return nameof(VelocityComponent);
        }

        public override void Apply(World world, Entity entity)
        {
            world.AddComponent(entity, new VelocityComponent());
        }
    }
}