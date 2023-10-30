using System;
using System.Collections.Generic;
using UnityEngine;

namespace MatrixSDK
{
    public class MatrixUnityEventDispatcher : MonoBehaviour
    {
        private static MatrixUnityEventDispatcher instance;

        private Dictionary<MatrixUnityEvent, Action<object>> _eventDictionary = new Dictionary<MatrixUnityEvent, Action<object>>();

        public static MatrixUnityEventDispatcher Instance()
        {
            if (instance == null)
                instance = new MatrixUnityEventDispatcher();
            return instance;
        }

        public void AddEventListener(MatrixUnityEvent eventName, Action<object> listener)
        {
            if (_eventDictionary.TryGetValue(eventName, out Action<object> thisEvent))
            {
                thisEvent += listener;
                _eventDictionary[eventName] = thisEvent;
            }
            else
            {
                thisEvent += listener;
                _eventDictionary.Add(eventName, thisEvent);
            }
        }

        public void RemoveEventListener(MatrixUnityEvent eventName, Action<object> listener)
        {
            if (_eventDictionary.TryGetValue(eventName, out Action<object> thisEvent))
            {
                thisEvent -= listener;
                if (thisEvent == null)
                {
                    _eventDictionary.Remove(eventName);
                }
                else
                {
                    _eventDictionary[eventName] = thisEvent;
                }
            }
        }

        public void DispatchEvent(MatrixUnityEvent eventName, object payload = null)
        {
            if (_eventDictionary.TryGetValue(eventName, out Action<object> thisEvent))
            {
                thisEvent.Invoke(payload);
            }
        }

    }
}
   