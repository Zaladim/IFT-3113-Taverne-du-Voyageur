using System;
using UnityEngine;

namespace Managers
{
    public class TimeManager : MonoBehaviour
    {
        [Header("Options")] [Range(0, 0)] private const int MINTimeScaleModifier = 0;
        private const int MAXTimeScaleModifier = 10;

        [Header("Debug (ReadOnly)")] [SerializeField] [Range(MINTimeScaleModifier, MAXTimeScaleModifier)]
        private float timeScaleModifier = 1;

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
            timeScaleModifier += n;


            if (timeScaleModifier > MAXTimeScaleModifier)
            {
                timeScaleModifier = MAXTimeScaleModifier;
            }
            else if (timeScaleModifier < MINTimeScaleModifier)
            {
                timeScaleModifier = MINTimeScaleModifier;
            }

            return this;
        }

        public TimeManager SetTimeScale(int n)
        {
            timeScaleModifier = n;


            if (timeScaleModifier > MAXTimeScaleModifier)
            {
                timeScaleModifier = MAXTimeScaleModifier;
            }
            else if (timeScaleModifier < MINTimeScaleModifier)
            {
                timeScaleModifier = MINTimeScaleModifier;
            }

            return this;
        }

        public TimeManager ScaleTimeBy(int k)
        {
            if (k >= 0)
            {
                timeScaleModifier *= k;
                if (timeScaleModifier > MAXTimeScaleModifier)
                    timeScaleModifier = MAXTimeScaleModifier;
            }
            else
            {
                timeScaleModifier /= Math.Abs(k);
                if (timeScaleModifier < MINTimeScaleModifier)
                    timeScaleModifier = MINTimeScaleModifier;
            }

            return this;
        }
    }
}