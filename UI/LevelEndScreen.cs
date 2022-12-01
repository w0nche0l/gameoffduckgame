using MoreMountains.Tools;
using MoreMountains.TopDownEngine;
using UnityEngine;

public class LevelEndScreen : MonoBehaviour, MMEventListener<TopDownEngineEvent>
{
    /// the canvas group containing the winner screen
    [Tooltip("the canvas group containing the winner screen")]
    public CanvasGroup WinnerScreen;

    /// <summary>
    /// On Start we make sure our screen is disabled
    /// </summary>
    protected virtual void Start()
    {
        WinnerScreen.gameObject.SetActive(false);
    }
    public virtual void OnMMEvent(TopDownEngineEvent tdEvent)
    {
        switch (tdEvent.EventType)
        {
            case TopDownEngineEventTypes.LevelComplete:
                WinnerScreen.gameObject.SetActive(true);
                WinnerScreen.alpha = 0f;
                StartCoroutine(MMFade.FadeCanvasGroup(WinnerScreen, 0.5f, 1.0f, true));
                break;
        }
    }

    /// <summary>
    /// OnDisable, we start listening to events.
    /// </summary>
    protected virtual void OnEnable()
    {
        this.MMEventStartListening<TopDownEngineEvent>();
    }

    /// <summary>
    /// OnDisable, we stop listening to events.
    /// </summary>
    protected virtual void OnDisable()
    {
        this.MMEventStopListening<TopDownEngineEvent>();
    }
}
