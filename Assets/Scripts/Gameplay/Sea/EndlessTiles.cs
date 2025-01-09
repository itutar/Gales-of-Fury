using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndlessTiles : MonoBehaviour
{
    #region Fields

    public GameObject tilePrefab;      // 400x400 boyutundaki tile prefab'i
    public int numberOfTiles = 4;      // Sahnede ayný anda duracak tile sayýsý
    public float tileSize = 400f;      // Her bir tile'ýn uzunluðu (z yönünde)
    public Transform player;           // Yerinde sabit duran player

    // Ýçeride spawn edilen tile objelerini tutacaðýmýz liste
    private List<GameObject> spawnedTiles = new List<GameObject>();

    // Sýradaki tile’ýn spawn edileceði z koordinatý
    private float nextSpawnZ = 0f;

    #endregion

    #region Unity Methods

    private void Start()
    {
        // Baþlangýçta 4 tile’ý art arda dizelim.
        for (int i = 0; i < numberOfTiles; i++)
        {
            SpawnTile(nextSpawnZ);
            nextSpawnZ += tileSize;
        }
    }

    private void Update()
    {
        // Ýlk (en eski) tile player’ýn 400 birim gerisine düþtüyse
        // onu en öne tekrar yerleþtir (yok etmek yerine tekrar kullanmak)
        if (spawnedTiles.Count > 0)
        {
            GameObject firstTile = spawnedTiles[0];

            // Bu tile'ýn merkezi z konumu
            float tileZ = firstTile.transform.position.z;

            // Player’ýn gerisine ne kadar düþmüþ, kontrol ediyoruz.
            // Örneðin tile’ýn +200 pozisyonu: Orta noktasý. 
            // Eðer bu hesaplamada tile, player’ýn z konumundan 400 birim gerideyse
            // (daha basit bir yaklaþým için direkt tileZ < player.position.z - tileSize þeklinde de düþünebilirsiniz)
            if (tileZ + tileSize < player.position.z)
            {
                // Bu tile’ý listeden çýkar
                spawnedTiles.RemoveAt(0);

                // Tile’ý en öne taþý
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
    /// Verilen z konumunda yeni bir tile oluþturur.
    /// </summary>
    /// <param name="zPos">Spawn edilecek tile’ýn z konumu</param>
    private void SpawnTile(float zPos)
    {
        // X=0, Z=zPos’ta (Y=0 da olabilir veya tilePrefab’e göre ayarlayabilirsiniz)
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
