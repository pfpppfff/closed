using OtpNet;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace 密码设置保护
{
    public class ActivationInfo
    {
        // <summary>
        /// 首次安装时间
        /// </summary>
        public DateTime FirstInstallTime { get; set; }

        public int SetDays { get; set; }
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
 
    public class ActivationManager
    {
        private const string ACTIVATION_FILE = "activation.dat";
        private const string ENCRYPTION_KEY = "SoftwareActivation2024Key!@#$%^&*()";
        private  int TRIAL_DAYS =7; // 
        private  int TRIAL_Hour = 0; // 试用
        private  int TRIAL_Mini = 0; // 试用
        private const string PERMANENT_PASSWORD = "PERM2025ACTIVE"; // 永久激活密码
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
        public string GenerateHardwareFingerprint()
        {
            // 模拟硬件信息组合
            string mockCpuId = "Intel_i7_12700K_ABC123DEF456";
            string mockMacAddress = "00-11-22-33-44-55";
            string mockMotherboard = "ASUS_Z690_SERIES_789XYZ";
            string mockHddSerial = "WD_SN850_1TB_ST123456789";

            //string combined = "zjddem";
            string combined = mockCpuId + mockMacAddress + mockMotherboard + mockHddSerial;
            // 生成MD5哈希作为指纹
            using (MD5 md5 = MD5.Create())
            {
                byte[] hashBytes = md5.ComputeHash(Encoding.UTF8.GetBytes(combined));
                return Convert.ToBase64String(hashBytes);
            }
        }

        private bool FileValidateError=false;
        /// <summary
        /// 加载激活信息
        /// </summary>
        private void LoadActivationInfo()
        {
            try
            {
                FileValidateError = false;
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
                        if (_activationInfo != null)
                        {
                            TRIAL_DAYS = _activationInfo.SetDays;
                        }
                        else
                        {
                            TRIAL_DAYS =7;
                        }
                           
                    }

                    // 验证数据完整性
                    if (!ValidateCheckSum())
                    {
                        FileValidateError = true;
                        MessageBox.Show("激活文件已被篡改");
                        throw new Exception("激活文件已被篡改");
                      
                    }

                    // 验证硬件指纹
                    if (_activationInfo.HardwareFingerprint != GenerateHardwareFingerprint())
                    {
                        FileValidateError = true;
                        MessageBox.Show("激活文件已被篡改");
                        throw new Exception("硬件环境已改变，需要重新激活");
                        
                    }
                }
                else
                {
                    FileValidateError = true;
                    // 首次运行，创建新的激活信息
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
                }
            }
            catch (Exception ex)
            {
                FileValidateError = true;
                Console.WriteLine($"加载激活信息失败: {ex.Message}");
                //CreateNewActivationInfo();
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
            }
        }

        /// <summary>
        /// 创建新的激活信息
        /// </summary>
        public  void CreateNewActivationInfo()
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
            SaveActivationInfo(false);
        }

        /// <summary>
        /// 保存激活信息
        /// </summary>
        private void SaveActivationInfo(bool fileValidateError)
        {
            try
            {
                if (fileValidateError) return;
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

            if (_activationInfo.TotalRunSeconds >= 86400)
            {
                if (now.Date < lastStart.Date)
                {
                    _activationInfo.TotalRunSeconds = 0;
                }
            }

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

                }
                _activationInfo.TotalRunSeconds = 0;//20250710
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

            SaveActivationInfo(FileValidateError);
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
            long tdays = 0;
            long thours = 0;
            long tminutes = 0;
            long tseconds1 = 0;
            long tseconds = _activationInfo.TotalRunSeconds;
            ConvertSecondsToDHMS(Convert.ToInt32(tseconds), out tdays, out thours, out tminutes, out tseconds1);
            return tseconds == 0 ? "0秒" : $"{totalDays}天";
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
            if(FileValidateError)
            {
                return false;
            }
            // 计算试用期剩余时间
            // 试用期 = 90天 - 实际运行天数 - (今日运行时间/24小时)            
            //double usedDays = _activationInfo.TotalRunDays + (_activationInfo.TodayRunSeconds / 86400.0);
            //double remainingDays = TRIAL_DAYS - usedDays;
            //return remainingDays > 0;
            //将天  时 分都加上换成秒
            double usedSeconds = _activationInfo.TotalRunSeconds;
            double remainingSeconds = TRIAL_DAYS - _activationInfo.TotalRunDays;
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

            double usedSeconds = _activationInfo.TotalRunDays * 86400;
            long remainingSeconds = (long)(TRIAL_DAYS * 86400 + TRIAL_Hour * 3600 + TRIAL_Mini * 60 - usedSeconds);

            if (remainingSeconds <= 0)
            {
                return "试用期已结束";
            }

            long days = 0;
            long hours = 0;
            long minutes = 0;
            long seconds = 0;
            long tseconds1 = 0;
            // 调用方法
            ConvertSecondsToDHMS(remainingSeconds, out days, out hours, out minutes, out tseconds1);

            return $"{days}天";
        }
        public void ConvertSecondsToDHMS(long totalSeconds, out long days, out long hours, out long minutes, out long seconds)
        {
            const int SecondsPerMinute = 60;
            const int SecondsPerHour = 60 * 60;
            const int SecondsPerDay = 24 * 60 * 60;

            // 计算天数
            days = totalSeconds / SecondsPerDay;
            long remainingAfterDays = totalSeconds % SecondsPerDay;

            // 计算小时
            hours = remainingAfterDays / SecondsPerHour;
            long remainingAfterHours = remainingAfterDays % SecondsPerHour;

            // 计算分钟
            minutes = remainingAfterHours / SecondsPerMinute;
            seconds = remainingAfterHours % SecondsPerMinute;
        }
        /// <summary>
        /// 使用密码激活
        /// </summary>
        public bool ActivateWithPassword(string password,byte[] code)
        {
            if (password == PERMANENT_PASSWORD)
            {
                _activationInfo.IsPermanentActivated = true;
                SaveActivationInfo(FileValidateError);
                return true;
            }

            string deviceId = "zjdpingABC";
            byte[] finalSecret = TotpHelper.DeriveSecret(code, deviceId);
            var totp = new Totp(finalSecret);
            //允许 ±30 秒误差（防时钟不同步）VerificationWindow.RfcSpecifiedNetworkDelay
            int positiveTolerance = 3;
            int negativeTolerance = 3;
            bool isValid = totp.VerifyTotp(
                password,
                out long timeStepMatched,
                new VerificationWindow(positiveTolerance, negativeTolerance)
            );

            if (isValid)
            {
                _activationInfo.TotalRunDays = 0;
                _activationInfo.TotalRunSeconds = 0;
                _activationInfo.TodayRunSeconds = 0;
                _activationInfo.TempActivationExpiry = DateTime.Now.AddMonths(3);
                SaveActivationInfo(FileValidateError);
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

        public void SetActivationIfo(ActivationInfo activationInfo)
        {
            _activationInfo = activationInfo;
            SaveActivationInfo(FileValidateError);
        }

    }

    public static class TotpHelper
    {
        // 合并两个字节数组
        public static byte[] Concat(byte[] a, byte[] b)
        {
            byte[] result = new byte[a.Length + b.Length];
            Buffer.BlockCopy(a, 0, result, 0, a.Length);
            Buffer.BlockCopy(b, 0, result, a.Length, b.Length);
            return result;
        }

        // 派生密钥：原始密钥 + 标识符 → SHA256 → 32字节密钥
        public static byte[] DeriveSecret(byte[] rawSecret, string identifier)
        {
            byte[] idBytes = Encoding.UTF8.GetBytes(identifier);
            byte[] combined = Concat(rawSecret, idBytes);

            using (var sha256 = SHA256.Create())
            {
                return sha256.ComputeHash(combined);
            }
        }
    }
    public static class KeyProtector
    {
        // 加密随机密钥 → 返回 Base64 字符串（约 44 字符，可缩短显示）
        public static string ProtectKey(byte[] secret, byte[] masterKey)
        {
             var aes = Aes.Create();
            aes.Key = masterKey;
            aes.GenerateIV(); // 随机 IV，增强安全

            var encryptor = aes.CreateEncryptor();
            byte[] encrypted = encryptor.TransformFinalBlock(secret, 0, secret.Length);

            // 把 IV + 密文 合并
            byte[] result = new byte[aes.IV.Length + encrypted.Length];
            Buffer.BlockCopy(aes.IV, 0, result, 0, aes.IV.Length);
            Buffer.BlockCopy(encrypted, 0, result, aes.IV.Length, encrypted.Length);

            return Convert.ToBase64String(result);
        }

        // 解密 Base64 字符串 → 还原原始密钥
        public static byte[] UnprotectKey(string protectedKey, byte[] masterKey)
        {
            byte[] data = Convert.FromBase64String(protectedKey);
             var aes = Aes.Create();
            aes.Key = masterKey;

            // 前 16 字节是 IV
            byte[] iv = new byte[16];
            byte[] cipher = new byte[data.Length - 16];
            Buffer.BlockCopy(data, 0, iv, 0, 16);
            Buffer.BlockCopy(data, 16, cipher, 0, cipher.Length);

            aes.IV = iv;
             var decryptor = aes.CreateDecryptor();
            return decryptor.TransformFinalBlock(cipher, 0, cipher.Length);
        }
    }
}
