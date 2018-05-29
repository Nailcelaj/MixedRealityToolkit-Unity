﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.﻿

using Microsoft.MixedReality.Toolkit.Internal.Definitions;
using Microsoft.MixedReality.Toolkit.Internal.Extensions.EditorClassExtensions;
using Microsoft.MixedReality.Toolkit.Internal.Managers;
using UnityEditor;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Inspectors
{
    [CustomEditor(typeof(MixedRealityConfigurationProfile))]
    public class MixedRealityConfigurationProfileInspector : MixedRealityBaseConfigurationProfileInspector
    {
        private static readonly GUIContent NewProfileContent = new GUIContent("+", "Create New Profile");

        private SerializedProperty enableInputSystem;
        private SerializedProperty inputSystemType;
        private SerializedProperty inputActionsProfile;
        private SerializedProperty enableSpeechCommands;
        private SerializedProperty speechCommandsProfile;
        private SerializedProperty renderMotionControllers;
        private SerializedProperty leftControllerModel;
        private SerializedProperty rightControllerModel;
        private SerializedProperty controllerMappingsProfile;
        private SerializedProperty enableBoundarySystem;

        private void OnEnable()
        {
            // Create The MR Manager if none exists.
            if (!MixedRealityManager.IsInitialized)
            {
                // Search the scene for one, in case we've just hot reloaded the assembly.
                var managerSearch = FindObjectsOfType<MixedRealityManager>();

                if (managerSearch.Length == 0)
                {
                    if (EditorUtility.DisplayDialog("Attention!",
                        "There is no active Mixed Reality Manager in your scene. Would you like to create one now?", "Yes",
                        "Later"))
                    {
                        MixedRealityManager.Instance.ActiveProfile = (MixedRealityConfigurationProfile)target;
                    }
                    else
                    {
                        Debug.LogWarning("No Mixed Reality Manager in your scene.");
                    }
                }
                else
                {
                    MixedRealityManager.ConfirmInitialized();
                }
            }

            enableInputSystem = serializedObject.FindProperty("enableInputSystem");
            inputSystemType = serializedObject.FindProperty("inputSystemType");
            inputActionsProfile = serializedObject.FindProperty("inputActionsProfile");
            enableSpeechCommands = serializedObject.FindProperty("enableSpeechCommands");
            speechCommandsProfile = serializedObject.FindProperty("speechCommandsProfile");
            renderMotionControllers = serializedObject.FindProperty("renderMotionControllers");
            leftControllerModel = serializedObject.FindProperty("leftControllerModel");
            rightControllerModel = serializedObject.FindProperty("rightControllerModel");
            controllerMappingsProfile = serializedObject.FindProperty("controllerMappingsProfile");
            enableBoundarySystem = serializedObject.FindProperty("enableBoundarySystem");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            RenderMixedRealityToolkitLogo();

            EditorGUILayout.LabelField("Input Settings", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(enableInputSystem);

            if (enableInputSystem.boolValue)
            {
                EditorGUILayout.PropertyField(inputSystemType);
                RenderProfile(inputActionsProfile);

                EditorGUILayout.PropertyField(enableSpeechCommands);
                if (enableSpeechCommands.boolValue)
                {
                    RenderProfile(speechCommandsProfile);
                }
            }

            EditorGUILayout.LabelField("Device Settings", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(controllerMappingsProfile);
            EditorGUILayout.PropertyField(controllerMappingsProfile.FindPropertyRelative("Array.size"));
            for (int i = 0; i < controllerMappingsProfile.arraySize; i++)
            {
                EditorGUILayout.PropertyField(controllerMappingsProfile.GetArrayElementAtIndex(i));
            }
            EditorGUILayout.PropertyField(renderMotionControllers);
            if (renderMotionControllers.boolValue)
            {
                EditorGUILayout.PropertyField(leftControllerModel);
                EditorGUILayout.PropertyField(rightControllerModel);
            }

            EditorGUILayout.LabelField("Boundary Settings", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(enableBoundarySystem);

            serializedObject.ApplyModifiedProperties();
        }

        private static void RenderProfile(SerializedProperty property)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PropertyField(property);

            if (property.objectReferenceValue == null)
            {
                if (GUILayout.Button(NewProfileContent, EditorStyles.miniButton))
                {
                    var profileTypeName = property.type.Replace("PPtr<$", string.Empty).Replace(">", string.Empty);
                    Debug.Assert(profileTypeName != null, "No Type Found");
                    ScriptableObject profile = CreateInstance(profileTypeName);
                    profile.CreateAsset();
                    property.objectReferenceValue = profile;
                }
            }

            EditorGUILayout.EndHorizontal();
        }
    }
}