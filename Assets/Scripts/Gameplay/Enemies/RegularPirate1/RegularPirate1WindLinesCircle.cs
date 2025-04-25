using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RegularPirate1WindLinesCircle : MonoBehaviour
{
    public Transform target;
    public float orbitSpeed = 50f;
    public float orbitRadius = 1.5f;

    private float angle;

    void Update()
    {
        if (target == null) return;

        angle += orbitSpeed * Time.deltaTime;
        float rad = angle * Mathf.Deg2Rad;
        Vector3 offset = new Vector3(Mathf.Cos(rad), 0, Mathf.Sin(rad)) * orbitRadius;
        transform.position = target.position + offset;
    }
}
