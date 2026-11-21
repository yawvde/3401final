using UnityEngine;
[RequireComponent(typeof(Collider))]
public class NoteTrigger : MonoBehaviour
{
    [Header("音符配置面板")]
    [SerializeField]
    private NotePanel notePanel;
    private void Awake()
    {
        GetComponent<Collider>().isTrigger = true;
    }
    private void OnTriggerEnter(Collider other)
    {
        // 获取食材音符组件
        RhythmIngredientNote foodNote = other.GetComponent<RhythmIngredientNote>();

        if (foodNote == null)
            return;

        if (notePanel == null)
        {
            Debug.LogWarning("NotePanel 未配置！");
            return;
        }

        // 判断音符是否匹配
        bool isMatch = notePanel.CheckNote(foodNote.noteType);

        if (isMatch)
        {
            Debug.Log($"音符 {foodNote.noteType} 匹配成功！");
            
            // 可以在这里添加食材销毁、播放音效等逻辑
        }
    }
}