using System.Collections;
using DG.Tweening;
using UnityEngine;

public class LoadingScreneUI : MonoBehaviour
{
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private float animationDuration = 1;
    private void Awake()
    {
        StartCoroutine(Fade());
    }

    private IEnumerator Fade()
    {
        yield return new WaitForSeconds(1);
        transform.DOScale(0, animationDuration).SetEase(Ease.InQuad);
        canvasGroup.DOFade(0, animationDuration).OnComplete(() =>
        {
            gameObject.SetActive(false);
        });
    }
}
