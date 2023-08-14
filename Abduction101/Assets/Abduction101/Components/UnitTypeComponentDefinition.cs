using Gemserk.Leopotam.Ecs;
using UnityEngine;

namespace Abduction101.Components
{
    public struct UnitTypeComponent : IEntityComponent
    {
        public int type;
    }
    
    public class UnitTypeComponentDefinition : ComponentDefinitionBase
    {
        public int type;
        
        public override string GetComponentName()
        {
            return nameof(UnitTypeComponent);
        }

        public override void Apply(World world, Entity entity)
        {
            world.AddComponent(entity, new UnitTypeComponent()
            {
                type = type
            });
        }
    }
}