using Gemserk.Leopotam.Ecs;

namespace Abduction101.Components
{
    public struct CanBeAbductedComponent : IEntityComponent
    {
        public int abductedTimeout;
        public bool isBeingAbducted => abductedTimeout > 0;
        public float abductionSpeed;
        public float abductionForce;
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