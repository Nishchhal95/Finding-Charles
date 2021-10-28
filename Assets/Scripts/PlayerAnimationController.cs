using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationController : MonoBehaviour
{
    private Animator myAnimator;
    [SerializeField] private PlayerController playerController;

    private void Awake()
    {
        myAnimator = GetComponentInChildren<Animator>();
    }

    private void OnEnable()
    {
        PlayerController.onJump += PlayJumpAnimation;
        PlayerController.onLand += PlayLandAnimation;
    }

    private void OnDisable()
    {
        PlayerController.onJump -= PlayJumpAnimation;
        PlayerController.onLand -= PlayLandAnimation;
    }

    private void Update()
    {
        HandleAnimations();
    }

    private void HandleAnimations()
    {
        if(playerController == null)
        {
            return;
        }

        //Moving
        bool isMoving = playerController.horizontalInput == 0 ? false : true;
        myAnimator.SetBool("isMoving", isMoving);
    }

    private void PlayJumpAnimation()
    {
        myAnimator.SetTrigger("Jump");
    }

    private void PlayLandAnimation()
    {
        myAnimator.SetTrigger("Land");
    }
}
