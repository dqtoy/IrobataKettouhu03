using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.Assertions;

using TMPro;

using CCGKit;

public class HeroPowerView : MonoBehaviour {

    public RuntimeHeroPower HeroPower { get; private set; }

    [SerializeField]
    protected SpriteRenderer glowSprite;

    [SerializeField]
    protected SpriteRenderer pictureSprite;

    [SerializeField]
    protected TextMeshPro costText;

    [SerializeField]
    protected TextMeshPro nameText;

    [SerializeField]
    protected TextMeshPro bodyText;

    protected GameObject previewHeroPower;

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

    public virtual void PopulateWithInfo(RuntimeHeroPower HeroPower)
    {
        this.HeroPower = HeroPower;

        var gameConfig = GameManager.Instance.config;
        /*
        
        var cardType = gameConfig.cardTypes.Find(x => x.id == card.cardId);
        Assert.IsNotNull(cardType);
        pictureSprite.sprite = Resources.Load<Sprite>(cardType.GetStringProperty("Picture"));
        
         */
         //ヒロパの一覧を取得
        var libraryHeroPower = gameConfig.GetHeroPower(HeroPower.HeroPowerId);
        Assert.IsNotNull(libraryHeroPower);
        nameText.text = libraryHeroPower.name;
        bodyText.text = libraryHeroPower.GetStringProperty("Text");

        var cost = libraryHeroPower.costs.Find(x => x is PayResourceCost);
        if (cost != null)
        {
            var payResourceCost = cost as PayResourceCost;
            manaCost = payResourceCost.value;
            costText.text = manaCost.ToString();
        }

        pictureSprite.sprite = Resources.Load<Sprite>(string.Format("Images/{0}", libraryHeroPower.GetStringProperty("Picture")));
        var material = libraryHeroPower.GetStringProperty("Material");
        if (!string.IsNullOrEmpty(material))
        {
            pictureSprite.material = Resources.Load<Material>(string.Format("Materials/{0}", material));
        }
    }

}
