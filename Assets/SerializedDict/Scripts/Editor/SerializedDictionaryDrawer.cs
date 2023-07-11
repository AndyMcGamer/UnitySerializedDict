using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using System;
using System.Linq;

namespace AndyL.Serialization
{
    public class SerializedDictionaryDrawer<TKey, TValue> : PropertyDrawer
    {
        private UnityEngine.Object target;
        private List<TKey> keys;

        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            var root = new VisualElement();

            SerializedProperty keyProperty = property.FindPropertyRelative("keyData").Copy();
            SerializedProperty valueProperty = property.FindPropertyRelative("valueData").Copy();

            keys = keyProperty.GetSerializedValue<List<TKey>>();

            SerializedProperty iterator = property.Copy();
            iterator.Next(true);
            do
            {
                if (iterator.name == "size")
                {
                    break;
                }
            } while (iterator.Next(true));

            PropertyField hiddenSize = new(iterator);
            hiddenSize.style.display = DisplayStyle.None;

            hiddenSize.RegisterValueChangeCallback(e =>
            {
                target = Selection.activeObject;
                Selection.activeObject = null;
                EditorApplication.delayCall += OnDelayedCall;
            });
            root.Add(hiddenSize);


            Func<VisualElement> makeKey = () =>
            {
                var container = new VisualElement();
                container.style.paddingLeft = 10;
                return container;
            };

            List<VisualElement> keysAndValues = new();

            for (int i = 0; i < keys.Count; i++)
            {

                SerializedProperty key = keyProperty.Copy();
                key.Next(true);
                key = key.GetArrayElementAtIndex(i);

                SerializedProperty value = valueProperty.Copy();
                value.Next(true);
                value = value.GetArrayElementAtIndex(i);

                VisualElement keyValuePair = new();
                var keyPropertyField = new PropertyField(key);
                keyPropertyField.BindProperty(key);
                keyPropertyField.label = "Key";
                keyPropertyField.name = "Key";
                keyPropertyField.style.flexGrow = 1;

                var valuePropertyField = new PropertyField(value);
                valuePropertyField.BindProperty(value);
                valuePropertyField.label = "Value";
                valuePropertyField.name = "Value";
                valuePropertyField.style.flexGrow = 1;

                keyValuePair.Add(keyPropertyField);
                keyValuePair.Add(valuePropertyField);

                keysAndValues.Add(keyValuePair);


            }



            Action<VisualElement, int> bindKey = (e, i) =>
            {
                e.Add(keysAndValues[i]);
            };

            ListView keyListView = new()
            {
                itemsSource = keysAndValues,
                makeItem = makeKey,
                bindItem = bindKey,
                virtualizationMethod = CollectionVirtualizationMethod.DynamicHeight,
                selectionType = (SelectionType)property.FindPropertyRelative("selectionType").enumValueIndex,
                showFoldoutHeader = true,
                headerTitle = property.displayName,
                showBorder = true,
                showBoundCollectionSize = !property.FindPropertyRelative("isReadonly").boolValue,
                showAddRemoveFooter = !property.FindPropertyRelative("isReadonly").boolValue,
                reorderable = true,
                reorderMode = ListViewReorderMode.Animated,
            };

            keyListView.itemsRemoved += (items) =>
            {
                foreach (var item in items)
                {
                    property.GetSerializedValue<Dictionary<TKey, TValue>>().Remove(keys[item]);
                }
            };

            if (keyListView.showAddRemoveFooter)
            {
                keyListView.Q<Button>("unity-list-view__add-button").clicked += (() =>
                {
                    TKey newKey = default;
                    TValue newValue = default;
                    
                    if (newKey is TKey && !property.GetSerializedValue<Dictionary<TKey, TValue>>().ContainsKey(newKey))
                    {
                        property.GetSerializedValue<Dictionary<TKey, TValue>>().Add(newKey, newValue);
                    }
                    else
                    {
                        target = Selection.activeObject;
                        Selection.activeObject = null;
                        EditorApplication.delayCall += OnDelayedCall;
                    }

                });
            }




            root.Add(keyListView);
            return root;
        }

        private void OnDelayedCall()
        {
            EditorApplication.update -= OnDelayedCall;
            Selection.activeObject = target;
        }
    }

    #region Specific Types

    [CustomPropertyDrawer(typeof(SerializedDictionary<string, SerializedList<string>>))]
    public class SerializedDictionaryDrawer_string_stringList : SerializedDictionaryDrawer<string, SerializedList<string>> { }

    [CustomPropertyDrawer(typeof(SerializedDictionary<int, SerializedList<string>>))]
    public class SerializedDictionaryDrawer_int_stringList : SerializedDictionaryDrawer<int, SerializedList<string>> { }

    [CustomPropertyDrawer(typeof(SerializedDictionary<float, SerializedList<string>>))]
    public class SerializedDictionaryDrawer_float_stringList : SerializedDictionaryDrawer<float, SerializedList<string>> { }

    [CustomPropertyDrawer(typeof(SerializedDictionary<string, string>))]
    public class SerializedDictionaryDrawer_string_string : SerializedDictionaryDrawer<string, string> { }

    [CustomPropertyDrawer(typeof(SerializedDictionary<int, string>))]
    public class SerializedDictionaryDrawer_int_string : SerializedDictionaryDrawer<int, string> { }

    [CustomPropertyDrawer(typeof(SerializedDictionary<float, string>))]
    public class SerializedDictionaryDrawer_float_string : SerializedDictionaryDrawer<float, string> { }

    [CustomPropertyDrawer(typeof(SerializedDictionary<int, SerializedList<int>>))]
    public class SerializedDictionaryDrawer_int_intList : SerializedDictionaryDrawer<int, SerializedList<int>> { }

    [CustomPropertyDrawer(typeof(SerializedDictionary<float, SerializedList<int>>))]
    public class SerializedDictionaryDrawer_float_intList : SerializedDictionaryDrawer<float, SerializedList<int>> { }

    [CustomPropertyDrawer(typeof(SerializedDictionary<string, SerializedList<int>>))]
    public class SerializedDictionaryDrawer_string_intList : SerializedDictionaryDrawer<string, SerializedList<int>> { }

    [CustomPropertyDrawer(typeof(SerializedDictionary<string, SerializedList<float>>))]
    public class SerializedDictionaryDrawer_string_floatList : SerializedDictionaryDrawer<string, SerializedList<float>> { }

    [CustomPropertyDrawer(typeof(SerializedDictionary<string, int>))]
    public class SerializedDictionaryDrawer_string_int : SerializedDictionaryDrawer<string, int> { }

    [CustomPropertyDrawer(typeof(SerializedDictionary<string, float>))]
    public class SerializedDictionaryDrawer_string_float : SerializedDictionaryDrawer<string, float> { }

    #endregion

}
