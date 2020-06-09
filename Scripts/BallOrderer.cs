using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallOrderer : MonoBehaviour
{

    private GameObject[] balls = new GameObject[3];
    public GameObject redExplosion;
    public GameObject greenExplosion;
    public GameObject blueExplosion;
    public GameObject yellowExplosion;
    private int ballCount = 0;
    private void OnTriggerEnter(Collider other)
    {
      if (GameManager.Instance.ballList.transform.childCount < 3) return;
        ballCount++;
        balls[ballCount - 1] = other.gameObject;
        if(ballCount == 3)
        {
            CheckColors();
        }
    }
    private void CheckColors()
    {
        Material material1 = balls[0].GetComponent<Renderer>().sharedMaterial;
        Material material2 = balls[1].GetComponent<Renderer>().sharedMaterial;
        Material material3 = balls[2].GetComponent<Renderer>().sharedMaterial;
      
        if(material2.Equals(material3))
        {
            if(material1.Equals(material2))
            {
                Color color = material1.color;

                if(color.r>0.5f && color.g >0.5f)
                {
                    //yellow
                    Instantiate(yellowExplosion, balls[0].transform.position,Quaternion.identity);
                    Instantiate(yellowExplosion, balls[1].transform.position, Quaternion.identity);
                    Instantiate(yellowExplosion, balls[2].transform.position, Quaternion.identity);
                }
                else if(color.b>0.5f)
                {
                    //blue
                    Instantiate(blueExplosion, balls[0].transform.position, Quaternion.identity);
                    Instantiate(blueExplosion, balls[1].transform.position, Quaternion.identity);
                    Instantiate(blueExplosion, balls[2].transform.position, Quaternion.identity);
                }
                else if(color.r > 0.5f)
                {
                    //red
                    Instantiate(redExplosion, balls[0].transform.position, Quaternion.identity);
                    Instantiate(redExplosion, balls[1].transform.position, Quaternion.identity);
                    Instantiate(redExplosion, balls[2].transform.position, Quaternion.identity);
                }
                else
                {
                    Instantiate(greenExplosion, balls[0].transform.position, Quaternion.identity);
                    Instantiate(greenExplosion, balls[1].transform.position, Quaternion.identity);
                    Instantiate(greenExplosion, balls[2].transform.position, Quaternion.identity);
                    //Green
                }
                Destroy(balls[0]);
                Destroy(balls[1]);
                Destroy(balls[2]);
                ballCount = 0;
                balls[1] = null;
                balls[2] = null;
                balls[0] = null;
                Debug.Log("Balls are destroyed");
            }
           else
            {
                ballCount = 2;
                balls[0] = balls[1];
                balls[1] = balls[2];
                balls[2] = null;
            }

        }
        else
        {
            ballCount = 1;
            balls[0] = balls[2];
            balls[1] = null;
            balls[2] = null;
        }
    }
}
