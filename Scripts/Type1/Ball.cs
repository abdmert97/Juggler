using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Ball : MonoBehaviour
{
    public enum BallState { FreeFall, Throw };
    public BallState ballState;
    public float throwHeight = 5;
    public bool isVerticalThrow = true;
    public bool isAddedInEditor = false;

    private const string leftHand = "LeftHand";
    private const string rightHand = "RightHand";
    private Rigidbody _rigidbody;
    private Collider _collider;
    private Renderer _renderer;
    private float _throwTime;
    private WaitForFixedUpdate _waitFixed = new WaitForFixedUpdate();
    private AnimationCurve _throwPatternX;
    private AnimationCurve _throwPatternY;
    private bool _circularEnabled = false;
    private Vector3 _circularSpeed = Vector3.right*5;
    private float _rotationSpeed;
    private Hand.HandType _currentHand;
    private const string _ballTag ="Ball";
    private GameObject _activate = null;
    private int _ballOrder;
    private void Start()
    {
        _throwTime = GameManager.Instance.throwTime;
        _throwPatternX = GameManager.Instance.throwPatternX;
        _throwPatternY = GameManager.Instance.throwPatternY;
        _collider = GetComponent<Collider>();
        _rigidbody = GetComponent<Rigidbody>();
        _renderer = GetComponent<Renderer>();
        _rotationSpeed = GameManager.Instance.rotationSpeed;
        if(isAddedInEditor)
            _renderer.sharedMaterial = GameManager.Instance.colors[Random.Range(0, GameManager.Instance.colors.Count)];
       Invoke("TargetFind", 0.05f);
        GameManager.ballCount++;
       _ballOrder = GameManager.ballCount;
        if (GameManager.Instance.throwType == GameManager.ThrowType.JugglerThrow)
        {
            throwHeight += 0.4f * GameManager.ballCount;
        }
    }
    void TargetFind()
    {
        Transform target = GameManager.Instance.getClosestHand(transform);
        _currentHand = target.Equals(GameManager.Instance.leftHand.transform) ? Hand.HandType.RIGHT : Hand.HandType.LEFT;
        CalculateFreethrow( false, 0);
    }

    private void FixedUpdate()
    {
        if(GameManager.Instance.throwType == GameManager.ThrowType.Circular&&_circularEnabled)
        {
           CircularThrow();
        }
        else
        {
            if (ballState == BallState.FreeFall)
                _rigidbody.velocity += Vector3.up * GameManager.Instance.gravityForce * Time.fixedDeltaTime;
        }
    }

    private void CircularThrow()
    {
        _rigidbody.velocity  = Quaternion.Euler(Vector3.forward * _rotationSpeed) * _rigidbody.velocity ;
    }


    private void OnTriggerExit(Collider other)
    {
        GameManager.Instance.twoHand = false;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(leftHand))
        {
            _currentHand = Hand.HandType.LEFT;
            GameManager.Instance.leftHandTarget = transform;
        }
        else if (other.CompareTag(rightHand))
        {
            _currentHand = Hand.HandType.RIGHT;
            GameManager.Instance.rightHandTarget = transform;
        }
        else if (other.CompareTag(_ballTag))
        {
            ResolveConflict(other.attachedRigidbody);
            return;
        }

        SetState();
    }

    private void ResolveConflict(Rigidbody otherRigid)
    {
        if (otherRigid == null) return;
        if(_rigidbody.velocity.y >otherRigid.velocity.y)
        {
            //2. daya aşağıda
            gameObject.SetActive(false);
            _activate = gameObject;
        }
        else
        {
            otherRigid.gameObject.SetActive(false);
            _activate = otherRigid.gameObject;
        }
        Invoke("Activate", Time.fixedDeltaTime*2);
    }
    private void Activate()
    {
        if(_activate != null)
        _activate.SetActive(true);
    }
    private void SetState()
    {
        if (GameManager.Instance.throwType == GameManager.ThrowType.FreeThrow)
            StartCoroutine(FreeThrow());
        else if(GameManager.Instance.throwType == GameManager.ThrowType.JugglerThrow)
            StartCoroutine(FreeThrow());
        else if (GameManager.Instance.throwType == GameManager.ThrowType.Circular)
        {

            if (isVerticalThrow)
            {
                CalculateFreethrow(false, 0.15f);
                GameManager.Instance.twoHand = true;
                _circularEnabled = false;
            }
            else
            {
                _circularEnabled = true;
                _rigidbody.velocity = _circularSpeed;
            }
        }
        else
        {
            if (isVerticalThrow)
            {
                CalculateFreethrow(false, 0.15f);
                GameManager.Instance.twoHand = true;
            }
            else
            {
                CalculateFreethrow(false, -1);
            }
        }
        isVerticalThrow = !isVerticalThrow;
    }
    private void Juggler()
    {
        StartCoroutine(FreeThrow());
    }

    IEnumerator FreeThrow()
    {
        OpenThrowState();
        _rigidbody.velocity = Vector3.zero;

        int iterationCount = (int)(_throwTime / Time.fixedDeltaTime);
        OpenThrowState();
        float curveTime = 0;
        Vector3 startPosition = transform.position;
        float curveAmountY = _throwPatternY.Evaluate(curveTime);
        float curveAmountX = _throwPatternX.Evaluate(curveTime);
        float curveLength = _throwPatternY[ _throwPatternY.length - 1].time;
        Vector3 throwX = transform.position.x < 0 ? Vector3.right : Vector3.left;
        Vector3 horizontalThrow = transform.position.x < 0 ? Vector3.right : Vector3.left;

        for (int i = 0; i < iterationCount; i++)
        {
            curveTime += Time.fixedDeltaTime;
            curveAmountY = _throwPatternY.Evaluate(curveTime);
            curveAmountX = _throwPatternX.Evaluate(curveTime) * (1f +0.3f* _ballOrder);

            _rigidbody.velocity = Vector3.up * curveAmountY * 5 + horizontalThrow * curveAmountX * 5;

            yield return _waitFixed;
        }

        transform.SetParent(null);
        CloseThrowState();
        CalculateFreethrow(false);
    }

    private void CalculateFreethrow(bool randomHeight = true, float givenHeight = -1)
    {
        Vector3 target = _currentHand == Hand.HandType.LEFT ? GameManager.Instance.rightHand.transform.position : GameManager.Instance.leftHand.transform.position;
        Vector3 distance = target - transform.position;
        float height;
        if (randomHeight == true)
            height = throwHeight + Random.Range(-throwHeight / 10f, throwHeight / 10f);
        else
            height = throwHeight;
        if (givenHeight != -1)
            height = givenHeight;
        float gravity = GameManager.Instance.gravityForce;

        float t_2 = (float)Math.Sqrt(Math.Abs((2 * (height - distance.y)) / gravity));
        float t_1 = (float)Math.Sqrt(Math.Abs(2 * height / gravity));
        float velocityX = distance.x / (t_1 + t_2);
        float velocityY = t_1 * -1 * gravity;
        _rigidbody.velocity = new Vector3(velocityX, velocityY, 0);
    }
  
    
    private void CloseThrowState()
    {
        ballState = BallState.FreeFall;
        Invoke("OpenCollider", 0.4f);
    }
    private void OpenCollider()
    {
        _collider.enabled = true;
    }
    private void OpenThrowState()
    {
        ballState = BallState.Throw;
        //  _collider.enabled = false;
    }
   

}
