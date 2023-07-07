using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AndyL.Serialization.Examples
{
    [ExecuteInEditMode]
    public class DictionaryExample : MonoBehaviour
    {
        [SerializeField]
        private SerializedDictionary<string, string> dictionaryStringString;

        [SerializeField]
        private SerializedDictionary<string, SerializedList<string>> dictionaryStringStringList;

        [SerializeField]
        private SerializedDictionary<int, string> dictionaryIntString;

        [SerializeField]
        private SerializedDictionary<string, SerializedList<int>> dictionaryStringIntList;

        private void Awake()
        {
            dictionaryStringString = new() { {"stringkey", "stringvalue" }, {"stringkey2", "stringvalue2" }  };
            dictionaryStringStringList = new() { { "stringkey", new SerializedList<string>() { "value1", "value2" } }, {"stringkey2", new SerializedList<string>() { "value1", "value2", "value3" } } };
            dictionaryIntString = new() { { 10, "stringValue" }, { -1, "value2" }, { 0, "value3" } };
            dictionaryStringIntList = new() { {"stringkey", new SerializedList<int>() { 0, 1, 9, 5} } };

            dictionaryStringString.selectionType = UnityEngine.UIElements.SelectionType.Single;
            dictionaryStringStringList.selectionType = UnityEngine.UIElements.SelectionType.Multiple;
        }
    }
}
