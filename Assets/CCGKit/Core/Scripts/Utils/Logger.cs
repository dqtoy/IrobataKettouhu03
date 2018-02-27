using System.Diagnostics;

using UnityEngine;

namespace CCGKit
{
    /// <summary>
    /// This class wraps Unity's Debug.Log with its own methods marked with the Conditional attribute
    /// so that logging only happens when explicitly enabled via a conditional compilation symbol.
    /// このクラスはUnityのDebug.LogをConditional属性でマークされた独自のメソッドでラップするので、条件付きコンパイルシンボルで明示的に有効にしたときにのみロギングが行われます。
    /// </summary>
    public sealed class Logger
    {
        public const string LOGGER_SYMBOL = "ENABLE_LOG";

        [Conditional(LOGGER_SYMBOL)]
        public static void Log(string message)
        {
            UnityEngine.Debug.Log(message);
        }

        [Conditional(LOGGER_SYMBOL)]
        public static void Log(string message, Object context)
        {
            UnityEngine.Debug.Log(message, context);
        }

        [Conditional(LOGGER_SYMBOL)]
        public static void LogWarning(string message)
        {
            UnityEngine.Debug.LogWarning(message);
        }

        [Conditional(LOGGER_SYMBOL)]
        public static void LogWarning(string message, Object context)
        {
            UnityEngine.Debug.LogWarning(message, context);
        }

        [Conditional(LOGGER_SYMBOL)]
        public static void LogError(string message)
        {
            UnityEngine.Debug.LogError(message);
        }

        [Conditional(LOGGER_SYMBOL)]
        public static void LogError(string message, Object context)
        {
            UnityEngine.Debug.LogError(message, context);
        }
    }
}