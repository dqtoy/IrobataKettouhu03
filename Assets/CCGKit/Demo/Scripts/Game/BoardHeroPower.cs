using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using DG.Tweening;
using TMPro;

using CCGKit;


/// <summary>
/// BoardCreatureクラスのパクリ
/// </summary>
public class BoardHeroPower : MonoBehaviour {

    public RuntimeCard card { get; private set; }

    [HideInInspector]
    public GameObject fightTargetingArrowPrefab;

    [SerializeField]
    protected SpriteRenderer glowSprite;

    [SerializeField]
    protected SpriteRenderer shadowSprite;

    [HideInInspector]
    public TargetingArrow abilitiesTargetingArrow;
    public Stat attackStat { get; protected set; }

    [HideInInspector]
    public bool isPlayable;

    protected Action<int, int> onAttackStatChangedDelegate;
    protected Action<int, int> onHealthStatChangedDelegate;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
