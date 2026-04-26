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

    public void AddResult(TurnResult result)
    {
        AddTotals(result.TotalDamage, result.TotalBlock, result.TotalHeal);
    }

    private void AddTotals(float damage, float block, float heal)
    {
        gameObject.SetActive(true);
        totalDamage += Mathf.RoundToInt(damage);
        totalBlock += Mathf.RoundToInt(block);
        totalHeal += Mathf.RoundToInt(heal);
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
        float fadeDuration = 0.2f;

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
