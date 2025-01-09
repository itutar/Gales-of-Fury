using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndlessTiles : MonoBehaviour
{
    #region Fields

    public GameObject tilePrefab;      // 400x400 boyutundaki tile prefab'i
    public int numberOfTiles = 4;      // Sahnede ayn� anda duracak tile say�s�
    public float tileSize = 400f;      // Her bir tile'�n uzunlu�u (z y�n�nde)
    public Transform player;           // Yerinde sabit duran player

    // ��eride spawn edilen tile objelerini tutaca��m�z liste
    private List<GameObject> spawnedTiles = new List<GameObject>();

    // S�radaki tile��n spawn edilece�i z koordinat�
    private float nextSpawnZ = 0f;

    #endregion

    #region Unity Methods

    private void Start()
    {
        // Ba�lang��ta 4 tile�� art arda dizelim.
        for (int i = 0; i < numberOfTiles; i++)
        {
            SpawnTile(nextSpawnZ);
            nextSpawnZ += tileSize;
        }
    }

    private void Update()
    {
        // �lk (en eski) tile player��n 400 birim gerisine d��t�yse
        // onu en �ne tekrar yerle�tir (yok etmek yerine tekrar kullanmak)
        if (spawnedTiles.Count > 0)
        {
            GameObject firstTile = spawnedTiles[0];

            // Bu tile'�n merkezi z konumu
            float tileZ = firstTile.transform.position.z;

            // Player��n gerisine ne kadar d��m��, kontrol ediyoruz.
            // �rne�in tile��n +200 pozisyonu: Orta noktas�. 
            // E�er bu hesaplamada tile, player��n z konumundan 400 birim gerideyse
            // (daha basit bir yakla��m i�in direkt tileZ < player.position.z - tileSize �eklinde de d���nebilirsiniz)
            if (tileZ + tileSize < player.position.z)
            {
                // Bu tile�� listeden ��kar
                spawnedTiles.RemoveAt(0);

                // Tile�� en �ne ta��
                float newZ = spawnedTiles[spawnedTiles.Count - 1].transform.position.z + tileSize;
                firstTile.transform.position = new Vector3(
                    firstTile.transform.position.x,
                    firstTile.transform.position.y,
                    newZ
                );

                // Listeye tekrar ekle (en yeni tile gibi)
                spawnedTiles.Add(firstTile);
            }
        }
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Verilen z konumunda yeni bir tile olu�turur.
    /// </summary>
    /// <param name="zPos">Spawn edilecek tile��n z konumu</param>
    private void SpawnTile(float zPos)
    {
        // X=0, Z=zPos�ta (Y=0 da olabilir veya tilePrefab�e g�re ayarlayabilirsiniz)
        Vector3 spawnPosition = new Vector3(0f, 5f, zPos);

        GameObject newTile = Instantiate(tilePrefab, spawnPosition, Quaternion.identity);
        spawnedTiles.Add(newTile);
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// A metod to get the list of endlessSeaTiles
    /// </summary>
    /// <returns>the list of endlessSeaTiles</returns>
    public List<GameObject> GetSpawnedTiles()
    {
        return spawnedTiles;
    }

    #endregion

}
