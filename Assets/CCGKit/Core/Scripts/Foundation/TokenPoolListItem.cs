using UnityEngine;

using TMPro;

using CCGKit;

/// <summary>
/// 構築中のデッキの表示を表現するクラス
/// </summary>
public class TokenPoolListItem : MonoBehaviour
{
    public DeckButton deckButton;
    public Token card;
    public TextMeshProUGUI cardNameText;
    public TextMeshProUGUI cardCostText;

    public int count = 1;

    public void AddCard()
    {
        ++count;
    }

    public void RemoveCard()
    {
        --count;
    }


    public void OnDeleteButtonPressed()
    {
        deckButton.deck.RemoveCards(card);
        deckButton.UpdateDeckInfo();
        Destroy(gameObject);
    }
}
