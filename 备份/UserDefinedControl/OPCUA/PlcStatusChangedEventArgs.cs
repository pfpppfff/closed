using System;

namespace UserDefinedControl.OPCUA
{
    /// <summary>
    /// PLC状态变化事件参数
    /// </summary>
    public class PlcStatusChangedEventArgs : EventArgs
    {
        /// <summary>
        /// PLC标识符
        /// </summary>
        public string PlcId { get; }

        /// <summary>
        /// PLC名称
        /// </summary>
        public string PlcName { get; }

        /// <summary>
        /// 之前的状态
        /// </summary>
        public bool PreviousStatus { get; }

        /// <summary>
        /// 当前状态
        /// </summary>
        public bool CurrentStatus { get; }

        /// <summary>
        /// 状态变化时间
        /// </summary>
        public DateTime ChangeTime { get; }

        /// <summary>
        /// 错误信息（如果有）
        /// </summary>
        public string ErrorMessage { get; }

        /// <summary>
        /// 创建PLC状态变化事件参数
        /// </summary>
        /// <param name="plcId">PLC标识符</param>
        /// <param name="plcName">PLC名称</param>
        /// <param name="previousStatus">之前的状态</param>
        /// <param name="currentStatus">当前状态</param>
        /// <param name="errorMessage">错误信息</param>
        public PlcStatusChangedEventArgs(string plcId, string plcName, bool previousStatus, 
            bool currentStatus, string errorMessage = "")
        {
            PlcId = plcId;
            PlcName = plcName;
            PreviousStatus = previousStatus;
            CurrentStatus = currentStatus;
            ChangeTime = DateTime.Now;
            ErrorMessage = errorMessage ?? "";
        }

        /// <summary>
        /// 是否为状态恶化（从正常变为异常）
        /// </summary>
        public bool IsDegraded => PreviousStatus && !CurrentStatus;

        /// <summary>
        /// 是否为状态恢复（从异常变为正常）
        /// </summary>
        public bool IsRecovered => !PreviousStatus && CurrentStatus;

        /// <summary>
        /// 获取状态变化描述
        /// </summary>
        /// <returns>状态变化描述字符串</returns>
        public string GetStatusChangeDescription()
        {
            if (IsDegraded)
                return $"{PlcName} 状态异常: {ErrorMessage}";
            else if (IsRecovered)
                return $"{PlcName} 状态恢复正常";
            else
                return $"{PlcName} 状态更新: {(CurrentStatus ? "正常" : "异常")}";
        }

        public override string ToString()
        {
            return GetStatusChangeDescription();
        }
    }
}