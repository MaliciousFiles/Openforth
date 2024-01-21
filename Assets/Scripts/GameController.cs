using UnityEngine;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;
using System.Collections;

public class GameController : MonoBehaviour
{
    public static int NUM_PLAYERS = 4;
    public static GameController INSTANCE;
    public static PlayerController CurrentPlayer
    {
        get { return INSTANCE.players[INSTANCE.currentPlayer]; }
    }

    private bool fadingIn = false;
    private int currentPlayer = -1;
    private readonly List<PlayerController> players = new(NUM_PLAYERS);

    public GameObject playerChangeScreen;
    public GameObject playerPrefab;
    public GameObject table;

    public bool nextTurn, makeClickable;

    public void NextTurn()
    {
        currentPlayer++;
        currentPlayer %= players.Count;

        playerChangeScreen.GetComponentInChildren<TextMeshProUGUI>().text = players[currentPlayer].playerName + "'s Turn";
        fadingIn = true;

        Clickable.SetAllClickable(true);
    }

    private IEnumerator FadePlayerChange()
    {
        while (true)
        {
            CanvasGroup group = playerChangeScreen.GetComponent<CanvasGroup>();

            if (group.alpha != (fadingIn ? 1 : -1))
            {
                group.alpha = Mathf.Clamp01(group.alpha + (fadingIn ? 1 : -1) * Time.deltaTime / 1.5f);

                if (group.alpha == 1)
                {
                    Transform curTransform = players[currentPlayer].transform;

                    Camera.main.transform.SetParent(curTransform, false);
                    table.transform.rotation = curTransform.rotation;
                    players.ForEach(p => p.TransformNamePlate());
                }
            }

            yield return new WaitForEndOfFrame();
        }
    }

    private PlayerController GeneratePlayer()
    {
        GameObject player = Instantiate(playerPrefab);
        Transform transform = player.transform;
        PlayerController script = player.GetComponent<PlayerController>();

        player.SetActive(true);
        script.playerName = "Player " + (players.Count + 1);

        float dist = 13.7f;
        Vector3 pos = transform.position;
        switch (players.Count)
        {
            case 0: pos.z = -dist; break;
            case 1: pos.x = dist; break;
            case 2: pos.z = dist; break;
            case 3: pos.x = -dist; break;
        }
        transform.position = pos;

        transform.rotation = Quaternion.Euler(0, -(90 * players.Count), 0);

        return script;
    }

    private void Start()
    {
        INSTANCE = this;

        playerChangeScreen.GetComponentInChildren<Button>().onClick.AddListener(() => fadingIn = false);

        StartCoroutine(FadePlayerChange());

        for (int i = 0; i < NUM_PLAYERS; i++) players.Add(GeneratePlayer());

        NextTurn();
    }

    private void Update()
    {
        // TODO: debug
        if (nextTurn) { NextTurn(); nextTurn = false; }
        if (makeClickable) { Clickable.SetAllClickable(true); makeClickable = false; }
    }
}
