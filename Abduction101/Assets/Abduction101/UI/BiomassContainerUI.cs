using UnityEngine;

namespace Abduction101.UI
{
    public class BiomassContainerUI : MonoBehaviour
    {
        public SpriteRenderer bar;
        public float factor;

        // Update is called once per frame
        private void LateUpdate()
        {
            bar.transform.localScale = new Vector3(1, factor, 1);
        }
    }
}
