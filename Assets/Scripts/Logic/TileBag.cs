using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class TileBag
{
    [Min(1)]
    public int startingCopiesPerTile = 5;

    private readonly List<GameObject> drawPile = new List<GameObject>();
    private readonly List<TileBagEntry> deck = new List<TileBagEntry>();

    public int Count => drawPile.Count;

    public void Initialize(GameObject[] tilePrefabs)
    {
        deck.Clear();
        if (tilePrefabs != null)
        {
            foreach (GameObject tilePrefab in tilePrefabs)
            {
                if (tilePrefab != null)
                {
                    AddToDeck(tilePrefab, startingCopiesPerTile);
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

    // Adds tiles to the bag
    public void AddTile(GameObject tilePrefab, int count = 1)
    {
        if (tilePrefab == null || count <= 0)
        {
            return;
        }

        AddToDeck(tilePrefab, count);

        for (int i = 0; i < count; i++)
        {
            drawPile.Add(tilePrefab);
        }
    }

    // Refills the bag when empty
    private void Refill()
    {
        drawPile.Clear();

        foreach (TileBagEntry entry in deck)
        {
            for (int i = 0; i < entry.count; i++)
            {
                drawPile.Add(entry.tilePrefab);
            }
        }
    }

    private void AddToDeck(GameObject tilePrefab, int count)
    {
        foreach (TileBagEntry entry in deck)
        {
            if (entry.tilePrefab == tilePrefab)
            {
                entry.count += count;
                return;
            }
        }

        deck.Add(new TileBagEntry(tilePrefab, count));
    }
}

public class TileBagEntry
{
    public GameObject tilePrefab;
    public int count;

    public TileBagEntry(GameObject tilePrefab, int count)
    {
        this.tilePrefab = tilePrefab;
        this.count = count;
    }
}
