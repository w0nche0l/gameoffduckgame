using MoreMountains.Tools;
using UnityEngine;

public class ObjectPoolManager : MonoBehaviour
{
    public static ObjectPoolManager Instance;

    [field: SerializeField]
    public MMSimpleObjectPooler fwooshPool { get; private set; }

    private void Awake()
    {
        ObjectPoolManager.Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
