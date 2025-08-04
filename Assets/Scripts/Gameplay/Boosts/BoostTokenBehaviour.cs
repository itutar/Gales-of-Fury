using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoostTokenBehaviour : MonoBehaviour
{
    public enum BoostTokenType { TwoXBoost, DolphinBoost, DoubleJump }

    private float moveSpeed = 5f;

    [Header("Token Ayarı")]
    [SerializeField]
    private BoostTokenType tokenType;

    [Header("Su Seviyesi")]
    [SerializeField] private FindWaterSurfaceLevel waterSurfaceLevelFinder;

    [Header("Player Referansı")]
    [SerializeField] private PlayerReference playerReference;

    private void Start()
    {
        // if waterSurfaceLevelFinder is not assigned, try to get it from the GameObject
        if (waterSurfaceLevelFinder == null)
            waterSurfaceLevelFinder = GetComponent<FindWaterSurfaceLevel>();

    }

    private void Update()
    {
        // –Z yönünde hareket (SpeedMultiplier ile çarpılır)
        float speedMultiplier = Blackboard.Instance.GetValue<float>(BlackboardKey.SpeedMultiplier);
        transform.Translate(Vector3.back * moveSpeed * speedMultiplier * Time.deltaTime);

        // Su yüzeyini takip et
        if (waterSurfaceLevelFinder != null)
        {
            Vector3 pos = transform.position;
            pos.y = waterSurfaceLevelFinder.GetWaterSurfaceY();
            transform.position = pos;
        }

        if (playerReference != null)
        {
            if (transform.position.z < playerReference.player.transform.position.z)
            {
                Invoke(nameof(DestroyToken), 5f);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        switch (tokenType)
        {
            case BoostTokenType.TwoXBoost:
                Debug.Log("Aktif multiplier: " + Blackboard.Instance.GetValue<float>(BlackboardKey.SpeedMultiplier));
                GetComponent<TwoXBoost>()?.Activate();
                Debug.Log("Token sonrası multiplier: " + Blackboard.Instance.GetValue<float>(BlackboardKey.SpeedMultiplier));
                break;

            case BoostTokenType.DolphinBoost:
                GetComponent<DolphinBoost>()?.Activate();
                break;

            case BoostTokenType.DoubleJump:
                GetComponent<DoubleJumpBoost>()?.Activate();
                break;
        }

        Destroy(gameObject);
    }
    private void DestroyToken()
    {
        if (this != null) 
            Destroy(gameObject);
    }

}
