using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace 控件拖拽功能
{
     class ApiHelper
    {
        // 静态异步方法
        public static  async  Task<string>  GetHttp()
        {
            using (HttpClient client = new HttpClient())
            {
                string url = "https://jsonplaceholder.typicode.com/posts/1"; // 示例API
                try
                {
                    HttpResponseMessage response = await client.GetAsync(url);
                    response.EnsureSuccessStatusCode(); // 确保请求成功
                    string responseBody = await response.Content.ReadAsStringAsync();
                    // 读取响应内容并返回
                    Console.WriteLine(responseBody);
                    return await response.Content.ReadAsStringAsync();
               
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"请求失败: {ex.Message}");
                    return $"请求失败: {ex.Message}";
                }

              
            }
        }

        public static async Task PostHttp()
        {
            using (HttpClient client = new HttpClient())
            {
               // string url = "https://jsonplaceholder.typicode.com/posts";
                string url = "http://127.0.0.1:4523/m2/5938716-5626596-default/265989139";
                // 设置请求头
                client.DefaultRequestHeaders.Add("Authorization", "Bearer your_token_here");
                client.DefaultRequestHeaders.Add("Accept", "application/json");
                var postData = new
                {
                    //title = "foo",
                    //body = "bar",
                    //userId = 1
                    flow = "100",
                   speed = "101"
                  
                };

                string jsonContent = Newtonsoft.Json.JsonConvert.SerializeObject(postData); // 需要引用 Newtonsoft.Json
                HttpContent content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                //try
                //{
                    HttpResponseMessage response = await client.PostAsync(url, content);
                    response.EnsureSuccessStatusCode(); // 确保请求成功
                   
                    if (response.IsSuccessStatusCode)
                    {
                        MessageBox.Show("发送成功！");
                    }
                    else
                    {
                        MessageBox.Show("发送失败！");
                    }
                    string responseBody = await response.Content.ReadAsStringAsync();
                    Console.WriteLine(responseBody);
                //}
                //catch (Exception ex)
                //{
                //    Console.WriteLine($"请求失败: {ex.Message}");
                //}
            }
        }
    }
}
