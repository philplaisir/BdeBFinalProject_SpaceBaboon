using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using System.Reflection;
using System;

namespace SpaceBaboon
{
    [CustomEditor(typeof(BaseStats<>), true)]
    [CanEditMultipleObjects]
    public class StatsEditor : Editor
    {
        [SerializeField] private VisualTreeAsset m_floatTemplate;
        [SerializeField] private VisualTreeAsset m_header;



        public override VisualElement CreateInspectorGUI()
        {
            VisualElement root = new VisualElement();
            root.Add(new IMGUIContainer(OnInspectorGUI));

            var feature = target as IStatsEditable;
            var so = feature.GetData();


            if (so is ScriptableObject scriptableObject)
            {
                VisualElement header = m_header.CloneTree();
                root.Add(header);

                Label title = header.Q<Label>("Title");
                title.text = scriptableObject.name + " - Floats";

                var fields = GetFields(scriptableObject);

                foreach (var item in fields)
                {
                    if (item.FieldType == typeof(float))
                    {
                        //Instead of 
                        //m_tree.CloneTree(root);
                        VisualElement clone = m_floatTemplate.CloneTree();
                        root.Add(clone);

                        StatInstance inst = new StatInstance();
                        inst.Create(clone, item, scriptableObject);

                        //Debug.Log(item.Name);
                    }
                }
            }




            return root;
        }

        private FieldInfo[] GetFields(ScriptableObject so)
        {
            return so.GetType().GetFields();
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            //DrawDefaultInspector();   //un ou l'autre ?


            //var feature = target as IStatsEditable;
            //var so = feature.GetData();
            //
            //
            //if (so is ScriptableObject scriptableObject)
            //{
            //    var fields = GetFields(scriptableObject);
            //
            //    foreach (var item in fields)
            //    {
            //        if (item.FieldType == typeof(float))
            //        {
            //            GUILayout.Label(item.Name);
            //        }
            //    }
            //}
        }
    }
}
