using UnityEngine;
using UnityEngine.Events;

public class NotePanel : MonoBehaviour
{
    [Header("当前关卡目标音符")]
    [SerializeField]
    private NoteType[] targetNotes;

    [Header("匹配完成事件")]
    public UnityEvent onAllNotesMatched;

    private int currentIndex = 0;
    
    public NoteType GetTargetNote(int index)
    {
        if (index < 0 || index >= targetNotes.Length)
            return NoteType.Note1;

        return targetNotes[index];
    }

    /// <summary>
    /// 检测音符是否符合当前目标
    /// </summary>
    public bool CheckNote(NoteType note)
    {
        if (currentIndex >= targetNotes.Length)
            return false;

        if (targetNotes[currentIndex] != note)
        {
            Debug.Log($"音符错误，当前需要：{targetNotes[currentIndex]}，实际为：{note}");
            return false;
        }

        currentIndex++;

        Debug.Log($"音符匹配成功：{note}，进度：{currentIndex}/{targetNotes.Length}");

        if (currentIndex >= targetNotes.Length)
        {
            Debug.Log("全部音符匹配完成！");
            onAllNotesMatched?.Invoke();
        }

        return true;
    }

    /// <summary>
    /// 重置当前关卡音符进度
    /// </summary>
    public void ResetNotes()
    {
        currentIndex = 0;
    }

    public int CurrentIndex => currentIndex;

    public int TotalNotes => targetNotes.Length;

    public bool IsCompleted => currentIndex >= targetNotes.Length;
}