using System;
using System.Collections;
using EachOneMatters.Audio;
using TMPro;
using UnityEngine;
using YG;

namespace EachOneMatters.Shops
{
    public class Upgrade : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _increaseSquadPrices;
        [SerializeField] private TextMeshProUGUI _extendFireRateDurationPrices;
        [SerializeField] private TextMeshProUGUI _callHelpOnBasePrices;
        [SerializeField] private TextMeshProUGUI _errorText;
        [SerializeField] private TextMeshProUGUI _warningMaxSquadSize;
        [SerializeField] private TextMeshProUGUI _balanceText;
        [SerializeField] private TextMeshProUGUI _callHelpOnBaseText;
        [SerializeField] private TextMeshProUGUI _uguipgradeCallHelpOnBaseText;
        [SerializeField] private TextMeshProUGUI _allScore;
        [SerializeField] private LevelSounds _levelSounds;
        [SerializeField] private float _duration = 1.5f;
        [SerializeField] private int _callHelpUpgradeButtonPrice = 50;

        private Shop _shop;
        private Coroutine _errorTextCoroutine;
        private UpgradeType _pendingUpgrade;

        private enum UpgradeType
        {
            None,
            IncreaseSquad,
            ExtendFireRate,
            CallHelpOnBase,
        }

        private void OnEnable()
        {
            UpdateTextMenu();
            _errorText.gameObject.SetActive(false);
            _warningMaxSquadSize.gameObject.SetActive(false);
            _balanceText.gameObject.SetActive(true);
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
            if (YG2.saves.player.CountPoliceOfficer >= YG2.saves.player.MaxPoliceCount)
            {
                if (_errorTextCoroutine != null)
                {
                    StopCoroutine(_errorTextCoroutine);
                }

                _errorTextCoroutine = StartCoroutine(ShowTemporaryWarning(_warningMaxSquadSize));

                return;
            }

            _pendingUpgrade = UpgradeType.IncreaseSquad;
            TryApplyUpgrade(YG2.saves.upgrades.IncreaseSquadPrices);
        }

        public void ExtendFireRateDuration()
        {
            _pendingUpgrade = UpgradeType.ExtendFireRate;
            TryApplyUpgrade(YG2.saves.upgrades.ExtendFireRateDurationPrices);
        }

        public void CallReinforcements()
        {
            if (YG2.saves.gameplay.CountHelpPoliceOfficer >= YG2.saves.player.MaxPoliceCount)
            {
                if (_errorTextCoroutine != null)
                {
                    StopCoroutine(_errorTextCoroutine);
                }

                _errorTextCoroutine = StartCoroutine(ShowTemporaryWarning(_warningMaxSquadSize));

                return;
            }

            _pendingUpgrade = UpgradeType.CallHelpOnBase;
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

                switch (_pendingUpgrade)
                {
                    case UpgradeType.IncreaseSquad:
                        YG2.saves.player.CountPoliceOfficer++;
                        YG2.saves.upgrades.IncreaseSquadPrices = newPrice;
                        _increaseSquadPrices.text = Convert.ToString(newPrice);
                        break;

                    case UpgradeType.ExtendFireRate:
                        YG2.saves.upgrades.BuffDuration++;
                        YG2.saves.upgrades.ExtendFireRateDurationPrices = newPrice;
                        _extendFireRateDurationPrices.text = Convert.ToString(newPrice);
                        break;

                    case UpgradeType.CallHelpOnBase:

                        if (YG2.saves.gameplay.IsCallHelpUpgradePurchased)
                        {
                            YG2.saves.gameplay.CountHelpPoliceOfficer++;
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
                        break;
                }

                _levelSounds.PlayUpgradeSound();
                YG2.SaveProgress();
                UpdateTextMenu();
            }
            else
            {
                if (YG2.saves.gameplay.Score < price)
                {
                    if (_errorTextCoroutine != null)
                    {
                        StopCoroutine(_errorTextCoroutine);
                    }

                    _errorTextCoroutine = StartCoroutine(ShowTemporaryWarning(_errorText));
                }
            }

            _pendingUpgrade = UpgradeType.None;
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

        private IEnumerator ShowTemporaryWarning(TextMeshProUGUI text)
        {
            WaitForSeconds delay = new WaitForSeconds(_duration);
            _errorText.gameObject.SetActive(false);
            _balanceText.gameObject.SetActive(false);
            _warningMaxSquadSize.gameObject.SetActive(false);
            _levelSounds.PlayErrorSound();
            text.gameObject.SetActive(true);

            yield return delay;

            text.gameObject.SetActive(false);
            _balanceText.gameObject.SetActive(true);
            _errorTextCoroutine = null;
        }
    }
}