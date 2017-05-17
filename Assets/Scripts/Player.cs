using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class Player : NetworkBehaviour
{
    GameMaster.PLAYERS PLAYER;
    public GameObject CardPrefab;

    public List<GameObject> Hand;

    void Start()
    {
        this.transform.SetParent(GameMaster.CANVAS.transform);
        this.transform.localScale = new Vector3(1, 1, 1);
    }

    void Update()
    {
        if (!isLocalPlayer)
        {
            return;
        }
        

    }

    public override void OnStartLocalPlayer()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        if (players.Length % 2 == 1)
            PLAYER = GameMaster.PLAYERS.P1;
        else
            PLAYER = GameMaster.PLAYERS.P2;

        Hand = new List<GameObject>();
        Hand.Add(GameObject.Instantiate<GameObject>(CardPrefab, GameMaster.CANVAS.transform));
        Hand[0].GetComponent<Card>().player = Network.player.guid;

        if(PLAYER == GameMaster.PLAYERS.P1)
        {
            Hand[0].GetComponent<RectTransform>().anchorMin.Set(0, 0);
            Hand[0].GetComponent<RectTransform>().anchorMax.Set(0, 0);

            foreach (GameObject card in Hand)
            {
                card.GetComponent<Image>().color = Color.green;
                card.GetComponentInChildren<Text>().text = "P1";
            }
        }
        else
        {
            Hand[0].GetComponent<RectTransform>().anchorMin.Set(0, 1);
            Hand[0].GetComponent<RectTransform>().anchorMax.Set(0, 1);
            Hand[0].GetComponent<RectTransform>().offsetMin.Set(75, -75);
            Hand[0].GetComponent<RectTransform>().offsetMax.Set(75, -75);

            foreach (GameObject card in Hand)
            {
                card.GetComponent<Image>().color = Color.red;
                card.GetComponentInChildren<Text>().text = "P2";
            }
        }
    }
}