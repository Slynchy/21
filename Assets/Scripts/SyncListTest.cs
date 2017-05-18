using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class SyncListTest : NetworkBehaviour
{
    public enum State
    {
        NOT_JOINED,
        IDLE,
        PLAYING,
        WON,
        LOST,
        STICKING
    }

    public enum TrumpCards
    {
        NONE,
        BET_UP_1,
        GO_FOR_24,
        NUM_OF_TRUMP_CARDS
    }

    [SyncVar]
    public SyncListInt Deck = new SyncListInt();

    [SyncVar]
    public SyncListInt P1Hand = new SyncListInt();

    [SyncVar]
    public SyncListInt P2Hand = new SyncListInt();

    [SyncVar]
    public SyncListInt P1Trumps = new SyncListInt();
    [SyncVar]
    public SyncListInt P1TrumpsINPLAY = new SyncListInt();
    [SyncVar]
    public SyncListInt P2Trumps = new SyncListInt();
    [SyncVar]
    public SyncListInt P2TrumpsINPLAY = new SyncListInt();

    [SyncVar]
    public State P1_STATE;
    [SyncVar]
    public State P2_STATE;

    [SyncVar]
    public int numOfPlayers = 0;

    [SyncVar]
    public GameMaster.PLAYERS currentTurn = GameMaster.PLAYERS.P1;

    [SyncVar]
    public int P1Coconuts = 5;
    [SyncVar]
    public int P2Coconuts = 5;

    [SyncVar]
    public int P1Bet = 1;
    [SyncVar]
    public int P2Bet = 1;

    [SyncVar]
    public bool P1Ready = false;
    [SyncVar]
    public bool P2Ready = false;

    [SyncVar]
    public bool RevealCards = false;

    [ClientRpc]
    public void RpcCloseAllClients()
    {
        Application.Quit();
    }

    [ClientRpc]
    public void RpcResetUI()
    {
        GameObject[] playerCards = GameObject.FindGameObjectsWithTag("PlayerCard");
        GameObject[] oppCards = GameObject.FindGameObjectsWithTag("OppCard");

        Sprite blankSprite = Resources.Load<Sprite>("0") as Sprite;

        foreach (var item in playerCards)
        {
            item.GetComponent<Image>().color = new Color(255, 255, 255, 0);
            item.GetComponent<Image>().sprite = blankSprite;
        }
        foreach (var item in oppCards)
        {
            item.GetComponent<Image>().color = new Color(255, 255, 255, 0);
            item.GetComponent<Image>().sprite = blankSprite;
        }
    }

    [ClientRpc]
    public void RpcHideTurn()
    {
        GameObject.Find("CurrentTurn").GetComponent<Text>().color = new Color(1, 1, 1, 0); ;
    }

    [ClientRpc]
    public void RpcShowTurn()
    {
        GameObject.Find("CurrentTurn").GetComponent<Text>().color = Color.white;
    }

    [ClientRpc]
    public void RpcUpdateUI()
    {
        GameObject[] tempGO = GameObject.FindGameObjectsWithTag("PlayerCard");
        for (int i = 0; i < P1Hand.Count; i++)
        {
            tempGO[i].GetComponent<Image>().color = Color.white;
        }
        tempGO = null;
        tempGO = GameObject.FindGameObjectsWithTag("OppCard");
        for (int i = 0; i < P2Hand.Count; i++)
        {
            tempGO[i].GetComponent<Image>().color = Color.white;
        }

        if (currentTurn == GameMaster.PLAYERS.P1)
        {
            GameObject.Find("CurrentTurn").GetComponent<Text>().text = "Turn: P1";
        }
        else
        {
            GameObject.Find("CurrentTurn").GetComponent<Text>().text = "Turn: P2";
        }
    }

    private void Awake()
    {

    }

    private void Start()
    {
        for (int i = 1; i < 12; i++)
        {
            Deck.Add(i);
        }
        GameMaster.ShuffleDeck(ref Deck);
        P1Trumps.Add((int)TrumpCards.BET_UP_1);
        P2Trumps.Add((int)TrumpCards.BET_UP_1);
        P1Trumps.Add((int)TrumpCards.NONE);
        P2Trumps.Add((int)TrumpCards.NONE);
        P1Trumps.Add((int)TrumpCards.NONE);
        P2Trumps.Add((int)TrumpCards.NONE);
        P1TrumpsINPLAY.Add((int)TrumpCards.NONE);
        P2TrumpsINPLAY.Add((int)TrumpCards.NONE);
        P1TrumpsINPLAY.Add((int)TrumpCards.NONE);
        P2TrumpsINPLAY.Add((int)TrumpCards.NONE);
        P1TrumpsINPLAY.Add((int)TrumpCards.NONE);
        P2TrumpsINPLAY.Add((int)TrumpCards.NONE);
    }

}
