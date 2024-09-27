using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace SpaceBaboon
{
    public class StatInstance
    {
        private ScriptableObject m_scriptableObject;
        private FieldInfo m_item;

        private FloatField m_value;


        public void Create(VisualElement clone, FieldInfo item, ScriptableObject scriptableObject)
        {
            m_scriptableObject = scriptableObject;
            m_item = item;
            
            Label name = clone.Q<Label>("Name");
            name.text = item.Name;

            m_value = clone.Q<FloatField>("Value");
            m_value.value = (float)item.GetValue(m_scriptableObject);
            m_value.RegisterValueChangedCallback(OnCurrentValueChanged);

        }

        private void OnCurrentValueChanged(ChangeEvent<float> evt)
        {
            m_item.SetValue(m_scriptableObject, evt.newValue);
            EditorUtility.SetDirty(m_scriptableObject);
        }
    }
}
