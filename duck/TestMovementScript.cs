using MoreMountains.Tools;
using MoreMountains.TopDownEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TestMovementScript : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField]
    Collider2D collider2d;

    bool waiting = false;
    public bool pickedUp { get; private set; }

    public int markedForDestroy = 0;

    private float directionDistance = 1.0f;

    public CharacterMovement characterMovement;

    public Animator animator;
    public SpriteRenderer spriteRenderer;
    public SpriteRenderer shadowRenderer;

    public int group = 0;

    public bool stationary = false;

    public bool offscreen = true;

    // GC allocation preventers
    private List<TestMovementScript> list = new List<TestMovementScript>();
    Collider2D[] results = new Collider2D[50];

    public UnityEvent onPickUp;

    public UnityEvent onPutMeDown;

    public UnityEvent onMatched;

    public UnityEvent onEndHit;

    void Start()
    {
        MMGameEvent.Trigger(Constants.DUCK_ENTER);
    }

    public void PutMeDown()
    {
        this.pickedUp = false;
        this.collider2d.enabled = true;
        this.animator.SetTrigger("OnDuckNormal");
        this.spriteRenderer.color = new Color(255, 255, 255, 1.0f);
        this.shadowRenderer.enabled = true;

        this.onPutMeDown?.Invoke();
    }
    public void PickMeUp()
    {
        this.pickedUp = true;
        this.collider2d.enabled = false;
        this.animator.SetTrigger("OnDuckPanic");
        this.spriteRenderer.color = new Color(255, 255, 255, Constants.TRANSPARENT_COLOR);
        this.shadowRenderer.enabled = false;

        this.onPickUp?.Invoke();

        //var fwoosh = ObjectPoolManager.Instance.fwooshPool.GetPooledGameObject();
        //fwoosh.transform.position = this.spriteRenderer.transform.position;
        //fwoosh.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        if (!this.pickedUp && this.markedForDestroy < 2)
        {

            if (!this.stationary)
            {
                this.characterMovement.SetHorizontalMovement(-1);
                this.characterMovement.SetVerticalMovement(0);
            }

            if (this.offscreen)
            {
                if (PlayZone.Instance.autoAttach.OverlapPoint(this.transform.position))
                {
                    this.spriteRenderer.enabled = true;
                    this.offscreen = false;
                }
                else
                {
                    if (this.spriteRenderer.enabled)
                    {
                        this.spriteRenderer.enabled = false;
                    }
                    // don't do these expensive raycasts 
                    return;
                }

            }

            bool selfDestroy = false;
            list.Clear();
            CheckForMatch(new Vector2(-1, 0));
            CheckForMatch(new Vector2(1, 0));
            if (list.Count >= 2)
            {
                Omnom(list);
                selfDestroy = true;
            }

            list.Clear();
            CheckForMatch(new Vector2(0, 1));
            CheckForMatch(new Vector2(0, -1));

            if (list.Count >= 2)
            {
                Omnom(list);
                selfDestroy = true;
            }

            if (selfDestroy)
            {
                this.Om();
            }
        }
        else
        {
            this.characterMovement.SetMovement(Vector2.zero);
        }
    }

    public void Om()
    {
        if (this.markedForDestroy > 0)
        {
            return;
        }
        this.collider2d.enabled = false;
        this.markedForDestroy = 1;

        this.onMatched?.Invoke();
    }

    public void LateUpdate()
    {
        if (this.markedForDestroy == 1)
        {
            this.markedForDestroy = 2;
            StartCoroutine(DestroyCoroutine());
        }
    }

    IEnumerator DestroyCoroutine()
    {
        this.animator.SetTrigger("OnDuckPanic");
        for (int i = 0; i < 5; i++)
        {
            this.transform.localScale = Vector3.one * 0.9f;
            yield return new WaitForSeconds(0.1f);
            this.transform.localScale = Vector3.one * 1.0f;
            yield return new WaitForSeconds(0.1f);
        }

        MMGameEvent.Trigger(Constants.DUCK_EXIT);
        this.animator.SetTrigger("OnDuckDisappear");
        yield return new WaitForSeconds(0.2f);
        Destroy(this.gameObject);
    }


    void Omnom(IEnumerable<TestMovementScript> list)
    {
        foreach (var x in list)
        {
            x.Om();
        }
    }

    List<TestMovementScript> CheckForMatch(Vector2 direction)
    {
        int num = 0;
        TestMovementScript previous = this;

        while (CheckOverlaps((Vector2)(previous.transform.position) + directionDistance * direction, out var c))
        {
            if (!list.Contains(c))
            {
                num++;
                list.Add(c);
                previous = c;
            } else
            {
                break;
            }
            
        }

        return list;
    }


    bool CheckOverlaps(Vector2 pos, out TestMovementScript d)
    {
        var overlaps = Physics2D.OverlapBoxNonAlloc(pos, new Vector2(.2f, 0.2f), 0, results);
        //DebugExtension.DebugBounds(new Bounds(pos, 0.2f * Vector3.one), Color.white);

        for (int i = 0; i < overlaps; i++)
        {
            var x = results[i];
            var duck = x.GetComponent<TestMovementScript>();
            if (duck is not null && !duck.pickedUp && duck != this)
            {
                d = duck;
                return true;
            }
        }

        d = null;
        return false;

    }




    IEnumerator JumpCoroutine()
    {
        yield return new WaitForSeconds(2.5f);
        var overlaps = Physics2D.OverlapBoxAll(new Vector2(this.gameObject.transform.position.x, this.gameObject.transform.position.y) - new Vector2(1, 0), new Vector2(0.4f, 0.4f), 0);
        waiting = false;

        if (this.pickedUp)
        {
            yield break;
        }

        foreach (var overlap in overlaps)
        {
            if (overlap.GetComponent<MCScript>())
            {

                // don't hop
                yield break;
            }
        }
        Hop();
    }

    void Hop()
    {
        this.gameObject.transform.Translate(new Vector2(-1, 0));
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.GetComponent<LoseZone>())
        {
            this.onEndHit?.Invoke();
            if (collision.gameObject.GetComponent<LoseZone>().testMode)
            {
                this.gameObject.transform.Translate(new Vector3(16, 0, 0));
            }
            else
            {
                HealthManager.Instance.TakeDamage();
                MMGameEvent.Trigger(Constants.DUCK_EXIT);
                Destroy(this.gameObject);
            }
        }
    }



    public void Bump(Vector3 direction)
    {

        if (this.CheckOverlaps(this.gameObject.transform.position + direction, out var nextBump))
        {
            nextBump.Bump(direction);
        }

        this.gameObject.transform.Translate(direction);
    }
}
