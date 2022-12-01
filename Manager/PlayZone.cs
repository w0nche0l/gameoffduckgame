using UnityEngine;

public class PlayZone : MonoBehaviour
{

    public static PlayZone Instance;
    public BoxCollider2D autoAttach;

    private void Awake()
    {
        PlayZone.Instance = this;
    }

}
