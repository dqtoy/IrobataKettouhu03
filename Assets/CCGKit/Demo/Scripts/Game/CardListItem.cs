using UnityEngine;

using TMPro;

using CCGKit;

/// <summary>
/// 構築中のデッキの表示を表現するクラス
/// </summary>
public class CardListItem : MonoBehaviour
{
    public DeckButton deckButton;
    public Card card;
    public TextMeshProUGUI cardNameText;
    public TextMeshProUGUI cardCostText;
    public TextMeshProUGUI cardAmountText;

    public int count = 1;

    public void AddCard()
    {
        ++count;
        cardAmountText.text = "x" + count;
    }

    public void RemoveCard()
    {
        --count;
        cardAmountText.text = "x" + count;
    }


    public void OnDeleteButtonPressed()
    {
        deckButton.deck.RemoveCards(card);
        deckButton.UpdateDeckInfo();
        Destroy(gameObject);
    }
}
