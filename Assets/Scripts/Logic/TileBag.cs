using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class TileBag
{
    [Min(1)]
    public int startingCopiesPerTile = 5;

    private readonly List<GameObject> drawPile = new List<GameObject>();
    private readonly List<GameObject> baseTiles = new List<GameObject>();

    public int Count => drawPile.Count;

    public void Initialize(GameObject[] tilePrefabs)
    {
        baseTiles.Clear();
        if (tilePrefabs != null)
        {
            foreach (GameObject tilePrefab in tilePrefabs)
            {
                if (tilePrefab != null)
                {
                    baseTiles.Add(tilePrefab);
                }
            }
        }

        Refill();
    }

    public GameObject Draw()
    {
        if (drawPile.Count == 0)
        {
            Refill();
        }

        if (drawPile.Count == 0)
        {
            return null;
        }

        int index = UnityEngine.Random.Range(0, drawPile.Count);
        GameObject tilePrefab = drawPile[index];
        drawPile.RemoveAt(index);
        return tilePrefab;
    }

    public GameObject DrawAvoiding(Func<GameObject, bool> rejected, int maxAttempts = 100)
    {
        List<GameObject> rejectedTiles = new List<GameObject>();
        GameObject selectedTile = null;

        for (int attempts = 0; attempts < maxAttempts; attempts++)
        {
            GameObject tilePrefab = Draw();
            if (tilePrefab == null)
            {
                break;
            }

            if (rejected == null || !rejected(tilePrefab))
            {
                selectedTile = tilePrefab;
                break;
            }

            rejectedTiles.Add(tilePrefab);
        }

        drawPile.AddRange(rejectedTiles);
        return selectedTile ?? Draw();
    }

    public void AddTile(GameObject tilePrefab, int count = 1)
    {
        if (tilePrefab == null || count <= 0)
        {
            return;
        }

        if (!baseTiles.Contains(tilePrefab))
        {
            baseTiles.Add(tilePrefab);
        }

        for (int i = 0; i < count; i++)
        {
            drawPile.Add(tilePrefab);
        }
    }

    private void Refill()
    {
        drawPile.Clear();

        foreach (GameObject tilePrefab in baseTiles)
        {
            for (int i = 0; i < startingCopiesPerTile; i++)
            {
                drawPile.Add(tilePrefab);
            }
        }
    }
}
