using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class GameMaster : NetworkBehaviour
{
    public enum PLAYERS
    {
        P1,
        P2
    }

    public enum GAMESTATES
    {
        WAITING_FOR_PLAYERS,
        STARTING,
        ENDING,
        OVER,
        WAITING_FOR_READY,
        INGAME
    }

    [SyncVar]
    public GAMESTATES STATE;

    static public int numOfPlayers = 0;
    static List<Player> connectedPlayers;

    static public Canvas CANVAS;

    static SyncListTest GAMEVARS;

    float timer = 0;

    private void Awake()
    {
        connectedPlayers = new List<Player>();
    }

    private void Start()
    {
        CANVAS = GameObject.Find("Canvas").GetComponent<Canvas>();
        GAMEVARS = GameObject.Find("GameVars").GetComponent<SyncListTest>();
    }

    static public void HideCards()
    {
        GameObject[] playerCards = GameObject.FindGameObjectsWithTag("PlayerCard");
        GameObject[] oppCards = GameObject.FindGameObjectsWithTag("OppCard");

        foreach (var item in playerCards)
        {
            item.GetComponent<Image>().color = new Color(255, 255, 255, 0);
        }
        foreach (var item in oppCards)
        {
            item.GetComponent<Image>().color = new Color(255, 255, 255, 0);
        }
    }

    static public void ShuffleDeck(ref SyncListInt Deck)
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

    void DealTrumpCards()
    {
        int card = Random.Range(
            (int)1, (int)SyncListTest.TrumpCards.NUM_OF_TRUMP_CARDS-1
        );
        if (GAMEVARS.P1Trumps[0] == 0)
        {
            // empty slot
            GAMEVARS.P1Trumps[0] = card;
        }
        else
        {
            if (GAMEVARS.P1Trumps[1] == 0)
            {
                // empty slot
                GAMEVARS.P1Trumps[1] = card;
            }
            else
            {
                if (GAMEVARS.P1Trumps[2] == 0)
                {
                    // empty slot
                    GAMEVARS.P1Trumps[2] = card;
                }
                else
                {
                    // Can't give card!
                }
            }
        }

        card = Random.Range(
            (int)1, (int)SyncListTest.TrumpCards.NUM_OF_TRUMP_CARDS - 1
        );
        if (GAMEVARS.P2Trumps[0] == 0)
        {
            // empty slot
            GAMEVARS.P2Trumps[0] = card;
        }
        else
        {
            if (GAMEVARS.P2Trumps[1] == 0)
            {
                // empty slot
                GAMEVARS.P2Trumps[1] = card;
            }
            else
            {
                if (GAMEVARS.P2Trumps[2] == 0)
                {
                    // empty slot
                    GAMEVARS.P2Trumps[2] = card;
                }
                else
                {
                    // Can't give card!
                }
            }
        }
    }

    private void Update()
    {
        if (!isServer) return;
        switch (STATE)
        {
            case GAMESTATES.WAITING_FOR_PLAYERS:
                if (numOfPlayers >= 2)
                {
                   // if (GAMEVARS.P1Ready == true && GAMEVARS.P2Ready == true)
                   // {
                        GAMEVARS.P1Ready = false;
                        GAMEVARS.P2Ready = false;
                        STATE = GAMESTATES.STARTING;
                        Debug.Log("Starting game!");
                   // }
                }
                break;
            case GAMESTATES.STARTING:
                GAMEVARS.P1_STATE = SyncListTest.State.IDLE;
                GAMEVARS.P2_STATE = SyncListTest.State.IDLE;
                DealTrumpCards();
                GAMEVARS.P1Hand.Add(GAMEVARS.Deck[0]);
                GAMEVARS.Deck.RemoveAt(0);
                GAMEVARS.P2Hand.Add(GAMEVARS.Deck[0]);
                GAMEVARS.Deck.RemoveAt(0);
                GAMEVARS.P1Hand.Add(GAMEVARS.Deck[0]);
                GAMEVARS.Deck.RemoveAt(0);
                GAMEVARS.P2Hand.Add(GAMEVARS.Deck[0]);
                GAMEVARS.Deck.RemoveAt(0);
                STATE = GAMESTATES.INGAME;
                Debug.Log("Going in-game!");
                break;
            case GAMESTATES.INGAME:
                //GAMEVARS.RpcUpdateUI();
                break;
            case GAMESTATES.ENDING:
                // check how close it is to 21
                int P1Score = 0;
                int P2Score = 0;
                foreach (var item in GAMEVARS.P1Hand)
                {
                    P1Score += item;
                }
                foreach (var item in GAMEVARS.P2Hand)
                {
                    P2Score += item;
                }

                // 21 - 23 = -2
                int P1Perf = 21 - P1Score;
                int P2Perf = 21 - P2Score;
                bool P1Over21 = false;
                bool P2Over21 = false;
                if (P1Perf < 0)
                {
                    P1Over21 = true;
                }
                if (P2Perf < 0)
                {
                    P2Over21 = true;
                }

                if (P1Over21 && P2Over21)
                {
                    P1Perf *= -1;
                    P2Perf *= -1;
                    if (P1Perf < P2Perf)
                    {
                        Debug.Log("P1 Wins!");
                        GAMEVARS.P2Coconuts -= GAMEVARS.P2Bet;
                        GAMEVARS.P1_STATE = SyncListTest.State.WON;
                        GAMEVARS.P2_STATE = SyncListTest.State.LOST;
                    }
                    else if (P1Perf > P2Perf)
                    {
                        Debug.Log("P2 Wins!");
                        GAMEVARS.P1Coconuts -= GAMEVARS.P1Bet;
                        GAMEVARS.P1_STATE = SyncListTest.State.LOST;
                        GAMEVARS.P2_STATE = SyncListTest.State.WON;
                    }
                    else if (P1Perf == P2Perf)
                    {
                        Debug.Log("Draw!");
                    }
                }
                else if (P1Over21 == false && P2Over21 == true)
                {
                    Debug.Log("P1 Wins!");
                    GAMEVARS.P2Coconuts -= GAMEVARS.P2Bet;
                    GAMEVARS.P1_STATE = SyncListTest.State.WON;
                    GAMEVARS.P2_STATE = SyncListTest.State.LOST;
                }
                else if (P1Over21 == true && P2Over21 == false)
                {
                    Debug.Log("P2 Wins!");
                    GAMEVARS.P1Coconuts -= GAMEVARS.P1Bet;
                    GAMEVARS.P1_STATE = SyncListTest.State.LOST;
                    GAMEVARS.P2_STATE = SyncListTest.State.WON;
                }
                else
                {
                    if (P1Perf < P2Perf)
                    {
                        Debug.Log("P1 Wins!");
                        GAMEVARS.P2Coconuts -= GAMEVARS.P2Bet;
                        GAMEVARS.P1_STATE = SyncListTest.State.WON;
                        GAMEVARS.P2_STATE = SyncListTest.State.LOST;
                    }
                    else if(P1Perf > P2Perf)
                    {
                        Debug.Log("P2 Wins!");
                        GAMEVARS.P1Coconuts -= GAMEVARS.P1Bet;
                        GAMEVARS.P1_STATE = SyncListTest.State.LOST;
                        GAMEVARS.P2_STATE = SyncListTest.State.WON;
                    }
                    else if(P1Perf == P2Perf)
                    {
                        Debug.Log("Draw!");
                    }
                }
                STATE = GAMESTATES.OVER;
                break;
            case GAMESTATES.OVER:
                GAMEVARS.RevealCards = true;
                GAMEVARS.RpcHideTurn();
                STATE = GAMESTATES.WAITING_FOR_READY;

                if (GAMEVARS.P1Coconuts == 0 || GAMEVARS.P2Coconuts == 0)
                    GAMEVARS.RpcCloseAllClients();
                break;
            case GAMESTATES.WAITING_FOR_READY:
                // if(GAMEVARS.P1Ready && GAMEVARS.P2Ready)
                // {
                timer += Time.deltaTime;
                if(timer > 5.0f)
                {
                    GAMEVARS.RpcShowTurn();
                    timer = 0;
                    GAMEVARS.P1Hand.Clear();
                    GAMEVARS.P2Hand.Clear();
                    GAMEVARS.P1Bet = 1;
                    GAMEVARS.P2Bet = 1;
                    GAMEVARS.P1TrumpsINPLAY[0] = 0;
                    GAMEVARS.P1TrumpsINPLAY[1] = 0;
                    GAMEVARS.P1TrumpsINPLAY[2] = 0;
                    GAMEVARS.P2TrumpsINPLAY[0] = 0;
                    GAMEVARS.P2TrumpsINPLAY[1] = 0;
                    GAMEVARS.P2TrumpsINPLAY[2] = 0;
                    GAMEVARS.Deck.Clear();
                    GAMEVARS.P1_STATE = SyncListTest.State.IDLE;
                    GAMEVARS.P2_STATE = SyncListTest.State.IDLE;
                    for (int i = 1; i < 12; i++)
                    {
                        GAMEVARS.Deck.Add(i);
                    }
                    GAMEVARS.RpcResetUI();
                    ShuffleDeck(ref GAMEVARS.Deck);
                    GAMEVARS.RevealCards = false;
                    GAMEVARS.P1Ready = false;
                    GAMEVARS.P2Ready = false;
                    STATE = GAMESTATES.STARTING;
                }
                // }
                break;
        }
    }
}
