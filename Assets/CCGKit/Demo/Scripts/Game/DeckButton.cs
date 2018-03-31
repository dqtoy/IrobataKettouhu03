using UnityEngine;
using UnityEngine.UI;

using DG.Tweening;
using TMPro;

using CCGKit;

public class DeckButton : MonoBehaviour
{
    [SerializeField]
    protected Image activeBackground;

    [SerializeField]
    protected TextMeshProUGUI nameText;

    [SerializeField]
    protected TextMeshProUGUI numCardsText;

    [SerializeField]
    protected TextMeshProUGUI numSpellsText;

    [SerializeField]
    protected TextMeshProUGUI numCreaturesText;

    [HideInInspector]
    public DeckBuilderScene scene;

//    [HideInInspector]
//    public KoumaDeckBuilderScene kScene;
//    [HideInInspector]
//   public HakugyokuDeckBuilderScene hScene;    
//    [HideInInspector]
//    public EienDeckBuilderScene eScene;

/// <summary>
/// ���݂̃f�b�L
/// </summary>
    public Deck deck { get; private set; }

    /// <summary>
    /// �N���b�N�����f�b�L�̃J�[�h�ꗗ��\������
    /// </summary>
    public void OnButtonPressed()
    {
        scene.SetActiveDeck(this);
    }

    public void OnDeleteButtonPressed()
    {
        scene.RemoveDeck(deck);
        Destroy(gameObject);
    }

    public void SetActive(bool active)
    {
        if (active)
        {
            //�����x1.0�܂�0.5�b�|���ăt�F�[�h(�����ڂ��A�N�e�B�u��)����
            activeBackground.DOFade(1.0f, 0.5f);
        }
        else
        {
            //�����x0.0�܂�0.2�b�|���ăt�F�[�h(�����ڂ��A�N�e�B�u��)����
            activeBackground.DOFade(0.0f, 0.2f);
        }
    }
    /// <summary>
    /// ���ݑ��삵�Ă���f�b�L���Z�b�g����
    /// </summary>
    /// <param name="deck"></param>
    public void SetDeck(Deck deck)
    {
        this.deck = deck;
        nameText.text = deck.name;
        UpdateDeckInfo();
    }

    public void SetDeckName(string name)
    {
        deck.name = name;
        nameText.text = name;
    }
    /// <summary>
    /// �N���b�N�����\�z�ς݃f�b�L�̏��ɕ\�����X�V����
    /// </summary>
    public void UpdateDeckInfo()
    {
        numCardsText.text = deck.GetNumCards().ToString() + " cards";
        numCreaturesText.text = deck.GetNumCards(GameManager.Instance.config, 0).ToString();
        numSpellsText.text = deck.GetNumCards(GameManager.Instance.config, 1).ToString();
        //scene��DeckBuilderScene�̃C���X�^���X
        scene.UpdateNumCardsText();
    }
}
