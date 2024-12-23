using UnityEngine;

public class Card : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private Sprite cardBack;
    private Sprite cardFront;

    public Sprite Sprite { get { return cardFront; } }

    private bool isMatched = false;

    public void Initialize(Sprite frontSprite, System.Action<Card> onClickAction)
    {
        cardFront = frontSprite;
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (spriteRenderer == null)
        {
            Debug.LogError("Card is missing SpriteRenderer component.");
            return;
        }

        cardBack = spriteRenderer.sprite; // Assume the initial sprite is the card back

        // Attach the onClick action to the OnMouseDown event
        gameObject.AddComponent<CardClickHandler>().Initialize(this, onClickAction);
    }

    public void Reveal()
    {
        spriteRenderer.sprite = cardFront;
    }

    public void Hide()
    {
        spriteRenderer.sprite = cardBack;
    }

    public void SetMatched()
    {
        isMatched = true;
    }
}
