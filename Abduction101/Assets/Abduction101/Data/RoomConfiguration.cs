using System;
using System.Collections.Generic;
using System.Linq;
using Gemserk.Utilities;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Abduction101.Data
{
    [Serializable]
    public class RoomSpawnData
    {
        public string tag;
        public float chance;
    }
    
    public class RoomConfiguration : MonoBehaviour
    {
        public List<RoomSpawnData> spawnData;

        public List<Transform> GetSpawnObjects()
        {
            var transforms = new List<Transform>();
            gameObject.GetComponentsInChildren(false, true, 1, transforms);
            return transforms;
        }

        public RoomSpawnData GetSpawnData(string tag)
        {
            return spawnData.FirstOrDefault(s => s.tag.Equals(tag, StringComparison.OrdinalIgnoreCase));
        }
    }
}