using UnityEngine;

/// <summary>
/// 手札の描画
/// </summary>

//RequireComponentでCardViewが持ってるコンポーネントをHandCardクラスの依存関係に加えてる
[RequireComponent(typeof(CardView))]
public class HandCard : MonoBehaviour
{
    public DemoHumanPlayer ownerPlayer;
    public GameObject boardZone;

    protected CardView cardView;

    protected bool startedDrag;
    protected Vector3 initialPos;

    //RequireComponent使ったらAwakeで初期化が必要
    private void Awake()
    {
        cardView = GetComponent<CardView>();
    }

    private void Start()
    {
        if (cardView.CanBePlayed(ownerPlayer))
        {
            cardView.SetHighlightingEnabled(true);
        }
        else
        {
            cardView.SetHighlightingEnabled(false);
        }
    }

    private void Update()
    {
        if (startedDrag)
        {
            transform.position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            var newPos = transform.position;
            newPos.z = 0;
            transform.position = newPos;
        }
    }

    public void OnSelected()
    {
        if (ownerPlayer.isActivePlayer &&
            cardView.CanBePlayed(ownerPlayer))
        {
            startedDrag = true;
            initialPos = transform.position;
            ownerPlayer.isCardSelected = true;
        }
    }

    public void OnMouseUp()
    {
        if (!startedDrag)
        {
            return;
        }

        startedDrag = false;
        ownerPlayer.isCardSelected = false;

        //positionが四角の中に含まれていれば
        if (boardZone.GetComponent<BoxCollider2D>().bounds.Contains(transform.position))
        {

            ownerPlayer.PlayCard(cardView);
            cardView.SetHighlightingEnabled(false);
        }
        else
        {
            transform.position = initialPos;
        }
    }

    public void ResetToInitialPosition()
    {
        transform.position = initialPos;
    }
}
