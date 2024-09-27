using UnityEngine;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using System.Reflection;
using System.Collections.Generic;
using System;

namespace SpaceBaboon
{
    /*[CustomEditor(typeof(TestPlayer))]
    public class PlayerStatsEditor : Editor
    {
        private float m_modifier = 0.0f;


        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            GUIStyle nameStyle = new GUIStyle();
            nameStyle.richText = true;
            nameStyle.normal.textColor = Color.white;
            nameStyle.fontSize = 14;
            nameStyle.fontStyle = FontStyle.Bold;



            TestPlayer player = (TestPlayer)target;

            var playerData = player.GetPlayerData();
            var fields = typeof(PlayerData).GetFields();
            //var fields = GetAllFields(playerData.GetType());

            float inspectorWidth = EditorGUIUtility.currentViewWidth;
            float halfWidth = inspectorWidth * 0.5f;

            foreach (var item in fields)
            {
                if (item.FieldType != typeof(float))
                {
                    continue;
                }
                

                var floatValue = (float)item.GetValue(playerData);

                GUILayout.Label(item.Name, nameStyle);
                GUILayout.Space(5f);

                GUILayout.BeginHorizontal();

                    GUILayout.Label("Value: " + floatValue, GUILayout.MaxWidth(halfWidth));
                    m_modifier = EditorGUILayout.FloatField("Modifier", m_modifier, GUILayout.MaxWidth(halfWidth));

                GUILayout.EndHorizontal();
                //GUILayout.Space(10f);

                GUILayout.BeginHorizontal();

                    var buttonUP = GUILayout.Button("UP", GUILayout.MaxWidth(halfWidth));
                    if (buttonUP)
                    {
                        item.SetValue(playerData, floatValue + m_modifier);
                    }

                    var buttonDOWN = GUILayout.Button("DOWN", GUILayout.MaxWidth(halfWidth));
                    if (buttonDOWN)
                    {
                        item.SetValue(playerData, floatValue - m_modifier);
                    }

                GUILayout.EndHorizontal();
            }
        }
    }*/
}
