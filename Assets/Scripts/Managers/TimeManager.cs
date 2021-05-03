using System;
using UnityEngine;

namespace Managers
{
    public class TimeManager : MonoBehaviour
    {
        [SerializeField] [Range(0, 10)] private int timeScaleModifier = 1;
        [SerializeField] private bool isFrozen = false;

        private void Awake()
        {
            Time.timeScale = timeScaleModifier;
        }

        public void FreezeTime()
        {
            isFrozen = true;
            Time.timeScale = 0;
        }

        public void DefrostTime()
        {
            isFrozen = false;
            Time.timeScale = timeScaleModifier;
        }

        public void DefrostTime(int n)
        {
            isFrozen = false;
            Time.timeScale = n;
        }

        public void Apply()
        {
            if (isFrozen)
                return;

            Time.timeScale = timeScaleModifier;
        }

        public void LockTime()
        {
            if (isFrozen)
                return;

            Time.timeScale = 0;
        }

        public TimeManager ScaleTime(int n)
        {
            timeScaleModifier = n;
            return this;
        }

        public TimeManager ScaleTimeBy(int k)
        {
            if (k >= 0)
            {
                timeScaleModifier *= k;
            }
            else
            {
                timeScaleModifier /= k;
            }

            return this;
        }
    }
}