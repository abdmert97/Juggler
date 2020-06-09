using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class BallLister : MonoBehaviour
{
    public GameObject ballPrefab;
    public int ballCount;
    private List<GameObject> ballList;
    private Vector3 _startPosition;
    public Transform spawnPoint;
    public Transform parentBall;
    private void Start()
    {
        ballList = new List<GameObject>();
        _startPosition = transform.position;
        for(int i = 0; i < ballCount;i++)
        {
            GameObject ball = Instantiate(ballPrefab, transform);
            ball.GetComponent<Renderer>().sharedMaterial = GameManager.Instance.colors[Random.Range(0, GameManager.Instance.colors.Count)];
            ballList.Add(ball);
        }
        InputHandler.Touch += AddBall;
        OrderBall();
    }

    private void AddBall()
    {
        GameObject ball = getBall();
        ball.transform.position = spawnPoint.position;
        ball.GetComponent<Ball>().enabled = true;
        ball.transform.SetParent(parentBall);  
    }

    public GameObject getBall()
    {
     GameObject ballDequed =  ballList[0];
     ballList.RemoveAt(0);
     GameObject ball = Instantiate(ballPrefab, transform);
     ball.GetComponent<Renderer>().sharedMaterial = GameManager.Instance.colors[Random.Range(0, GameManager.Instance.colors.Count)];
     ballList.Add(ball);
     OrderBall();
     return ballDequed;
    }
    private void OrderBall()
    {
        for(int i  = 0; i < ballCount;i++)
        {
            GameObject ball = ballList[i];
            ball.transform.position =_startPosition+ Vector3.right * i * 2 * ball.transform.localScale.x;
        }
    }
}
