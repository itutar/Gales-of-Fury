using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    // test
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            ScoreManager.Instance.Add(1111); // test i�in puan ekle
        }
        else if (Input.GetKeyDown(KeyCode.R))
        {
            ScoreManager.Instance.Add(100000); // test i�in puan� s�f�rla
        }
        else if (Input.GetKeyDown(KeyCode.T))
        {
            ScoreManager.Instance.Add(1000000); // test i�in puan� s�f�rla
        }
        else if (Input.GetKeyDown(KeyCode.Y))
        {
            ScoreManager.Instance.Add(10000000); // test i�in puan� s�f�rla
        }
        else if (Input.GetKeyDown(KeyCode.A))
        {
            ScoreManager.Instance.Add(10000); // test i�in puan� s�f�rla
        }
        else if (Input.GetKeyDown(KeyCode.I))
        {
            ScoreManager.Instance.Add(5000); // test i�in puan� s�f�rla
        }

    }
}
