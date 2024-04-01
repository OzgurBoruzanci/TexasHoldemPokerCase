using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public void DealCardsToPlayersPressed()
    {
        GameManager.Instance.DealCardsToPlayers();
    }
    public void DealCardsToBoardPressed()
    {
        GameManager.Instance.DealCardsToBoard();
    }
    public void UpdateHandComparisonUI(int playerNumber)
    {
        GameManager.Instance.GetHandStrength(playerNumber);
    }
}
