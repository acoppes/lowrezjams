using Gemserk.Leopotam.Ecs;

namespace Abduction101.Components
{
    public struct RunAwayComponent : IEntityComponent
    {
        public float speed;
    }
    
    public class RunAwayComponentDefinition : ComponentDefinitionBase
    {
        public float speed;
        
        public override string GetComponentName()
        {
            return nameof(RunAwayComponent);
        }

        public override void Apply(World world, Entity entity)
        {
            world.AddComponent(entity, new RunAwayComponent()
            {
                speed = speed
            });
        }
    }
}