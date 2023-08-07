using Gemserk.Leopotam.Ecs;

namespace Abduction101.Components
{
    public struct LimitMovementComponent : IEntityComponent
    {
        public float minZ, maxZ;
    }
    
    public class LimitMovementComponentDefinition : ComponentDefinitionBase
    {
        public float minZ, maxZ;
        
        public override string GetComponentName()
        {
            return nameof(LimitMovementComponent);
        }

        public override void Apply(World world, Entity entity)
        {
            world.AddComponent(entity, new LimitMovementComponent()
            {
                minZ = minZ,
                maxZ = maxZ
            });
        }
    }
}