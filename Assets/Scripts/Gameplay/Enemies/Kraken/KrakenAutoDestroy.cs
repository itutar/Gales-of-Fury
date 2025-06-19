using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KrakenAutoDestroy : MonoBehaviour
{
    //bool isFalling = false;

    private void Start()
    {
        //Invoke("StartFalling", 0.5f);
        Destroy(gameObject, 1.05f);
    }

    //private void Update()
    //{
    //    if (isFalling)
    //    {
    //        // Move the Kraken downwards
    //        transform.Translate(Vector3.down * Time.deltaTime * 5f, Space.World);
    //    }
    //}

    //void StartFalling()
    //{
    //    isFalling = true;
    //}
}
