using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class GameMaster : NetworkManager
{
    public enum PLAYERS
    {
        P1,
        P2
    }

    static List<int> Deck = new List<int>(11);
    static List<int> P1Hand = new List<int>();
    static List<int> P2Hand = new List<int>();
    public PLAYERS currentTurn = PLAYERS.P1;

    static public Canvas CANVAS;

    private void Start()
    {
        for (int i = 1; i < 12; i++)
        {
            Deck.Add(i);
        }
        ShuffleDeck();
        CANVAS = GameObject.Find("Canvas").GetComponent<Canvas>();
    }

    static void ShuffleDeck()
    {
        int n = Deck.Count;
        while (n > 0)
        {
            n--;
            int k = Random.Range(0, Deck.Count-1);
            var value = Deck[k];
            Deck[k] = Deck[n];
            Deck[n] = value;
        }
    }

    public override void OnStopServer()
    {
        GameObject[] cards = GameObject.FindGameObjectsWithTag("Card");

        for (int i = 0; i < cards.Length; i++)
        {
            Destroy(cards[i]);
        }
    }
}
