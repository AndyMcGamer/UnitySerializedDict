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
        UnityEngine.Object target;
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            var root = new VisualElement();

            SerializedProperty keyProperty = property.FindPropertyRelative("keyData").Copy();
            SerializedProperty valueProperty = property.FindPropertyRelative("valueData").Copy();

            var keyData = keyProperty.GetSerializedValue<List<TKey>>();
            var valueData = valueProperty.GetSerializedValue<List<TValue>>();


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

            List<ListView> keysAndValues = new();

            for (int i = 0; i < keyData.Count; i++)
            {
                List<object> values = new();
                if (typeof(TValue).IsGenericType && typeof(TValue).GetGenericTypeDefinition().GetInterfaces().Contains(typeof(IList)))
                {
                    var value = valueData[i] as IList;
                    foreach (var item in value)
                    {
                        values.Add(item);
                    }
                }
                else
                {
                    values.Add(valueData[i]);
                }

                ListView valueListView = new()
                {
                    itemsSource = values.ToList(),
                    makeItem = () =>
                    {
                        var container = new VisualElement();
                        container.Add(new Label());
                        return container;
                    },
                    bindItem = (e, i) =>
                    {
                        e.Q<Label>().text = values[i].ToString();
                    },
                    virtualizationMethod = CollectionVirtualizationMethod.DynamicHeight,
                    selectionType = (SelectionType)property.FindPropertyRelative("selectionType").enumValueIndex,
                    showFoldoutHeader = true,
                    showBoundCollectionSize = property.FindPropertyRelative("showCollectionSize").boolValue,
                    showAlternatingRowBackgrounds = AlternatingRowBackground.All,
                    showBorder = true,
                    headerTitle = keyData[i].ToString(),
                    showAddRemoveFooter = false,
                };
                valueListView.style.paddingLeft = 5;

                keysAndValues.Add(valueListView);
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
                showBoundCollectionSize = property.FindPropertyRelative("showCollectionSize").boolValue,
                showAddRemoveFooter = false,
                reorderable = true,
                reorderMode = ListViewReorderMode.Animated,
            };
            keyListView.style.minHeight = 50;
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
