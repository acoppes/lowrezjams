using Gemserk.Leopotam.Ecs;
using UnityEngine;

namespace Abduction101.Data
{
    public class RoomSpawn : MonoBehaviour
    {
        public RoomConfiguration roomConfiguration;

        private void Start()
        {
            var world = World.Instance;
            if (world == null)
            {
                return;
            }

            var transforms = roomConfiguration.GetSpawnObjects();

            foreach (var t in transforms)
            {
                var spawnData = roomConfiguration.GetSpawnData(t.gameObject.name);
                if (spawnData == null)
                {
                    continue;
                }

                if (UnityEngine.Random.Range(0f, 1f) > spawnData.chance)
                {
                    continue;
                }

                var instance = t.GetComponent<EntityPrefabInstance>();
                instance.InstantiateEntity();
            }
            
        }
    }
}