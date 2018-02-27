using UnityEngine;

using CCGKit;

public class DeckBuilderCard : MonoBehaviour
{
    public DeckBuilderScene scene;
//    public KoumaDeckBuilderScene kScene;
    public Card card;

    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && scene.currentPopup == null)
        {
            var mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            var hit = Physics2D.Raycast(mousePos, Vector2.zero);
            if (hit.collider != null && hit.collider.gameObject == gameObject)
            {
                scene.AddCardToDeck(card);
            }
        }
    }
}
