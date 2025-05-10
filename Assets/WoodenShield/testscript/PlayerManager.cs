using System;
using System.Collections.Generic;
using Spine;
using Spine.Unity;
using UnityEngine;
using UnityEngine.Serialization;

public class PlayerManager : MonoBehaviour
{
     public SkeletonAnimation skeletonAnimation;
    
     [SerializeField] private string selectedAnimationName;
     [SerializeField] private int selectedAnimationIndex;
    public List<string> animationNames = new List<string>();
    
     [SerializeField] private string selectedBoneName;
     [SerializeField] private int selectedBoneIndex;
    public List<string> boneNames = new List<string>();
    
    [Serializable]
    public enum PlayerState
    {
        Idle,
        Walk,
        Run,
    }
    
    [SerializeField] private PlayerState playerState;

    // 애니메이션 프로퍼티
  public string SelectedAnimation
  {
      get => selectedAnimationName;
      set => selectedAnimationName = value;
  }

  public int SelectedAnimationIndex
  {
      get => selectedAnimationIndex;
      set => selectedAnimationIndex = value;
  }
    
    // 본 프로퍼티
    public Bone SelectedBone
    {
        get
        {
            if (skeletonAnimation != null && skeletonAnimation.skeleton != null && 
                !string.IsNullOrEmpty(selectedBoneName))
                return skeletonAnimation.skeleton.FindBone(selectedBoneName);
            return null;
        }
        set
        {
            if (value != null)
                selectedBoneName = value.Data.Name;
        }
    }

   public string SelectedBoneName
   {
       get => selectedBoneName;
       set => selectedBoneName = value;
   }
   
   public int SelectedBoneIndex
   {
       get => selectedBoneIndex;
       set => selectedBoneIndex = value;
   }
    
    private void Awake()
    {
        LoadAnimations();
        LoadBones();
    }
    
    private void OnValidate()
    {
        LoadAnimations();
        LoadBones();
    }

    // 애니메이션 관련 메서드
    private void LoadAnimations()
    {
        if (skeletonAnimation == null || skeletonAnimation.skeleton == null)
            return;
            
        animationNames.Clear();
        foreach (var animation in skeletonAnimation.skeleton.Data.Animations)
        {
            if (animation != null)
                animationNames.Add(animation.Name);
        }
        
        // 애니메이션 선택 처리
        if (animationNames.Count > 0)
        {
            if (string.IsNullOrEmpty(selectedAnimationName) || !animationNames.Contains(selectedAnimationName))
            {
                selectedAnimationName = animationNames[0];
                selectedAnimationIndex = 0;
            }
            else
            {
                selectedAnimationIndex = animationNames.IndexOf(selectedAnimationName);
            }
        }
    }
    
    public void GetAnimationNames(List<string> outList)
    {
        if (skeletonAnimation != null && skeletonAnimation.skeleton != null && 
            skeletonAnimation.skeleton.Data != null)
        {
            foreach (var animation in skeletonAnimation.skeleton.Data.Animations)
                outList.Add(animation.Name);
        }
    }

    public int GetSelectedAnimationIndex()
    {
        return selectedAnimationIndex;
    }
    
    public void ChangeAnimation(int index)
    {
        if (animationNames != null && index >= 0 && index < animationNames.Count)
        {
            selectedAnimationIndex = index;
            selectedAnimationName = animationNames[index];
           // SetAnimation(selectedAnimationName);
        }
    }
    
    //에니메이션 실행 메서드 
    public void SetAnimation(string animationName, int trackIndex = 0)
    {
        if (skeletonAnimation != null && !string.IsNullOrEmpty(animationName))
            skeletonAnimation.AnimationState.SetAnimation(trackIndex, animationName, true);
    }

    // 본 관련 메서드
    private void LoadBones()
    {
        if (skeletonAnimation == null || skeletonAnimation.skeleton == null)
            return;
            
        boneNames.Clear();
        foreach (var bone in skeletonAnimation.skeleton.Bones)
        {
            if (bone != null)
                boneNames.Add(bone.Data.Name);
        }
    
        // 뼈 선택 처리
        if (boneNames.Count > 0)
        {
            if (SelectedBone == null || !boneNames.Contains(selectedBoneName))
            {
                selectedBoneName = boneNames[0];
                selectedBoneIndex = 0;
            }
            else
            {
                selectedBoneIndex = boneNames.IndexOf(selectedBoneName);
            }
        }
    }
    
    public void GetBoneNames(List<string> outList)
    {
        if (skeletonAnimation != null && skeletonAnimation.skeleton != null)
        {
            foreach (var bone in skeletonAnimation.skeleton.Bones)
            {
                if (bone != null)
                    outList.Add(bone.Data.Name);
            }
        }
    }
    
    public int GetSelectedBoneIndex()
    {
        return selectedBoneIndex;
    }
    
    public void ChangeBone(int index)
    {
        if (boneNames != null && index >= 0 && index < boneNames.Count)
        {
            selectedBoneIndex = index;
            selectedBoneName = boneNames[index];
        }
    }
}