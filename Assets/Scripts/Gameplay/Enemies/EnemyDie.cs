using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDie : MonoBehaviour
{
    #region Fields

    [SerializeField] EnemyType enemyType;

    #endregion

    #region Unity Methods

    private void OnEnable()
    {
        EnemyEventManager.Instance.OnEnemyDied.AddListener(OnDie);
    }

    private void OnDisable()
    {
        EnemyEventManager.Instance?.OnEnemyDied.RemoveListener(OnDie);
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Triggered when the enemy dies
    /// </summary>
    /// <param name="enemy">Enemy gameobject</param>
    private void OnDie(GameObject enemy)
    {
        if (enemy == this.gameObject)
        {
            switch (enemyType)
            {
                case EnemyType.Shark:
                    SharkDie();
                    break;
                case EnemyType.RegularPirate1:
                    PirateDie();
                    break;
                case EnemyType.Kraken:
                    KrakenDie();
                    break;
            }

            Destroy(gameObject);
        }
    }

    private void SharkDie()
    {
        // Shark için özel ölüm animasyonu veya efekt
        Debug.Log("Shark is dying!");
    }

    private void PirateDie()
    {
        // Pirate için özel ölüm animasyonu veya efekt
        Debug.Log("Pirate is dying!");
    }

    private void KrakenDie()
    {
        // Kraken için özel ölüm animasyonu veya efekt
        Debug.Log("Kraken is dying!");
    }


    #endregion
}
