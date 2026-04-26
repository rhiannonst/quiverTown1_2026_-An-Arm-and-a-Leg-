using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TileRewardManager : MonoBehaviour
{
    public Board board;
    public string overlaySceneName = "PickTileOverlay";
    public GameObject[] rewardTilePrefabs;
    [Min(1)]
    public int choiceCount = 3;

    // DEBUG ONLY: temporary Play Mode hotkey for testing tile rewards.
    [Header("Debug")]
    public bool enableDebugHotkey = true;
    public KeyCode debugRewardKey = KeyCode.T;

    private bool isOfferingReward;

    void Awake()
    {
        if (board == null)
        {
            board = FindAnyObjectByType<Board>();
        }
    }

    void Update()
    {
        if (enableDebugHotkey && Input.GetKeyDown(debugRewardKey))
        {
            OfferTileReward();
        }
    }

    public void OfferTileReward()
    {
        if (isOfferingReward) return;

        if (board == null)
        {
            UnityEngine.Debug.LogWarning("Tile reward cannot open because no Board is assigned.", this);
            return;
        }

        GameObject[] choices = ChooseRewardTiles();
        if (choices.Length == 0)
        {
            UnityEngine.Debug.LogWarning("Tile reward cannot open because no reward tile prefabs are assigned.", this);
            return;
        }

        StartCoroutine(OfferTileRewardCo(choices));
    }

    private IEnumerator OfferTileRewardCo(GameObject[] choices)
    {
        isOfferingReward = true;
        board.ClearTilesForReward();

        TileRewardSession.Begin(choices, OnTilePicked);

        AsyncOperation loadOperation = SceneManager.LoadSceneAsync(overlaySceneName, LoadSceneMode.Additive);
        if (loadOperation == null)
        {
            UnityEngine.Debug.LogWarning($"Tile reward overlay scene '{overlaySceneName}' could not be loaded.", this);
            TileRewardSession.Clear();
            board.RedrawFromCurrentDeckAvoidingMatches();
            isOfferingReward = false;
            yield break;
        }

        while (loadOperation != null && !loadOperation.isDone)
        {
            yield return null;
        }
    }

    private void OnTilePicked(GameObject selectedTilePrefab)
    {
        if (selectedTilePrefab != null)
        {
            board.AddTilesToBag(selectedTilePrefab, 1);
        }

        StartCoroutine(CloseRewardCo());
    }

    private IEnumerator CloseRewardCo()
    {
        AsyncOperation unloadOperation = SceneManager.UnloadSceneAsync(overlaySceneName);
        while (unloadOperation != null && !unloadOperation.isDone)
        {
            yield return null;
        }

        board.RedrawFromCurrentDeckAvoidingMatches();
        TileRewardSession.Clear();
        isOfferingReward = false;
    }

    private GameObject[] ChooseRewardTiles()
    {
        List<GameObject> availableTiles = new List<GameObject>();
        if (rewardTilePrefabs != null)
        {
            foreach (GameObject rewardTilePrefab in rewardTilePrefabs)
            {
                if (rewardTilePrefab != null && !availableTiles.Contains(rewardTilePrefab))
                {
                    availableTiles.Add(rewardTilePrefab);
                }
            }
        }

        List<GameObject> choices = new List<GameObject>();
        int numberOfChoices = Mathf.Min(choiceCount, availableTiles.Count);

        for (int i = 0; i < numberOfChoices; i++)
        {
            int index = Random.Range(0, availableTiles.Count);
            choices.Add(availableTiles[index]);
            availableTiles.RemoveAt(index);
        }

        return choices.ToArray();
    }
}
