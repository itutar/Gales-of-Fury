using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Enemy possible states
public enum EnemyState
{
    Idle,       // Before starting attack (can be spawn delay or entrance)
    Attacking,  // Actively attacking the player
    Escaping,   // Leaving the screen after attack time expired
    Dying,      // Playing death animation (HP depleted)
    Dead        // Ready to be removed from the scene
}