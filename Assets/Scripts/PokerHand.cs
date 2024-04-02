using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class PokerHand 
{
    public int Strength { get; private set; }
    private bool RYL_FLUSH;
    private bool STRAIGHT_FLUSH;
    private bool FOUR_A_KIND;
    private bool FULL_HOUSE;
    private bool FLUSH;
    private bool STRAIGHT;
    private bool THREE_A_KIND;
    private bool TWO_PAIR;
    private bool PAIR;
    private bool HIGH_CARD;
    private List<Card> highCard;
    private int strength;

    public PokerHand()
    {
        this.RYL_FLUSH = false;
        this.STRAIGHT_FLUSH = false;
        this.FOUR_A_KIND = false;
        this.FULL_HOUSE = false;
        this.FLUSH = false;
        this.STRAIGHT = false;
        this.THREE_A_KIND = false;
        this.TWO_PAIR = false;
        this.PAIR = false;
        this.HIGH_CARD = false;
        this.highCard = new List<Card>();
        this.strength = 0;
    }
    public void SetPokerHand(Card[] cardArray)
    {
        List<Card> cards = new List<Card>();
        foreach (Card c in cardArray)
        {
            cards.Add(c);
        }
        if (IsRoyal(cards)) return;
        if (IsStraightFlush(cards)) return;
        if (IsFourAKind(cards)) return;
        if (IsFullHouse(cards)) return;
        if (IsFlush(cards)) return;
        if (IsStraight(cards)) return;
        if (IsThreeAKind(cards)) return;
        if (IsTwoPair(cards)) return;
        if (IsPair(cards)) return;
        IsHigh(cards);
    }
    public String PrintResult()
    {
        if (this.RYL_FLUSH)
        {
            return "Royal Flush" + "- Strength:" + this.strength;
        }
        else if (this.STRAIGHT_FLUSH)
        {
            return "Straight Flush" + "- Strength:" + this.strength;
        }
        else if (this.FOUR_A_KIND)
        {
            return "Four of a kind" + "- Strength:" + this.strength;
        }
        else if (this.FULL_HOUSE)
        {
            return "Full house" + "- Strength:" + this.strength;
        }
        else if (this.FLUSH)
        {
            return "Flush" + "- Strength:" + this.strength;
        }
        else if (this.STRAIGHT)
        {
            return "Straight" + "- Strength:" + this.strength;
        }
        else if (this.THREE_A_KIND)
        {
            return "Three of a kind" + "- Strength:" + this.strength;
        }
        else if (this.TWO_PAIR)
        {
            return "Two Pair" + "- Strength:" + this.strength;
        }
        else if (this.PAIR)
        {
            return "Pair" + "- Strength:" + this.strength;
        }
        else if (this.HIGH_CARD)
        {
            return "High card" + "- Strength:" + this.strength;
        }
        else
            return "error setting hand.";
    }
    private bool IsRoyal(List<Card> cards)
    {
        this.highCard = new List<Card>();
        this.highCard.Add(cards.ElementAt(4));
        if (IsFlush(cards) && IsStraight(cards) && this.highCard.ElementAt(0).cardValue == CardValue.ace)
        {
            this.RYL_FLUSH = true;
            this.strength = 9;
        }
        return this.RYL_FLUSH;
    }
    private bool IsStraightFlush(List<Card> cards)
    {
        this.highCard = new List<Card>();
        this.highCard.Add(cards.ElementAt(4));
        if (IsFlush(cards) && IsStraight(cards))
        {
            this.STRAIGHT_FLUSH = true;
            this.strength = 8;
            return true;
        }
        return false;
    }
    private bool IsFourAKind(List<Card> cards)
    {
        this.highCard = new List<Card>();
        int count;

        for (int i = 0; i < 5; i++)
        {
            count = 0;
            Card curentCard = cards.ElementAt(i);
            foreach (Card card in cards)
            {
                if (curentCard.cardValue == card.cardValue)
                    count++;
                if (count == 4)
                {
                    this.FOUR_A_KIND = true;
                    this.highCard.Add(card);
                    this.strength = 7;
                    foreach (Card c in cards)
                    {
                        if ((int)c.cardValue != (int)highCard.ElementAt(0).cardValue)
                            this.highCard.Add(c);
                    }
                    return true;
                }
            }
        }
        return false;
    }
    private bool IsFullHouse(List<Card> cards)
    {
        this.highCard = new List<Card>();
        int count;
        Card match1;
        Card match2 = null;
        for (int i = 0; i < 5; i++)
        {
            count = 0;
            Card curentCard = cards.ElementAt(i);
            foreach (Card card in cards)
            {
                if (curentCard.cardValue == card.cardValue)
                    count++;
                if (count == 3)
                {
                    match1 = card;
                    foreach (Card c2 in cards)
                    {
                        if (match1.cardValue != c2.cardValue)
                        {
                            match2 = c2;
                        }
                    }
                    if (match2 != null)
                    {
                        count = 0;
                        foreach (Card c3 in cards)
                        {
                            if (match2.cardValue == c3.cardValue)
                            {
                                count++;
                                if (count == 2)
                                {
                                    this.highCard.Add(match1);
                                    this.highCard.Add(match2);
                                    this.FULL_HOUSE = true;
                                    this.strength = 6;
                                    return true;
                                }
                            }
                        }
                    }
                }
            }
        }
        return false;
    }
    private bool IsFlush(List<Card> cards)
    {
        this.highCard = new List<Card>();
        int playerSuite;
        Card card = cards.ElementAt(0);
        playerSuite = (int)card.cardColor;

        foreach (Card c in cards)
        {
            if ((int)c.cardColor != playerSuite)
                return false;
        }
        for (int i = 4; i >= 0; i--)
        {
            this.highCard.Add(cards.ElementAt(i));
        }
        this.FLUSH = true;
        this.strength = 5;
        return true;
    }
    private bool IsStraight(List<Card> cards)
    {
        this.highCard = new List<Card>();
        Card card1 = cards.ElementAt(0);
        Card card2 = cards.ElementAt(1);
        Card card3 = cards.ElementAt(2);
        Card card4 = cards.ElementAt(3);
        Card card5 = cards.ElementAt(4);
        if (((int)card3.cardValue - (int)card4.cardValue == -1) && ((int)card2.cardValue - (int)card3.cardValue == -1) && ((int)card1.cardValue - (int)card2.cardValue == -1))
        {
            if ((card4.cardValue - card5.cardValue == -1))
            {
                this.STRAIGHT = true;
                this.strength = 4;
                for (int i = 4; i >= 0; i--)
                {
                    this.highCard.Add(cards.ElementAt(i));
                }
            }
            else if ((int)card1.cardValue == 2 && (int)card5.cardValue == 14 /* ACE */)
            {
                this.STRAIGHT = true;
                this.strength = 4;
                for (int i = 4; i >= 0; i--)
                {
                    this.highCard.Add(cards.ElementAt(i));
                }
            }
            else
                return this.STRAIGHT;
        }
        return this.STRAIGHT;
    }
    private bool IsThreeAKind(List<Card> cards)
    {
        this.highCard = new List<Card>();
        int count;
        for (int i = 0; i < 5; i++)
        {
            count = 0;
            Card curentCard = cards.ElementAt(i);
            foreach (Card card in cards)
            {
                if (curentCard.cardValue == card.cardValue)
                    count++;
                if (count == 3)
                {
                    this.highCard.Add(card);
                    this.THREE_A_KIND = true;
                    this.strength = 3;
                    for (int j = 4; j >= 0; j--)
                    {
                        if (cards.ElementAt(j).cardValue != this.highCard.ElementAt(0).cardValue)
                            this.highCard.Add(cards.ElementAt(j));
                    }
                }
            }
        }
        return this.THREE_A_KIND;
    }
    private bool IsTwoPair(List<Card> cards)
    {
        this.highCard = new List<Card>();
        int count;
        Card match1 = null;
        for (int i = 0; i < 5; i++)
        {
            count = 0;
            Card curentCard = cards.ElementAt(i);
            foreach (Card card in cards)
            {
                if (curentCard.cardValue == card.cardValue)
                    count++;
                if (count == 2)
                {
                    match1 = card;
                    break;
                }
            }
            if (match1 != null) break;
        }
        if (match1 != null)
        {
            for (int i = 0; i < 5; i++)
            {
                count = 0;
                Card curentCard = cards.ElementAt(i);
                if (curentCard.cardValue != match1.cardValue)
                {
                    foreach (Card card in cards)
                    {
                        if (curentCard.cardValue == card.cardValue)
                            count++;
                        if (count == 2)
                        {
                            this.highCard.Add(card);
                            this.highCard.Add(match1);
                            this.TWO_PAIR = true;
                            this.strength = 2;
                            for (int j = 4; j >= 0; j--)
                            {
                                if (cards.ElementAt(j).cardValue != this.highCard.ElementAt(0).cardValue
                                        && cards.ElementAt(j).cardValue != this.highCard.ElementAt(1).cardValue)
                                    this.highCard.Add(cards.ElementAt(j));
                            }
                        }
                    }
                }
            }
        }
        return this.TWO_PAIR;
    }
    private bool IsPair(List<Card> cards)
    {
        this.highCard = new List<Card>();
        int count;
        for (int i = 0; i < 5; i++)
        {
            count = 0;
            Card curentCard = cards.ElementAt(i);
            foreach (Card card in cards)
            {
                if (curentCard.cardValue == card.cardValue)  
                    count++;
                if (count == 2)
                {
                    this.PAIR = true;
                    this.highCard.Add(card);
                    this.strength = 1;
                    for (int j = 4; j >= 0; j--)
                    {
                        if (this.highCard.ElementAt(0).cardValue != cards.ElementAt(j).cardValue)
                            this.highCard.Add(cards.ElementAt(j));
                    }
                }
            }
        }
        return this.PAIR;
    }
    private void IsHigh(List<Card> cards)
    {
        this.highCard = new List<Card>();
        for (int j = 4; j >= 0; j--)
        {
            this.highCard.Add(cards.ElementAt(j));
        }
        this.HIGH_CARD = true;
    }
}