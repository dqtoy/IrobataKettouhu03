using UnityEngine;
using UnityEngine.Assertions;

using TMPro;

using CCGKit;

/// <summary>
/// カードの描画情報
/// RuntimeCardクラスも持ってる。各スタッツ、使用可能の光とか
/// </summary>
public class CardView : MonoBehaviour
{
    public RuntimeCard card { get; private set; }

    //使用可能の青い光
    [SerializeField]
    protected SpriteRenderer glowSprite;

    //イラスト
    [SerializeField]
    protected SpriteRenderer pictureSprite;

    [SerializeField]
    protected TextMeshPro costText;

    [SerializeField]
    protected TextMeshPro nameText;

    [SerializeField]
    protected TextMeshPro bodyText;

    protected GameObject previewCard;

    public int manaCost { get; protected set; }

    [HideInInspector]
    public bool isPreview;

    protected virtual void Awake()
    {
        Assert.IsNotNull(glowSprite);
        Assert.IsNotNull(pictureSprite);
        Assert.IsNotNull(costText);
        Assert.IsNotNull(nameText);
        Assert.IsNotNull(bodyText);
    }

    /// <summary>
    /// カードの情報を取り込む
    /// </summary>
    /// <param name="card"></param>
    public virtual void PopulateWithInfo(RuntimeCard card)
    {
        this.card = card;

        var gameConfig = GameManager.Instance.config;
        /*
        
        var cardType = gameConfig.cardTypes.Find(x => x.id == card.cardId);
        Assert.IsNotNull(cardType);
        pictureSprite.sprite = Resources.Load<Sprite>(cardType.GetStringProperty("Picture"));
        
         */

        var libraryCard = gameConfig.GetCard(card.cardId);
        Assert.IsNotNull(libraryCard);
        nameText.text = libraryCard.name;
        bodyText.text = libraryCard.GetStringProperty("Text");

        var cost = libraryCard.costs.Find(x => x is PayResourceCost);
        if (cost != null)
        {
            var payResourceCost = cost as PayResourceCost;
            manaCost = payResourceCost.value;
            costText.text = manaCost.ToString();
        }

        pictureSprite.sprite = Resources.Load<Sprite>(string.Format("Images/{0}", libraryCard.GetStringProperty("Picture")));
        var material = libraryCard.GetStringProperty("Material");
        if (!string.IsNullOrEmpty(material))
        {
            pictureSprite.material = Resources.Load<Material>(string.Format("Materials/{0}", material));
        }
    }

    /// <summary>
    /// ライブラリーの情報を移行
    /// </summary>
    /// <param name="card"></param>
    public virtual void PopulateWithLibraryInfo(Card card)
    {
        nameText.text = card.name;
        bodyText.text = card.GetStringProperty("Text");

        var cost = card.costs.Find(x => x is PayResourceCost);
        if (cost != null)
        {
            var payResourceCost = cost as PayResourceCost;
            manaCost = payResourceCost.value;
            costText.text = manaCost.ToString();
        }

        pictureSprite.sprite = Resources.Load<Sprite>(string.Format("Images/{0}", card.GetStringProperty("Picture")));
        var material = card.GetStringProperty("Material");
        if (!string.IsNullOrEmpty(material))
        {
            pictureSprite.material = Resources.Load<Material>(string.Format("Materials/{0}", material));
        }
    }

    /// <summary>
    /// プレイできるか
    /// </summary>
    /// <param name="owner"></param>
    /// <returns></returns>
    public virtual bool CanBePlayed(DemoHumanPlayer owner)
    {
        return owner.isActivePlayer && owner.manaStat.effectiveValue >= manaCost;
    }

    /// <summary>
    /// ハイライト付いてるか判定
    /// </summary>
    /// <returns></returns>
    public bool IsHighlighted()
    {
        return glowSprite.enabled;
    }

    /// <summary>
    /// ハイライトをセット
    /// </summary>
    /// <param name="enabled"></param>
    public void SetHighlightingEnabled(bool enabled)
    {
        glowSprite.enabled = enabled;
    }
}
