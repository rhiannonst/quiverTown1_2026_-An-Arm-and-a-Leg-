using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TileRewardManager : MonoBehaviour
{
    public Board board;
    public string overlaySceneName = "PickTileOverlay";
    public GameObject[] rewardTilePrefabs;
    public Relic[] rewardRelics;
    [Min(1)]
    public int choiceCount = 3;
    [Min(1)]
    public int fullRewardPoolStartsAtStage = 3;

    // DEBUG ONLY: temporary Play Mode hotkey for testing tile rewards.
    [Header("Debug")]
    public bool enableDebugHotkey = false;
    public KeyCode debugRewardKey = KeyCode.T;

    private bool isOfferingReward;
    private Action onRewardComplete;
    private Relic pendingBonusRelic;
    private int currentDefeatedEnemyNumber = 1;

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
        OfferTileReward(null, currentDefeatedEnemyNumber);
    }

    public bool OfferTileReward(Action rewardCompleteCallback)
    {
        return OfferTileReward(rewardCompleteCallback, currentDefeatedEnemyNumber);
    }

    public bool OfferTileReward(Action rewardCompleteCallback, int defeatedEnemyNumber)
    {
        if (isOfferingReward) return false;

        if (board == null)
        {
            UnityEngine.Debug.LogWarning("Tile reward cannot open because no Board is assigned.", this);
            return false;
        }

        GameObject[] choices = ChooseRewardTiles();
        if (choices.Length == 0)
        {
            UnityEngine.Debug.LogWarning("Tile reward cannot open because no reward tile prefabs are assigned.", this);
            return false;
        }

        currentDefeatedEnemyNumber = Mathf.Max(1, defeatedEnemyNumber);
        onRewardComplete = rewardCompleteCallback;
        pendingBonusRelic = ChooseBonusRelic();
        StartCoroutine(OfferTileRewardCo(choices));
        return true;
    }

    private IEnumerator OfferTileRewardCo(GameObject[] choices)
    {
        isOfferingReward = true;
        board.ClearTilesForReward();

        TileRewardSession.Begin(choices, pendingBonusRelic, OnTilePicked);

        AsyncOperation loadOperation = SceneManager.LoadSceneAsync(overlaySceneName, LoadSceneMode.Additive);
        if (loadOperation == null)
        {
            UnityEngine.Debug.LogWarning($"Tile reward overlay scene '{overlaySceneName}' could not be loaded.", this);
            TileRewardSession.Clear();
            pendingBonusRelic = null;
            board.RedrawFromCurrentDeckAvoidingMatches();
            isOfferingReward = false;
            CompleteReward();
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

        if (pendingBonusRelic != null && board != null && board.player != null)
        {
            AddRelicToPlayer(board.player, pendingBonusRelic);
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
        pendingBonusRelic = null;
        isOfferingReward = false;
        CompleteReward();
    }

    private void CompleteReward()
    {
        Action callback = onRewardComplete;
        onRewardComplete = null;
        callback?.Invoke();
    }

    private GameObject[] ChooseRewardTiles()
    {
        List<GameObject> availableTiles = new List<GameObject>();
        if (rewardTilePrefabs != null)
        {
            int rewardPoolSize = GetCurrentRewardPoolSize();
            for (int i = 0; i < rewardPoolSize; i++)
            {
                GameObject rewardTilePrefab = rewardTilePrefabs[i];
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
            int index = UnityEngine.Random.Range(0, availableTiles.Count);
            choices.Add(availableTiles[index]);
            availableTiles.RemoveAt(index);
        }

        return choices.ToArray();
    }

    private Relic ChooseBonusRelic()
    {
        if (board == null || board.player == null)
        {
            return null;
        }

        if (currentDefeatedEnemyNumber != 1 && currentDefeatedEnemyNumber % 3 != 0)
        {
            return null;
        }

        return GetRandomAvailableRelic(board.player);
    }

    private Relic GetRandomAvailableRelic(Player player)
    {
        if (player == null || rewardRelics == null || rewardRelics.Length == 0)
        {
            return null;
        }

        if (player.RelicList == null)
        {
            player.RelicList = new List<Relic>();
        }

        List<Relic> availableRelics = new List<Relic>();
        foreach (Relic relic in rewardRelics)
        {
            if (relic != null && !player.RelicList.Contains(relic))
            {
                availableRelics.Add(relic);
            }
        }

        if (availableRelics.Count == 0)
        {
            return null;
        }

        return availableRelics[UnityEngine.Random.Range(0, availableRelics.Count)];
    }

    private void AddRelicToPlayer(Player player, Relic relic)
    {
        if (player.RelicList == null)
        {
            player.RelicList = new List<Relic>();
        }

        if (player.RelicList.Contains(relic))
        {
            return;
        }

        player.RelicList.Add(relic);
        UnityEngine.Debug.Log($"[TileRewardManager] Added relic: {relic.Name}");
    }

    private int GetCurrentRewardPoolSize()
    {
        if (rewardTilePrefabs == null) return 0;

        if (currentDefeatedEnemyNumber < fullRewardPoolStartsAtStage)
        {
            return Mathf.Max(1, rewardTilePrefabs.Length / 2);
        }

        return rewardTilePrefabs.Length;
    }
}
