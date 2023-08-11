using Abduction101.Data;
using Gemserk.Leopotam.Ecs;
using UnityEngine;

namespace Abduction101.Components
{
    public struct HueModelComponent : IEntityComponent
    {
        public MaterialPropertyBlock materialPropertyBlock;
    }
    
    public class HueModelComponentDefinition : ComponentDefinitionBase
    {
        public override string GetComponentName()
        {
            return nameof(HueModelComponent);
        }

        public override void Apply(World world, Entity entity)
        {
            world.AddComponent(entity, new HueModelComponent());
        }
    }
}