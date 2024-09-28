using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace Universal.UnityEvents
{
    public class EventDelayer : MonoBehaviour
    {
        //[Header("Set Values")]
        [SerializeField, Min(0)] float delay;
        [SerializeField] UnityEvent eventToDelay;
        //[Header("Runtime Values")]
        Coroutine eventDelay;

        //Unity Events

        //Methods
        public void StartEvent()
        {
            if (eventDelay == null)
                eventDelay = StartCoroutine(DelayEvent());
        }
        IEnumerator DelayEvent()
        {
            yield return new WaitForSecondsRealtime(delay);
            eventToDelay.Invoke();
            eventDelay = null;
        }
    }
}
