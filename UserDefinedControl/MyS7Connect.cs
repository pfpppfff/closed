using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using UcAsp.Opc;

namespace UserDefinedControl
{
    //public class OpcUa
    //{
    //    //public static UcAsp.Opc.OpcClient objUa = new OpcClient(new Uri("opc.tcp://169.254.116.244:49320"));
    //    //public static UcAsp.Opc.OpcClient objUa = new OpcClient(new Uri("opc.tcp://192.168.10.35:49320"));
    //    public static UcAsp.Opc.OpcClient objUa = new OpcClient(new Uri("opc.tcp://127.0.0.1:49320"));
    //    public static string[] DataAdder = {
    //        "1214.PLC1._System._NoError",
    //        "1214.PLC1.DisData.Flow.Flow_1",
    //        "1214.PLC1.DisData.Flow.Flow_2",
    //         "1214.PLC1.DisData.Flow.Flow_3",
    //        "1214.PLC1.DisData.Press.Inpress_1",
    //        "1214.PLC1.DisData.Press.Outpress_1",
    //        "1214.PLC1.DisData.Press.Outpress_2",
    //        "1214.PLC1.DisData.Press.Outpress_3",
    //        "1214.PLC1.DisData.Press.Outpress_4",
    //        "1214.PLC1.DisData.Press.Outpress_5",
    //        "1214.PLC1.DisData.PowerM.item1.Voltage",
    //        "1214.PLC1.DisData.PowerM.item1.Current",
    //        "1214.PLC1.DisData.PowerM.item1.Power",
    //        "1214.PLC1.DisData.PowerM.item1.Powerf",
    //        "1214.PLC1.DisData.PowerM.item1.Fry",
    //        "1214.PLC1.DisData.Temp.temp_1",
    //        "1214.PLC1.DisData.Temp.temp_2",
    //        "1214.PLC1.DisData.Temp.temp_3",
    //        "1214.PLC1.DisData.otherM.item1.EleSpeed",
    //        "1214.PLC1.DisData.otherM.item1.FlowSpeed",
    //        "1214.PLC1.DisData.otherM.item1.SlioRatio"

    //    };
    //    private static bool systemNoErr = false;
    //    static public float[] ReadData()
    //    {
    //        float[] data = new float[DataAdder.Length - 1];
    //        int countLen = DataAdder.Length - 1;
    //        List<OpcItemValue> resA = null;
    //        resA = OpcUa.objUa.Read(DataAdder);
    //        systemNoErr = (bool)resA[0].Value;
    //        if (systemNoErr && resA[1].Value != null)
    //        {
    //            data = Enumerable.Range(1, countLen).Select(n => (float)resA[n].Value).ToArray();
    //            return data;
    //        }
    //        else
    //        {
    //            return Enumerable.Range(0, countLen).Select(i => 0f).ToArray();
    //        }
    //    }
    //    static public void BoolWrite(string adrName, bool value)
    //    {
    //        try
    //        {
    //            objUa.Write(adrName, value);
    //        }
    //        catch
    //        { }
    //    }
    //    static public async Task BoolWriteAsync(bool enable, string adrName, bool value)
    //    {
    //        if (!enable) return;
    //        if (string.IsNullOrWhiteSpace(adrName))
    //            throw new ArgumentException("Address name cannot be null or empty.", nameof(adrName));
    //        try
    //        {
    //           await objUa.WriteAsync(adrName, value);
    //        }
    //        catch
    //        { }
    //    }

    //    public static async Task<bool> ReadBoolAsync(bool enable, string adrName)
    //    {
    //        if (!enable) return false;
    //        if (string.IsNullOrWhiteSpace(adrName))
    //            throw new ArgumentException("Address name cannot be null or empty.", nameof(adrName));
    //        try
    //        {
    //            return await objUa.ReadAsync<bool>(adrName);
    //        }
    //        catch (Exception ex)
    //        {
    //            // 🚨 记录日志
    //            // Logger.LogError(ex, "Failed to read boolean OPC item: {AdrName}", adrName);
    //            return false; // 默认返回 false
    //        }
    //    }

    //    public static async Task<float> ReadRealAsync(bool enable, string adrName)
    //    {
    //        if (!enable) return 0f;
    //        if (string.IsNullOrWhiteSpace(adrName))
    //            throw new ArgumentException("Address name cannot be null or empty.", nameof(adrName));
    //        try
    //        {
    //            return await objUa.ReadAsync<float>(adrName);
    //        }
    //        catch (Exception ex)
    //        {
    //            // 🚨 记录日志
    //            // Logger.LogError(ex, "Failed to read boolean OPC item: {AdrName}", adrName);
    //            return 0f; 
    //        }
    //    }
    //    public static async Task WriteRealAsync(bool enable, string adrName,float value)
    //    {
    //        if (!enable) return ;
    //        if (string.IsNullOrWhiteSpace(adrName))
    //            throw new ArgumentException("Address name cannot be null or empty.", nameof(adrName));
    //        try
    //        {
    //           await objUa.WriteAsync(adrName, value);
    //        }
    //        catch (Exception ex)
    //        {
               
    //        }
    //    }

    //    public static async Task<Int16> ReadIntAsync(bool enable, string adrName)
    //    {
    //        if (!enable) return 0;
    //        if (string.IsNullOrWhiteSpace(adrName))
    //            throw new ArgumentException("Address name cannot be null or empty.", nameof(adrName));
    //        try
    //        {
    //            return await objUa.ReadAsync<Int16>(adrName);
    //        }
    //        catch (Exception ex)
    //        {
    //            // 🚨 记录日志
    //            // Logger.LogError(ex, "Failed to read boolean OPC item: {AdrName}", adrName);
    //            return 0;
    //        }
    //    }
    //    public static async Task WriteIntAsync(bool enable, string adrName, Int16 value)
    //    {
    //        if (!enable) return;
    //        if (string.IsNullOrWhiteSpace(adrName))
    //            throw new ArgumentException("Address name cannot be null or empty.", nameof(adrName));
    //        try
    //        {
    //            await objUa.WriteAsync(adrName, value);
    //        }
    //        catch (Exception ex)
    //        {

    //        }
    //    }
    //    static public void BoolSwith(string adrName)
    //    {
    //        try
    //        {
    //            string[] strs = new string[1] { adrName };

    //            List<OpcItemValue> res = OpcUa.objUa.Read(strs);
    //            bool sta = (bool)res[0].Value;
    //            if (sta == false)
    //            {
    //                BoolWrite(adrName, true);
    //            }
    //            else
    //            {
    //                BoolWrite(adrName, false);
    //            }

    //        }
    //        catch
    //        { }

    //    }
    //    static public void FloatWrite(string adrName, float value)
    //    {
    //        try
    //        {

    //            objUa.Write(adrName, value);
    //        }
    //        catch
    //        { }

    //    }
    //    static public void FloatWriteStr(string adrName, string value)
    //    {
    //        try
    //        {
    //            if (IsFloat(value))
    //            {
    //                objUa.Write(adrName, Convert.ToSingle(value));
    //            }
    //            else
    //            {
    //                MessageBox.Show("数据 输入错误！");
    //            }
    //        }
    //        catch
    //        {
    //            MessageBox.Show("错误！");
    //        }

    //    }
    //    //static public void FloatAdd(string adrName, string add, ref float tempvalue, UITextBox textBox1, UITextBox textBox2)
    //    //{
    //    //    try
    //    //    {
    //    //        float addvalue = 0;
    //    //        //float tempvalue ;
    //    //        if (IsFloat(add))
    //    //        {
    //    //            tempvalue = Convert.ToSingle(Math.Round(Convert.ToSingle(textBox2.Text), 1));
    //    //            addvalue = Convert.ToSingle(add);
    //    //            tempvalue = tempvalue + addvalue;
    //    //            float outvalue = 0;
    //    //            if (tempvalue < 0)
    //    //            {
    //    //                tempvalue = 0;
    //    //                outvalue = tempvalue;
    //    //            }
    //    //            else if (tempvalue > 100)
    //    //            {
    //    //                tempvalue = 100;
    //    //                outvalue = tempvalue;
    //    //            }
    //    //            else
    //    //            {
    //    //                outvalue = tempvalue;
    //    //            }
    //    //            string st1 = Convert.ToSingle(Math.Round(outvalue, 1)).ToString();
    //    //            textBox1.Invoke(new Action(() => { textBox1.Text = st1; }));
    //    //            objUa.Write(adrName, tempvalue);
    //    //        }
    //    //        else
    //    //        {
    //    //            MessageBox.Show("数据 输入错误！");
    //    //        }
    //    //    }
    //    //    catch
    //    //    {
    //    //        MessageBox.Show("错误！");
    //    //    }

    //    //}
    //    //static public void FloatSub(string adrName, string add, ref float tempvalue, UITextBox textBox1, UITextBox textBox2)
    //    //{
    //    //    try
    //    //    {
    //    //        float addvalue = 0;
    //    //        //float tempvalue ;
    //    //        if (IsFloat(add))
    //    //        {
    //    //            tempvalue = Convert.ToSingle(Math.Round(Convert.ToSingle(textBox2.Text), 1));
    //    //            addvalue = Convert.ToSingle(add);
    //    //            tempvalue = tempvalue - addvalue;
    //    //            float outvalue = 0;
    //    //            if (tempvalue < 0)
    //    //            {
    //    //                tempvalue = 0;
    //    //                outvalue = tempvalue;
    //    //            }
    //    //            else if (tempvalue > 100)
    //    //            {
    //    //                tempvalue = 100;
    //    //                outvalue = tempvalue;
    //    //            }
    //    //            else
    //    //            {
    //    //                outvalue = tempvalue;
    //    //            }
    //    //            string st1 = Convert.ToSingle(Math.Round(outvalue, 1)).ToString();
    //    //            textBox1.Invoke(new Action(() => { textBox1.Text = st1; }));
    //    //            objUa.Write(adrName, Convert.ToSingle(tempvalue));
    //    //        }
    //    //        else
    //    //        {
    //    //            MessageBox.Show("数据 输入错误！");
    //    //        }
    //    //    }
    //    //    catch
    //    //    {
    //    //        MessageBox.Show("错误！");
    //    //    }

    //    //}
    //    static public void BoolSetOP(string adrName)
    //    {
    //        try
    //        {
    //            string[] strs = new string[1] { adrName };

    //            List<OpcItemValue> res = OpcUa.objUa.Read(strs);
    //            bool sta = (bool)res[0].Value;
    //            if (sta)
    //            {
    //                BoolWrite(adrName, false);
    //            }
    //            else
    //            {
    //                BoolWrite(adrName, true);
    //            }

    //        }
    //        catch
    //        { }

    //    }
    //    static public void IntSet(string adrName, Int16 value)
    //    {
    //        try
    //        {
    //            IntWrite(adrName, value);

    //        }
    //        catch
    //        { }

    //    }
    //    static public void IntWrite(string adrName, Int16 value)
    //    {
    //        try
    //        {
    //            objUa.Write(adrName, value);
    //        }
    //        catch
    //        { }

    //    }
    //    static public void IntSetOP(string adrName, Int16 value)
    //    {
    //        try
    //        {
    //            string[] strs = new string[1] { adrName };

    //            List<OpcItemValue> res = OpcUa.objUa.Read(strs);
    //            int sta = (Int16)res[0].Value;
    //            if (sta == 0)
    //            {
    //                IntWrite(adrName, value);
    //            }
    //            else
    //            {
    //                MessageBox.Show("先关闭才能写入！");
    //            }

    //        }
    //        catch
    //        { }

    //    }

    //    static public bool IsFloat(string str)
    //    {
    //        if (str == "")
    //        {
    //            return false;
    //        }
    //        else
    //        {
    //            try
    //            {
    //                float.Parse(str);
    //                return true;
    //            }
    //            catch
    //            {
    //                return false;
    //            }
    //        }
    //    }
    //}
}
