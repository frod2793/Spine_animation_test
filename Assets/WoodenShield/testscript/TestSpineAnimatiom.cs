using System.Collections.Generic;
using Spine.Unity;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TestSpineAnimatiom : MonoBehaviour
{
    #region 변수

    [SpineAnimation] public string testamAnimation;

    public SpineAnimation TestamSpineAnimation;
    [SerializeField] SkeletonAnimation testamSkeletonAnimation;

    [SerializeField] private Button testamButton;

    [SerializeField] private TMP_Dropdown testamDropdown;

    [SerializeField] private Toggle isRoopToggle;

    #endregion

    #region Unity 라이프 사이클
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        testamButton.onClick.AddListener(Testanimation);

        // Set the animation to play
        testamSkeletonAnimation.AnimationState.SetAnimation(0, testamAnimation, false);
        AddDropdownOptions();
    }

    #endregion

    #region Dropdown 
    private void AddDropdownOptions()
    {
        // 기존 옵션 초기화
        testamDropdown.ClearOptions();

        // 스켈레톤 애니메이션 컴포넌트 확인
        if (testamSkeletonAnimation == null || testamSkeletonAnimation.skeleton == null ||
            testamSkeletonAnimation.skeleton.Data == null)
        {
            Debug.LogError("스켈레톤 애니메이션이 없거나 데이터가 없습니다");
            return;
        }

        // 스켈레톤 데이터에서 모든 애니메이션 이름 가져오기
        var animations = testamSkeletonAnimation.skeleton.Data.Animations;
        List<TMP_Dropdown.OptionData> options = new List<TMP_Dropdown.OptionData>();

        // 각 애니메이션 이름을 옵션으로 추가
        foreach (var animation in animations)
        {
            options.Add(new TMP_Dropdown.OptionData(text: animation.Name));
        }

        // 드롭다운에 옵션 추가
        testamDropdown.AddOptions(options);

        // 값 변경 시 이벤트 리스너 추가
        testamDropdown.onValueChanged.AddListener(OnDropdownValueChanged);
    }

    private void OnDropdownValueChanged(int index)
    {
        testamAnimation = testamDropdown.options[index].text;
        testamSkeletonAnimation.AnimationState.SetAnimation(0, testamAnimation, isRoopToggle);
    }
    #endregion

    #region Animation 메소드
    private void Testanimation()
    {
        testamSkeletonAnimation.AnimationState.SetAnimation(0, testamAnimation, isRoopToggle);
    }
    #endregion
}