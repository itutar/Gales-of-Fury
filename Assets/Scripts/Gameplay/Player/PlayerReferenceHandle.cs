using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerReferenceHandle : MonoBehaviour
{
    public PlayerReference playerReference;

    private void Awake()
    {
        playerReference.player = this.gameObject;
    }
}
