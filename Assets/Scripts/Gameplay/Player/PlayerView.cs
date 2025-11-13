using System;
using TMPro;
using UnityEngine;

namespace EachOneMatters.Gameplay.Player
{
    public class PlayerView : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _countPoliceOfficer;

        public void ShowPoliceCount(int count)
        {
            _countPoliceOfficer.text = Convert.ToString(count);
        }
    }
}