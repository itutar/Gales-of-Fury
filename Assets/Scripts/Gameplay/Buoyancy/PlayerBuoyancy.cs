using Pinwheel.Poseidon;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBuoyancy : MonoBehaviour
{
    #region Fields

    public PWater water;         // Su yüzeyini temsil eden varlýk
    public bool applyRipple;     // Dalga etkisini uygulayýp uygulamayacaðýmýz
    private Rigidbody rb;

    public float buoyancyStrength = 10f;  // Kaldýrma kuvveti için katsayý
    public float damping = 0.5f;          // Sürtünme kuvveti için katsayý
    public float verticalDamping = 2f;   // Y eksenindeki sönümleme katsayýsý

    //Jumping flag
    bool isJumping = false;
    Coroutine buoyancyCoroutine;

    // EndlessTiles reference to get the list of the tiles
    private EndlessTiles endlessTiles;

    #endregion

    #region Unity Methods

    private void OnEnable()
    {
        InputManager.OnSwipeUp += DetectJump;
    }

    private void OnDisable()
    {
        InputManager.OnSwipeUp -= DetectJump;
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            Debug.LogError("Buoyancy için Rigidbody gerekli!");
        }
    }

    private void Start()
    {
        endlessTiles = FindObjectOfType<EndlessTiles>();
        if (endlessTiles == null)
        {
            Debug.LogError("EndlessTiles script bulunamadý!");
        }
    }

    private void Update()
    {
        if (endlessTiles != null)
        {
            water = GetClosestTile();
        }
    }

    private void FixedUpdate()
    {
        if (water == null || rb == null)
            return;

        // Su yüzeyindeki pozisyonu hesapla
        Vector3 localPos = water.transform.InverseTransformPoint(transform.position);
        localPos.y = 0;
        localPos = water.GetLocalVertexPosition(localPos, applyRipple);
        Vector3 worldPos = water.transform.TransformPoint(localPos);

        // Kaldýrma kuvvetini hesapla (zýplama durumuna göre ayarlanmýþ)
        float displacement = worldPos.y - transform.position.y;
        float adjustedBuoyancyStrength = isJumping ? buoyancyStrength * 0.2f : buoyancyStrength; // Kuvveti azalt
        Vector3 buoyancyForce = new Vector3(0, adjustedBuoyancyStrength * displacement, 0);

        // Sürtünme kuvvetini hesapla
        Vector3 dragForce = -rb.velocity * damping;

        // Y ekseni için sönümleme kuvvetini hesapla
        Vector3 verticalDrag = new Vector3(0, -rb.velocity.y * verticalDamping, 0);

        // Kuvvetleri uygula
        rb.AddForce(buoyancyForce, ForceMode.Force); // Kaldýrma kuvveti
        rb.AddForce(dragForce, ForceMode.Force);    // Genel sürtünme kuvveti
        rb.AddForce(verticalDrag, ForceMode.Force); // Y ekseni için sönümleme
    }

    #endregion

    #region Private Methods

    void DetectJump()
    {
        isJumping = true;
        if (buoyancyCoroutine != null)
        {
            StopCoroutine(buoyancyCoroutine);
        }
        // Yeni Coroutine baþlat ve referansýný sakla
        buoyancyCoroutine = StartCoroutine(EnableBuoyancyAfterJump());
        
    }

    private IEnumerator EnableBuoyancyAfterJump()
    {
        yield return new WaitForSeconds(1f); // Zýplamanýn tamamlanmasý için süre
        isJumping = false;
    }

    /// <summary>
    /// Gets the closest tile to the attached object
    /// </summary>
    /// <returns>the closest tile</returns>
    private PWater GetClosestTile()
    {
        float closestDistance = float.MaxValue;
        PWater closestTile = null;

        foreach (GameObject tile in endlessTiles.GetSpawnedTiles())
        {
            float distance = Vector3.Distance(transform.position, tile.transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestTile = tile.GetComponent<PWater>();
            }
        }

        return closestTile;
    }

    #endregion
}
