using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MemoryGame : MonoBehaviour
{
    [SerializeField] private GameObject cardPrefab;

    [SerializeField] private Transform cardParent;

    [SerializeField] private Sprite[] arbeitstandSprites; // Expected to contain 3 sprites

    [SerializeField] private Text matchedCountText;
    [SerializeField] private Text turnCountText;

    [SerializeField] private int rows = 4;
    [SerializeField] private int columns = 6;

    [SerializeField] private float spacingX = 1.0f; // Horizontal spacing
    [SerializeField] private float spacingZ = 1.0f; // Vertical spacing

    [SerializeField] private TaskController taskController; // Reference to TaskController

    [SerializeField] private GameObject gameWinUI; // Reference to the Game Win UI
    [SerializeField] private GameObject gameLostUI; // Reference to the Game Lost UI

    private List<Card> cards = new List<Card>();

    private Card firstCard, secondCard;
    
    private int matchedPairs = 0;
    private int turnCount = 0;

    private bool isGameActive = false; // Tracks if the game has started

    void Start()
    {
        InitializeCards();
    }

    private void InitializeCards()
    {
        // Ensure we have exactly 3 sprites and required references.
        if (arbeitstandSprites.Length < 3 || cardPrefab == null || cardParent == null)
        {
            Debug.LogError("Setup incomplete. Ensure you have assigned cardPrefab, cardParent, and exactly 3 sprites.");
            return;
        }

        matchedPairs = 0;
        turnCount = 0;
        UpdateMatchedText();
        UpdateTurnText();

        List<Sprite> spritePool = new List<Sprite>();

        // Add cards with specific counts: 8 for arbeitstandSprites[0], 10 for arbeitstandSprites[1], 6 for arbeitstandSprites[2]
        for (int i = 0; i < 8; i++) spritePool.Add(arbeitstandSprites[0]);
        for (int i = 0; i < 10; i++) spritePool.Add(arbeitstandSprites[1]);
        for (int i = 0; i < 6; i++) spritePool.Add(arbeitstandSprites[2]);

        // Shuffle the spritePool
        for (int i = 0; i < spritePool.Count; i++)
        {
            Sprite temp = spritePool[i];
            int randomIndex = Random.Range(0, spritePool.Count);
            spritePool[i] = spritePool[randomIndex];
            spritePool[randomIndex] = temp;
        }

        // Create cards in a grid
        int cardIndex = 0;
        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < columns; col++)
            {
                Vector3 position = new Vector3(col * spacingX, 0, row * spacingZ);
                GameObject cardObject = Instantiate(cardPrefab, cardParent);
                cardObject.transform.localPosition = position;
                Card card = cardObject.GetComponent<Card>();

                if (card == null)
                {
                    Debug.LogError("Card prefab is missing the Card component.");
                    return;
                }

                card.Initialize(spritePool[cardIndex], OnCardClicked);
                cards.Add(card);
                cardIndex++;
            }
        }
    }

    private void OnCardClicked(Card clickedCard)
    {
        if (!isGameActive)
        {
            isGameActive = true; // Game officially starts on the first card click
        }

        if (firstCard == null)
        {
            firstCard = clickedCard;
            firstCard.Reveal();
        }
        else if (secondCard == null && clickedCard != firstCard)
        {
            secondCard = clickedCard;
            secondCard.Reveal();
            turnCount++;
            UpdateTurnText();
            StartCoroutine(CheckMatch());
        }
    }

    private IEnumerator CheckMatch()
    {
        yield return new WaitForSeconds(1f);

        int arbeitstandType = DetermineArbeitstandType(firstCard.Sprite);

        // Ensure valid match logic based on preceding columns
        bool isValidMatch = false;

        if (arbeitstandType == 1)
        {
            // Always allow matches for Arbeitstand 1
            isValidMatch = true;
        }
        else
        {
            // Check preceding column for tasks
            isValidMatch = taskController.PrecedingColumnHasTasks(arbeitstandType);
        }

        if (firstCard.Sprite == secondCard.Sprite && isValidMatch)
        {
            matchedPairs++;
            UpdateMatchedText();

            taskController.OnCardsMatched(arbeitstandType);

            firstCard.SetMatched();
            secondCard.SetMatched();
        }
        else
        {
            // Flip back cards if no tasks in preceding columns or invalid match
            firstCard.Hide();
            secondCard.Hide();
        }

        firstCard = null;
        secondCard = null;
    }


    /*private IEnumerator CheckMatch()
    {
        yield return new WaitForSeconds(1f);

        if (firstCard.Sprite == secondCard.Sprite)
        {
            matchedPairs++;
            UpdateMatchedText();
            firstCard.SetMatched();
            secondCard.SetMatched();
        }
        else
        {
            firstCard.Hide();
            secondCard.Hide();
        }

        firstCard = null;
        secondCard = null;
    }*/

    private int DetermineArbeitstandType(Sprite matchedSprite)
    {
        if (matchedSprite == arbeitstandSprites[0]) return 1; // Arbeitstand 1
        if (matchedSprite == arbeitstandSprites[1]) return 2; // Arbeitstand 2
        if (matchedSprite == arbeitstandSprites[2]) return 3; // Arbeitstand 3
        return 0; // Default, should never happen
    }


    private void UpdateMatchedText()
    {
        matchedCountText.text = "Matched: " + matchedPairs;

        // Only check for Game Lost if the game has started
        if (isGameActive && matchedPairs == cards.Count / 2)
        {
            if (taskController.Arbeitstand3TaskCount < 3)
            {
                OnGameLost(); // Show Game Lost UI
            }
        }
    }

    private void UpdateTurnText()
    {
        turnCountText.text = "Turns: " + turnCount;
    }

    public void OnGameWon()
    {
        gameWinUI.SetActive(true);
    }

    public void OnGameLost()
    {
        gameLostUI.SetActive(true);
    }

    public void RestartGame()
    {
        // Reset the game state
        taskController.ResetToInitialTasks();

        foreach (Card card in cards)
        {
            card.Hide();
        }

        matchedPairs = 0;
        turnCount = 0;
        isGameActive = false; // Reset game active state
        UpdateMatchedText();
        UpdateTurnText();

        gameWinUI.SetActive(false);
        gameLostUI.SetActive(false);
    }
}
