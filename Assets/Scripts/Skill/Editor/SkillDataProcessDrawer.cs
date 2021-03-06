#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

[CustomPropertyDrawer(typeof(SkillData.SkillDataProcess))]
[CanEditMultipleObjects]
public class SkillDataProcessDrawer : PropertyDrawer
{

    SerializedProperty castEffectDataProp;
    SerializedProperty typeUsedBulletDataProp;
    SerializedProperty bulletDataProp;
    SerializedProperty bulletTargetDataProp;

    SerializedProperty typeUsedIncreaseHealthProp;
    SerializedProperty increaseNowHealthTargetDataProp;
    SerializedProperty increaseNowHealthValueProp;
    SerializedProperty increaseNowHealthEffectDataProp;

    SerializedProperty typeUsedDecreaseHealthProp;
    SerializedProperty decreaseNowHealthTargetDataProp;
    SerializedProperty decreaseNowHealthValueProp;
    SerializedProperty decreaseNowHealthEffectDataProp;

    SerializedProperty typeUsedStatusDataProp;
    SerializedProperty statusTargetDataProp;
    SerializedProperty statusDataProp;

    SerializedProperty typeUsedUnitDataProp;
    SerializedProperty unitTargetDataProp;
    SerializedProperty unitDataProp;


    StatusDataDrawer statusDataDrawer = new StatusDataDrawer();

    SerializedProperty _statusDataProperty;
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {

        var nowProperty = property.FindPropertyRelative("_statusData");
        if (_statusDataProperty != nowProperty)
        {
            if (nowProperty.objectReferenceValue != null)
            {
                var statusData = nowProperty.objectReferenceValue as StatusData;
                statusDataDrawer.Initialize(new SerializedObject(statusData), statusData);
                _statusDataProperty = nowProperty;
            }
            else
            {
                statusDataDrawer.Clear();
            }
        }

        castEffectDataProp = property.FindPropertyRelative("_castEffectData");

        typeUsedBulletDataProp = property.FindPropertyRelative("_typeUsedBulletData");
        bulletDataProp = property.FindPropertyRelative("_bulletData");
        bulletTargetDataProp = property.FindPropertyRelative("_bulletTargetData");

        typeUsedIncreaseHealthProp = property.FindPropertyRelative("_typeUsedIncreaseHealth");
        increaseNowHealthTargetDataProp = property.FindPropertyRelative("_increaseNowHealthTargetData");
        increaseNowHealthValueProp = property.FindPropertyRelative("_increaseNowHealthValue");
        increaseNowHealthEffectDataProp = property.FindPropertyRelative("_increaseNowHealthEffectData");

        typeUsedDecreaseHealthProp = property.FindPropertyRelative("_typeUsedDecreaseHealth");
        decreaseNowHealthTargetDataProp = property.FindPropertyRelative("_decreaseNowHealthTargetData");
        decreaseNowHealthValueProp = property.FindPropertyRelative("_decreaseNowHealthValue");
        decreaseNowHealthEffectDataProp = property.FindPropertyRelative("_decreaseNowHealthEffectData");


        typeUsedStatusDataProp = property.FindPropertyRelative("_typeUsedStatusData");
        statusTargetDataProp = property.FindPropertyRelative("_statusTargetData");
        statusDataProp = property.FindPropertyRelative("_statusData");

        typeUsedUnitDataProp = property.FindPropertyRelative("_typeUsedUnitData");
        unitTargetDataProp = property.FindPropertyRelative("_unitTargetData");
        unitDataProp = property.FindPropertyRelative("_unitData");


        var totalHeight = 0f;
        totalHeight += EditorGUI.GetPropertyHeight(castEffectDataProp, true);

        totalHeight += EditorGUI.GetPropertyHeight(typeUsedBulletDataProp, true);
        if (typeUsedBulletDataProp.boolValue)
        {
            totalHeight += EditorGUI.GetPropertyHeight(bulletDataProp, true);
            totalHeight += EditorGUI.GetPropertyHeight(bulletTargetDataProp, true);
        }

        totalHeight += EditorGUI.GetPropertyHeight(typeUsedIncreaseHealthProp, true);
        if (typeUsedIncreaseHealthProp.boolValue)
        {
            totalHeight += EditorGUI.GetPropertyHeight(increaseNowHealthTargetDataProp, true);
            totalHeight += EditorGUI.GetPropertyHeight(increaseNowHealthValueProp, true);
            totalHeight += EditorGUI.GetPropertyHeight(increaseNowHealthEffectDataProp, true);
        }

        totalHeight += EditorGUI.GetPropertyHeight(typeUsedDecreaseHealthProp, true);
        if (typeUsedDecreaseHealthProp.boolValue)
        {
            totalHeight += EditorGUI.GetPropertyHeight(decreaseNowHealthTargetDataProp, true);
            totalHeight += EditorGUI.GetPropertyHeight(decreaseNowHealthValueProp, true);
            totalHeight += EditorGUI.GetPropertyHeight(decreaseNowHealthEffectDataProp, true);
        }

        totalHeight += EditorGUI.GetPropertyHeight(typeUsedStatusDataProp, true);
        if (typeUsedStatusDataProp.boolValue)
        {
            totalHeight += EditorGUI.GetPropertyHeight(statusTargetDataProp, true);
            totalHeight += EditorGUI.GetPropertyHeight(statusDataProp, true);
        }

        totalHeight += EditorGUI.GetPropertyHeight(typeUsedUnitDataProp, true);
        if (typeUsedUnitDataProp.boolValue)
        {
            totalHeight += EditorGUI.GetPropertyHeight(unitTargetDataProp, true);
            totalHeight += EditorGUI.GetPropertyHeight(unitDataProp, true);
        }

        return totalHeight;
    }


    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {      

        EditorGUI.BeginProperty(position, label, property);

        position.height = EditorGUIUtility.singleLineHeight;

        EditorGUI.PropertyField(position, castEffectDataProp, true);
        position = PropertyDrawerExtend.AddAxisY(position, EditorGUI.GetPropertyHeight(castEffectDataProp));

        EditorGUI.indentLevel++;

        EditorGUI.PropertyField(position, typeUsedBulletDataProp, true);
        position = PropertyDrawerExtend.AddAxisY(position, EditorGUI.GetPropertyHeight(typeUsedBulletDataProp));

        if (typeUsedBulletDataProp.boolValue)
        {
            EditorGUI.indentLevel++;

            EditorGUI.PropertyField(position, bulletDataProp, true);
            position = PropertyDrawerExtend.AddAxisY(position, EditorGUI.GetPropertyHeight(bulletDataProp));

            EditorGUI.PropertyField(position, bulletTargetDataProp, true);
            position = PropertyDrawerExtend.AddAxisY(position, EditorGUI.GetPropertyHeight(bulletTargetDataProp));

            EditorGUI.indentLevel--;

        }


        EditorGUI.PropertyField(position, typeUsedIncreaseHealthProp, true);
        position = PropertyDrawerExtend.AddAxisY(position, EditorGUI.GetPropertyHeight(typeUsedIncreaseHealthProp));


        if (typeUsedIncreaseHealthProp.boolValue)
        {
            EditorGUI.indentLevel++;

            EditorGUI.PropertyField(position, increaseNowHealthValueProp, true);
            position = PropertyDrawerExtend.AddAxisY(position, EditorGUI.GetPropertyHeight(increaseNowHealthValueProp));


            EditorGUI.PropertyField(position, increaseNowHealthTargetDataProp, true);
            position = PropertyDrawerExtend.AddAxisY(position, EditorGUI.GetPropertyHeight(increaseNowHealthTargetDataProp));

            EditorGUI.PropertyField(position, increaseNowHealthEffectDataProp, true);
            position = PropertyDrawerExtend.AddAxisY(position, EditorGUI.GetPropertyHeight(increaseNowHealthEffectDataProp));

            EditorGUI.indentLevel--;
        }

        EditorGUI.PropertyField(position, typeUsedDecreaseHealthProp, true);
        position = PropertyDrawerExtend.AddAxisY(position, EditorGUI.GetPropertyHeight(typeUsedDecreaseHealthProp));


        if (typeUsedDecreaseHealthProp.boolValue)
        {
            EditorGUI.indentLevel++;

            EditorGUI.PropertyField(position, decreaseNowHealthValueProp, true);
            position = PropertyDrawerExtend.AddAxisY(position, EditorGUI.GetPropertyHeight(decreaseNowHealthValueProp));


            EditorGUI.PropertyField(position, decreaseNowHealthTargetDataProp, true);
            position = PropertyDrawerExtend.AddAxisY(position, EditorGUI.GetPropertyHeight(decreaseNowHealthTargetDataProp));

            EditorGUI.PropertyField(position, decreaseNowHealthEffectDataProp, true);
            position = PropertyDrawerExtend.AddAxisY(position, EditorGUI.GetPropertyHeight(decreaseNowHealthEffectDataProp));

            EditorGUI.indentLevel--;
        }

        EditorGUI.PropertyField(position, typeUsedStatusDataProp, true);
        position = PropertyDrawerExtend.AddAxisY(position, EditorGUI.GetPropertyHeight(typeUsedStatusDataProp));

        if (typeUsedStatusDataProp.boolValue)
        {
            EditorGUI.indentLevel++;

            EditorGUI.PropertyField(position, statusDataProp, true);
            position = PropertyDrawerExtend.AddAxisY(position, EditorGUI.GetPropertyHeight(statusDataProp));

            EditorGUI.PropertyField(position, statusTargetDataProp, true);
            position = PropertyDrawerExtend.AddAxisY(position, EditorGUI.GetPropertyHeight(statusTargetDataProp));

            EditorGUI.indentLevel--;
        }


        EditorGUI.PropertyField(position, typeUsedUnitDataProp, true);
        position = PropertyDrawerExtend.AddAxisY(position, EditorGUI.GetPropertyHeight(typeUsedStatusDataProp));

        if (typeUsedUnitDataProp.boolValue)
        {
            EditorGUI.indentLevel++;

            EditorGUI.PropertyField(position, unitTargetDataProp, true);
            position = PropertyDrawerExtend.AddAxisY(position, EditorGUI.GetPropertyHeight(unitTargetDataProp));

            EditorGUI.PropertyField(position, unitDataProp, true);
            //position = PropertyDrawerExtend.AddAxisY(position, EditorGUI.GetPropertyHeight(statusTargetDataProp));

            EditorGUI.indentLevel--;
        }

        EditorGUI.indentLevel--;
        EditorGUI.EndProperty();
    }

}

#endif