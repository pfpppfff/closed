using S7.Net;
using System;
using System.Threading.Tasks;

namespace S7NET.Test
{
    /// <summary>
    /// 测试S7.NET库的异步方法
    /// </summary>
    public class S7NetAsyncTest
    {
        private Plc _plc;

        public S7NetAsyncTest()
        {
            _plc = new Plc(CpuType.S71500, "192.168.1.10", 0, 1);
        }

        public async Task TestAsyncMethods()
        {
            try
            {
                // 测试异步连接
                await _plc.OpenAsync();

                // 测试异步读取
                var readResult = await _plc.ReadAsync(DataType.Memory, 0, 0, VarType.Int, 1);
                Console.WriteLine($"异步读取结果: {readResult}");

                // 测试异步写入
                await _plc.WriteAsync(DataType.Memory, 0, 0, 12345);
                Console.WriteLine("异步写入完成");

                // 测试异步关闭
                _plc.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"异步操作失败: {ex.Message}");
            }
        }
    }
}
