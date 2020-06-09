using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hand : MonoBehaviour
{
    public Animator animator;
    public enum HandType { LEFT, RIGHT };
    // Start is called before the first frame update
    public HandType handType;
    public Animator jugglerAnimator;    
    public static readonly int LeftHand = Animator.StringToHash("LeftHand");
    public static readonly int RightHand = Animator.StringToHash("RightHand");
    public static readonly int TwoHand = Animator.StringToHash("TwoHand");
    public Transform leftHand;
    public Transform rightHand;
    public IKControl IKController;
    private void Start()
    {
    }
    private void OnTriggerEnter(Collider other)
    {
        ActiveHand(other.transform);
        if (IKController.lookObj == null)
        IKController.lookObj = other.transform;
    }
    private void OnTriggerStay(Collider other)
    {
        if (GameManager.Instance.throwType != GameManager.ThrowType.FreeThrow)
            ActiveHand(other.transform);
    }
    private void OnTriggerExit(Collider other)
    {
        DeactiveHand();
      
    }

    private void DeactiveHand()
    {
        if (handType == HandType.LEFT)
        {
            animator.SetBool(LeftHand, false);
            Invoke("LeftHandSet", 0.05f);
        }
        else
        {
            animator.SetBool(RightHand, false);
            Invoke("RightHandSet", 0.05f);
        }
    }
    private void LeftHandSet()
    {
        IKController.leftHandObj = null;
    }

    private void RightHandSet()
    {
        IKController.rightHandObj = null;
    }


    private void ActiveHand(Transform ball)
    {
        if (GameManager.Instance.twoHand == true)
        {
            animator.SetBool(TwoHand, true);
            GameManager.Instance.twoHand = false;
        }
        if (handType == HandType.LEFT)
        {
            animator.SetBool(LeftHand, true);
            IKController.leftHandObj=ball;
        }
        else
        {
            animator.SetBool(RightHand, true);
            IKController.rightHandObj = ball;
        }
    }
}
