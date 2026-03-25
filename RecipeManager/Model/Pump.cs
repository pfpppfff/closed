using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace RecipeManager.Model
{
    public class PumpModel
    {
        //[XmlIgnore] // 避免在 XML 中序列化此属性
        public bool IsSelected { get; set; } // 用于保存选中状态
        public string ModelName { get; set; } // 泵型号
        public List<PowerEfficiency> PowerEfficiencyList { get; set; } // 多组功率和效率
        public PumpModel()
        {
            PowerEfficiencyList = new List<PowerEfficiency>();
        }
       

    }

    public class PumpSelectionState
    {
        public string ModelName { get; set; }
        public bool IsSelected { get; set; }
    }
    public class PowerEfficiency
    {
        public double Power { get; set; } // 功率
        public double Efficiency { get; set; } // 效率
    }
}
