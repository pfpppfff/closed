using System;
using System.Threading.Tasks;
using UserDefinedControl.OPCUA;

namespace UserDefinedControl.OPCUA
{
    /// <summary>
    /// 多PLC状态管理功能测试示例
    /// 演示如何使用优化后的OPC UA服务
    /// </summary>
    public static class MultiPlcTestExample
    {
        /// <summary>
        /// 运行多PLC状态管理测试
        /// </summary>
        public static async Task RunTest()
        {
            Console.WriteLine("=== 多PLC状态管理功能测试 ===\n");

            // 1. 获取服务实例
            var opcService = OpcUaServiceManager.Current;
            
            // 2. 注册PLC状态变化事件
            opcService.PlcStatusChanged += (sender, e) =>
            {
                Console.WriteLine($"📡 PLC状态变化: {e.GetStatusChangeDescription()}");
                if (e.IsDegraded)
                {
                    Console.WriteLine($"⚠️  警告: {e.PlcName} 出现异常！");
                }
                else if (e.IsRecovered)
                {
                    Console.WriteLine($"✅ 恢复: {e.PlcName} 状态恢复正常");
                }
            };

            // 3. 添加多个PLC配置
            Console.WriteLine("📋 添加PLC配置...");
            var plcConfigs = new[]
            {
                new PlcStatusModel("PLC1", "主控PLC", "1214.PLC1._System._NoError", "主控制系统PLC", 1),
                new PlcStatusModel("PLC2", "包装PLC", "1214.PLC2._System._NoError", "自动包装机PLC", 2),
                new PlcStatusModel("PLC3", "质检PLC", "1214.PLC3._System._NoError", "产品质检系统PLC", 3)
            };

            foreach (var plc in plcConfigs)
            {
                bool success = opcService.AddOrUpdatePlc(plc);
                Console.WriteLine($"   {(success ? "✅" : "❌")} 添加 {plc.PlcName}: {(success ? "成功" : "失败")}");
            }

            // 4. 显示所有PLC状态
            Console.WriteLine("\n📊 当前PLC配置:");
            var allPlcs = opcService.GetAllPlcStatus();
            foreach (var plc in allPlcs)
            {
                Console.WriteLine($"   🔧 {plc.PlcId}: {plc.PlcName} (优先级: {plc.Priority})");
                Console.WriteLine($"      状态节点: {plc.SystemStatusNodeId}");
                Console.WriteLine($"      启用状态: {(plc.IsEnabled ? "是" : "否")}");
            }

            // 5. 测试状态更新
            Console.WriteLine("\n🔄 测试状态更新...");
            for (int i = 0; i < 3; i++)
            {
                Console.WriteLine($"\n--- 第 {i + 1} 轮状态更新 ---");
                
                // 更新所有PLC状态
                int successCount = await opcService.UpdateAllPlcSystemStatusAsync();
                Console.WriteLine($"✅ 成功更新 {successCount}/{allPlcs.Count} 个PLC状态");
                
                // 显示状态统计
                int normalCount = opcService.GetNormalPlcCount();
                int abnormalCount = opcService.GetAbnormalPlcCount();
                Console.WriteLine($"📈 状态统计 - 正常: {normalCount}, 异常: {abnormalCount}");
                
                // 显示详细状态
                foreach (var plc in allPlcs)
                {
                    string status = plc.IsSystemNormal ? "🟢 正常" : "🔴 异常";
                    string connection = plc.IsConnected ? "已连接" : "断开";
                    Console.WriteLine($"   {plc.PlcName}: {status} ({connection})");
                }
                
                await Task.Delay(2000); // 等待2秒
            }

            // 6. 测试安全读取功能
            Console.WriteLine("\n🛡️ 测试安全读取功能...");
            foreach (var plc in allPlcs)
            {
                bool canRead = opcService.CanSafelyReadPlcData(plc.PlcId);
                string result = canRead ? "✅ 可以安全读取" : "❌ 不能读取";
                Console.WriteLine($"   {plc.PlcName}: {result}");
                
                if (canRead)
                {
                    // 演示安全读取数据
                    try
                    {
                        // 这里可以放置实际的数据读取代码
                        Console.WriteLine($"      💡 可以读取 {plc.PlcId} 的生产数据");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"      ⚠️ 读取数据失败: {ex.Message}");
                    }
                }
            }

            // 7. 测试PLC移除功能
            Console.WriteLine("\n🗑️ 测试移除PLC功能...");
            bool removeSuccess = opcService.RemovePlc("PLC3");
            Console.WriteLine($"移除PLC3: {(removeSuccess ? "✅ 成功" : "❌ 失败")}");
            
            var remainingPlcs = opcService.GetAllPlcStatus();
            Console.WriteLine($"剩余PLC数量: {remainingPlcs.Count}");

            Console.WriteLine("\n=== 测试完成 ===");
        }

        /// <summary>
        /// 工业场景应用示例
        /// </summary>
        public static async Task IndustrialScenarioExample()
        {
            Console.WriteLine("=== 工业场景应用示例 ===\n");

            var opcService = OpcUaServiceManager.Current;
            
            // 工业生产线PLC配置
            var productionLinePlcs = new[]
            {
                new PlcStatusModel("MAIN_PLC", "生产线主控", "1214.MainPLC._System._NoError", "生产线主控制器", 1),
                new PlcStatusModel("CONV_PLC", "传送带控制", "1214.ConveyorPLC._System._NoError", "传送带控制系统", 2),
                new PlcStatusModel("ROBOT_PLC", "机器人控制", "1214.RobotPLC._System._NoError", "焊接机器人控制", 3),
                new PlcStatusModel("QC_PLC", "质检系统", "1214.QualityPLC._System._NoError", "产品质量检测", 4),
                new PlcStatusModel("PKG_PLC", "包装系统", "1214.PackagingPLC._System._NoError", "自动包装机", 5)
            };

            Console.WriteLine("🏭 配置生产线PLC系统...");
            foreach (var plc in productionLinePlcs)
            {
                opcService.AddOrUpdatePlc(plc);
                Console.WriteLine($"   ✅ 配置 {plc.PlcName}");
            }

            // 模拟生产过程
            Console.WriteLine("\n🔄 模拟生产过程监控...");
            for (int cycle = 1; cycle <= 5; cycle++)
            {
                Console.WriteLine($"\n--- 生产周期 {cycle} ---");
                
                // 更新所有PLC状态
                await opcService.UpdateAllPlcSystemStatusAsync();
                
                // 按优先级检查PLC状态
                var allPlcs = opcService.GetAllPlcStatus();
                bool productionOk = true;
                
                foreach (var plc in allPlcs)
                {
                    if (!opcService.CanSafelyReadPlcData(plc.PlcId))
                    {
                        Console.WriteLine($"⚠️ {plc.PlcName} 状态异常，生产受影响");
                        
                        // 根据PLC类型决定处理策略
                        switch (plc.PlcId)
                        {
                            case "MAIN_PLC":
                                Console.WriteLine("🚨 主控PLC异常，紧急停产！");
                                productionOk = false;
                                break;
                            case "QC_PLC":
                                Console.WriteLine("⚠️ 质检系统异常，切换人工检验");
                                break;
                            default:
                                Console.WriteLine($"⚠️ {plc.PlcName} 异常，监控中...");
                                break;
                        }
                    }
                    else
                    {
                        Console.WriteLine($"✅ {plc.PlcName} 运行正常");
                    }
                }
                
                if (productionOk)
                {
                    Console.WriteLine("🟢 生产线运行正常");
                }
                else
                {
                    Console.WriteLine("🔴 生产线需要维护");
                }
                
                await Task.Delay(1500);
            }

            Console.WriteLine("\n=== 工业场景示例完成 ===");
        }
    }
}