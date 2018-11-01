using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//RequireComponentでCardViewが持ってるコンポーネントをMulliganCardListクラスの依存関係に加えてる
[RequireComponent(typeof(CardView))]
public class MulliganCardList : MonoBehaviour {


    [SerializeField]
    private GameObject creatureCardViewPrefab;

    [SerializeField]
    private GameObject spellCardViewPrefab;

    [SerializeField]
    private GameObject opponentCardPrefab;

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

    // Use this for initialization
    void Start () {
        if (cardView.CanBePlayed(ownerPlayer))
        {
            cardView.SetHighlightingEnabled(true);
        }
        else
        {
            cardView.SetHighlightingEnabled(false);
        }

    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
