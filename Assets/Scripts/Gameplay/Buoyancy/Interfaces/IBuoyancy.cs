using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Used to Sink or Float an object in water.
/// </summary>
public interface IBuoyancy 
{
    float SinkFactor { get; set; }

}
