using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IMoveToLane 
{
    /// <summary>
    /// Starts movement to Lane X position
    /// </summary>
    void Initialize(float targetX);

    /// <summary>
    /// It's true when you get to Lane, complete the turn and don't move anymore.
    /// </summary>
    bool IsFinished { get; }
}
