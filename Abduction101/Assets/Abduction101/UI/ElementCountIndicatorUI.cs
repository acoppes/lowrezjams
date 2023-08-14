using UnityEngine;

namespace Abduction101.UI
{
    public class ElementCountIndicatorUI : MonoBehaviour
    {
        private int previousCount = -1;
        public int count = 0;

        public Transform countIconsParent;
        
        private void LateUpdate()
        {
            if (previousCount != count)
            {
                for (var i = 0; i < countIconsParent.childCount; i++)
                {
                    var child = countIconsParent.GetChild(i);
                    child.gameObject.SetActive(i < count);
                }

                previousCount = count;
            }
        }
    }
}