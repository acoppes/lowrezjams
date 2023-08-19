using UnityEngine;

public class TestTeleportObjectBounds : MonoBehaviour
{
    public float minX, maxX;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        var p = transform.position;
        if (p.x > maxX)
        {
            p.x = minX;
        }

        if (p.x < minX)
        {
            p.x = maxX;
        }

        transform.position = p;
    }
}
