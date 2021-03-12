using CommonUtility;
using Microsoft.Extensions.Options;
using System;
using WinServiceApp.Models;

namespace WinServiceApp.WinServices
{
    /// <summary>
    /// 主要邏輯程式
    /// </summary>
    public class SampleWinService
    {
        private readonly AppSettings _config;

        public SampleWinService(IOptions<AppSettings> config)
        {
            _config = config.Value;
        }

        /// <summary>
        /// 執行主要邏輯程式
        /// </summary>
        public void Start()
        {
            ConsoleHelper.WriteLine($"{nameof(SampleWinService)}: Service Start!", ConsoleColor.Magenta);
        }

        /// <summary>
        /// 停止主要邏輯程式後的動作
        /// </summary>
        public void Stop()
        {
            ConsoleHelper.WriteLine($"{nameof(SampleWinService)}: Service Stop!", ConsoleColor.Magenta);
        }
    }
}
