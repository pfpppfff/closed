using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using System.Windows.Forms;

namespace Frm_NpshConfig
{
    /// <summary>
    /// 数据持久化管理类
    /// </summary>
    public static class DataManager
    {
        private static readonly string DataFilePath = Path.Combine(Application.StartupPath, "CavitationTestConfig.xml");

        /// <summary>
        /// 保存配置数据到本地文件
        /// </summary>
        /// <param name="config">配置数据</param>
        public static void SaveConfig(CavitationTestConfig config)
        {
            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(CavitationTestConfig));
                using (FileStream fs = new FileStream(DataFilePath, FileMode.Create))
                {
                    serializer.Serialize(fs, config);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"保存配置数据失败：{ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// 从本地文件加载配置数据
        /// </summary>
        /// <returns>配置数据</returns>
        public static CavitationTestConfig LoadConfig()
        {
            try
            {
                if (File.Exists(DataFilePath))
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(CavitationTestConfig));
                    using (FileStream fs = new FileStream(DataFilePath, FileMode.Open))
                    {
                        return (CavitationTestConfig)serializer.Deserialize(fs);
                    }
                }
                else
                {
                    // 如果文件不存在，创建默认配置
                    return CreateDefaultConfig();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"加载配置数据失败：{ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return CreateDefaultConfig();
            }
        }

        /// <summary>
        /// 创建默认配置数据
        /// </summary>
        /// <returns>默认配置数据</returns>
        private static CavitationTestConfig CreateDefaultConfig()
        {
            CavitationTestConfig config = new CavitationTestConfig();
            // 添加默认的一行数据
            config.TestDataList.Add(new CavitationTestData
            {
                TestNumber = "001",
                Flow = 100.0,
                Status = "待测",
                Head = 50.0,
                Retest = false,
                Remark = "默认测试数据"
            });
            return config;
        }
    }
}