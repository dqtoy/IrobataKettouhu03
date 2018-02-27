using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace CCGKit
{
    /// <summary>
    /// Holds information about the in-game chat dialog.
    /// ゲーム内のチャットダイアログに関する情報を保持します。
    /// </summary>
    public class ChatDialog : MonoBehaviour
    {
        /// <summary>
        /// Chat scroll view.
        /// チャットスクロールビュー。
        /// </summary>
        public ScrollRect ChatScrollView;

        /// <summary>
        /// Chat scroll view content.
        /// チャットスクロールビューのコンテンツ。
        /// </summary>
        public GameObject ChatScrollViewContent;

        /// <summary>
        /// Chat entry prefab.
        /// チャットエントリプレハブ。
        /// </summary>
        public GameObject ChatEntryPrefab;

        /// <summary>
        /// Input field component.
        /// 入力フィールドコンポーネント。
        /// </summary>
        public InputField InputField;

        /// <summary>
        /// Maximum length (in characters) allowed for a single chat message.
        /// 1つのチャットメッセージに許容される最大長（文字数）。
        /// </summary>
        private static readonly int maxChatMessageLength = 50;

        private void Awake()
        {
            Assert.IsTrue(ChatScrollView != null);
            Assert.IsTrue(ChatScrollViewContent != null);
            Assert.IsTrue(ChatEntryPrefab != null);
            Assert.IsTrue(InputField != null);
        }

        private void Start()
        {
            InputField.ActivateInputField();
        }

        /// <summary>
        /// Send button callback.
        /// ボタンコールバックを送信します。
        /// </summary>
        public void OnSendButtonPressed()
        {
            SubmitText();
        }

        /// <summary>
        /// Close button callback.
        /// 閉じるボタンのコールバック。
        /// </summary>
        public void OnCloseButtonPressed()
        {
            Hide();
        }

        /// <summary>
        /// Chat input field
        /// チャット入力フィールド
        /// </summary>
        public void OnChatInputFieldEditEnded()
        {
            // It seems Unity's InputField OnEndEdit event is called in a lot of contexts
            // other than submitting the text from an input field (e.g, clicking on a
            // scrollbar), so make sure we got here only by pressing Enter on an input
            // field.
            //ユニティのInputField OnEndEditイベントは、入力フィールド（例えば、スクロールバーをクリックする）からテキストを送信する以外の多くのコンテキストで呼び出されるようですので、
            //入力フィールドでEnterを押すだけでここに来ていることを確認してください。
            if (!Input.GetButtonDown("Submit"))
                return;

            SubmitText();
        }

        /// <summary>
        /// Performs the actual work of submitting the chat text.
        /// チャットテキストを送信する実際の作業を行います。
        /// </summary>
        public void SubmitText()
        {
            var localPlayer = NetworkingUtils.GetHumanLocalPlayer();
            if (localPlayer != null)
            {
                var msg = new SendChatTextMessage();
                msg.senderNetId = localPlayer.netId;
                msg.text = InputField.text;
                if (msg.text.Length > maxChatMessageLength)
                    msg.text = msg.text.Substring(0, maxChatMessageLength);
                NetworkManager.singleton.client.Send(NetworkProtocol.SendChatTextMessage, msg);
                InputField.text = string.Empty;
                InputField.ActivateInputField();
            }
        }

        /// <summary>
        /// Adds the specified text to the chat dialog.
        /// 指定されたテキストをチャットダイアログに追加します。
        /// </summary>
        /// <param name="text">Text to add to the chat dialog.</param>
        public void AddTextEntry(string text)
        {
            var go = Instantiate(ChatEntryPrefab) as GameObject;
            go.transform.SetParent(ChatScrollViewContent.transform, false);
            go.GetComponent<Text>().text = text;
            ChatScrollView.velocity = new Vector2(0.0f, 1000.0f);
        }

        /// <summary>
        /// Shows the chat dialog.
        /// チャットダイアログを表示します。
        /// </summary>
        public void Show()
        {
            gameObject.GetComponent<CanvasGroup>().alpha = 1;
            gameObject.GetComponent<CanvasGroup>().interactable = true;
            gameObject.GetComponent<CanvasGroup>().blocksRaycasts = true;
        }

        /// <summary>
        /// Hides the chat dialog.
        /// チャットダイアログを非表示にします。
        /// </summary>
        public void Hide()
        {
            gameObject.GetComponent<CanvasGroup>().alpha = 0;
            gameObject.GetComponent<CanvasGroup>().interactable = false;
            gameObject.GetComponent<CanvasGroup>().blocksRaycasts = false;
        }
    }
}
