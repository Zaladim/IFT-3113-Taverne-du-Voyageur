﻿using System;
using UnityEngine;
using UnityEngine.UI;

namespace Interface
{
    public class RessourcesManager : MonoBehaviour
    {
        private class Resource
        {
            private Text display;
            private uint amount;

            public Resource(Text display, uint amount = 1)
            {
                this.display = display;
                this.display.text = amount.ToString();
            }

            public uint Amount
            {
                get => amount;
                set
                {
                    amount = value;
                    display.text = amount.ToString();
                }
            }
        }

        private Resource gold;
        [SerializeField] private Text goldDisplay;
        [SerializeField] private uint goldAmount;

        private Resource reputation;
        [SerializeField] private Text reputationDisplay;

        private void Awake()
        {
            gold = new Resource(goldDisplay, goldAmount);
            reputation = new Resource(reputationDisplay);
        }

        public uint Gold
        {
            get => gold.Amount;
            set => gold.Amount = value;
        }

        public uint Reputation
        {
            get => reputation.Amount;
            set => reputation.Amount = value;
        }
    }
}