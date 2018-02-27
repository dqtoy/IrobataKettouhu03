using System.Collections;

using UnityEngine;

using DG.Tweening;

public class PopupTurnStart : Popup
{
    private void Start()
    {
        gameObject.transform.DOScale(1.0f, 0.4f).SetEase(Ease.InOutBack);
        StartCoroutine(AutoClose());
    }

    private IEnumerator AutoClose()
    {
        yield return new WaitForSeconds(2.0f);
        var sequence = DOTween.Sequence();
        sequence.Append(gameObject.transform.DOScale(0.0f, 0.2f).SetEase(Ease.OutCubic));
        sequence.OnComplete(() => { Close(); });
    }
}