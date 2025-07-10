using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// CPU-side replica of the curved-world deformation used in the water vertex shader.
/// Call BendPosition() to convert a *flat* world position into its curved counterpart.
/// </summary>
public static class CurvedWorldUtility
{
    #region CONFIGURATION

    ///// <summary>Property name in the shader (must match!)</summary>
    //private const string ShaderProp_BendRadius = "_BendRadius";

    /// <summary>
    ///   Smaller radius  →  stronger curvature.  
    ///   Default value equals the hard-coded 550 in your shader screenshot.
    ///   Call SyncFromMaterial() once at start-up if you expose the radius in the Inspector.
    /// </summary>
    public static float BendRadius = 550f;

    #endregion

    #region INITIALISATION HELPERS

    ///// <summary>
    ///// Pulls the current bend radius from a material that uses the curved shader.
    ///// Call this in Awake/Start *once* (or whenever you change the material at runtime).
    ///// </summary>
    //public static void SyncFromMaterial(Material mat)
    //{
    //    if (mat != null && mat.HasProperty(ShaderProp_BendRadius))
    //        BendRadius = mat.GetFloat(ShaderProp_BendRadius);
    //}

    ///// <summary>
    ///// Convenience overload when you already have a Renderer reference.
    ///// </summary>
    //public static void SyncFromRenderer(Renderer r)
    //{
    //    if (r != null)
    //        SyncFromMaterial(r.sharedMaterial);
    //}

    #endregion

    #region CURVE TRANSFORMS

    /// <summary>
    /// Applies y' = y − (z² / R) exactly like the vertex shader.
    /// Use this when you start with *flat* world coordinates (e.g. from physics or path-finding).
    /// </summary>
    public static Vector3 BendPosition(Vector3 worldPos)
    {
        worldPos.y -= (worldPos.z * worldPos.z) / BendRadius;
        return worldPos;
    }

    /// <summary>
    /// Inverse of the bend – useful if you already have a *curved* world position
    /// and need its flat equivalent (rare, but handy for gizmos or editor tools).
    /// </summary>
    public static Vector3 UnbendPosition(Vector3 curvedWorldPos)
    {
        curvedWorldPos.y += (curvedWorldPos.z * curvedWorldPos.z) / BendRadius;
        return curvedWorldPos;
    }

    /// <summary>
    /// Approximates how a normal/direction should bend so objects stay visually aligned
    /// (optional – buoyancy usually needs only BendPosition).
    /// </summary>
    public static Vector3 BendNormal(Vector3 worldNormal, Vector3 worldPos)
    {
        // Derivative of y = −z²/R is dy/dz = −2z/R.
        float slope = (-2f * worldPos.z) / BendRadius;          // tangent of rotation angle
        Quaternion q = Quaternion.AngleAxis(Mathf.Rad2Deg * Mathf.Atan(slope), Vector3.right);
        return q * worldNormal;
    }

    #endregion
}
