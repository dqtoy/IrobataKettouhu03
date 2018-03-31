﻿using UnityEngine;
using UnityEngine.Assertions;

using TMPro;

using CCGKit;

public class CreatureCardView : CardView
{
    [SerializeField]
    protected SpriteRenderer attackSprite;

    [SerializeField]
    protected TextMeshPro attackText;

    [SerializeField]
    protected SpriteRenderer defenseSprite;

    [SerializeField]
    protected TextMeshPro defenseText;


//    [SerializeField]
 //   protected TextMeshPro KeywordsText;

    public Stat attackStat { get; protected set; }
    public Stat defenseStat { get; protected set; }

    protected override void Awake()
    {
        base.Awake();
        Assert.IsNotNull(attackSprite);
        Assert.IsNotNull(attackText);
        Assert.IsNotNull(defenseSprite);
        Assert.IsNotNull(defenseText);
        //Assert.IsNotNull(KeywordsText);
    }

    public override void PopulateWithInfo(RuntimeCard card)
    {
        base.PopulateWithInfo(card);
        attackStat = card.namedStats["Attack"];
        defenseStat = card.namedStats["Life"];
        attackText.text = attackStat.effectiveValue.ToString();
        defenseText.text = defenseStat.effectiveValue.ToString();

        attackStat.onValueChanged += (oldValue, newValue) => { attackText.text = attackStat.effectiveValue.ToString(); };
        defenseStat.onValueChanged += (oldValue, newValue) => { defenseText.text = defenseStat.effectiveValue.ToString(); };
    }

    public override void PopulateWithLibraryInfo(Card card)
    {
        base.PopulateWithLibraryInfo(card);
        attackText.text = card.stats[0].effectiveValue.ToString();
        defenseText.text = card.stats[1].effectiveValue.ToString();
    }
}