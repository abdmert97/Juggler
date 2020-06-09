using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Action = System.Action;

public class JugglerController : MonoBehaviour
{
    private Animator _animator;

    private void Start()
    {
        _animator = GetComponent<Animator>();
        if(GameManager.Instance.throwType == GameManager.ThrowType.Circular)
        {
            transform.position = new Vector3(0.77f, -2.4f, 0.34f);
        }
  
    }
    private void Update()
    {
        GetComponent<Rigidbody>().velocity = Vector3.back * 5;
    }
}
