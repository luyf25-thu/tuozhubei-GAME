#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEditor.Animations;
using UnityEditor.SceneManagement;
using UnityEngine;

[InitializeOnLoad]
public static class PlayerAnimatorAutoSetup
{
    private const string ControllerPath = "Assets/Animations/Controllers/PlayerAnimator.controller";
    private const string IdleClipPath = "Assets/Animations/Clips/cat idling/idle Animation.anim";
    private const string RunningClipPath = "Assets/Animations/Clips/cat running/running Animation.anim";
    private const string JumpingClipPath = "Assets/Animations/Clips/cat jumping/jumping Animation.anim";

    static PlayerAnimatorAutoSetup()
    {
        EditorApplication.delayCall += EnsureAnimatorSetupSilently;
    }

    [MenuItem("Tools/Player/Force Setup Animator")]
    private static void ForceSetupAnimator()
    {
        if (Application.isPlaying)
        {
            Debug.LogWarning("请先退出 Play Mode，再执行 Force Setup Animator。", null);
            return;
        }

        EnsureAnimatorSetup(true);
    }

    private static void EnsureAnimatorSetupSilently()
    {
        if (Application.isPlaying)
        {
            return;
        }

        EnsureAnimatorSetup(false);
    }

    private static void EnsureAnimatorSetup(bool verbose)
    {
        AnimationClip idleClip = AssetDatabase.LoadAssetAtPath<AnimationClip>(IdleClipPath);
        AnimationClip runningClip = AssetDatabase.LoadAssetAtPath<AnimationClip>(RunningClipPath);
        AnimationClip jumpingClip = AssetDatabase.LoadAssetAtPath<AnimationClip>(JumpingClipPath);

        if (idleClip == null || runningClip == null || jumpingClip == null)
        {
            if (verbose)
            {
                Debug.LogWarning("Player Animator 自动配置失败：缺少 idle/running/jumping 动画资源。", null);
            }
            return;
        }

        string controllerDirectory = Path.GetDirectoryName(ControllerPath);
        if (!string.IsNullOrEmpty(controllerDirectory) && !AssetDatabase.IsValidFolder(controllerDirectory))
        {
            Directory.CreateDirectory(controllerDirectory);
            AssetDatabase.Refresh();
        }

        AnimatorController controller = AssetDatabase.LoadAssetAtPath<AnimatorController>(ControllerPath);
        if (controller == null)
        {
            controller = AnimatorController.CreateAnimatorControllerAtPath(ControllerPath);
        }

        AnimatorStateMachine stateMachine = controller.layers[0].stateMachine;
        EnsureIntParameter(controller, "State");
        AnimatorState idleState = EnsureState(stateMachine, "idle animation", idleClip);
        AnimatorState runningState = EnsureState(stateMachine, "running animation", runningClip);
        AnimatorState jumpingState = EnsureState(stateMachine, "jumping animation", jumpingClip);
        stateMachine.defaultState = idleState;
        EnsureAnyStateTransition(stateMachine, idleState, "State", 0);
        EnsureAnyStateTransition(stateMachine, runningState, "State", 1);
        EnsureAnyStateTransition(stateMachine, jumpingState, "State", 2);
        EnsureDirectTransition(idleState, runningState, "State", 1);
        EnsureDirectTransition(idleState, jumpingState, "State", 2);
        EnsureDirectTransition(runningState, idleState, "State", 0);
        EnsureDirectTransition(runningState, jumpingState, "State", 2);
        EnsureDirectTransition(jumpingState, idleState, "State", 0);
        EnsureDirectTransition(jumpingState, runningState, "State", 1);

        PlayerController playerController = Object.FindObjectOfType<PlayerController>();
        if (playerController == null)
        {
            if (verbose)
            {
                Debug.LogWarning("Player Animator 自动配置完成了控制器，但当前已打开场景里没有 PlayerController。", null);
            }
            AssetDatabase.SaveAssets();
            return;
        }

        Animator animator = playerController.GetComponent<Animator>();
        if (animator == null)
        {
            animator = playerController.gameObject.AddComponent<Animator>();
        }

        animator.runtimeAnimatorController = controller;

        SerializedObject serializedPlayer = new SerializedObject(playerController);
        SetIfExists(serializedPlayer, "animator", animator);
        SetIfExists(serializedPlayer, "spriteRenderer", playerController.GetComponentInChildren<SpriteRenderer>());
        SetIfExists(serializedPlayer, "idleAnimationStateName", "idle animation");
        SetIfExists(serializedPlayer, "runningAnimationStateName", "running animation");
        SetIfExists(serializedPlayer, "jumpingAnimationStateName", "jumping animation");
        serializedPlayer.ApplyModifiedPropertiesWithoutUndo();

        EditorUtility.SetDirty(playerController);
        EditorUtility.SetDirty(animator);
        if (!Application.isPlaying)
        {
            EditorSceneManager.MarkSceneDirty(playerController.gameObject.scene);
        }
        AssetDatabase.SaveAssets();

        if (verbose)
        {
            Debug.Log("Player Animator 已完成强制配置：Controller/Animator/状态名绑定完成。", playerController);
        }
    }

    private static AnimatorState EnsureState(AnimatorStateMachine stateMachine, string stateName, Motion motion)
    {
        ChildAnimatorState[] states = stateMachine.states;
        for (int i = 0; i < states.Length; i++)
        {
            if (states[i].state.name != stateName)
            {
                continue;
            }

            states[i].state.motion = motion;
            return states[i].state;
        }

        AnimatorState newState = stateMachine.AddState(stateName);
        newState.motion = motion;
        return newState;
    }

    private static void EnsureIntParameter(AnimatorController controller, string parameterName)
    {
        for (int i = 0; i < controller.parameters.Length; i++)
        {
            AnimatorControllerParameter parameter = controller.parameters[i];
            if (parameter.name == parameterName && parameter.type == AnimatorControllerParameterType.Int)
            {
                return;
            }
        }

        controller.AddParameter(parameterName, AnimatorControllerParameterType.Int);
    }

    private static void EnsureAnyStateTransition(AnimatorStateMachine stateMachine, AnimatorState destination, string parameterName, int expectedValue)
    {
        ChildAnimatorState[] states = stateMachine.states;
        for (int i = 0; i < states.Length; i++)
        {
            AnimatorStateTransition[] transitions = states[i].state.transitions;
            for (int j = 0; j < transitions.Length; j++)
            {
                AnimatorStateTransition transition = transitions[j];
                if (transition.destinationState != destination)
                {
                    continue;
                }

                if (transition.conditions.Length == 1
                    && transition.conditions[0].mode == AnimatorConditionMode.Equals
                    && transition.conditions[0].parameter == parameterName
                    && Mathf.Approximately(transition.conditions[0].threshold, expectedValue))
                {
                    transition.hasExitTime = false;
                    transition.duration = 0f;
                    return;
                }
            }
        }

        AnimatorStateTransition anyStateTransition = stateMachine.AddAnyStateTransition(destination);
        anyStateTransition.hasExitTime = false;
        anyStateTransition.duration = 0f;
        anyStateTransition.canTransitionToSelf = false;
        anyStateTransition.AddCondition(AnimatorConditionMode.Equals, expectedValue, parameterName);
    }

    private static void EnsureDirectTransition(AnimatorState source, AnimatorState destination, string parameterName, int expectedValue)
    {
        AnimatorStateTransition[] transitions = source.transitions;
        for (int i = 0; i < transitions.Length; i++)
        {
            AnimatorStateTransition transition = transitions[i];
            if (transition.destinationState != destination)
            {
                continue;
            }

            if (transition.conditions.Length == 1
                && transition.conditions[0].mode == AnimatorConditionMode.Equals
                && transition.conditions[0].parameter == parameterName
                && Mathf.Approximately(transition.conditions[0].threshold, expectedValue))
            {
                transition.hasExitTime = false;
                transition.duration = 0f;
                return;
            }
        }

        AnimatorStateTransition stateTransition = source.AddTransition(destination);
        stateTransition.hasExitTime = false;
        stateTransition.duration = 0f;
        stateTransition.canTransitionToSelf = false;
        stateTransition.AddCondition(AnimatorConditionMode.Equals, expectedValue, parameterName);
    }

    private static void SetIfExists(SerializedObject serializedObject, string propertyName, Object value)
    {
        SerializedProperty property = serializedObject.FindProperty(propertyName);
        if (property != null)
        {
            property.objectReferenceValue = value;
        }
    }

    private static void SetIfExists(SerializedObject serializedObject, string propertyName, bool value)
    {
        SerializedProperty property = serializedObject.FindProperty(propertyName);
        if (property != null)
        {
            property.boolValue = value;
        }
    }

    private static void SetIfExists(SerializedObject serializedObject, string propertyName, string value)
    {
        SerializedProperty property = serializedObject.FindProperty(propertyName);
        if (property != null)
        {
            property.stringValue = value;
        }
    }
}
#endif
