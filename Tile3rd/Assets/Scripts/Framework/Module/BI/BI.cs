namespace CSFramework
{
    public enum BIType
    {
        None,
        UIEvent, // UI事件
        GameEvent, // 游戏事件
        ResourceEvent, // 资源事件
    }

    public enum BI_UIEventType
    {
        // WindowUI
        WindowOpened,
        WindowClosed,
        WindowShow,
        WindowHide,

        // BaseUI
        ButtonClicked,
        ToggleChanged,
        // todo more
    }

    public enum BI_GameEventType
    {
        FrameworkInitFinished,
    }

    public enum BI_ResourceEventType
    {
        Increase,
        Decrease,
    }

    [System.Serializable]
    public class BI 
    {
        public BIType BIType; // BI类型
        public string EventType; // 事件类型的名称
        public string EventParam; // 事件参数
        public string Target; // 触发事件的目标
        public string Reason; // 触发事件的原因
        public ulong Timestamp; // 时间戳
    }
}