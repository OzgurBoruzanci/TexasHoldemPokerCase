using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public void StartGame()
    {
        GameManager.Instance.DealCardsToBoard();
    }
    public void PlayerInputPressed(int enumNum)
    {
        GameManager.Instance.PlayerInput(enumNum);
    }
}
