using Gemserk.Leopotam.Ecs;

namespace Abduction101.Components
{
    public struct FallStateComponent : IEntityComponent
    {
        public float yPositionOnStart;
        public float height;
    }
    
    public class FallStateComponentDefinition : ComponentDefinitionBase
    {
        public override string GetComponentName()
        {
            return nameof(FallStateComponent);
        }

        public override void Apply(World world, Entity entity)
        {
            world.AddComponent(entity, new FallStateComponent());
        }
    }
}