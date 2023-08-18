using System;
using System.Collections.Generic;
using System.Linq;
using Gemserk.Leopotam.Ecs;

namespace Game.Components.Abilities
{
    public struct AbilitiesComponent : IEntityComponent
    {
        public static AbilitiesComponent Create()
        {
            return new AbilitiesComponent()
            {
                abilities = new List<Ability>()
            };
        }
        
        public List<Ability> abilities;
        public bool hasExecutingAbility;

        public Ability GetAbility(string name)
        {
            return abilities.FirstOrDefault(a => a.name.Equals(name, StringComparison.OrdinalIgnoreCase));
        }
    }
}