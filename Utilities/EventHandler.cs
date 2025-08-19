using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class EventHandler 
{
    public static event Action<InventoryLocation, List<InventoryItem>> UpdateInventoryUI;
    public static void CallUpdateInventoryUI(InventoryLocation location, List<InventoryItem> list)
    {
        UpdateInventoryUI?.Invoke(location, list);
    }

    public static event Action<int, Vector3> InstantiateItemScene;
    public static void CallInstantiateItemScene(int ID,Vector3 pos)
    {
        InstantiateItemScene?.Invoke(ID, pos);
    }

    public static event Action<int, Vector3> DropItemEvent;
    public static void CallDropItemEvent(int ID,Vector3 pos)
    {
        DropItemEvent?.Invoke(ID, pos);
    }

    public static event Action<string, Vector3> TransitiongEvent;
    public static void CallTransitionEvent(string sceneName,Vector3 pos)
    {
        TransitiongEvent?.Invoke(sceneName, pos);
    }

    public static event Action BeforeSceneUnloadEvent;
    public static void CallBeforeSceneUnloadEvent()
    {
        BeforeSceneUnloadEvent?.Invoke();
    }

    public static event Action AfterSceneLoadedEvent;
    public static void CallAfterSceneLoadedEvent()
    {
        AfterSceneLoadedEvent?.Invoke();
    }

    public static event Action<Vector3> MoveToPosition;
    public static void CallMoveToPosition(Vector3 targetPosition)
    {
        MoveToPosition?.Invoke(targetPosition);
    }

    public static event Action<ItemDetails, bool> ItemSelectedEvent;
    public static void CallItemSelectedEvent(ItemDetails itemDetails, bool isSelected)
    {
        ItemSelectedEvent?.Invoke(itemDetails, isSelected);
    }

    public static event Action<Vector3, ItemDetails> MouseClickedEvent;
    public static void CallMouseClickEvent(Vector3 pos,ItemDetails itemDetails)
    {
        MouseClickedEvent?.Invoke(pos, itemDetails);
    }

    public static event Action<Vector3, ItemDetails> ExecuteActionAfterAnimation;
    public static void CallExecuteActionAfterAnimation(Vector3 pos,ItemDetails itemDetails)
    {
        ExecuteActionAfterAnimation?.Invoke(pos, itemDetails);
    }

    //建造
    public static event Action<int,Vector3> BuildFurnitureEvent;
    public static void CallBuildFurnitureEvent(int ID,Vector3 pos)
    {
        BuildFurnitureEvent?.Invoke(ID,pos);
    }
    //音效
    public static event Action<SoundDetails> InitSoundEffect;
    public static void CallInitSoundEffect(SoundDetails soundDetails)
    {
        InitSoundEffect?.Invoke(soundDetails);
    }
    //视频
    public static event Action VideoPlaybackFinishedEvent;
    public static void CallVideoPlaybackFinishedEvent()
    {
        VideoPlaybackFinishedEvent?.Invoke();
    }

    public static event Action<ParticleEffectType, Vector3> ParticleEffectEvent;//粒子效果
    public static void CallParticleEffectEvent(ParticleEffectType effectType, Vector3 pos)
    {
        ParticleEffectEvent?.Invoke(effectType, pos);
    }
}
