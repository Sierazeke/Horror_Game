using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxScare : MonoBehaviour 
{
    public GameObject BoxHolder;

    void OnTriggerEnter(Collider other)
    {
        BoxHolder.SetActive(false);
    }
}
