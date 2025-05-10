using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using Spine;

[CustomEditor(typeof(PlayerManager))]
public class PlayerManagerEditor : Editor
{
    private PlayerManager _targetScript;
    private int _selectedAnimationIndex = 0;
    private int _selectedBoneIndex = 0;
    private List<string> _animationNames = new List<string>();
    private List<string> _boneNames = new List<string>();
    private SerializedProperty _selectedAnimationNameProp;
    private SerializedProperty _selectedBoneNameProp;
    private SerializedProperty _selectedBoneIndexProp;

    private void OnEnable()
    {
        _targetScript = (PlayerManager)target;
        _selectedAnimationNameProp = serializedObject.FindProperty("_selectedAnimationName");
        _selectedBoneNameProp = serializedObject.FindProperty("_selectedBoneName");
        _selectedBoneIndexProp = serializedObject.FindProperty("_selectedBoneIndex");

        // 애니메이션과 본 목록 업데이트
        UpdateAnimationList();
        UpdateBonesList();

        // 현재 선택된 애니메이션 인덱스 가져오기
        _selectedAnimationIndex = _targetScript.GetSelectedAnimationIndex();

        // 현재 선택된 본 인덱스 가져오기
        if (_targetScript.SelectedBoneName != null && _boneNames.Count > 0)
        {
            _selectedBoneIndex = _boneNames.IndexOf(_targetScript.SelectedBoneName);
            if (_selectedBoneIndex < 0) _selectedBoneIndex = 0;
        }
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        DrawPropertiesExcluding(serializedObject, "_selectedAnimationName", "_selectedAnimationIndex", "_selectedBoneName", "_selectedBoneIndex");

        // 애니메이션/본 목록이 비어있을 때 새로고침 버튼
        if (_animationNames.Count == 0 || _boneNames.Count == 0)
        {
            if (GUILayout.Button("애니메이션/본 목록 새로고침"))
            {
                UpdateAnimationList();
                UpdateBonesList();
            }

            if (_animationNames.Count == 0)
                EditorGUILayout.HelpBox("애니메이션 목록을 불러올 수 없습니다.", MessageType.Warning);

            if (_boneNames.Count == 0)
                EditorGUILayout.HelpBox("본 목록을 불러올 수 없습니다.", MessageType.Warning);
        }

        // 애니메이션 선택 드롭다운
        EditorGUI.BeginDisabledGroup(_animationNames.Count == 0);
        EditorGUI.BeginChangeCheck();
        _selectedAnimationIndex = EditorGUILayout.Popup("애니메이션 선택", _selectedAnimationIndex, _animationNames.ToArray());
        if (EditorGUI.EndChangeCheck() && _selectedAnimationIndex >= 0 && _selectedAnimationIndex < _animationNames.Count)
        {
            Undo.RecordObject(_targetScript, "애니메이션 변경");
            _targetScript.ChangeAnimation(_selectedAnimationIndex);
            EditorUtility.SetDirty(_targetScript);
        }
        EditorGUI.EndDisabledGroup();

        // 본 선택 드롭다운
        EditorGUI.BeginDisabledGroup(_boneNames.Count == 0);
        EditorGUI.BeginChangeCheck();
        _selectedBoneIndex = EditorGUILayout.Popup("본 선택", _selectedBoneIndex, _boneNames.ToArray());
        if (EditorGUI.EndChangeCheck() && _selectedBoneIndex >= 0 && _selectedBoneIndex < _boneNames.Count)
        {
            Undo.RecordObject(_targetScript, "본 선택 변경");
            
            // PlayerManager에 값 직접 설정
            _targetScript.SelectedBoneName = _boneNames[_selectedBoneIndex];
            _targetScript.SelectedBoneIndex = _selectedBoneIndex;
            
            // SerializedProperty 업데이트
            _selectedBoneNameProp.stringValue = _boneNames[_selectedBoneIndex];
            _selectedBoneIndexProp.intValue = _selectedBoneIndex;
            
            EditorUtility.SetDirty(_targetScript);
        }
        EditorGUI.EndDisabledGroup();

        // 테스트 버튼
        if (GUILayout.Button("선택한 애니메이션 재생"))
        {
            if (_targetScript.skeletonAnimation != null && _selectedAnimationIndex < _animationNames.Count)
                _targetScript.SetAnimation(_animationNames[_selectedAnimationIndex]);
        }

        serializedObject.ApplyModifiedProperties();
    }

    private void UpdateAnimationList()
    {
        _animationNames.Clear();
        if (_targetScript != null)
            _targetScript.GetAnimationNames(_animationNames);
    }

    private void UpdateBonesList()
    {
        _boneNames.Clear();
        if (_targetScript != null)
            _targetScript.GetBoneNames(_boneNames);
    }
}