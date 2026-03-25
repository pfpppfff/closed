using System;

namespace UserDefinedControl.OPCUA
{
    /// <summary>
    /// PLC状态信息模型
    /// 用于管理单个PLC的状态信息
    /// </summary>
    public class PlcStatusModel
    {
        #region 属性
        /// <summary>
        /// PLC唯一标识符
        /// </summary>
        public string PlcId { get; set; }

        /// <summary>
        /// PLC名称
        /// </summary>
        public string PlcName { get; set; }

        /// <summary>
        /// PLC描述信息
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 系统状态节点地址（如：1214.PLC1._System._NoError）
        /// </summary>
        public string SystemStatusNodeId { get; set; }

        /// <summary>
        /// 当前系统状态（true=正常，false=错误）
        /// </summary>
        public bool IsSystemNormal { get; private set; }

        /// <summary>
        /// 最后一次状态更新时间
        /// </summary>
        public DateTime LastUpdateTime { get; private set; }

        /// <summary>
        /// 连接状态
        /// </summary>
        public bool IsConnected { get; private set; }

        /// <summary>
        /// 错误信息
        /// </summary>
        public string ErrorMessage { get; private set; }

        /// <summary>
        /// 是否启用（可以临时禁用某个PLC的监控）
        /// </summary>
        public bool IsEnabled { get; set; }

        /// <summary>
        /// 优先级（用于处理顺序，数值越小优先级越高）
        /// </summary>
        public int Priority { get; set; }
        #endregion

        #region 构造函数
        /// <summary>
        /// 创建PLC状态模型
        /// </summary>
        /// <param name="plcId">PLC标识符</param>
        /// <param name="plcName">PLC名称</param>
        /// <param name="systemStatusNodeId">系统状态节点地址</param>
        /// <param name="description">描述信息</param>
        /// <param name="priority">优先级</param>
        public PlcStatusModel(string plcId, string plcName, string systemStatusNodeId, 
            string description = "", int priority = 0)
        {
            PlcId = plcId ?? throw new ArgumentNullException(nameof(plcId));
            PlcName = plcName ?? throw new ArgumentNullException(nameof(plcName));
            SystemStatusNodeId = systemStatusNodeId ?? throw new ArgumentNullException(nameof(systemStatusNodeId));
            Description = description ?? "";
            Priority = priority;
            IsEnabled = true;
            IsSystemNormal = false;
            IsConnected = false;
            LastUpdateTime = DateTime.MinValue;
            ErrorMessage = "";
        }
        #endregion

        #region 公共方法
        /// <summary>
        /// 更新系统状态
        /// </summary>
        /// <param name="isNormal">系统是否正常</param>
        /// <param name="errorMessage">错误信息（可选）</param>
        public void UpdateSystemStatus(bool isNormal, string errorMessage = "")
        {
            IsSystemNormal = isNormal;
            ErrorMessage = errorMessage ?? "";
            LastUpdateTime = DateTime.Now;
            IsConnected = true;
        }

        /// <summary>
        /// 设置连接失败状态
        /// </summary>
        /// <param name="errorMessage">错误信息</param>
        public void SetConnectionFailed(string errorMessage)
        {
            IsConnected = false;
            IsSystemNormal = false;
            ErrorMessage = errorMessage ?? "";
            LastUpdateTime = DateTime.Now;
        }

        /// <summary>
        /// 重置状态
        /// </summary>
        public void Reset()
        {
            IsSystemNormal = false;
            IsConnected = false;
            ErrorMessage = "";
            LastUpdateTime = DateTime.MinValue;
        }

        /// <summary>
        /// 获取状态摘要信息
        /// </summary>
        /// <returns>状态摘要字符串</returns>
        public string GetStatusSummary()
        {
            if (!IsEnabled)
                return $"{PlcName}: 已禁用";
                
            if (!IsConnected)
                return $"{PlcName}: 连接失败 - {ErrorMessage}";
                
            return $"{PlcName}: {(IsSystemNormal ? "正常" : "异常")} ({LastUpdateTime:HH:mm:ss})";
        }

        /// <summary>
        /// 检查状态是否过期
        /// </summary>
        /// <param name="timeoutSeconds">超时秒数</param>
        /// <returns>是否过期</returns>
        public bool IsStatusExpired(int timeoutSeconds = 30)
        {
            if (LastUpdateTime == DateTime.MinValue)
                return true;
                
            return (DateTime.Now - LastUpdateTime).TotalSeconds > timeoutSeconds;
        }
        #endregion

        #region 重写方法
        public override string ToString()
        {
            return GetStatusSummary();
        }

        public override bool Equals(object obj)
        {
            if (obj is PlcStatusModel other)
                return PlcId.Equals(other.PlcId, StringComparison.OrdinalIgnoreCase);
            return false;
        }

        public override int GetHashCode()
        {
            return PlcId?.GetHashCode() ?? 0;
        }
        #endregion
    }
}