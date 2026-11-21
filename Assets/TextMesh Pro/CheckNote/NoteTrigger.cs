
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class NoteTrigger : MonoBehaviour
{
    public static NoteTrigger Instance;
    [Header("音符配置面板")]
    [SerializeField] private NotePanel notePanel;
    [SerializeField]  private CustomerReaction customerReaction;

    private int requiredCount = 5;

    [Header("检测结果事件")]
    public UnityEvent onSuccess;
    public UnityEvent onFail;

    public bool HasChecked
    {
        get => hasChecked;
        set => hasChecked = value;
    }
    private bool hasChecked = false;

    // 本轮已经检测过的食材，避免重复计数
    private HashSet<RhythmIngredientNote> detectedFoods = new HashSet<RhythmIngredientNote>();

    // 本轮检测到的音符
    private List<NoteType> collectedNotes = new List<NoteType>();

    private bool isFinished;

    private void Awake()
    {
        Instance = this;
        GetComponent<Collider>().isTrigger = true;
        requiredCount = notePanel.TotalNotes;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isFinished)
            return;

        RhythmIngredientNote foodNote = other.GetComponentInParent<RhythmIngredientNote>();

        if (foodNote == null)
            return;

        // 防止同一个食材重复检测
        if (!detectedFoods.Add(foodNote))
            return;

        collectedNotes.Add(foodNote.noteType);

        Debug.Log($"检测到音符：{foodNote.noteType}，进度：{collectedNotes.Count}/{requiredCount}");
        
        // 所有食材检测完毕后统一判断
        if (collectedNotes.Count >= requiredCount)
        {
            Debug.Log("All notes checked!");
            CheckResult();
        }
    }

    public void CheckResult()
    {
        isFinished = true;
        if (hasChecked)
            return;

        bool isSuccess = true;

        // 判断本轮收集的音符是否全部符合目标序列
        if (notePanel == null)
        {
            Debug.LogError("NotePanel 未配置！");
            isSuccess = false;
        }
        else if (collectedNotes.Count != notePanel.TotalNotes)
        {
            isSuccess = false;
        }
        else
        {
            for (int i = 0; i < collectedNotes.Count; i++)
            {
                if (collectedNotes[i] != GetTargetNote(i))
                {
                    isSuccess = false;
                    break;
                }
            }
        }

        if (isSuccess)
        {
            Debug.Log("Success！");
            onSuccess?.Invoke();
            customerReaction.OnBurgerSuccess();
            
        }
        else
        {
            Debug.Log("Fail！");
            onFail?.Invoke();
            customerReaction.OnBurgerFail();
        }
        hasChecked = true;
    }

    // 获取目标音符
    private NoteType GetTargetNote(int index)
    {
        return notePanel.GetTargetNote(index);
    }

    public void ResetTrigger()
    {
        detectedFoods.Clear();
        collectedNotes.Clear();
        isFinished = false;
    }
}
