using System;
using AVG_System.Scripts.Interface;
using UnityEngine;
namespace AVG_System.IngameFunctionPanel.Scripts
{
    public class InGameFunctionPanel : CavansGroupBehaviour
    {
        private void Awake()
        {
            Out();
        }

        [SerializeField] private float fadeTime;
        public void Open()
        {
            FadeIn(fadeTime);
        }
        public void Close()
        {
            FadeOut(fadeTime);
        }
    }
}