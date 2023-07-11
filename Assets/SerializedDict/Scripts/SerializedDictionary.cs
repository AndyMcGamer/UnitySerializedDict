using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace AndyL.Serialization
{
    [Serializable]
    public class SerializedDictionary<TKey, TValue> : Dictionary<TKey, TValue>, ISerializationCallbackReceiver
    {
        [SerializeField]
        private List<TKey> keyData = new();

        [SerializeField]
        private List<TValue> valueData = new();

        // Property Drawer Vars
        public SelectionType selectionType = SelectionType.None;
        public bool isReadonly = false;

        public void OnAfterDeserialize()
        {
            Clear();
            for (int i = 0; i < keyData.Count && i < valueData.Count; i++)
            {
                this[keyData[i]] = valueData[i];
            }
        }

        public void OnBeforeSerialize()
        {
            keyData.Clear();
            valueData.Clear();

            foreach (var item in this)
            {
                keyData.Add(item.Key);
                valueData.Add(item.Value);
            }
        }
    }

    [Serializable]
    public class SerializedList<T> 
    {
        public List<T> list = new();
    }
}
