using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomIsRun : MonoBehaviour
{
    private Animator animator;
    void Start()
    {
        animator = GetComponent<Animator>();
        animator.SetBool("isRun", true);

    }


}
