using MoreMountains.TopDownEngine;
using System.Text;
using UnityEngine;

public class MCAnimator : MonoBehaviour
{
    private static string animationPrefix = "base.";
    public string currentAnimation { get; private set; }


    public TopDownController controller;
    public CharacterOrientation2D characterOrientation2D;
    public PickupAbility pickupAbility;

    public Animator animator;

    private void Start()
    {
        SetAnimation("farmer_idle_down");
    }

    private void LateUpdate()
    {
        var animationName = new StringBuilder();
        animationName.Append(MCAnimator.animationPrefix);
        animationName.Append(pickupAbility.pickedUpDuck is not null ? "farmer_carry" : "farmer");

        if (pickupAbility.pickedUpThisFrame)
        {
            animationName.Append("_pickup");
        }
        else if (controller.CurrentMovement.sqrMagnitude > 0)
        {
            animationName.Append("_run");
        }
        else
        {
            animationName.Append("_idle");
        }

        switch (characterOrientation2D.CurrentFacingDirection)
        {
            case Character.FacingDirections.North:
                animationName.Append("_up");
                break;
            case Character.FacingDirections.East:
                animationName.Append("_right");
                break;
            case Character.FacingDirections.South:
                animationName.Append("_down");
                break;
            case Character.FacingDirections.West:
                animationName.Append("_left");
                break;
        }

        SetAnimation(animationName.ToString());
    }

    public void SetAnimation(string s)
    {
        if (this.currentAnimation != s)
        {
            this.currentAnimation = s;
            animator.Play(s);
        }
    }
}
