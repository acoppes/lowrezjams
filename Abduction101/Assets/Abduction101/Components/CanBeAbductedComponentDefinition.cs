using Gemserk.Leopotam.Ecs;

namespace Abduction101.Components
{
    public struct CanBeAbductedComponent : IEntityComponent
    {
        public bool isBeingAbducted;
        public float abductionSpeed;
    }
    
    public class CanBeAbductedComponentDefinition : ComponentDefinitionBase
    {
        public override string GetComponentName()
        {
            return nameof(CanBeAbductedComponent);
        }

        public override void Apply(World world, Entity entity)
        {
            world.AddComponent(entity, new CanBeAbductedComponent());
        }
    }
}