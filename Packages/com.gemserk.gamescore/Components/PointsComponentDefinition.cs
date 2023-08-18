using Gemserk.Leopotam.Ecs;
using Gemserk.Utilities;

namespace Game.Components
{
    public struct PointsComponent : IEntityComponent
    {
        public float points;
        public float baseMultiplier;
        public float extraMultiplier;
        public float currentMultiplier => baseMultiplier + extraMultiplier;

        public Cooldown multiplierDuration;

        public float maxMultiplier;
    }
    
    public class PointsComponentDefinition : ComponentDefinitionBase
    {
        public float startingPoints = 0;
        public float baseMultiplier = 1;
        public float multiplierDuration = 1;
        
        public override string GetComponentName()
        {
            return nameof(PointsComponent);
        }

        public override void Apply(World world, Entity entity)
        {
            world.AddComponent(entity, new PointsComponent()
            {
                points = startingPoints,
                baseMultiplier = baseMultiplier,
                multiplierDuration = new Cooldown(multiplierDuration)
            });
        }
    }
}