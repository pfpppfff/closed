using Sunny.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using UcAsp.Opc;
namespace 控件拖拽功能
{
    public class OpcUa
    {

        public static UcAsp.Opc.OpcClient objUa = new OpcClient(new Uri("opc.tcp://127.0.0.1:49320"));
        static public void BoolWrite(string adrName, bool value)
        {
            try
            {
                objUa.Write(adrName, value);
            }
            catch(Exception ex)
            { MessageBox.Show("写入错误！"); }

        }
        static public void BoolSwith(string adrName)
        {
            try
            {
                string[] strs = new string[1] { adrName };

                List<OpcItemValue> res = OpcUa.objUa.Read(strs);
                bool sta = (bool)res[0].Value;
                if (sta == false)
                {
                    BoolWrite(adrName, true);
                }
                else
                {
                    BoolWrite(adrName, false);
                }

            }
            catch (Exception ex)
            { MessageBox.Show("写入错误！"); }

        }
        static public void FloatWrite(string adrName, float value)
        {
            try
            {

                objUa.Write(adrName, value);
            }
            catch (Exception ex)
            { MessageBox.Show("写入错误！"); }

        }
        static public void FloatWriteStr(string adrName, string value)
        {
            try
            {
                if (IsFloat(value))
                {
                    objUa.Write(adrName, Convert.ToSingle(value));
                }
                else
                {
                    MessageBox.Show("数据 输入错误！");
                }
            }
            catch
            {
                MessageBox.Show("错误！");
            }

        }
        static public void FloatAdd(string adrName, string add, ref float tempvalue, UITextBox textBox1, UITextBox textBox2)
        {
            try
            {
                float addvalue = 0;
                //float tempvalue ;
                if (IsFloat(add))
                {
                    tempvalue = Convert.ToSingle(Math.Round(Convert.ToSingle(textBox2.Text), 1));
                    addvalue = Convert.ToSingle(add);
                    tempvalue = tempvalue + addvalue;
                    float outvalue = 0;
                    if (tempvalue < 0)
                    {
                        tempvalue = 0;
                        outvalue = tempvalue;
                    }
                    else if (tempvalue > 100)
                    {
                        tempvalue = 100;
                        outvalue = tempvalue;
                    }
                    else
                    {
                        outvalue = tempvalue;
                    }
                    string st1 = Convert.ToSingle(Math.Round(outvalue, 1)).ToString();
                    textBox1.Invoke(new Action(() => { textBox1.Text = st1; }));
                    objUa.Write(adrName, tempvalue);
                }
                else
                {
                    MessageBox.Show("数据 输入错误！");
                }
            }
            catch
            {
                MessageBox.Show("错误！");
            }

        }
        static public void FloatSub(string adrName, string add, ref float tempvalue, UITextBox textBox1, UITextBox textBox2)
        {
            try
            {
                float addvalue = 0;
                //float tempvalue ;
                if (IsFloat(add))
                {
                    tempvalue = Convert.ToSingle(Math.Round(Convert.ToSingle(textBox2.Text), 1));
                    addvalue = Convert.ToSingle(add);
                    tempvalue = tempvalue - addvalue;
                    float outvalue = 0;
                    if (tempvalue < 0)
                    {
                        tempvalue = 0;
                        outvalue = tempvalue;
                    }
                    else if (tempvalue > 100)
                    {
                        tempvalue = 100;
                        outvalue = tempvalue;
                    }
                    else
                    {
                        outvalue = tempvalue;
                    }
                    string st1 = Convert.ToSingle(Math.Round(outvalue, 1)).ToString();
                    textBox1.Invoke(new Action(() => { textBox1.Text = st1; }));
                    objUa.Write(adrName, Convert.ToSingle(tempvalue));
                }
                else
                {
                    MessageBox.Show("数据 输入错误！");
                }
            }
            catch
            {
                MessageBox.Show("错误！");
            }

        }
        static public void BoolSetOP(string adrName)
        {
            try
            {
                string[] strs = new string[1] { adrName };

                List<OpcItemValue> res = OpcUa.objUa.Read(strs);
                bool sta = (bool)res[0].Value;
                if (sta)
                {
                    BoolWrite(adrName, false);
                }
                else
                {
                    BoolWrite(adrName, true);
                }

            }
            catch (Exception ex)
            { MessageBox.Show("写入错误！"); }

        }
        static public void IntSet(string adrName, Int16 value)
        {
            try
            {


                IntWrite(adrName, value);


            }
            catch (Exception ex)
            { MessageBox.Show("写入错误！"); }

        }
        static public void IntWrite(string adrName, Int16 value)
        {
            try
            {
                objUa.Write(adrName, value);
            }
            catch (Exception ex)
            { MessageBox.Show("写入错误！"); }

        }
        static public void IntSetOP(string adrName, Int16 value)
        {
            try
            {
                string[] strs = new string[1] { adrName };

                List<OpcItemValue> res = OpcUa.objUa.Read(strs);
                int sta = (Int16)res[0].Value;
                if (sta == 0)
                {
                    IntWrite(adrName, value);
                }
                else
                {
                    MessageBox.Show("先关闭才能写入！");
                }

            }
            catch (Exception ex)
            { MessageBox.Show("写入错误！"); }

        }
        static public void IntSwith(string adrName, Int16 value)
        {
            try
            {
                string[] strs = new string[1] { adrName };

                List<OpcItemValue> res = OpcUa.objUa.Read(strs);
                int sta = (Int16)res[0].Value;
                if (sta == 0)
                {
                    IntWrite(adrName, value);
                }
                else if (sta == value)
                {
                    IntWrite(adrName, 0);
                }
                else
                {
                    MessageBox.Show("先关闭其他状态！");
                }

            }
            catch (Exception ex)
            { MessageBox.Show("写入错误！"); }

        }
        static public Single ReadFloatOP(string adrName)
        {
            try
            {
               
                string[] strs = new string[1] { adrName };

                List<OpcItemValue> res = OpcUa.objUa.Read(strs);
              
                if (res != null)
                {
                   // float sta = (Single)res[0].Value;
                    float sta = res[0].Value==null? 0f: (Single)res[0].Value;
                    return sta;
                }
                else
                {
                    return 0;
                 
                }

            }
            catch
            { return 0; }

        }
        static public int ReadIntOP(string adrName)
        {
            try
            {

                string[] strs = new string[1] { adrName };

                List<OpcItemValue> res = OpcUa.objUa.Read(strs);

                if (res != null)
                {
                    int sta = (Int16)res[0].Value;
                    return sta;
                }
                else
                {
                    return 0;

                }

            }
            catch
            { return 0; }

        }
        static public bool IsFloat(string str)
        {
            if (str == "")
            {
                return false;
            }
            else
            {
                try
                {
                    float.Parse(str);
                    return true;
                }
                catch
                {
                    return false;
                }
            }
        }
    }
}
