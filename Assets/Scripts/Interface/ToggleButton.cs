﻿using System;
using UnityEngine;

namespace Interface
{
    public class ToggleButton : MonoBehaviour
    {
        [SerializeField] private GameObject element;
        [SerializeField] private bool defaultVisibility = false;

        private void Start()
        {
            element.SetActive(defaultVisibility);
        }

        public void ToggleElement()
        {
            defaultVisibility = !defaultVisibility;
            element.SetActive(defaultVisibility);
        }
    }
}