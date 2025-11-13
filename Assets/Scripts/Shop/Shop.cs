using System;
using UnityEngine;
using YG;

public class Shop
{
    private float _increaseFactor = 1.2f;
    
    public int ProcessTransaction(int price)
    {
        float newPrice = price;
        int score = YG2.saves.gameplay.Score;
        score -= price;
        YG2.saves.gameplay.Score = score;
        price = GetUpdatePrice(newPrice);

        return price;
    }

    private int GetUpdatePrice(float price)
    {
        price *= _increaseFactor;
        price = Mathf.Ceil(price);
        
        return Convert.ToInt32(price);
    }
}