using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace 密码保护
{
    public class ActivationInfo
    {
        // <summary>
        /// 首次安装时间
        /// </summary>
        public DateTime FirstInstallTime { get; set; }

        /// <summary>
        /// 上次启动时间
        /// </summary>
        public DateTime LastStartTime { get; set; }

        /// <summary>
        /// 累计运行天数
        /// </summary>
        public int TotalRunDays { get; set; }

        /// <summary>
        /// 今日累计运行时间（秒）
        /// </summary>
        public int TodayRunSeconds { get; set; }

        /// <summary>
        /// 总累计运行时间（秒）
        /// </summary>
        public long TotalRunSeconds { get; set; }

        /// <summary>
        /// 是否永久激活
        /// </summary>
        public bool IsPermanentActivated { get; set; }

        /// <summary>
        /// 临时激活到期时间
        /// </summary>
        public DateTime TempActivationExpiry { get; set; }

        /// <summary>
        /// 硬件指纹
        /// </summary>
        public string HardwareFingerprint { get; set; }

        /// <summary>
        /// 数据完整性校验码
        /// </summary>
        public string CheckSum { get; set; }
    }
    /// <summary>
    /// 软件激活管理器
    /// </summary>
    /// <summary>
    /// 软件激活管理器
    /// </summary>
    /// <summary>
    /// 软件激活管理器
    /// </summary>
    public class ActivationManager
    {
        private const string ACTIVATION_FILE = "activation.dat";
        private const string ENCRYPTION_KEY = "SoftwareActivation2024Key!@#$%^&*()";
        private const int TRIAL_DAYS = 90; // 试用90天
        private const int TRIAL_Hour =0; // 试用20小时
        private const int TRIAL_Mini = 0; // 试用10分
        private const string PERMANENT_PASSWORD = "PERM2024ACTIVE"; // 永久激活密码
        private const string TEMKEY = "TEMPZJD109238";
        public ActivationInfo _activationInfo;
        public DateTime _sessionStartTime;

        public ActivationManager()
        {
            _sessionStartTime = DateTime.Now;
            LoadActivationInfo();
        }

        /// <summary>
        /// 生成自定义硬件指纹（模拟）
        /// 实际使用中可以获取真实硬件信息
        /// </summary>
        private string GenerateHardwareFingerprint()
        {
            // 模拟硬件信息组合
            string mockCpuId = "Intel_i7_12700K_ABC123DEF456";
            string mockMacAddress = "00-11-22-33-44-55";
            string mockMotherboard = "ASUS_Z690_SERIES_789XYZ";
            string mockHddSerial = "WD_SN850_1TB_ST123456789";
            
            //string combined = "zjddem";
            string combined = mockCpuId+ mockMacAddress+ mockMotherboard+mockHddSerial;
            // 生成MD5哈希作为指纹
            using (MD5 md5 = MD5.Create())
            {
                byte[] hashBytes = md5.ComputeHash(Encoding.UTF8.GetBytes(combined));
                return Convert.ToBase64String(hashBytes);
            }
        }

        /// <summary>
        /// 加载激活信息
        /// </summary>
        private void LoadActivationInfo()
        {
            try
            {
                if (File.Exists(ACTIVATION_FILE))
                {
                    // 读取并解密文件
                    string encryptedData = File.ReadAllText(ACTIVATION_FILE);
                    string decryptedXml = DecryptString(encryptedData);

                    // 反序列化XML
                    XmlSerializer serializer = new XmlSerializer(typeof(ActivationInfo));
                    using (StringReader reader = new StringReader(decryptedXml))
                    {
                        _activationInfo = (ActivationInfo)serializer.Deserialize(reader);
                    }

                    // 验证数据完整性
                    if (!ValidateCheckSum())
                    {
                        throw new Exception("激活文件已被篡改");
                    }

                    // 验证硬件指纹
                    if (_activationInfo.HardwareFingerprint != GenerateHardwareFingerprint())
                    {
                        throw new Exception("硬件环境已改变，需要重新激活");
                    }
                }
                else
                {
                    // 首次运行，创建新的激活信息
                    CreateNewActivationInfo();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"加载激活信息失败: {ex.Message}");
                CreateNewActivationInfo();
            }
        }

        /// <summary>
        /// 创建新的激活信息
        /// </summary>
        private void CreateNewActivationInfo()
        {
            _activationInfo = new ActivationInfo
            {
                FirstInstallTime = DateTime.Now,
                LastStartTime = DateTime.Now,
                TotalRunDays = 0,
                TodayRunSeconds = 0,
                TotalRunSeconds = 0,
                IsPermanentActivated = false,
                TempActivationExpiry = DateTime.Now.AddDays(TRIAL_DAYS),
                HardwareFingerprint = GenerateHardwareFingerprint()
            };
            SaveActivationInfo();
        }

        /// <summary>
        /// 保存激活信息
        /// </summary>
        private void SaveActivationInfo()
        {
            try
            {
                // 计算校验码
                _activationInfo.CheckSum = CalculateCheckSum();

                // 序列化为XML
                XmlSerializer serializer = new XmlSerializer(typeof(ActivationInfo));
                string xmlData;
                using (StringWriter writer = new StringWriter())
                {
                    serializer.Serialize(writer, _activationInfo);
                    xmlData = writer.ToString();
                }

                // 加密并保存
                string encryptedData = EncryptString(xmlData);
                File.WriteAllText(ACTIVATION_FILE, encryptedData);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"保存激活信息失败: {ex.Message}");
            }
        }

        /// <summary>
        /// 计算数据校验码
        /// </summary>
        private string CalculateCheckSum()
        {
            string data = $"{_activationInfo.FirstInstallTime:yyyy-MM-dd HH:mm:ss}" +
                         $"{_activationInfo.TotalRunDays}" +
                         $"{_activationInfo.TotalRunSeconds}" +
                         $"{_activationInfo.IsPermanentActivated}" +
                         $"{_activationInfo.HardwareFingerprint}";

            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(data));
                return Convert.ToBase64String(hashBytes);
            }
        }

        /// <summary>
        /// 验证数据完整性
        /// </summary>
        private bool ValidateCheckSum()
        {
            string currentCheckSum = CalculateCheckSum();
            return currentCheckSum == _activationInfo.CheckSum;
        }

        /// <summary>
        /// AES加密
        /// </summary>
        private string EncryptString(string plainText)
        {
            using (Aes aes = Aes.Create())
            {
                // 从密钥生成固定的Key和IV
                byte[] keyBytes = Encoding.UTF8.GetBytes(ENCRYPTION_KEY);
                byte[] key = new byte[32];
                byte[] iv = new byte[16];

                Array.Copy(keyBytes, 0, key, 0, Math.Min(keyBytes.Length, 32));
                Array.Copy(keyBytes, 0, iv, 0, Math.Min(keyBytes.Length, 16));

                aes.Key = key;
                aes.IV = iv;

                ICryptoTransform encryptor = aes.CreateEncryptor();
                byte[] plainBytes = Encoding.UTF8.GetBytes(plainText);
                byte[] encryptedBytes = encryptor.TransformFinalBlock(plainBytes, 0, plainBytes.Length);

                return Convert.ToBase64String(encryptedBytes);
            }
        }

        /// <summary>
        /// AES解密
        /// </summary>
        private string DecryptString(string cipherText)
        {
            using (Aes aes = Aes.Create())
            {
                byte[] keyBytes = Encoding.UTF8.GetBytes(ENCRYPTION_KEY);
                byte[] key = new byte[32];
                byte[] iv = new byte[16];

                Array.Copy(keyBytes, 0, key, 0, Math.Min(keyBytes.Length, 32));
                Array.Copy(keyBytes, 0, iv, 0, Math.Min(keyBytes.Length, 16));

                aes.Key = key;
                aes.IV = iv;

                ICryptoTransform decryptor = aes.CreateDecryptor();
                byte[] encryptedBytes = Convert.FromBase64String(cipherText);
                byte[] decryptedBytes = decryptor.TransformFinalBlock(encryptedBytes, 0, encryptedBytes.Length);

                return Encoding.UTF8.GetString(decryptedBytes);
            }
        }

        /// <summary>
        /// 更新运行时间统计
        /// 实现按天数+当天实际运行时间的计算逻辑
        /// </summary>
        public void UpdateRuntime()
        {
            DateTime now = DateTime.Now;
            DateTime lastStart = _activationInfo.LastStartTime;

            // 计算本次会话运行时间（秒）
            int sessionRunSeconds = (int)(now - _sessionStartTime).TotalSeconds;

            // 检查是否跨天了
            if (now.Date > lastStart.Date)
            {
                // 跨天了，需要更新天数统计
                int daysPassed = (now.Date - lastStart.Date).Days;

                if (daysPassed == 1)
                {
                    // 只跨了一天
                    _activationInfo.TotalRunDays += 1;
                    _activationInfo.TodayRunSeconds = sessionRunSeconds; // 重置今日运行时间
                }
                else if (daysPassed > 1)
                {
                    // 跨了多天（比如好几天没用软件）
                    _activationInfo.TotalRunDays += daysPassed;
                    _activationInfo.TodayRunSeconds = sessionRunSeconds;
                }
            }
            else
            {
                // 同一天内，累加今日运行时间
                _activationInfo.TodayRunSeconds += sessionRunSeconds;
            }

            // 更新总运行时间
            _activationInfo.TotalRunSeconds += sessionRunSeconds;
            _activationInfo.LastStartTime = now;

            // 重置会话开始时间
            _sessionStartTime = now;

            SaveActivationInfo();
        }

        /// <summary>
        /// 获取格式化的运行时间显示
        /// 例如：90天1小时10分钟 或 1小时18分钟 或 20分钟
        /// </summary>
        public string GetFormattedRuntime()
        {
            int totalDays = _activationInfo.TotalRunDays;
            int todaySeconds = _activationInfo.TodayRunSeconds;

            int hours = todaySeconds / 3600;
            int minutes = (todaySeconds % 3600) / 60;
            int seconds = todaySeconds % 60;

            string result = "";
            //_activationInfo.TodayRunSeconds = _activationInfo.TodayRunSeconds + 60;
            //_activationInfo.TotalRunSeconds= _activationInfo.TotalRunSeconds + 60;
            //if (totalDays > 0)
            //{
            //    result += $"{totalDays}天";
            //}

            //if (hours > 0)
            //{
            //    result += $"{hours}小时";
            //}

            //if (minutes > 0)
            //{
            //    result += $"{minutes}分钟";
            //}

            //if (hours == 0 && minutes == 0 && seconds > 0)
            //{
            //    result += $"{seconds}秒";
            //}
            int tdays = 0; 
            int thours = 0; 
            int tminutes = 0;
            int tseconds = totalDays*86400+ _activationInfo.TodayRunSeconds;
            ConvertSecondsToDHMS(Convert.ToInt32(tseconds), out tdays, out thours, out tminutes, out seconds);
            return tseconds==0 ? "0秒" : $"{tdays}天{thours}时{tminutes}分";
        }

        /// <summary>
        /// 检查激活状态
        /// </summary>
        public bool IsActivated()
        {
            // 永久激活
            if (_activationInfo.IsPermanentActivated)
            {
                return true;
            }

            // 检查试用期（按天数+运行时间计算）
            DateTime now = DateTime.Now;

            // 防止修改系统时间：检查当前时间不能早于首次安装时间
            if (now < _activationInfo.FirstInstallTime)
            {
                Console.WriteLine("检测到系统时间异常！");
                return false;
            }

            // 防止修改系统时间：检查当前时间不能早于上次启动时间
            if (now < _activationInfo.LastStartTime.AddMinutes(-5)) // 允许5分钟时间差
            {
                Console.WriteLine("检测到系统时间被修改！");
                return false;
            }

            // 计算试用期剩余时间
            // 试用期 = 90天 - 实际运行天数 - (今日运行时间/24小时)            
            //double usedDays = _activationInfo.TotalRunDays + (_activationInfo.TodayRunSeconds / 86400.0);
            //double remainingDays = TRIAL_DAYS - usedDays;
            //return remainingDays > 0;
            //将天  时 分都加上换成秒
            double usedSeconds = _activationInfo.TotalRunSeconds ;
            double remainingSeconds = TRIAL_DAYS * 86400+TRIAL_Hour*3600+TRIAL_Mini*60 - usedSeconds;
            return remainingSeconds > 0;
        }

        /// <summary>
        /// 获取剩余试用时间
        /// </summary>
        public string GetRemainingTrialTime()
        {
            if (_activationInfo.IsPermanentActivated)
            {
                return "永久激活";
            }

            //double usedDays = _activationInfo.TotalRunDays + (_activationInfo.TodayRunSeconds / 86400.0);
            //double remainingDays = TRIAL_DAYS - usedDays;

            //if (remainingDays <= 0)
            //{
            //    return "试用期已结束";
            //}

            //int days = (int)remainingDays;
            //int hours = (int)((remainingDays - days) * 24);

            //return $"剩余试用时间：{days}天{hours}小时";

            double usedSeconds = _activationInfo.TotalRunSeconds   ;
            double remainingSeconds = TRIAL_DAYS * 86400 + TRIAL_Hour * 3600 + TRIAL_Mini * 60 - usedSeconds;

            if (remainingSeconds <= 0)
            {
                return "试用期已结束";
            }

            int days = 0;
            int hours = 0;
            int minutes = 0;
            int seconds = 0;
            // 调用方法
            ConvertSecondsToDHMS((Int32)remainingSeconds, out days, out  hours, out  minutes, out  seconds);

            return $"{days}天{hours}时{minutes}分";
        }
        public  void ConvertSecondsToDHMS(int totalSeconds, out int days, out int hours, out int minutes, out int seconds)
        {
            const int SecondsPerMinute = 60;
            const int SecondsPerHour = 60 * 60;
            const int SecondsPerDay = 24 * 60 * 60;

            // 计算天数
            days = totalSeconds / SecondsPerDay;
            int remainingAfterDays = totalSeconds % SecondsPerDay;

            // 计算小时
            hours = remainingAfterDays / SecondsPerHour;
            int remainingAfterHours = remainingAfterDays % SecondsPerHour;

            // 计算分钟
            minutes = remainingAfterHours / SecondsPerMinute;
            seconds = remainingAfterHours % SecondsPerMinute;
        }
        /// <summary>
        /// 使用密码激活
        /// </summary>
        public bool ActivateWithPassword(string password)
        {
            if (password == PERMANENT_PASSWORD)
            {
                _activationInfo.IsPermanentActivated = true;
                SaveActivationInfo();
                return true;
            }

            // 可以在这里添加临时激活码逻辑
            // 例如：生成3个月有效的激活码
            if (password==TEMKEY/*IsValidTempActivationCode(password)*/)
            {
                _activationInfo.TotalRunDays = 0;
                _activationInfo.TotalRunSeconds = 0;
                _activationInfo.TodayRunSeconds = 0;
                _activationInfo.TempActivationExpiry = DateTime.Now.AddMonths(3);
                SaveActivationInfo();
                return true;
            }

            return false;
        }

        /// <summary>
        /// 验证临时激活码（示例实现）
        /// </summary>
        private bool IsValidTempActivationCode(string code)
        {
            // 简单的临时激活码验证逻辑
            // 实际应用中可以实现更复杂的算法
            string expectedCode = GenerateTempActivationCode();
            return code == expectedCode;
        }

        /// <summary>
        /// 生成临时激活码（示例）
        /// </summary>
        private string GenerateTempActivationCode()
        {
            // 基于硬件指纹和当前月份生成激活码
            string baseString = _activationInfo.HardwareFingerprint + DateTime.Now.ToString("yyyy-MM");
            using (MD5 md5 = MD5.Create())
            {
                byte[] hashBytes = md5.ComputeHash(Encoding.UTF8.GetBytes(baseString));
                string hash = Convert.ToBase64String(hashBytes);
                return "TEMP" + hash.Substring(0, 8).ToUpper(); // 取前8位
            }
        }

        /// <summary>
        /// 程序退出时调用，保存运行时间
        /// </summary>
        public void OnApplicationExit()
        {
            UpdateRuntime();
        }

        /// <summary>
        /// 获取激活信息摘要
        /// </summary>
        public string GetActivationSummary()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("=== 软件激活信息 ===");
            sb.AppendLine($"首次安装时间: {_activationInfo.FirstInstallTime:yyyy-MM-dd HH:mm:ss}");
            sb.AppendLine($"累计运行时间: {GetFormattedRuntime()}");
            sb.AppendLine($"激活状态: {(IsActivated() ? "已激活" : "未激活")}");
            sb.AppendLine($"剩余时间: {GetRemainingTrialTime()}");
            sb.AppendLine($"硬件指纹: {_activationInfo.HardwareFingerprint.Substring(0, 8)}...");

            return sb.ToString();
        }
    }
}
