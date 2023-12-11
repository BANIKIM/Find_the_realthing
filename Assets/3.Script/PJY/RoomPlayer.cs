using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
public class RoomPlayer : NetworkRoomPlayer
{
    public bool isOK = false;
    Animator animator;

    private void Awake()
    {
        animator = FindObjectOfType<Animator>();
    }

    private void Update()
    {
        Ready();

        if(isOK)
        {
            animator.SetBool("isRun",true);
        }
        else
        {
            animator.SetBool("isRun", false);

        }
    }
    private void Ready()
    {
        if(readyToBegin)
        {
            isOK = true;
        }
        else
        {
            isOK = false;
        }
    }
}
