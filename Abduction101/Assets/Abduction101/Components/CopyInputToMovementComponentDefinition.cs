using Gemserk.Leopotam.Ecs;

namespace Abduction101.Components
{
    public struct CopyInputToMovementComponent : IEntityComponent
    {
        
    }
    
    public class CopyInputToMovementComponentDefinition : ComponentDefinitionBase
    {
        public override string GetComponentName()
        {
            return nameof(CopyInputToMovementComponent);
        }

        public override void Apply(World world, Entity entity)
        {
            world.AddComponent(entity, new CopyInputToMovementComponent());
        }
    }
}