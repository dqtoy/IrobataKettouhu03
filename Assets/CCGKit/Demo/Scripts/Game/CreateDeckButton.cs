// Copyright (C) 2016-2017 David Pol. All rights reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement,
// a copy of which is available at http://unity3d.com/company/legal/as_terms.

using UnityEngine;

public class CreateDeckButton : MonoBehaviour
{
    [HideInInspector]
    public DeckBuilderScene scene;
//    [HideInInspector]
//    public KoumaDeckBuilderScene kScene;
 //   [HideInInspector]
 //   public HakugyokuDeckBuilderScene hScene;
 //   [HideInInspector]
 //   public EinenDeckBuilderScene eScene;   
    public void OnButtonPressed()
    {
        scene.CreateNewDeck();
    }
}