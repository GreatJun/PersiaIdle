using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class test : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("Enter" + other.name);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        Debug.Log("Exit" + other.name);
    }
}
