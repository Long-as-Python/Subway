using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CoinController : MonoBehaviour
{
    public float speed = 20;
    void Update()
    {
        transform.Rotate(Vector3.up, speed * Time.deltaTime);
    }

    void OnTriggerEnter(Collider other)
    {
        UIManager.Instance.Coins++;
        PlayerPrefs.SetInt("Coins", UIManager.Instance.Coins);
        UIManager.Instance.CoinsText.text = "" + UIManager.Instance.Coins;
        Debug.Log("Coin collected");
        Destroy(this.gameObject);
    }

    void Awake()
    {
        float check = Random.Range(0, 100);
        if (check < 60)
        {
            gameObject.SetActive(false);
        }
    }
}
