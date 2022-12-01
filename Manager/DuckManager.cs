using MoreMountains.Tools;
using MoreMountains.TopDownEngine;
using System.Collections;
using UnityEngine;

public class DuckManager : MonoBehaviour, MMEventListener<MMGameEvent>

{
    public static DuckManager Instance;

    int numDucks = 0;

    bool isFinishing = false; 

    public void OnMMEvent(MMGameEvent eventType)
    {
        if (eventType.EventName == Constants.DUCK_ENTER)
        {
            this.numDucks++;
        }
        else if (eventType.EventName == Constants.DUCK_EXIT)
        {
            this.numDucks--;
            if (SubLevelManager.Instance.currentLevel == 0)
            {
                return;
            }
            if (this.numDucks < 3 && isFinishing == false)
            {
                this.isFinishing = true;
                StartCoroutine(this.Finish());
            }
        }
    }

    private IEnumerator Finish()
    {
        yield return new WaitForSeconds(.4f);
        if (HealthManager.Instance.health <= 0)
        {
            yield break;
        }

        if (this.numDucks > 0 && !SubLevelManager.Instance.isTutorial)
        {
            DialoguePanel.Instance.ShowText("Less than 3 ducks left! Clearing them off for you!", 2.0f, false);
            TestMovementScript[] ducks = (TestMovementScript[])GameObject.FindObjectsOfType(typeof(TestMovementScript));
            foreach (var duck in ducks)
            {
                duck.Om();
            }
        }
        
        yield return new WaitForSeconds(2.0f);
        DialoguePanel.Instance.ShowText("Level complete!", 2.0f, false);

        yield return new WaitForSeconds(4.0f);
        
        SubLevelManager.Instance.FinishLevel();
        this.isFinishing = false;
    }

    private void Awake()
    {
        DuckManager.Instance = this;
    }


    void OnEnable()
    {
        this.MMEventStartListening<MMGameEvent>();
    }
    void OnDisable()
    {
        this.MMEventStopListening<MMGameEvent>();
    }

    public void Reset()
    {
        this.numDucks = 0;
        this.isFinishing = false;
    }
}
