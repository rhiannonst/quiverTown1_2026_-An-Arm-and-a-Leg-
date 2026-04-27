using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TileRewardCard : MonoBehaviour
{
    public Image tileImage;
    public TMP_Text nameLabel;
    public TMP_Text descriptionLabel;
    public Button button;

    private GameObject tilePrefab;
    private Action<GameObject> onPicked;

    public void SetTile(GameObject newTilePrefab, Action<GameObject> newOnPicked)
    {
        tilePrefab = newTilePrefab;
        onPicked = newOnPicked;

        Tile tileData = GetTileData(tilePrefab);
        if (tileData == null)
        {
            gameObject.SetActive(false);
            return;
        }

        gameObject.SetActive(true);

        if (tileImage != null)
        {
            tileImage.sprite = tileData._Sprite;
            tileImage.enabled = tileData._Sprite != null;
        }

        if (nameLabel != null)
        {
            nameLabel.text = tileData.Name;
        }

        if (descriptionLabel != null)
        {
            descriptionLabel.text = BuildDescription(tileData);
        }

        if (button != null)
        {
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(Pick);
        }
    }

    private void Pick()
    {
        onPicked?.Invoke(tilePrefab);
    }

    private Tile GetTileData(GameObject prefab)
    {
        if (prefab == null) return null;

        TileInstance tileInstance = prefab.GetComponent<TileInstance>();
        return tileInstance != null ? tileInstance.tileData : null;
    }

    private string BuildDescription(Tile tileData)
    {
        List<string> parts = new List<string>();

        if (tileData.Damage > 0) parts.Add($"{tileData.Damage:g} Damage");
        if (tileData.Block > 0) parts.Add($"{tileData.Block:g} Block");
        if (tileData.Heal > 0) parts.Add($"{tileData.Heal:g} Heal");

        return parts.Count > 0 ? string.Join(", ", parts) : "No effect";
    }
}
