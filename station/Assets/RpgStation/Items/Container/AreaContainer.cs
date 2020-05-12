﻿using System;
using System.Collections.Generic;
using UnityEngine;

namespace Station
{
    public class AreaContainer : Interactible
    {
        [SerializeField] private  List<ItemStack> _defaultItems = new List<ItemStack>();
        
        private AreaContainerSystem _containerSystem;
        private ItemsDb _itemDb;
        private ItemsSettingsDb _itemsSettingsDb;
        public string _id;
        
        protected override void Setup()
        {
            GameGlobalEvents.OnSceneLoadObjects.AddListener(OnLoadContainer);
        }
        
        protected override void Dispose()
        {
            GameGlobalEvents.OnSceneLoadObjects.RemoveListener(OnLoadContainer); 
        }

        private void OnLoadContainer()
        {
            InitializeWithDefaultItems(Guid.NewGuid().ToString(), _defaultItems, true);
        }

        private void Initialize(string id)
        {
            _id = id;
            _containerSystem = RpgStation.GetSystemStatic<AreaContainerSystem>();
            var dbSystems = RpgStation.GetSystemStatic<DbSystem>();
            _itemDb = dbSystems.GetDb<ItemsDb>();
            _itemsSettingsDb = dbSystems.GetDb<ItemsSettingsDb>();
        }

        private void InitializeWithDefaultItems(string id, List<ItemStack> items, bool saved)
        {
            Initialize(id);
            var containerState = new ContainerState(8, items);
            var container = new ItemContainer(id, containerState, _itemDb);
            _containerSystem.AddContainer(container, saved);
        }

        
        

        public override void TryInteract(BaseCharacter user)
        {
            var prefab = _itemsSettingsDb.Get().ContainerSettings.ContainerPopup;
            if (prefab == null)
            {
                Debug.LogError($" missing prefab container popup");
            }
            else
            {
                var popup = UiSystem.GetUniquePopup<UiContainerPopup>(UiContainerPopup.POPUP_KEY, prefab);
                popup.Setup(new ContainerReference(_id, RpgStation.GetSystemStatic<AreaContainerSystem>()), user);
                popup.Show();
            }
        }
    }

}
