using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR;

public enum GamePhase
{
    Bet,
    Fold,
    Call
}
public class GameManager : MonoBehaviour
{
    private static GameManager _instance;
    public static GameManager Instance { get { return _instance; } }

    [SerializeField] private List<Card_SO> deck;
    [SerializeField] private GameObject cardPrefab;
    [SerializeField] private List<Card_SO> listofCards;
    [SerializeField] private int offset_card = 50;
    #region placeholders variables
    [SerializeField] private GameObject placeholderBoard;
    [SerializeField] private GameObject placeholderHand;
    [SerializeField] private GameObject[] placeholderPlayerHands;
    #endregion
    [SerializeField] private List<Card> board_cards;
    [SerializeField] private List<Card>[] player_cards = new List<Card>[4];
    private GameObject cardTempObj;
    private GamePhase gamePhase;
    private bool gameEnded;
    [SerializeField] private int money = 100;
    private int bet;
    private int num_hand = 1;
    private int num_cards_board = 0;
    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
    }
    void Start()
    {
        for (int i = 0; i < player_cards.Length; i++)
        {
            player_cards[i] = new List<Card>();
        }
        deck = new List<Card_SO>(listofCards);
    }
    public void DealCardsToBoard()
    {
        deck.Shuffle();
        if (num_cards_board < 3)
        {
            for (int i = 0; i < 3; i++)
            {
                var cardObj = InstantiateCard(deck[i], placeholderBoard.transform.position, placeholderHand.transform, i);
                //num_hand++;
                board_cards.Add(cardObj.GetComponent<Card>());
                deck.Remove(deck[i]);
                num_cards_board++;
            }
            DealCardsToPlayers();
        }
        else if (num_cards_board < 5)
        {
            var cardObj = InstantiateCard(deck[0], placeholderBoard.transform.position, placeholderHand.transform, num_cards_board++);
            board_cards.Add(cardObj.GetComponent<Card>());
            deck.Remove(deck[0]);
            if (num_cards_board== 5)
            {
                CheckWinner();
                bet = 0;
            }
        }
        else
        {
            ResetGame();
        }
    }
    private void DealCardsToPlayers()
    {
        if (player_cards[0].Count == 0)
        {
            deck.Shuffle();
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 2; j++)
                {
                    var cardObj = InstantiateCard(deck[j], placeholderPlayerHands[i].transform.position, placeholderPlayerHands[i].transform, j);

                    player_cards[i].Add(cardObj.GetComponent<Card>());
                    deck.Remove(deck[j]);
                }
            }
        }
    }
    private void GetHandStrength(int player_num)
    {
        List<Card> handToCompare = new List<Card>();
        for (int i = 0; i < board_cards.Count; i++)
        {
            handToCompare.Add(board_cards[i]);
        }
        for (int i = 0; i < player_cards[player_num].Count; i++)
        {
            handToCompare.Add(player_cards[player_num][i]);
        }
        PokerHand pokerHand = new PokerHand();
        pokerHand.SetPokerHand(handToCompare.ToArray());
    }
    private GameObject InstantiateCard(Card_SO sO, Vector3 pos, Transform parent, int num_card)
    {
        cardTempObj = Instantiate(cardPrefab, new Vector3(pos.x + (offset_card * num_card), pos.y - (2 * offset_card), 0), Quaternion.identity, parent);
        cardTempObj.GetComponent<Card>().cardValue = sO.cardValue;
        cardTempObj.GetComponent<Card>().cardColor = sO.cardColor;
        cardTempObj.name = sO.cardValue.ToString() + sO.cardColor.ToString();
        cardTempObj.transform.GetChild(1).GetComponent<Image>().sprite = sO.cardSprite;
        return cardTempObj;
    }
    private void ResetGame()
    {
        gameEnded= false;
        num_hand = 1;
        num_cards_board = 0;
        foreach (var card in board_cards)
        {
            Destroy(card.gameObject);
        }
        board_cards.Clear();
        for (int i = 0; i < player_cards.Length; i++)
        {
            foreach (var card in player_cards[i])
            {
                Destroy(card.gameObject);
            }
            player_cards[i].Clear();
        }
        for (int i = 0; i < placeholderPlayerHands.Length; i++)
        {
            placeholderPlayerHands[i].SetActive(true);
        }
        deck = new List<Card_SO>(listofCards);
        Debug.Log("****  ****");
        Debug.Log("****  ****");
        Debug.Log("****  New Round  ****");
        DealCardsToBoard();
    }
    private void CheckWinner()
    {
        gameEnded = true;
        List<int> winningPlayers = new List<int>();
        int highestStrength = -1;

        for (int i = 0; i < placeholderPlayerHands.Length; i++)
        {
            // Fold yapm�� oyuncular� atla
            if (!placeholderPlayerHands[i].activeSelf) continue;

            List<Card> handToCompare = new List<Card>();
            handToCompare.AddRange(board_cards);
            handToCompare.AddRange(player_cards[i]);

            PokerHand pokerHand = new PokerHand();
            pokerHand.SetPokerHand(handToCompare.ToArray());

            if (pokerHand.Strength > highestStrength)
            {
                winningPlayers.Clear();
                winningPlayers.Add(i);
                highestStrength = pokerHand.Strength;
            }
            else if (pokerHand.Strength == highestStrength)
            {
                winningPlayers.Add(i);
            }
        }

        if (winningPlayers.Count > 0)
        {
            Debug.LogWarning("*****    Winning players    *****");
            foreach (int playerIndex in winningPlayers)
            {
                List<Card> winningHandCards = new List<Card>();
                winningHandCards.AddRange(board_cards);
                winningHandCards.AddRange(player_cards[playerIndex]);
                PokerHand winningHand = new PokerHand();
                winningHand.SetPokerHand(winningHandCards.ToArray());
                if (playerIndex == 0)
                {
                    money += bet * 4 * 2;
                }
                Debug.Log("Player " + (playerIndex + 1) + ", Hand: " + winningHand.PrintResult());
            }
        }
    }
    public void PlayerInput(int value)
    {
        switch (value)
        {
            case 0:
                gamePhase = GamePhase.Bet;
                bet += 10;
                money -= bet;
                BotInput();
                DealCardsToBoard();
                break;
            case 1:
                gamePhase = GamePhase.Fold;
                FoldPlayer(0);
                BotInput();
                DealCardsToBoard();
                
                break;
            case 2:
                gamePhase = GamePhase.Call;
                money -= bet;
                BotInput();
                DealCardsToBoard();
                break;
        }
    }
    private void FoldPlayer(int playerIndex)
    {
        foreach (var card in player_cards[playerIndex])
        {
            Destroy(card.gameObject);
        }
        player_cards[playerIndex].Clear();
        placeholderPlayerHands[playerIndex].SetActive(false);
        Debug.Log("Player " + (playerIndex + 1) + " folded.");
    }
    private bool CheckTheMoney()
    {
        return money > 0;
    }
    private void BotInput()
    {
        if (gamePhase != GamePhase.Fold)
        {
            for (int i = 1; i < placeholderPlayerHands.Length; i++)
            {
                float randomValue = Random.Range(0f, 1.5f);
                if (randomValue < 0.25f)
                {
                    FoldPlayer(i);
                }
                else if (randomValue > 0.25f && randomValue < 0.75f)
                {
                    placeholderPlayerHands[i].SetActive(true);
                    bet += 10;
                    Debug.Log("Player " + (i + 1) + " bet increased.");
                }
            }
        }
        else if (gamePhase == GamePhase.Fold && !gameEnded)
        {
            DealCardsToBoard();
        }
        else if (gameEnded)
        {
            ResetGame();
            Debug.Log("****  Game Ended  ****");
        }
    }
}
