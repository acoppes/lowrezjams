using Gemserk.Leopotam.Ecs;

namespace Abduction101.Components
{
    public struct FallStateComponent : IEntityComponent
    {
        public float yPositionOnStart;
        public float height;
        public bool falling;
        public float minHeightForDamage;
    }
    
    public class FallStateComponentDefinition : ComponentDefinitionBase
    {
        public float minHeightForDamage = 0.1f;
        
        public override string GetComponentName()
        {
            return nameof(FallStateComponent);
        }

        public override void Apply(World world, Entity entity)
        {
            world.AddComponent(entity, new FallStateComponent()
            {
                minHeightForDamage = minHeightForDamage
            });
        }
    }
}