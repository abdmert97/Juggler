
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public enum ThrowType { FreeThrow, StraigthThrow, Circular, JugglerThrow};
    public static GameManager Instance { get; private set; }
    public ThrowType throwType;
    public AnimationCurve throwPatternX;
    public AnimationCurve throwPatternY;
    public float throwTime;
    public GameObject leftHand;
    public GameObject rightHand;
    public Transform rightHandTarget;
    public Transform leftHandTarget;
    public float oneHandAnimationTime;
    public float gravityForce;
    public bool twoHand = false;
    public List<Material> colors;
    public GameObject ballList;
    public float rotationSpeed;
    public static int ballCount = 0;
    private void Awake()
    {
        if (Instance)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }
    public Transform getClosestHand(Transform obj)
    {
        if (Vector3.Distance(obj.position, leftHand.transform.position) > Vector3.Distance(obj.position, rightHand.transform.position))
            return rightHand.transform;
        return leftHand.transform;
    }
}

