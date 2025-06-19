using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class MoveSharkToLane : MonoBehaviour, IMoveToLane
{
    private Rigidbody rb;
    private float targetX;
    public float forceStrength = 20f;
    public float stopThreshold = 0.1f;

    public float turnDuration = 0.8f;
    private bool isArrived;
    private float turnTimer;
    private Quaternion startRot, targetRot;

    // IMoveToLane
    public bool IsFinished { get; private set; }

    public void Initialize(float targetX)
    {
        this.targetX = targetX;
        IsFinished = false;
        isArrived = false;
        turnTimer = 0f;
        enabled = true;

        // ① Spawn pozisyonuna göre hangi yöne bakması gerektiğini belirle
        //    - Eğer spawn X < targetX ise, lane sağda → sağa bak
        //    - Aksi durumda, lane solda → sola bak
        Vector3 initialDir = (transform.position.x < targetX) ? Vector3.right : Vector3.left;

        // ② O yöne bakacak şekilde rotasyonu ayarla
        transform.rotation = Quaternion.LookRotation(initialDir, Vector3.up);
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        // Yalnız Y ekseninde dönmeye izin ver, diğerlerini fiziksel dondur:
        rb.constraints = RigidbodyConstraints.FreezeRotationX
                       | RigidbodyConstraints.FreezeRotationZ;
    }

    private void FixedUpdate()
    {
        if (!isArrived)
        {
            // 1) X eksenine kuvvetle yana taşı
            float dx = targetX - transform.position.x;
            if (Mathf.Abs(dx) > stopThreshold)
            {
                rb.AddForce(Vector3.right * Mathf.Sign(dx) * forceStrength, ForceMode.Force);
            }
            else
            {
                rb.velocity = new Vector3(0f, rb.velocity.y, rb.velocity.z);
                // lane’e varıldı, dönme zamanlayıcısını ayarla
                isArrived = true;
                startRot = transform.rotation;
                targetRot = Quaternion.LookRotation(Vector3.back, Vector3.up);
                turnTimer = 0f;
            }
        }
        else
        {
            // 2) Dönüş (smooth)
            turnTimer += Time.fixedDeltaTime;
            float t = Mathf.Clamp01(turnTimer / turnDuration);
            transform.rotation = Quaternion.Slerp(startRot, targetRot, t);

            if (t >= 1f)
            {
                // Dönüş bitti → swim attack script’i aktif olabilir
                IsFinished = true;
                enabled = false;
            }
        }
    }
}
