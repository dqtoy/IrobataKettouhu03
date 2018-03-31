using UnityEngine;

using TMPro;

using CCGKit;

/// <summary>
/// �\�z���̃f�b�L�̕\����\������N���X
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

    public void OnDeleteButtonPressed()
    {
        deckButton.deck.RemoveCards(card);
        deckButton.UpdateDeckInfo();
        Destroy(gameObject);
    }
}
