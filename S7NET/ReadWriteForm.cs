using S7NET.Services;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace S7NET
{
    public partial class ReadWriteForm : Form
    {
        private readonly MultiPlcServiceManager _multiPlcService;
        
        // UI控件
        private ComboBox cmbPlc;
        private ComboBox cmbDataType;
        private TextBox txtAddress;
        private TextBox txtValue;
        private Button btnRead;
        private Button btnWrite;
        private TextBox txtResult;
        private ListBox lstHistory;

        public ReadWriteForm(MultiPlcServiceManager multiPlcService)
        {
            _multiPlcService = multiPlcService ?? throw new ArgumentNullException(nameof(multiPlcService));
            InitializeComponent();
            InitializeData();
        }

        private void InitializeData()
        {
            // 初始化PLC列表
            cmbPlc.Items.Add("默认PLC");
            foreach (var plcId in _multiPlcService.GetAllPlcIds())
            {
                cmbPlc.Items.Add(plcId);
            }
            cmbPlc.SelectedIndex = 0;

            // 初始化数据类型
            cmbDataType.Items.AddRange(new string[] 
            {
                "float", "int", "short", "bool", "byte"
            });
            cmbDataType.SelectedIndex = 0;

            // 设置默认地址
            txtAddress.Text = "DB15.DBD0";
        }

        private async void btnRead_Click(object sender, EventArgs e)
        {
            try
            {
                btnRead.Enabled = false;
                var plcId = GetSelectedPlcId();
                var address = txtAddress.Text.Trim();
                var dataType = cmbDataType.SelectedItem.ToString();

                if (string.IsNullOrEmpty(address))
                {
                    MessageBox.Show("请输入地址", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                AddHistory($"读取 [{plcId ?? "默认"}] {address} ({dataType})");

                object result = null;
                switch (dataType)
                {
                    case "float":
                        if (address.Contains("DB"))
                        {
                            var parts = ParseDBAddress(address);
                            var floatResult = await _multiPlcService.ReadDBAsync<float>(parts.DbNumber, parts.StartByte, 1, plcId);
                            result = floatResult[0];
                        }
                        break;
                    case "int":
                        if (address.Contains("DB"))
                        {
                            var parts = ParseDBAddress(address);
                            var intResult = await _multiPlcService.ReadDBAsync<int>(parts.DbNumber, parts.StartByte, 1, plcId);
                            result = intResult[0];
                        }
                        break;
                    case "short":
                        if (address.Contains("DB"))
                        {
                            var parts = ParseDBAddress(address);
                            var shortResult = await _multiPlcService.ReadDBAsync<short>(parts.DbNumber, parts.StartByte, 1, plcId);
                            result = shortResult[0];
                        }
                        break;
                    case "bool":
                        if (address.Contains("."))
                        {
                            result = await _multiPlcService.ReadBitAsync(address, plcId);
                        }
                        break;
                    case "byte":
                        if (address.Contains("DB"))
                        {
                            var parts = ParseDBAddress(address);
                            var byteResult = await _multiPlcService.ReadDBAsync<byte>(parts.DbNumber, parts.StartByte, 1, plcId);
                            result = byteResult[0];
                        }
                        break;
                }

                if (result != null)
                {
                    txtResult.Text = result.ToString();
                    AddHistory($"读取成功: {result}");
                }
                else
                {
                    txtResult.Text = "读取失败";
                    AddHistory("读取失败: 不支持的地址格式或数据类型");
                }
            }
            catch (Exception ex)
            {
                txtResult.Text = $"错误: {ex.Message}";
                AddHistory($"读取异常: {ex.Message}");
            }
            finally
            {
                btnRead.Enabled = true;
            }
        }

        private async void btnWrite_Click(object sender, EventArgs e)
        {
            try
            {
                btnWrite.Enabled = false;
                var plcId = GetSelectedPlcId();
                var address = txtAddress.Text.Trim();
                var dataType = cmbDataType.SelectedItem.ToString();
                var valueText = txtValue.Text.Trim();

                if (string.IsNullOrEmpty(address) || string.IsNullOrEmpty(valueText))
                {
                    MessageBox.Show("请输入地址和值", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                AddHistory($"写入 [{plcId ?? "默认"}] {address} = {valueText} ({dataType})");

                bool success = false;
                switch (dataType)
                {
                    case "float":
                        if (float.TryParse(valueText, out float floatValue) && address.Contains("DB"))
                        {
                            var parts = ParseDBAddress(address);
                            success = await _multiPlcService.WriteDBAsync<float>(parts.DbNumber, parts.StartByte, floatValue, plcId);
                        }
                        break;
                    case "int":
                        if (int.TryParse(valueText, out int intValue) && address.Contains("DB"))
                        {
                            var parts = ParseDBAddress(address);
                            success = await _multiPlcService.WriteDBAsync<int>(parts.DbNumber, parts.StartByte, intValue, plcId);
                        }
                        break;
                    case "short":
                        if (short.TryParse(valueText, out short shortValue) && address.Contains("DB"))
                        {
                            var parts = ParseDBAddress(address);
                            success = await _multiPlcService.WriteDBAsync<short>(parts.DbNumber, parts.StartByte, shortValue, plcId);
                        }
                        break;
                    case "bool":
                        if (bool.TryParse(valueText, out bool boolValue) && address.Contains("."))
                        {
                            success = await _multiPlcService.WriteBitAsync(address, boolValue, plcId);
                        }
                        break;
                    case "byte":
                        if (byte.TryParse(valueText, out byte byteValue) && address.Contains("DB"))
                        {
                            var parts = ParseDBAddress(address);
                            success = await _multiPlcService.WriteDBAsync<byte>(parts.DbNumber, parts.StartByte, byteValue, plcId);
                        }
                        break;
                }

                txtResult.Text = success ? "写入成功" : "写入失败";
                AddHistory(success ? "写入成功" : "写入失败");
            }
            catch (Exception ex)
            {
                txtResult.Text = $"错误: {ex.Message}";
                AddHistory($"写入异常: {ex.Message}");
            }
            finally
            {
                btnWrite.Enabled = true;
            }
        }

        private string GetSelectedPlcId()
        {
            return cmbPlc.SelectedIndex == 0 ? null : cmbPlc.SelectedItem.ToString();
        }

        private (int DbNumber, int StartByte) ParseDBAddress(string address)
        {
            // 解析DB地址，如: DB15.DBD0, DB15.DBW4, DB15.DBB8
            address = address.ToUpper().Trim();
            if (!address.StartsWith("DB"))
                throw new ArgumentException("无效的DB地址格式");

            var parts = address.Split('.');
            if (parts.Length != 2)
                throw new ArgumentException("无效的DB地址格式");

            var dbNumber = int.Parse(parts[0].Substring(2));
            
            var offsetPart = parts[1];
            int startByte;
            if (offsetPart.StartsWith("DBD"))
                startByte = int.Parse(offsetPart.Substring(3));
            else if (offsetPart.StartsWith("DBW"))
                startByte = int.Parse(offsetPart.Substring(3));
            else if (offsetPart.StartsWith("DBB"))
                startByte = int.Parse(offsetPart.Substring(3));
            else
                throw new ArgumentException("无效的DB地址格式");

            return (dbNumber, startByte);
        }

        private void AddHistory(string message)
        {
            var timestamp = DateTime.Now.ToString("HH:mm:ss");
            var historyMessage = $"[{timestamp}] {message}";
            
            lstHistory.Items.Add(historyMessage);
            
            // 保持最新的100条历史
            while (lstHistory.Items.Count > 100)
            {
                lstHistory.Items.RemoveAt(0);
            }
            
            // 滚动到最新
            lstHistory.TopIndex = lstHistory.Items.Count - 1;
        }
    }
}
