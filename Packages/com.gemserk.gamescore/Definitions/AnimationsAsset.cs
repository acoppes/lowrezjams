using System;
using System.Collections.Generic;
using Game.Components;
using MyBox;
using UnityEngine;

namespace Game.Definitions
{
    [CreateAssetMenu(menuName = "Gemserk/Animation")]
    public class AnimationsAsset : ScriptableObject
    {
        public string sourceFolder;
        
        public List<AnimationDefinition> animations = new();

        private IDictionary<string, int> cachedAnimations;

        [ButtonMethod]
        private void ClearCache()
        {
            if (cachedAnimations != null)
            {
                cachedAnimations.Clear();
            }
            cachedAnimations = null;
        }
       
        private void CreateCache()
        {
            if (cachedAnimations != null)
            {
                return;
            }

            cachedAnimations = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
            
            for (var i = 0; i < animations.Count; i++)
            {
                var animation = animations[i];
                cachedAnimations[animation.name] = i;
            }
        }
        
        public int GetAnimationIndexByName(string animationName)
        {
            CreateCache();
            return cachedAnimations[animationName];
        }

        public bool HasAnimation(string animationName)
        {
            CreateCache();
            return cachedAnimations.ContainsKey(animationName);
        }
    }
}