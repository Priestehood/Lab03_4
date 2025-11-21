using System;
using System.Diagnostics;

namespace Lab03_4.MyForms.FeatureClassManagement.Helpers
{
    /// <summary>
    /// 简单的日志记录工具
    /// </summary>
    public static class Logger
    {
        public static void Info(string message)
        {
            Debug.WriteLine($"[INFO] {DateTime.Now:yyyy-MM-dd HH:mm:ss} - {message}");
        }

        public static void Warn(string message)
        {
            Debug.WriteLine($"[WARN] {DateTime.Now:yyyy-MM-dd HH:mm:ss} - {message}");
        }

        public static void Error(string message, Exception ex = null)
        {
            Debug.WriteLine($"[ERROR] {DateTime.Now:yyyy-MM-dd HH:mm:ss} - {message}");
            if (ex != null)
            {
                Debug.WriteLine($"[EXCEPTION] {ex}");
            }
        }
    }
}