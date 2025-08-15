using System;
using System.Collections;
using TMPro;
using UnityEngine;
using YG;

public class Upgrade : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _increaseSquadPrices;
    [SerializeField] private TextMeshProUGUI _extendFireRateDurationPrices;
    [SerializeField] private TextMeshProUGUI _callHelpOnBasePrices;
    [SerializeField] private TextMeshProUGUI _errorText;
    [SerializeField] private TextMeshProUGUI _balanceText;
    [SerializeField] private TextMeshProUGUI _callHelpOnBaseText;
    [SerializeField] private TextMeshProUGUI _uguipgradeCallHelpOnBaseText;
    [SerializeField] private float _duration = 1.5f;
    
    private Shop _shop;
    private Coroutine _coroutine;
    private bool _canIncreaseSquad;
    private bool _canExtendFireRate;
    private bool _canCallHelpOnBase;
    private string _defaultText;

    private void Start()
    {
        _defaultText = _balanceText.text;
        _shop = new Shop();
        _increaseSquadPrices.text = Convert.ToString(YG2.saves.IncreaseSquadPrices);
        _extendFireRateDurationPrices.text = Convert.ToString(YG2.saves.ExtendFireRateDurationPrices);
        _callHelpOnBasePrices.text = Convert.ToString(YG2.saves.CallHelpOnBasePrices);
        UpdateTextMenu();
    }

    public void AddPoliceToSquad()
    {
        _canIncreaseSquad = true;
        TryApplyUpgrade(YG2.saves.IncreaseSquadPrices);
    }

    public void ExtendFireRateDuration()
    {
        _canExtendFireRate = true;
        TryApplyUpgrade(YG2.saves.ExtendFireRateDurationPrices);
    }

    public void CallReinforcements()
    {
        _canCallHelpOnBase = true;
        TryApplyUpgrade(YG2.saves.CallHelpOnBasePrices);
    }

    private void TryApplyUpgrade(int price)
    {
        if (YG2.saves.Score >= price)
        {
            int newPrice = _shop.ProcessTransaction(price);
            
            if (_canIncreaseSquad)
            {
                int countPolice = YG2.saves.CountPoliceOfficer;
                countPolice++;
                YG2.saves.CountPoliceOfficer = countPolice;
                YG2.saves.IncreaseSquadPrices = newPrice;
                _increaseSquadPrices.text = Convert.ToString(newPrice);
                _canIncreaseSquad = false;
            }
            else if (_canExtendFireRate)
            {
                float buffDuration = YG2.saves.BuffDuration;
                buffDuration++;
                YG2.saves.BuffDuration = buffDuration;
                YG2.saves.ExtendFireRateDurationPrices = newPrice;
                _extendFireRateDurationPrices.text = Convert.ToString(newPrice);
                _canExtendFireRate = false;
            }
            else if (_canCallHelpOnBase)
            {
                if (YG2.saves.IsCallHelpUpgradePurchased)
                {
                    int countHelpPoliceOfficer = YG2.saves.CountHelpPoliceOfficer;
                    countHelpPoliceOfficer++;
                    YG2.saves.CountHelpPoliceOfficer = countHelpPoliceOfficer;
                }
                else
                {
                    YG2.saves.IsCallHelpUpgradePurchased = true;
                }
                
                YG2.saves.CallHelpOnBasePrices = newPrice;
                _callHelpOnBasePrices.text = Convert.ToString(newPrice);
                _canCallHelpOnBase = false;
            }
            
            YG2.SaveProgress();
            UpdateTextMenu();
        }
        else
        {
            if (_coroutine != null)
            {
                StopCoroutine(_coroutine);
            }
            
            _coroutine = StartCoroutine(ShowErrorMessage());
        }
    }

    private void UpdateTextMenu()
    {
        string newText = _defaultText;
        newText += " " + Convert.ToString(YG2.saves.Score);
        _balanceText.text = newText;

        if (YG2.saves.IsCallHelpUpgradePurchased)
        {
            _callHelpOnBaseText.gameObject.SetActive(false);
            _uguipgradeCallHelpOnBaseText.gameObject.SetActive(true);
        }
        else
        {
            _callHelpOnBaseText.gameObject.SetActive(true);
            _uguipgradeCallHelpOnBaseText.gameObject.SetActive(false);
        }
    }

    private IEnumerator ShowErrorMessage()
    {
        float delay = _duration;
        _balanceText.gameObject.SetActive(false);
        _errorText.gameObject.SetActive(true);
        
        while (delay >= 0)
        {
            delay -= Time.deltaTime;
            
            yield return null;
        }
        
        _errorText.gameObject.SetActive(false);
        _balanceText.gameObject.SetActive(true);
        _coroutine = null;
    }
}