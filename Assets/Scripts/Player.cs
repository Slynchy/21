using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class Player : NetworkBehaviour
{
    public GameMaster.PLAYERS PLAYER;
    public GameObject CardPrefab;

    public List<GameObject> Hand;

    public List<Sprite> cardSprites;
    public List<Sprite> trumpCardSprites;

    public GameObject messagePrefab;

    bool revealedUI = false;

    public override void OnStartServer()
    {
        //if (GameMaster.numOfPlayers == 1)
        //    PLAYER = GameMaster.PLAYERS.P1;
        //else if (GameMaster.numOfPlayers == 2)
        //    PLAYER = GameMaster.PLAYERS.P2;
    }

    private void Awake()
    {
        cardSprites = new List<Sprite>();
        for (int i = 0; i < 12; i++)
        {
            cardSprites.Add(Resources.Load<Sprite>(i.ToString()) as Sprite);
        }

        for (int i = -1; i < (int)SyncListTest.TrumpCards.NUM_OF_TRUMP_CARDS; i++)
        {
            trumpCardSprites.Add(Resources.Load<Sprite>("TrumpCards/" + i.ToString()) as Sprite);
        }
    }

    void Start()
    {
        //this.transform.SetParent(GameObject.Find("Canvas").GetComponent<Canvas>().transform);
        //this.transform.localScale = new Vector3(1, 1, 1);
        //GameMaster.HideCards();
    }

    [Command]
    public void CmdSetState(SyncListTest.State _state, GameMaster.PLAYERS _player)
    {
        SyncListTest temp = GameObject.Find("GameVars").GetComponent<SyncListTest>();
        switch (_player)
        {
            case GameMaster.PLAYERS.P1:
                temp.P1_STATE = _state;
                break;
            case GameMaster.PLAYERS.P2:
                temp.P2_STATE = _state;
                break;
        }
    }

    void UpdateUI()
    {
        SyncListTest temp = GameObject.Find("GameVars").GetComponent<SyncListTest>();
        GameObject[] tempGO = GameObject.FindGameObjectsWithTag("PlayerCard");//P1
        for (int i = 0; i < temp.P1Hand.Count; i++)
        {
            tempGO[i].GetComponent<Image>().color = Color.white;
            if ((i == 0 && PLAYER != GameMaster.PLAYERS.P1) && temp.RevealCards == false) { }
            else
            {
                if (i == 0) tempGO[i].GetComponent<Image>().color = new Color(1, 0, 0, 1);
                tempGO[i].GetComponent<Image>().sprite = cardSprites[temp.P1Hand[i]];
            }
        }
        tempGO = null;
        tempGO = GameObject.FindGameObjectsWithTag("OppCard");//P2
        for (int i = 0; i < temp.P2Hand.Count; i++)
        {
            tempGO[i].GetComponent<Image>().color = Color.white;
            if ((i == 0 && PLAYER != GameMaster.PLAYERS.P2) && temp.RevealCards == false)
            { /* do nothing */ }
            else
            {
                if (i == 0 && temp.RevealCards == false) tempGO[i].GetComponent<Image>().color = new Color(1, 0, 0, 1);
                tempGO[i].GetComponent<Image>().sprite = cardSprites[temp.P2Hand[i]];
            }
        }
        if (temp.currentTurn == PLAYER)
        {
            GameObject.Find("CurrentTurn").GetComponent<Text>().text = "Turn: Yours";
            GameObject.Find("CurrentTurn").GetComponent<Text>().color = new Color(0, 1, 0, GameObject.Find("CurrentTurn").GetComponent<Text>().color.a);
        }
        else
        {
            GameObject.Find("CurrentTurn").GetComponent<Text>().text = "Turn: Theirs";
            GameObject.Find("CurrentTurn").GetComponent<Text>().color = new Color(1, 0, 0, GameObject.Find("CurrentTurn").GetComponent<Text>().color.a);
        }

        if (temp.currentTurn == GameMaster.PLAYERS.P1)
        {
            GameObject.Find("PlayerPanel").GetComponent<Image>().color = new Color(0.5f, 1, 0.5f);
            GameObject.Find("OppPanel").GetComponent<Image>().color = Color.white;
        }
        else
        {
            GameObject.Find("OppPanel").GetComponent<Image>().color = new Color(0.5f, 1, 0.5f);
            GameObject.Find("PlayerPanel").GetComponent<Image>().color = Color.white;
        }

        int totalScore = 0;
        foreach (var score in temp.P1Hand)
        {
            totalScore += score;
        }
        if ((PLAYER == GameMaster.PLAYERS.P2 && temp.P1Hand.Count > 0) && temp.RevealCards == false) totalScore -= temp.P1Hand[0];
        GameObject.Find("21CounterPlayer").GetComponent<Text>().text = totalScore.ToString() + " / " + "21";
        totalScore = 0;
        foreach (var score in temp.P2Hand)
        {
            totalScore += score;
        }
        if ((PLAYER == GameMaster.PLAYERS.P1 && temp.P2Hand.Count > 0) && temp.RevealCards == false) totalScore -= temp.P2Hand[0];
        GameObject.Find("21CounterOpp").GetComponent<Text>().text = totalScore.ToString() + " / " + "21";

        GameObject.Find("P1Coconuts").GetComponent<Text>().text = temp.P1Coconuts.ToString() + " Coconuts\nStake: +" + temp.P1Bet.ToString();
        GameObject.Find("P2Coconuts").GetComponent<Text>().text = temp.P2Coconuts.ToString() + " Coconuts\nStake: +" + temp.P2Bet.ToString();

        switch (temp.P1_STATE)
        {
            case SyncListTest.State.STICKING:
                GameObject.Find("StickingWaitingPlayingP1").GetComponent<Text>().text = "Sticking...";
                GameObject.Find("StickingWaitingPlayingP1").GetComponent<Text>().color = Color.yellow;
                break;
            case SyncListTest.State.PLAYING:
                GameObject.Find("StickingWaitingPlayingP1").GetComponent<Text>().text = "Playing...";
                GameObject.Find("StickingWaitingPlayingP1").GetComponent<Text>().color = Color.green;
                break;
            case SyncListTest.State.IDLE:
                GameObject.Find("StickingWaitingPlayingP1").GetComponent<Text>().text = "Waiting...";
                GameObject.Find("StickingWaitingPlayingP1").GetComponent<Text>().color = Color.red;
                break;
            case SyncListTest.State.WON:
                GameObject.Find("StickingWaitingPlayingP1").GetComponent<Text>().text = "WON!";
                GameObject.Find("StickingWaitingPlayingP1").GetComponent<Text>().color = Color.green;
                break;
            case SyncListTest.State.LOST:
                GameObject.Find("StickingWaitingPlayingP1").GetComponent<Text>().text = "LOST!";
                GameObject.Find("StickingWaitingPlayingP1").GetComponent<Text>().color = Color.red;
                break;
        }
        switch (temp.P2_STATE)
        {
            case SyncListTest.State.STICKING:
                GameObject.Find("StickingWaitingPlayingP2").GetComponent<Text>().text = "Sticking...";
                GameObject.Find("StickingWaitingPlayingP2").GetComponent<Text>().color = Color.yellow;
                break;
            case SyncListTest.State.PLAYING:
                GameObject.Find("StickingWaitingPlayingP2").GetComponent<Text>().text = "Playing...";
                GameObject.Find("StickingWaitingPlayingP2").GetComponent<Text>().color = Color.green;
                break;
            case SyncListTest.State.IDLE:
                GameObject.Find("StickingWaitingPlayingP2").GetComponent<Text>().text = "Waiting...";
                GameObject.Find("StickingWaitingPlayingP2").GetComponent<Text>().color = Color.red;
                break;
            case SyncListTest.State.WON:
                GameObject.Find("StickingWaitingPlayingP2").GetComponent<Text>().text = "WON!";
                GameObject.Find("StickingWaitingPlayingP2").GetComponent<Text>().color = Color.green;
                break;
            case SyncListTest.State.LOST:
                GameObject.Find("StickingWaitingPlayingP2").GetComponent<Text>().text = "LOST!";
                GameObject.Find("StickingWaitingPlayingP2").GetComponent<Text>().color = Color.red;
                break;
        }

        Image[] trumpCards = GameObject.Find("TrumpDisplay").GetComponentsInChildren<Image>();
        trumpCards[0].GetComponent<Image>().color = new Color(1, 1, 1, 0.66f);
        for (int i = 0; i < trumpCards.Length - 1; i++)
        {
            if (PLAYER == GameMaster.PLAYERS.P1)
            {
                if (temp.P1Trumps[i] == (int)SyncListTest.TrumpCards.NONE)
                {
                    trumpCards[i + 1].color = new Color(1, 1, 1, 0);
                    trumpCards[i + 1].sprite = trumpCardSprites[0];
                }
                else
                {
                    trumpCards[i + 1].color = new Color(1, 1, 1, 1);
                    trumpCards[i + 1].sprite = trumpCardSprites[((int)temp.P1Trumps[i])];
                }
            }
            else
            {
                if (temp.P2Trumps[i] == (int)SyncListTest.TrumpCards.NONE)
                {
                    trumpCards[i + 1].color = new Color(1, 1, 1, 0);
                    trumpCards[i + 1].sprite = trumpCardSprites[0];
                }
                else
                {
                    trumpCards[i + 1].color = new Color(1, 1, 1, 1);
                    trumpCards[i + 1].sprite = trumpCardSprites[((int)temp.P2Trumps[i])];
                }
            }
        }

        trumpCards = GameObject.Find("PlayerTrumps").GetComponentsInChildren<Image>();
        trumpCards[0].GetComponent<Image>().color = new Color(1, 1, 1, 0.66f);
        for (int i = 0; i < trumpCards.Length - 1; i++)
        {
            if (temp.P1TrumpsINPLAY[i] == (int)SyncListTest.TrumpCards.NONE)
            {
                trumpCards[i + 1].color = new Color(1, 1, 1, 0);
                trumpCards[i + 1].sprite = trumpCardSprites[0];
            }
            else
            {
                trumpCards[i + 1].color = new Color(1, 1, 1, 1);
                trumpCards[i + 1].sprite = trumpCardSprites[((int)temp.P1TrumpsINPLAY[i])];
            }
        }

        trumpCards = GameObject.Find("OppTrumps").GetComponentsInChildren<Image>();
        trumpCards[0].GetComponent<Image>().color = new Color(1, 1, 1, 0.66f);
        for (int i = 0; i < trumpCards.Length - 1; i++)
        {
            if (temp.P2TrumpsINPLAY[i] == (int)SyncListTest.TrumpCards.NONE)
            {
                trumpCards[i + 1].color = new Color(1, 1, 1, 0);
                trumpCards[i + 1].sprite = trumpCardSprites[0];
            }
            else
            {
                trumpCards[i + 1].color = new Color(1, 1, 1, 1);
                trumpCards[i + 1].sprite = trumpCardSprites[((int)temp.P2TrumpsINPLAY[i])];
            }
        }
    }

    [Command]
    void CmdReadyUp(GameMaster.PLAYERS _player)
    {
        SyncListTest temp = GameObject.Find("GameVars").GetComponent<SyncListTest>();
        if (_player == GameMaster.PLAYERS.P1)
        {
            temp.P1Ready = true;
        }
        else
        {
            temp.P2Ready = true;
        }
    }

    void Update()
    {
        if (!isLocalPlayer)
        {
            return;
        }

        UpdateUI();

        SyncListTest temp = GameObject.Find("GameVars").GetComponent<SyncListTest>();
        if (revealedUI == false)
        {
            RevealUI();
        }

        if(Input.GetKeyDown(KeyCode.Space))
        {
            CmdReadyUp(PLAYER);
        }

        if (PLAYER != temp.currentTurn) return;

        if(Input.GetKeyDown(KeyCode.Alpha1))
        {
            if(temp.Deck.Count == 0)
            {
                GameObject.Instantiate<GameObject>(messagePrefab, GameObject.Find("Canvas").transform);
            }
            else
            {
                switch(PLAYER)
                {
                    case GameMaster.PLAYERS.P1:
                        if(temp.P1Hand.Count == 7)
                        {
                            GameObject tempMsg = GameObject.Instantiate<GameObject>(messagePrefab, GameObject.Find("Canvas").transform);
                            tempMsg.GetComponentInChildren<Text>().text = "Cannot hit!\n\nToo many cards!";
                        }
                        else
                            CmdHit();
                        break;
                    case GameMaster.PLAYERS.P2:
                        if (temp.P2Hand.Count == 7)
                        {
                            GameObject tempMsg = GameObject.Instantiate<GameObject>(messagePrefab, GameObject.Find("Canvas").transform);
                            tempMsg.GetComponentInChildren<Text>().text = "Cannot hit!\n\nToo many cards!";
                        }
                        else
                            CmdHit();
                        break;
                }
            }
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            CmdSetState(SyncListTest.State.STICKING, PLAYER);
            CmdFinishTurn(PLAYER);
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            PlayTrumpCard(0);
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            PlayTrumpCard(1);
        }
        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            PlayTrumpCard(2);
        }
    }

    void RevealUI()
    {
        GameObject.Find("21CounterOpp").GetComponent<Text>().color = Color.white;
        GameObject.Find("21CounterPlayer").GetComponent<Text>().color = Color.white;
        GameObject.Find("HelpText").GetComponent<Text>().color = Color.white;
        GameObject.Find("CurrentTurn").GetComponent<Text>().color = Color.white;
        GameObject.Find("P1Coconuts").GetComponent<Text>().color = Color.white;
        GameObject.Find("P2Coconuts").GetComponent<Text>().color = Color.white;
        GameObject.Find("StickingWaitingPlayingP1").GetComponent<Text>().color = Color.white;
        GameObject.Find("StickingWaitingPlayingP2").GetComponent<Text>().color = Color.white;
        revealedUI = true;
    }

    [Command]
    public void CmdPlayTrumpCard(int which, GameMaster.PLAYERS player)
    {
        SyncListTest temp = GameObject.Find("GameVars").GetComponent<SyncListTest>();
        int trump = 0;
        if (player == GameMaster.PLAYERS.P1)
        {
            temp.P1TrumpsINPLAY[which] = temp.P1Trumps[which];
            temp.P1Trumps[which] = (int)SyncListTest.TrumpCards.NONE;
            trump = temp.P1TrumpsINPLAY[which];
        }
        else
        {
            temp.P2TrumpsINPLAY[which] = temp.P2Trumps[which];
            temp.P2Trumps[which] = (int)SyncListTest.TrumpCards.NONE;
            trump = temp.P2TrumpsINPLAY[which];
        }
        switch (trump)
        {
            case (int)SyncListTest.TrumpCards.BET_UP_1:
                if(player == GameMaster.PLAYERS.P1)
                {
                    temp.P2Bet += 1;
                }
                else
                {
                    temp.P1Bet += 1;
                }
                break;
            case (int)SyncListTest.TrumpCards.SHIELD:
                if (player == GameMaster.PLAYERS.P1)
                {
                    temp.P1Bet -= 1;
                    if (temp.P1Bet < 0) temp.P1Bet = 0;
                }
                else
                {
                    temp.P2Bet -= 1;
                    if (temp.P2Bet < 0) temp.P2Bet = 0;
                }
                break;
            case (int)SyncListTest.TrumpCards.CARD1:
            case (int)SyncListTest.TrumpCards.CARD2:
            case (int)SyncListTest.TrumpCards.CARD3:
            case (int)SyncListTest.TrumpCards.CARD4:
            case (int)SyncListTest.TrumpCards.CARD5:
            case (int)SyncListTest.TrumpCards.CARD6:
            case (int)SyncListTest.TrumpCards.CARD7:
            case (int)SyncListTest.TrumpCards.CARD8:
            case (int)SyncListTest.TrumpCards.CARD9:
            case (int)SyncListTest.TrumpCards.CARD10:
            case (int)SyncListTest.TrumpCards.CARD11:
                int card = trump - 2;
                // find card in deck
                for (int i = 0; i < temp.Deck.Count; i++)
                {
                    if(temp.Deck[i] == card)
                    {
                        if(player == GameMaster.PLAYERS.P1)
                        {
                            temp.P1Hand.Add(card);
                            temp.Deck.RemoveAt(i);
                        }
                        else
                        {
                            temp.P2Hand.Add(card);
                            temp.Deck.RemoveAt(i);
                        }
                        break;
                    }
                }
                break;
        }
    }

    void PlayTrumpCard(int which)
    {
        SyncListTest temp = GameObject.Find("GameVars").GetComponent<SyncListTest>();
        if (PLAYER == GameMaster.PLAYERS.P1)
        {
            if (temp.P1Trumps[which] == (int)SyncListTest.TrumpCards.NONE)
            {
                // no card!
                GameObject tempMsg = GameObject.Instantiate<GameObject>(messagePrefab, GameObject.Find("Canvas").transform);
                tempMsg.GetComponentInChildren<Text>().text = "Cannot trump!\n\nYou don't have one!";
                return;
            }
            CmdPlayTrumpCard(which, PLAYER);
        }
        else
        {
            if (temp.P2Trumps[which] == (int)SyncListTest.TrumpCards.NONE)
            {
                // no card!
                GameObject tempMsg = GameObject.Instantiate<GameObject>(messagePrefab, GameObject.Find("Canvas").transform);
                tempMsg.GetComponentInChildren<Text>().text = "Cannot trump!\n\nYou don't have one!";
                return;
            }
            CmdPlayTrumpCard(which, PLAYER);
        }
    }
    
    void HideUI()
    {
        Color hidden = new Color(255, 255, 255, 0);
        GameObject.Find("21CounterOpp").GetComponent<Text>().color = hidden;
        GameObject.Find("21CounterPlayer").GetComponent<Text>().color = hidden;
        GameObject.Find("HelpText").GetComponent<Text>().color = hidden;
        GameObject.Find("CurrentTurn").GetComponent<Text>().color = hidden;
        GameObject.Find("P1Coconuts").GetComponent<Text>().color = hidden;
        GameObject.Find("P2Coconuts").GetComponent<Text>().color = hidden;
        GameObject.Find("StickingWaitingPlayingP1").GetComponent<Text>().color = hidden;
        GameObject.Find("StickingWaitingPlayingP2").GetComponent<Text>().color = hidden;
        revealedUI = false;
    }

    [Command]
    public void CmdFinishTurn(GameMaster.PLAYERS _player)
    {
        SyncListTest temp = GameObject.Find("GameVars").GetComponent<SyncListTest>();
        if (temp.currentTurn == GameMaster.PLAYERS.P1)
        {
            temp.currentTurn = GameMaster.PLAYERS.P2;
        }
        else
        {
            temp.currentTurn = GameMaster.PLAYERS.P1;
        }

        if (temp.P1_STATE == SyncListTest.State.STICKING && temp.P2_STATE == SyncListTest.State.STICKING)
        {
            GameObject.Find("GameMaster").GetComponent<GameMaster>().STATE = GameMaster.GAMESTATES.ENDING;
            return;
        }

        if (temp.currentTurn == GameMaster.PLAYERS.P1)
        {
            temp.P1_STATE = SyncListTest.State.PLAYING;
        }
        else
        {
            temp.P2_STATE = SyncListTest.State.PLAYING;
        }
    }

    void OnDisconnectedFromServer(NetworkDisconnection info)
    {
        Debug.Log("Disconnected from server: " + info);
    }

    [Command]
    public void CmdHit()
    {
        SyncListTest temp = GameObject.Find("GameVars").GetComponent<SyncListTest>();
        if (temp.Deck.Count == 0)
        {
            Debug.Log("Cannot draw card; deck empty!");
        }
        if(temp.currentTurn == GameMaster.PLAYERS.P1)
        {
            temp.P1Hand.Add(temp.Deck[0]);
            temp.Deck.RemoveAt(0);
            temp.currentTurn = GameMaster.PLAYERS.P2;
        }
        else
        {
            temp.P2Hand.Add(temp.Deck[0]);
            temp.Deck.RemoveAt(0);
            temp.currentTurn = GameMaster.PLAYERS.P1;
        }
        GameObject.Find("GameVars").GetComponent<SyncListTest>().RpcUpdateUI();
    }

    public override void OnStartLocalPlayer()
    {
        //GameObject[] players;
        //if (PLAYER == GameMaster.PLAYERS.P1)
        //{
        //    players = GameObject.FindGameObjectsWithTag("PlayerCard");
        //    Hand = new List<GameObject>();
        //    for (int i = 0; i < players.Length; i++)
        //    {
        //        Hand.Add(players[i]);
        //        Hand[Hand.Count-1].GetComponent<Card>().player = Network.player.guid;
        //    }
        //}
        //else
        //{
        //    players = GameObject.FindGameObjectsWithTag("OppCard");
        //    Hand = new List<GameObject>();
        //    for (int i = 0; i < players.Length; i++)
        //    {
        //        Hand.Add(players[i]);
        //        Hand[Hand.Count - 1].GetComponent<Card>().player = Network.player.guid;
        //    }
        //}
        SyncListTest temp = GameObject.Find("GameVars").GetComponent<SyncListTest>();
        if (temp.numOfPlayers <= 1)
        {
            PLAYER = GameMaster.PLAYERS.P1;
            temp.P1_STATE = SyncListTest.State.IDLE;
        }
        else
        {
            PLAYER = GameMaster.PLAYERS.P2;
            temp.P2_STATE = SyncListTest.State.IDLE;
        }
    }
}