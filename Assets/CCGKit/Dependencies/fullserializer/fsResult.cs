using System;
using System.Collections.Generic;
using System.Linq;

namespace FullSerializer {
    /// <summary>
    /// The result of some sort of operation. A result is either successful or not, but if it
    /// is successful then there may be a set of warnings/messages associated with it. These
    /// warnings describe the performed error recovery operations.
    /// 何らかの操作の結果。 結果は成功したかどうかですが、成功した場合はそれに関連する一連の警告/メッセージが存在する可能性があります。 これらの警告は、実行されたエラー回復操作を記述します。
    /// </summary>
    public struct fsResult
    {
        // We cache the empty string array so we can unify some collections processing code.
        //私たちは空の文字列配列をキャッシュして、いくつかのコレクション処理コードを統一することができます。
        private static readonly string[] EmptyStringArray = { };

        /// <summary>
        /// Is this result successful?
        /// </summary>
        /// <remarks>This is intentionally a `success` state so that when the object
        /// is default constructed it defaults to a failure state.</remarks>
        /// これは、オブジェクトがデフォルトで構築されたときにデフォルトで失敗状態になるように、意図的に「成功」状態です。
        private bool _success;

        /// <summary>
        /// The warning or error messages associated with the result. This may be null if
        /// there are no messages.
        /// 結果に関連する警告メッセージまたはエラーメッセージ。 メッセージがない場合は、nullになることがあります。
        /// </summary>
        private List<string> _messages;

        /// <summary>
        /// Adds a new message to this result.
        /// この結果に新しいメッセージを追加します。
        /// </summary>
        /// <param name="message"></param>
        public void AddMessage(string message) {
            if (_messages == null) {
                _messages = new List<string>();
            }

            _messages.Add(message);
        }

        /// <summary>
        /// Adds only the messages from the other result into this result, ignoring
        /// the success/failure status of the other result.
        /// 他の結果の成功/失敗のステータスを無視して、この結果に他の結果のメッセージのみを追加します。
        /// </summary>
        public void AddMessages(fsResult result) {
            if (result._messages == null) {
                return;
            }

            if (_messages == null) {
                _messages = new List<string>();
            }

            _messages.AddRange(result._messages);
        }

        /// <summary>
        /// Merges the other result into this one. If the other result failed, then
        /// this one too will have failed.
        /// 他の結果をこれにマージします。 他の結果が失敗した場合、これも失敗します。
        /// </summary>
        /// <remarks>
        /// Note that you can use += instead of this method so that you don't bury
        /// the actual method call that is generating the other fsResult.
        /// このメソッドの代わりに+ =を使用すると、他のfsResultを生成している実際のメソッド呼び出しを埋め込まないことに注意してください。
        /// </remarks>
        public fsResult Merge(fsResult other) {
            // Copy success over
            _success = _success && other._success;

            // Copy messages over
            if (other._messages != null) {
                if (_messages == null) _messages = new List<string>(other._messages);
                else _messages.AddRange(other._messages);
            }

            return this;
        }

        /// <summary>
        /// A successful result.
        /// </summary>
        public static fsResult Success = new fsResult {
            _success = true
        };

        /// <summary>
        /// Create a result that is successful but contains the given warning message.
        /// 成功したが、指定された警告メッセージを含む結果を作成します。
        /// </summary>
        public static fsResult Warn(string warning) {
            return new fsResult {
                _success = true,
                _messages = new List<string> { warning }
            };
        }

        /// <summary>
        /// Create a result that failed.
        /// 失敗した結果を作成します。
        /// </summary>
        public static fsResult Fail(string warning) {
            return new fsResult {
                _success = false,
                _messages = new List<string> { warning }
            };
        }

        // TODO: how to make sure this is only used as +=?
        //これが+ =としてのみ使用されていることを確認する方法？

        /// <summary>
        /// Only use this as +=!
        /// これを+ =！としてのみ使用してください。
        /// </summary>
        public static fsResult operator +(fsResult a, fsResult b) {
            return a.Merge(b);
        }

        /// <summary>
        /// Did this result fail? If so, you can see the reasons why in `RawMessages`.
        /// この結果は失敗しましたか？ もしそうなら、RawMessagesで理由を見ることができます。
        /// </summary>
        public bool Failed {
            get {
                return _success == false;
            }
        }

        /// <summary>
        /// Was the result a success? Note that even successful operations may have
        /// warning messages (`RawMessages`) associated with them.
        /// 結果は成功しましたか？ 正常な操作でさえ警告メッセージ（ `RawMessages`）がそれらに関連付けられていることに注意してください。
        /// </summary>
        public bool Succeeded {
            get {
                return _success;
            }
        }

        /// <summary>
        /// Does this result have any warnings? This says nothing about if it failed
        /// or succeeded, just if it has warning messages associated with it.
        /// この結果に警告がありますか？ これは、警告メッセージが関連付けられている場合にのみ、失敗したか成功したかについては何も言いません。
        /// </summary>
        public bool HasWarnings {
            get {
                return _messages != null && _messages.Any();
            }
        }

        /// <summary>
        /// A simply utility method that will assert that this result is successful. If it
        /// is not, then an exception is thrown.
        /// この結果が成功すると主張する単純なユーティリティメソッドです。 そうでない場合は、例外がスローされます。
        /// </summary>
        public fsResult AssertSuccess() {
            if (Failed) throw AsException;
            return this;
        }

        /// <summary>
        /// A simple utility method that will assert that this result is successful and that
        /// there are no warning messages. This throws an exception if either of those
        /// asserts are false.
        /// この結果が成功し、警告メッセージがないことを主張する簡単なユーティリティメソッド。 これらのアサーションのいずれかがfalseの場合、例外がスローされます。
        /// </summary>
        public fsResult AssertSuccessWithoutWarnings() {
            if (Failed || RawMessages.Any()) throw AsException;
            return this;
        }

        /// <summary>
        /// Utility method to convert the result to an exception. This method is only defined
        /// is `Failed` returns true.
        /// 結果を例外に変換するユーティリティメソッド。 このメソッドは `Failed`がtrueを返すだけで定義されます。
        /// </summary>
        public Exception AsException {
            get {
                if (!Failed && !RawMessages.Any()) throw new Exception("Only a failed result can be converted to an exception");
                return new Exception(FormattedMessages);
            }
        }

        public IEnumerable<string> RawMessages {
            get {
                if (_messages != null) {
                    return _messages;
                }
                return EmptyStringArray;
            }
        }

        public string FormattedMessages {
            get {
                return string.Join(",\n", RawMessages.ToArray());
            }
        }
    }
}