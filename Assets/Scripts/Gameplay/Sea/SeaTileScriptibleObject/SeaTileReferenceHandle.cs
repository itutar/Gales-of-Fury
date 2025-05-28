using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeaTileReferenceHandle : MonoBehaviour
{
    public SeaTileReference seaTileReference;

    private void Awake()
    {
        seaTileReference.seaTileReferenceObject = this.gameObject;
    }
}
