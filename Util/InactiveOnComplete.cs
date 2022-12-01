using UnityEngine;

public class InactiveOnComplete
    : MonoBehaviour
{

    public void OnAnimationComplete()
    {
        this.gameObject.SetActive(false);
    }
}
