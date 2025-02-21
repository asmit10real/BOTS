using UnityEngine;
using TMPro; // For TextMeshPro
using System.Collections;

public class SectorCompleteUI : MonoBehaviour {
    public GameObject sectorClearBanner;
    public TextMeshProUGUI sectorClearText;

    void Start() {
        sectorClearBanner.SetActive(false);
    }

    public void ShowSectorClear() {
        sectorClearBanner.SetActive(true);
        //StartCoroutine(FadeOutText());
    }

    private IEnumerator FadeOutText() {
        float duration = 3f;
        float elapsedTime = 0f;
        Color startColor = sectorClearText.color;
        startColor.a = 1;

        while (elapsedTime < duration) {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Lerp(1, 0, elapsedTime / duration);
            sectorClearText.color = new Color(startColor.r, startColor.g, startColor.b, alpha);
            yield return null;
        }
    }
}
