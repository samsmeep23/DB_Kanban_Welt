using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardClickHandler : MonoBehaviour
{
    private Card card;
    private System.Action<Card> onClickAction;

    public void Initialize(Card card, System.Action<Card> onClickAction)
    {
        this.card = card;
        this.onClickAction = onClickAction;
    }

    private void OnMouseDown()
    {
        if (card != null && onClickAction != null)
        {
            onClickAction(card);
        }
    }
}

