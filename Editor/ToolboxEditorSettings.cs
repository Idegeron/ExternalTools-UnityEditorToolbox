﻿using System;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Events;

namespace Toolbox.Editor
{
    using Toolbox.Editor.Drawers;

    public interface IToolboxHierarchySettings
    {
        void AddRowDataItem(HierarchyObjectDataItem item);
        void RemoveRowDataItem(HierarchyObjectDataItem item);
        void RemoveRowDataItemAt(int index);
        HierarchyObjectDataItem GetRowDataItemAt(int index);

        bool UseToolboxHierarchy { get; }
        bool DrawHorizontalLines { get; }

        int RowDataItemsCount { get; }
    }

    public interface IToolboxProjectSettings
    {
        void AddCustomFolder(FolderData path);
        void RemoveCustomFolder(FolderData path);
        void RemoveCustomFolderAt(int index);
        FolderData GetCustomFolderAt(int index);

        bool UseToolboxProject { get; }

        float LargeIconScale { get; }
        float SmallIconScale { get; }

        Vector2 LargeIconPadding { get; }
        Vector2 SmallIconPadding { get; }

        int CustomFoldersCount { get; }
    }

    public interface IToolboxInspectorSettings
    {
        void SetAllPossibleDecoratorDrawers();
        void SetAllPossibleConditionDrawers();
        void SetAllPossibleFieldPropertyDrawers();
        void SetAllPossibleArrayPropertyDrawers();
        void SetAllPossibleTargetTypeDrawers();

        void AddDecoratorDrawerHandler(SerializedType drawerReference);
        void AddConditionDrawerHandler(SerializedType drawerReference);
        void AddFieldPropertyDrawerHandler(SerializedType drawerReference);
        void AddArrayPropertyDrawerHandler(SerializedType drawerReference);
        void AddTargetTypeDrawerHandler(SerializedType drawerReference);

        void RemoveDecoratorDrawerHandler(SerializedType drawerReference);
        void RemoveConditionDrawerHandler(SerializedType drawerReference);
        void RemoveFieldPropertyDrawerHandler(SerializedType drawerReference);
        void RemoveArrayPropertyDrawerHandler(SerializedType drawerReference);
        void RemoveTargetTypeDrawerHandler(SerializedType drawerReference);

        Type GetDecoratorDrawerTypeAt(int index);
        Type GetConditionDrawerTypeAt(int index);
        Type GetFieldPropertyDrawerTypeAt(int index);
        Type GetArrayPropertyDrawerTypeAt(int index);
        Type GetTargetTypeDrawerTypeAt(int index);

        bool UseToolboxDrawers { get; }

        int DecoratorDrawersCount { get; }
        int ConditionDrawersCount { get; }
        int FieldPropertyDrawersCount { get; }
        int ArrayPropertyDrawersCount { get; }
        int TargetTypeDrawersCount { get; }
    }

    public class ToolboxEditorSettings : ScriptableObject, IToolboxHierarchySettings, IToolboxProjectSettings, IToolboxInspectorSettings
    {
        [SerializeField]
        private bool useToolboxHierarchy = true;
        [SerializeField]
        private bool useToolboxFolders = true;
        [SerializeField]
        private bool useToolboxDrawers = true;

        [SerializeField, ReorderableList(ListStyle.Boxed)]
        private List<HierarchyObjectDataItem> rowDataItems = Defaults.rowDataItems;

        [SerializeField]
        private bool drawHorizontalLines = true;

        [SerializeField, Clamp(0.0f, float.MaxValue)]
        private float largeIconScale = Defaults.largeFolderIconScaleDefault;
        [SerializeField, Clamp(0.0f, float.MaxValue)]
        private float smallIconScale = Defaults.smallFolderIconScaleDefault;

        [SerializeField]
        private Vector2 largeIconPadding = new Vector2(Defaults.largeFolderIconXPaddingDefault, Defaults.largeFolderIconYPaddingDefault);
        [SerializeField]
        private Vector2 smallIconPadding = new Vector2(Defaults.smallFolderIconXPaddingDefault, Defaults.smallFolderIconYPaddingDefault);

        [SerializeField, ReorderableList(ListStyle.Boxed)]
        private List<FolderData> customFolders;

        [SerializeField, ReorderableList(ListStyle.Boxed), ClassExtends(typeof(ToolboxDecoratorDrawer<>))]
        private List<SerializedType> decoratorDrawerHandlers;
        [SerializeField, ReorderableList(ListStyle.Boxed), ClassExtends(typeof(ToolboxConditionDrawer<>))]
        private List<SerializedType> conditionDrawerHandlers;
        [SerializeField, ReorderableList(ListStyle.Boxed), ClassExtends(typeof(ToolboxFieldPropertyDrawer<>))]
        private List<SerializedType> fieldPropertyDrawerHandlers;
        [SerializeField, ReorderableList(ListStyle.Boxed), ClassExtends(typeof(ToolboxArrayPropertyDrawer<>))]
        private List<SerializedType> arrayPropertyDrawerHandlers;

        [SerializeField, ReorderableList(ListStyle.Boxed), ClassExtends(typeof(ToolboxTargetTypeDrawer))]
        private List<SerializedType> targetTypeDrawerHandlers;

        [SerializeField, HideInInspector]
        private bool needsUpdate;

        private UnityEvent onSettingsUpdated = new UnityEvent();


        internal void OnValidate()
        {
            needsUpdate = true;
        }

        internal void ForceUpdate()
        {
            needsUpdate = false;
            onSettingsUpdated?.Invoke();
        }


        internal void AddOnSettingsUpdatedListener(UnityAction listener)
        {
            onSettingsUpdated.AddListener(listener);
        }

        internal void RemoveOnSettingsUpdatedListener(UnityAction listener)
        {
            onSettingsUpdated.RemoveListener(listener);
        }

        internal void RemoveAllOnSettingsUpdatedListeners()
        {
            onSettingsUpdated.RemoveAllListeners();
        }


        public void AddRowDataItem(HierarchyObjectDataItem item)
        {
            if (rowDataItems == null)
            {
                rowDataItems = new List<HierarchyObjectDataItem>();
            }

            rowDataItems.Add(item);
        }

        public void RemoveRowDataItem(HierarchyObjectDataItem item)
        {
            rowDataItems?.Remove(item);
        }

        public void RemoveRowDataItemAt(int index)
        {
            rowDataItems?.RemoveAt(index);
        }

        public HierarchyObjectDataItem GetRowDataItemAt(int index)
        {
            return rowDataItems[index];
        }


        public void AddCustomFolder(FolderData data)
        {
            if (customFolders == null)
            {
                customFolders = new List<FolderData>();
            }

            customFolders.Add(data);
        }

        public void RemoveCustomFolder(FolderData data)
        {
            customFolders?.Remove(data);
        }

        public void RemoveCustomFolderAt(int index)
        {
            customFolders?.RemoveAt(index);
        }

        public FolderData GetCustomFolderAt(int index)
        {
            return customFolders[index];
        }


        public void SetAllPossibleDecoratorDrawers()
        {
            decoratorDrawerHandlers?.Clear();

            var types = ToolboxDrawerModule.GetAllPossibleDecoratorDrawers();
            for (var i = 0; i < types.Count; i++)
            {
                AddDecoratorDrawerHandler(new SerializedType(types[i]));
            }
        }

        public void SetAllPossibleConditionDrawers()
        {
            conditionDrawerHandlers?.Clear();

            var types = ToolboxDrawerModule.GetAllPossibleConditionDrawers();
            for (var i = 0; i < types.Count; i++)
            {
                AddConditionDrawerHandler(new SerializedType(types[i]));
            }
        }

        public void SetAllPossibleFieldPropertyDrawers()
        {
            fieldPropertyDrawerHandlers?.Clear();

            var types = ToolboxDrawerModule.GetAllPossibleFieldPropertyDrawers();
            for (var i = 0; i < types.Count; i++)
            {
                AddFieldPropertyDrawerHandler(new SerializedType(types[i]));
            }
        }

        public void SetAllPossibleArrayPropertyDrawers()
        {
            arrayPropertyDrawerHandlers?.Clear();

            var types = ToolboxDrawerModule.GetAllPossibleArrayPropertyDrawers();
            for (var i = 0; i < types.Count; i++)
            {
                AddArrayPropertyDrawerHandler(new SerializedType(types[i]));
            }
        }

        public void SetAllPossibleTargetTypeDrawers()
        {
            targetTypeDrawerHandlers?.Clear();

            var types = ToolboxDrawerModule.GetAllPossibleTargetTypeDrawers();
            for (var i = 0; i < types.Count; i++)
            {
                AddTargetTypeDrawerHandler(new SerializedType(types[i]));
            }
        }

        public void AddDecoratorDrawerHandler(SerializedType drawerReference)
        {
            if (decoratorDrawerHandlers == null)
            {
                decoratorDrawerHandlers = new List<SerializedType>();
            }

            decoratorDrawerHandlers.Add(drawerReference);
        }

        public void AddConditionDrawerHandler(SerializedType drawerReference)
        {
            if (conditionDrawerHandlers == null)
            {
                conditionDrawerHandlers = new List<SerializedType>();
            }

            conditionDrawerHandlers.Add(drawerReference);
        }

        public void AddFieldPropertyDrawerHandler(SerializedType drawerReference)
        {
            if (fieldPropertyDrawerHandlers == null)
            {
                fieldPropertyDrawerHandlers = new List<SerializedType>();
            }

            fieldPropertyDrawerHandlers.Add(drawerReference);
        }

        public void AddArrayPropertyDrawerHandler(SerializedType drawerReference)
        {
            if (arrayPropertyDrawerHandlers == null)
            {
                arrayPropertyDrawerHandlers = new List<SerializedType>();
            }

            arrayPropertyDrawerHandlers.Add(drawerReference);
        }

        public void AddTargetTypeDrawerHandler(SerializedType drawerReference)
        {
            if (targetTypeDrawerHandlers == null)
            {
                targetTypeDrawerHandlers = new List<SerializedType>();
            }

            targetTypeDrawerHandlers.Add(drawerReference);
        }

        public void RemoveDecoratorDrawerHandler(SerializedType drawerReference)
        {
            decoratorDrawerHandlers?.Remove(drawerReference);
        }

        public void RemoveConditionDrawerHandler(SerializedType drawerReference)
        {
            conditionDrawerHandlers?.Remove(drawerReference);
        }

        public void RemoveFieldPropertyDrawerHandler(SerializedType drawerReference)
        {
            fieldPropertyDrawerHandlers?.Remove(drawerReference);
        }

        public void RemoveArrayPropertyDrawerHandler(SerializedType drawerReference)
        {
            arrayPropertyDrawerHandlers?.Remove(drawerReference);
        }

        public void RemoveTargetTypeDrawerHandler(SerializedType drawerReference)
        {
            targetTypeDrawerHandlers?.Remove(drawerReference);
        }

        public Type GetDecoratorDrawerTypeAt(int index)
        {
            return decoratorDrawerHandlers[index];
        }

        public Type GetConditionDrawerTypeAt(int index)
        {
            return conditionDrawerHandlers[index].Type;
        }

        public Type GetFieldPropertyDrawerTypeAt(int index)
        {
            return fieldPropertyDrawerHandlers[index].Type;
        }

        public Type GetArrayPropertyDrawerTypeAt(int index)
        {
            return arrayPropertyDrawerHandlers[index].Type;
        }

        public Type GetTargetTypeDrawerTypeAt(int index)
        {
            return targetTypeDrawerHandlers[index].Type;
        }


        public void ResetIconProperties()
        {
            largeIconScale = Defaults.largeFolderIconScaleDefault;
            smallIconScale = Defaults.smallFolderIconScaleDefault;

            largeIconPadding = new Vector2(Defaults.largeFolderIconXPaddingDefault,
                Defaults.largeFolderIconYPaddingDefault);
            smallIconPadding = new Vector2(Defaults.smallFolderIconXPaddingDefault,
                Defaults.smallFolderIconYPaddingDefault);
        }


        public bool UseToolboxHierarchy
        {
            get => useToolboxHierarchy;
            set => useToolboxHierarchy = value;
        }

        public bool UseToolboxProject
        {
            get => useToolboxFolders;
            set => useToolboxFolders = value;
        }

        public bool UseToolboxDrawers
        {
            get => useToolboxDrawers;
            set => useToolboxDrawers = value;
        }

        public bool DrawHorizontalLines
        {
            get => drawHorizontalLines;
            set => drawHorizontalLines = value;
        }

        public float LargeIconScale
        {
            get => largeIconScale;
            set => largeIconScale = value;
        }

        public float SmallIconScale
        {
            get => smallIconScale;
            set => smallIconScale = value;
        }

        public Vector2 LargeIconPadding
        {
            get => largeIconPadding;
            set => largeIconPadding = value;
        }

        public Vector2 SmallIconPadding
        {
            get => smallIconPadding;
            set => smallIconPadding = value;
        }


        public int RowDataItemsCount => rowDataItems != null ? rowDataItems.Count : 0;

        public int CustomFoldersCount => customFolders != null ? customFolders.Count : 0;


        public int DecoratorDrawersCount => decoratorDrawerHandlers != null ? decoratorDrawerHandlers.Count : 0;

        public int ConditionDrawersCount => conditionDrawerHandlers != null ? conditionDrawerHandlers.Count : 0;

        public int FieldPropertyDrawersCount => fieldPropertyDrawerHandlers != null ? fieldPropertyDrawerHandlers.Count : 0;

        public int ArrayPropertyDrawersCount => arrayPropertyDrawerHandlers != null ? arrayPropertyDrawerHandlers.Count : 0;

        public int TargetTypeDrawersCount => targetTypeDrawerHandlers != null ? targetTypeDrawerHandlers.Count : 0;


        internal bool NeedsUpdate => needsUpdate;


        private static class Defaults
        {
            internal const float largeFolderIconScaleDefault = 0.8f;
            internal const float smallFolderIconScaleDefault = 0.7f;

            internal const float largeFolderIconXPaddingDefault = 0.0f;
            internal const float largeFolderIconYPaddingDefault = 0.15f;
            internal const float smallFolderIconXPaddingDefault = 0.15f;
            internal const float smallFolderIconYPaddingDefault = 0.15f;

            internal readonly static List<HierarchyObjectDataItem> rowDataItems = new List<HierarchyObjectDataItem>()
            {
                HierarchyObjectDataItem.Icon,
                HierarchyObjectDataItem.Toggle,
                HierarchyObjectDataItem.Tag,
                HierarchyObjectDataItem.Layer,
                HierarchyObjectDataItem.Script
            };
        }
    }
}