using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Rigidbody2D myRb;
    private SpriteRenderer mySpRenderer;

    public float playerSpeed = 5f;
    public float jumpForce = 100f;
    public bool airControls = true;

    public delegate void OnJump();
    public static OnJump onJump;

    public delegate void OnLand();
    public static OnLand onLand;

    public float horizontalInput = 0f;
    private bool jump = false;
    private bool isGrounded = false;
    private string GROUND_TAG = "GROUND";

    private bool isMoving = false;
    private float jumpKeyHoldCurrentDuration = 0f;
    private float MAX_JUMPKEY_HOLD_DURATION = .5f;
    private float MAX_JUMP_FORCE = 20000f;
    private float MIN_JUMP_FORCE = 15000f;

    private void Awake()
    {
        myRb = GetComponent<Rigidbody2D>();
        mySpRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    private void Update()
    {
        HandleInputs();
        GroundDetection();
    }

    private void FixedUpdate()
    {
        myRb.velocity = new Vector2(horizontalInput * playerSpeed * Time.fixedDeltaTime, myRb.velocity.y);

        if(jump)
        {
            onJump?.Invoke();
            jump = false;
            myRb.AddForce(Vector2.up * jumpForce * Time.fixedDeltaTime);
        }
    }

    private void HandleInputs()
    {
        if (airControls || (!airControls && isGrounded))
        {
            horizontalInput = Input.GetAxis("Horizontal");
        }

        //Jump
        if(Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            jumpKeyHoldCurrentDuration = MAX_JUMPKEY_HOLD_DURATION;
        }

        if(Input.GetKey(KeyCode.Space) && isGrounded && jumpKeyHoldCurrentDuration > 0)
        {
            jumpKeyHoldCurrentDuration -= Time.deltaTime;
            if(jumpKeyHoldCurrentDuration <= 0)
            {
                jumpForce = Remap(jumpKeyHoldCurrentDuration, 0, MAX_JUMPKEY_HOLD_DURATION, MAX_JUMP_FORCE, MIN_JUMP_FORCE);
                jumpKeyHoldCurrentDuration = 0;
                jump = true;
            }
        }

        if(Input.GetKeyUp(KeyCode.Space) && jumpKeyHoldCurrentDuration > 0)
        {
            jumpForce = Remap(jumpKeyHoldCurrentDuration, 0, MAX_JUMPKEY_HOLD_DURATION, MAX_JUMP_FORCE, MIN_JUMP_FORCE);
            jumpKeyHoldCurrentDuration = 0;
            jump = true;
        }

        if (horizontalInput != 0)
        {
            TurnCharacter(horizontalInput < 0 ? true : false);
        }
    }

    private void TurnCharacter(bool flip)
    {
        if (mySpRenderer.flipX != flip)
        {
            mySpRenderer.flipX = flip;
        }
    }

    private void GroundDetection()
    {
        //TODO : Add Layermask
        Physics2D.queriesStartInColliders = false;
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, mySpRenderer.bounds.extents.y + .2f);
        if(hit.collider != null)
        {
            if(hit.collider.CompareTag(GROUND_TAG))
            {
                if (!isGrounded)
                {
                    Debug.Log("I Hit GROUND");
                    onLand?.Invoke();
                    isGrounded = true;
                }
            }

            else
            {
                Debug.Log("I Hit Something but not GROUND");
                isGrounded = false;
            }
        }

        else
        {
            if (isGrounded)
            {
                Debug.Log("I did not hit anything");
                isGrounded = false;
            }
        }
    }

    //private void OnDrawGizmos()
    //{
    //    Debug.DrawRay(transform.position, Vector2.down * (mySpRenderer.bounds.extents.y + .1f));
    //}

    public static float Remap(float value, float from1, float to1, float from2, float to2)
    {
        return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
    }
}
