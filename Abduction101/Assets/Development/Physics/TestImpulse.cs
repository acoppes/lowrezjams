using UnityEngine;

public class TestImpulse : MonoBehaviour
{
    public float impulse = 1000;
    // Start is called before the first frame update
    void Start()
    {
        Invoke(nameof(MegaImpulse), 1);
    }

    void MegaImpulse()
    {
        GetComponent<Rigidbody>().AddForce(Vector3.up * impulse, ForceMode.Impulse);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionStay(Collision other)
    {
        Debug.Log(other.gameObject.name);
    }
}
