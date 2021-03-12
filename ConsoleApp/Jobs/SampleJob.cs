using CommonUtility;
using ConsoleApp.Models;
using Microsoft.Extensions.Options;
using System;

namespace ConsoleApp.Jobs
{
    /// <summary>
    /// 主要邏輯程式
    /// </summary>
    public class SampleJob
    {
        private readonly AppSettings _config;

        public SampleJob(IOptions<AppSettings> config)
        {
            _config = config.Value;
        }

        /// <summary>
        /// 執行主要邏輯程式
        /// </summary>
        public void Start()
        {
            ConsoleHelper.WriteLine($"{nameof(SampleJob)}: Job Start!", ConsoleColor.Magenta);
            ConsoleHelper.WriteLine($"{nameof(SampleJob)}: Press enter to continue.", ConsoleColor.Magenta);
            Console.ReadKey();
        }

        /// <summary>
        /// 停止主要邏輯程式後的動作
        /// </summary>
        public void Stop()
        {
            ConsoleHelper.WriteLine($"{nameof(SampleJob)}: Job Stop!", ConsoleColor.Magenta);
        }
    }
}
