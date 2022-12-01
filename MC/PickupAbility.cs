using MoreMountains.Tools;
using MoreMountains.TopDownEngine;
using System.Linq;
using UnityEngine;

#nullable enable


public class PickupAbility : CharacterAbility
{
    public TopDownController2D topDownController;
    public BoxCollider2D boxCollider;
    public SpriteRenderer characterRenderer;

    public TestMovementScript? pickedUpDuck;
    public Transform cursor;
    public SpriteRenderer cursorRenderer;

    private float pickupReach = 1.6f;
    private float putdownReach = 1.6f;

    private float pickupRadius = .8f;
    private Vector3 boxSize = new Vector3(2f, 1.4f, 0);

    public TestMovementScript? pickupTarget { get; private set; }

    public bool drawDebug = true;

    public bool pickedUpThisFrame { get; private set; }

    float GetReach()
    {
        if (pickedUpDuck)
        {
            return putdownReach;
        }
        else
        {
            return pickupReach;
        }
    }

    Vector3 _GetCenter()
    {
        return gameObject.transform.position + new Vector3(0, boxCollider.bounds.size.y);
    }

    Vector3 GetWalkingFaceVector()
    {
        var direction = topDownController.CurrentDirection.normalized;

        var inFrontPos = gameObject.transform.position + new Vector3(0, boxCollider.bounds.size.y / 2) + direction;

        return inFrontPos;
    }
    Vector3 getTargetVector(float reach)
    {

        /* Getting based on walking direction */
        //var direction = PickupAbility.getBiggerDirection(topDownController.CurrentDirection);

        //var inFrontPos = gameObject.transform.position + new Vector3(0, boxCollider.bounds.size.y / 2) + direction;

        /* Getting based on mouse */
        //var inFrontPos = (Vector2)(Camera.main.ScreenToWorldPoint(Input.mousePosition));
        //var center = (Vector2)GetCenter();

        //var delta = (inFrontPos - center);

        //if (delta.magnitude > reach)
        //{
        //    delta = delta.normalized * reach;
        //}

        //var targetPos = center + delta;

        //return targetPos;

        /* Getting based on position */

        return this.gameObject.transform.position + new Vector3(0, boxCollider.bounds.size.y);
    }

    Vector3 _getBottomLeft(Vector3 input)
    {
        return new Vector3(Mathf.Floor(input.x), Mathf.Floor(input.y));
    }

    // Update is called once per frame
    void Update()
    {
        this.pickedUpThisFrame = false;
        var target = getTargetVector(GetReach());
        //var boxCoordBottomLeft = getBottomLeft(target);
        //DebugExtension.DebugBounds(new Bounds(boxCoordBottomLeft + Vector3.one * .5f, Vector3.one), Color.red);
        //DebugExtension.DebugCircle(target, Vector3.forward, Color.red, radius: pickupRadius, depthTest:   false);

        if (this.pickedUpDuck is not null)
        {
            this.pickedUpDuck.gameObject.transform.position = this.gameObject.transform.position + new Vector3(0, 1, 0);

            var placingPosition = GetPlacingPosition(out var blocked);

            this.cursor.gameObject.SetActive(true);
            if (blocked)
            {
                this.cursorRenderer.color = Color.red;
            }
            else
            {
                this.cursorRenderer.color = Color.white;
            }
            this.cursor.position = placingPosition;
        }
        else
        {
            this.pickupTarget = GetPickupDuck();
            if (this.pickupTarget is not null)
            {
                this.cursor.gameObject.SetActive(true);
                this.cursor.position = this.pickupTarget.transform.position;
            }
            else
            {
                this.cursor.gameObject.SetActive(false);
            }
        }


    }

    static Vector3 getBiggerDirection(Vector3 input)
    {
        if (Mathf.Abs(input.x) > Mathf.Abs(input.y))
        {
            return new Vector3(input.x, 0, 0).normalized;
        }
        else
        {
            return new Vector3(0, input.y, 0).normalized;
        }
    }

    bool _testForDucks(Vector3 position)
    {
        var overlaps = Physics2D.OverlapPointAll(position);
        foreach (var overlap in overlaps)
        {
            if (overlap.GetComponent<TestMovementScript>())
            {
                return true;
            }
        }

        return false;
    }

    TestMovementScript? GetPickupDuck()
    {
        var target = getTargetVector(GetReach());

        var overlaps = Physics2D.OverlapBoxAll(target, boxSize, 0);
        if (this.drawDebug) DebugExtension.DebugBounds(new Bounds(target, boxSize));
        //DebugExtension.DebugCircle(target, Vector3.forward, Color.red, radius: pickupRadius, depthTest: false);

        var faceVector = this.GetWalkingFaceVector();
        if (this.drawDebug) DebugExtension.DebugCircle(faceVector, Vector3.forward, Color.red, radius: 0.1f, depthTest: false);

        var distance = Mathf.Infinity;
        TestMovementScript? duck = null;

        foreach (var overlap in overlaps)
        {
            var duckScript = overlap.GetComponent<TestMovementScript>();
            if (duckScript)
            {
                var calcDistance = (faceVector - duckScript.transform.position).magnitude;
                if (calcDistance < distance)
                {
                    duck = duckScript;
                    distance = calcDistance;
                }
            }
        }

        return duck;
    }

    Vector3 GetPlacingPosition(out bool blocked)
    {
        blocked = false;
        var placing = this.pickedUpDuck is not null;
        var target = getTargetVector(GetReach()) + new Vector3(0, 0.2f);

        if (!PlayZone.Instance.autoAttach.OverlapPoint(target))
        {
            blocked = true;
            return target;
        }

        var overlaps = Physics2D.OverlapCircleAll((target), (placing) ? 0.1f : pickupRadius);

        if (overlaps.Any(x => x.GetComponent<TestMovementScript>() is not null))
        {
            blocked = true;
            //int i = 0;
            //for (; i < Constants.CardinalDirections.Length; ++i)
            //{

            //    var nextDirection = (target + Constants.CardinalDirections[i] * 0.3f);
            //    DebugExtension.DebugPoint(nextDirection, 0.1f, depthTest: false);
            //    if (!this.testForDucks(nextDirection))
            //    {
            //        target = nextDirection;
            //        break;
            //    }
            //}

            //if (i == Constants.CardinalDirections.Length)
            //{
            //    return this.transform.position;
            //}
        }

        return new Vector3(target.x, Mathf.Floor(target.y) + 0.5f, 0);
    }

    protected override void HandleInput()
    {
        if (_inputManager.InteractButton.State.CurrentState == MMInput.ButtonStates.ButtonDown)
        {
            PlayAbilityStartFeedbacks();

            if (this.pickedUpDuck is not null)
            {
                var position = GetPlacingPosition(out var blocked);

                if (!blocked)
                {
                    this.pickedUpDuck.gameObject.transform.position = position;
                    this.pickedUpDuck.PutMeDown();
                    this.pickedUpDuck = null;
                    this.characterRenderer.color = new Color(255, 255, 255, 1.0f);
                }
            }
            else
            {
                if (this.pickupTarget is not null)
                {
                    this.characterRenderer.color = new Color(255, 255, 255, Constants.TRANSPARENT_COLOR);
                    this.AttachDuck(this.pickupTarget);
                    this.pickupTarget = null;
                    this.pickedUpThisFrame = true;
                    return;
                }
            }


        }
    }

    private void AttachDuck(TestMovementScript duck)
    {
        this.pickedUpDuck = duck;
        duck.PickMeUp();
    }

    private void _BumpDuck(TestMovementScript duckScript)
    {
        var delta = duckScript.gameObject.transform.position - this.gameObject.transform.position;
        var direction = new Vector3(0, 0, 0);
        if (Mathf.Abs(delta.x) > Mathf.Abs(delta.y))
        {
            direction.x = Mathf.Sign(delta.x);
        }
        else
        {
            direction.y = Mathf.Sign(delta.y);
        }
        Debug.Log(string.Format("Is bumping! {0}", direction));
        duckScript.Bump(direction);
    }
}
