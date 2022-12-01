using MoreMountains.TopDownEngine;
using UnityEngine;

public class MCScript : MonoBehaviour
{
    public static MCScript Instance;

    public Health health;
    public Character character;

    // Start is called before the first frame update
    void Start()
    {
        MCScript.Instance = this;
    }

    public void TakeDamage()
    {
        health.Damage(1, this.gameObject, 0, 0, Vector3.zero);
    }
}
