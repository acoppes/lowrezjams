using System.Collections.Generic;
using System.Linq;
using Gemserk.Leopotam.Ecs;

namespace Game.Components
{
    public struct TypesComponent : IEntityComponent
    {
        public List<string> types;
    }
    
    public class TypesComponentDefinition : ComponentDefinitionBase
    {
        public List<string> types;
        
        public override string GetComponentName()
        {
            return nameof(TypesComponent);
        }

        public override void Apply(World world, Entity entity)
        {
            world.AddComponent(entity, new TypesComponent()
            {
                types = types.Select(s => s.ToLower()).ToList()
            });
        }
    }
}