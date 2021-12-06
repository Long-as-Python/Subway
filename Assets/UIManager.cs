using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { private set; get; }
    public TextMeshProUGUI CoinsText;
    public int Coins = 0;

    void Start()
    {
        Coins = PlayerPrefs.GetInt("Coins", 0);
        CoinsText.text = "" + Coins;
        if (Instance == null)
            Instance = this;
    }
}
