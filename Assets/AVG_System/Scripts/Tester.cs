using System;
using System.Collections;
using UnityEngine;

namespace AVG_System.Scripts
{
    public class Tester : MonoBehaviour
    {
        [SerializeField] private GameObject delTarget;
        public void ClickToStart()
        {
            StartCoroutine(ieStart());
        }
        private IEnumerator ieStart()
        {
            AVG_System.LoadAndBegin("1",2);
            //Destroy(delTarget);
            yield break;
        }
    }
}
