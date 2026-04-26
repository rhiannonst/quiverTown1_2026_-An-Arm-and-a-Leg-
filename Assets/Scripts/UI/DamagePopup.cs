using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DamagePopup : MonoBehaviour
{
    public TMP_Text label;
    public float scaleIncrement = 0.15f;

    private int totalDamage;
    private int totalBlock;
    private int totalHeal;
    private Vector3 baseScale;

    void Awake()
    {
        baseScale = transform.localScale;
    }

    public void AddResults(List<Player.MatchResult> results)
    {
        foreach (Player.MatchResult match in results)
        {
            this.gameObject.SetActive(true);
            switch (match.tileType)
            {
                case TileType.Head:
                    totalDamage += match.count;
                    totalBlock += match.count;
                    break;
                case TileType.Arm:
                case TileType.Leg:
                    totalDamage += match.count;
                    break;
                case TileType.Torso:
                    totalBlock += match.count;
                    break;
                case TileType.Heart:
                    totalHeal += match.count;
                    break;
                case TileType.Spine:
                    break;
            }
        }
        transform.localScale += Vector3.one * scaleIncrement;
        Refresh();
    }

    public void Clear()
    {
        StopAllCoroutines();
        StartCoroutine(FadeOut());
    }

    private IEnumerator FadeOut()
    {
        float elapsed = 0f;
        float fadeDuration = 0.75f;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            if (label != null) label.alpha = 1f - (elapsed / fadeDuration);
            yield return null;
        }

        totalDamage = 0;
        totalBlock = 0;
        totalHeal = 0;
        transform.localScale = baseScale;
        if (label != null) label.alpha = 0f;
        this.gameObject.SetActive(false);
    }

    private void Refresh()
    {
        if (label == null) return;

        var parts = new List<string>();
        if (totalDamage > 0) parts.Add($"+{totalDamage} Damage");
        if (totalBlock > 0)  parts.Add($"+{totalBlock} Block");
        if (totalHeal > 0)   parts.Add($"+{totalHeal} Heal");

        label.text = string.Join("\n", parts);
        label.alpha = 1f;
    }
}
