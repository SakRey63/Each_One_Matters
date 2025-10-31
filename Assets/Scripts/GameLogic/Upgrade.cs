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
    [SerializeField] private TextMeshProUGUI _allScore;
    [SerializeField] private LevelSounds _levelSounds;
    [SerializeField] private float _duration = 1.5f;
    [SerializeField] private int _callHelpUpgradeButtonPrice = 50;
    
    private Shop _shop;
    private Coroutine _coroutine;
    private bool _canIncreaseSquad;
    private bool _canExtendFireRate;
    private bool _canCallHelpOnBase;

    private void OnEnable()
    {
        UpdateTextMenu();
    }

    private void Start()
    {
        _shop = new Shop();
        _increaseSquadPrices.text = Convert.ToString(YG2.saves.upgrades.IncreaseSquadPrices);
        _extendFireRateDurationPrices.text = Convert.ToString(YG2.saves.upgrades.ExtendFireRateDurationPrices);
        _callHelpOnBasePrices.text = Convert.ToString(YG2.saves.upgrades.CallHelpOnBasePrices);
    }

    public void AddPoliceToSquad()
    {
        _canIncreaseSquad = true;
        TryApplyUpgrade(YG2.saves.upgrades.IncreaseSquadPrices);
    }

    public void ExtendFireRateDuration()
    {
        _canExtendFireRate = true;
        TryApplyUpgrade(YG2.saves.upgrades.ExtendFireRateDurationPrices);
    }

    public void CallReinforcements()
    {
        _canCallHelpOnBase = true;
        TryApplyUpgrade(YG2.saves.upgrades.CallHelpOnBasePrices);
    }
    
    private void UpdatePriceText()
    {
        _allScore.text = Convert.ToString(YG2.saves.gameplay.Score);
    }

    private void TryApplyUpgrade(int price)
    {
        if (YG2.saves.gameplay.Score >= price)
        {
            int newPrice = _shop.ProcessTransaction(price);
            
            if (_canIncreaseSquad)
            {
                int countPolice = YG2.saves.player.CountPoliceOfficer;
                countPolice++;
                YG2.saves.player.CountPoliceOfficer = countPolice;
                YG2.saves.upgrades.IncreaseSquadPrices = newPrice;
                _increaseSquadPrices.text = Convert.ToString(newPrice);
                _canIncreaseSquad = false;
            }
            else if (_canExtendFireRate)
            {
                float buffDuration = YG2.saves.upgrades.BuffDuration;
                buffDuration++;
                YG2.saves.upgrades.BuffDuration = buffDuration;
                YG2.saves.upgrades.ExtendFireRateDurationPrices = newPrice;
                _extendFireRateDurationPrices.text = Convert.ToString(newPrice);
                _canExtendFireRate = false;
            }
            else if (_canCallHelpOnBase)
            {
                if (YG2.saves.gameplay.IsCallHelpUpgradePurchased)
                {
                    int countHelpPoliceOfficer = YG2.saves.gameplay.CountHelpPoliceOfficer;
                    countHelpPoliceOfficer++;
                    YG2.saves.gameplay.CountHelpPoliceOfficer = countHelpPoliceOfficer;
                    int priceButton = YG2.saves.gameplay.CallHelpButtonPrice;
                    priceButton += _callHelpUpgradeButtonPrice;
                    YG2.saves.gameplay.CallHelpButtonPrice = priceButton;
                }
                else
                {
                    YG2.saves.gameplay.IsCallHelpUpgradePurchased = true;
                }
                
                YG2.saves.upgrades.CallHelpOnBasePrices = newPrice;
                _callHelpOnBasePrices.text = Convert.ToString(newPrice);
                _canCallHelpOnBase = false;
            }
            
            _levelSounds.PlayUpgradeSound();
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
        UpdatePriceText();

        if (YG2.saves.gameplay.IsCallHelpUpgradePurchased)
        {
            _uguipgradeCallHelpOnBaseText.gameObject.SetActive(true);
            _callHelpOnBaseText.gameObject.SetActive(false);
        }
        else
        {
            _callHelpOnBaseText.gameObject.SetActive(true);
        }
    }

    private IEnumerator ShowErrorMessage()
    {
        float delay = _duration;
        _levelSounds.PlayErrorSound();
        _balanceText.gameObject.SetActive(false);
        _errorText.gameObject.SetActive(true);
        
        while (delay >= 0)
        {
            delay -= Time.deltaTime;
            
            yield return null;
        }
        
        _errorText.gameObject.SetActive(false);
        _balanceText.gameObject.SetActive(true);
        UpdatePriceText();
        _coroutine = null;
    }
}