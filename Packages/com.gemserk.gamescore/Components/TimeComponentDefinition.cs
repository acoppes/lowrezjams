using Gemserk.Leopotam.Ecs;
using Gemserk.Utilities;

namespace Game.Components
{
    public struct TimeComponent : IEntityComponent
    {
        public float time;
    }
    
    public class TimeComponentDefinition : ComponentDefinitionBase
    {
        public override string GetComponentName()
        {
            return nameof(TimeComponent);
        }

        public override void Apply(World world, Entity entity)
        {
            world.AddComponent(entity, new TimeComponent());
        }
    }
}