using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace CCGKit
{
    /// <summary>
    /// Holds information about the in-game chat dialog.
    /// �Q�[�����̃`���b�g�_�C�A���O�Ɋւ������ێ����܂��B
    /// </summary>
    public class ChatDialog : MonoBehaviour
    {
        /// <summary>
        /// Chat scroll view.
        /// �`���b�g�X�N���[���r���[�B
        /// </summary>
        public ScrollRect ChatScrollView;

        /// <summary>
        /// Chat scroll view content.
        /// �`���b�g�X�N���[���r���[�̃R���e���c�B
        /// </summary>
        public GameObject ChatScrollViewContent;

        /// <summary>
        /// Chat entry prefab.
        /// �`���b�g�G���g���v���n�u�B
        /// </summary>
        public GameObject ChatEntryPrefab;

        /// <summary>
        /// Input field component.
        /// ���̓t�B�[���h�R���|�[�l���g�B
        /// </summary>
        public InputField InputField;

        /// <summary>
        /// Maximum length (in characters) allowed for a single chat message.
        /// 1�̃`���b�g���b�Z�[�W�ɋ��e�����ő咷�i�������j�B
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
        /// �{�^���R�[���o�b�N�𑗐M���܂��B
        /// </summary>
        public void OnSendButtonPressed()
        {
            SubmitText();
        }

        /// <summary>
        /// Close button callback.
        /// ����{�^���̃R�[���o�b�N�B
        /// </summary>
        public void OnCloseButtonPressed()
        {
            Hide();
        }

        /// <summary>
        /// Chat input field
        /// �`���b�g���̓t�B�[���h
        /// </summary>
        public void OnChatInputFieldEditEnded()
        {
            // It seems Unity's InputField OnEndEdit event is called in a lot of contexts
            // other than submitting the text from an input field (e.g, clicking on a
            // scrollbar), so make sure we got here only by pressing Enter on an input
            // field.
            //���j�e�B��InputField OnEndEdit�C�x���g�́A���̓t�B�[���h�i�Ⴆ�΁A�X�N���[���o�[���N���b�N����j����e�L�X�g�𑗐M����ȊO�̑����̃R���e�L�X�g�ŌĂяo�����悤�ł��̂ŁA
            //���̓t�B�[���h��Enter�����������ł����ɗ��Ă��邱�Ƃ��m�F���Ă��������B
            if (!Input.GetButtonDown("Submit"))
                return;

            SubmitText();
        }

        /// <summary>
        /// Performs the actual work of submitting the chat text.
        /// �`���b�g�e�L�X�g�𑗐M������ۂ̍�Ƃ��s���܂��B
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
        /// �w�肳�ꂽ�e�L�X�g���`���b�g�_�C�A���O�ɒǉ����܂��B
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
        /// �`���b�g�_�C�A���O��\�����܂��B
        /// </summary>
        public void Show()
        {
            gameObject.GetComponent<CanvasGroup>().alpha = 1;
            gameObject.GetComponent<CanvasGroup>().interactable = true;
            gameObject.GetComponent<CanvasGroup>().blocksRaycasts = true;
        }

        /// <summary>
        /// Hides the chat dialog.
        /// �`���b�g�_�C�A���O���\���ɂ��܂��B
        /// </summary>
        public void Hide()
        {
            gameObject.GetComponent<CanvasGroup>().alpha = 0;
            gameObject.GetComponent<CanvasGroup>().interactable = false;
            gameObject.GetComponent<CanvasGroup>().blocksRaycasts = false;
        }
    }
}
