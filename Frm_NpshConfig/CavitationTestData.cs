using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Frm_NpshConfig
{
    /// <summary>
    /// 汽蚀试验数据模型
    /// </summary>
    [Serializable]
    public class CavitationTestData
    {
        /// <summary>
        /// 序号
        /// </summary>
        public string TestNumber { get; set; }

        /// <summary>
        /// 取样流量 (m³/h)
        /// </summary>
        public double Flow { get; set; }

        /// <summary>
        /// 状态 (待测/已测)
        /// </summary>
        public string Status { get; set; }

        /// <summary>
        /// 临界扬程 (m)
        /// </summary>
        public double Head { get; set; }

        /// <summary>
        /// 是否重测扬程
        /// </summary>
        public bool Retest { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }

        public CavitationTestData()
        {
            TestNumber = "";
            Flow = 0.0;
            Status = "待测";
            Head = 0.0;
            Retest = false;
            Remark = "";
        }
    }

    /// <summary>
    /// 汽蚀试验配置数据集合
    /// </summary>
    [Serializable]
    public class CavitationTestConfig
    {
        public List<CavitationTestData> TestDataList { get; set; }

        public CavitationTestConfig()
        {
            TestDataList = new List<CavitationTestData>();
        }
    }
}