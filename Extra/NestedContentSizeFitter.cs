//This code actually comes from https://gist.github.com/TobiasPott/47b02cfb27d2d31ffd7f7b8658535c98
//It's not a part of Night Optimization Kit.

using System;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.UI;
#endif
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace VRTX.UI
{
    [ExecuteAlways]
    [RequireComponent(typeof(RectTransform))]
    public class NestedContentSizeFitter : UIBehaviour, ILayoutSelfController
    {
        [Serializable]
        public enum FitMode
        {
            /// <summary>
            /// Don't perform any resizing.
            /// </summary>
            Unconstrained = 0,
            /// <summary>
            /// Resize to the minimum size of the content.
            /// </summary>
            MinSize,
            /// <summary>
            /// Resize to the preferred size of the content.
            /// </summary>
            PreferredSize,
            /// <summary>
            /// Resize to the preferred size of the content with size limit applied
            /// </summary>
            LimitedPreferredSize
        }

        protected Vector2 m_ContentSize = Vector2.zero;
        public RectTransform m_ReferenceTransform = null;

        [SerializeField] protected FitMode m_HorizontalFit = FitMode.Unconstrained;

        [SerializeField] protected float m_HorizontalLimit = 200;
        [SerializeField] protected float m_HorizontalAllowance = 0;

        [SerializeField] protected FitMode m_VerticalFit = FitMode.Unconstrained;

        [SerializeField] protected float m_VerticalLimit = 200;
        [SerializeField] protected float m_VerticalAllowance = 0;



        /// <summary>
        /// The fit mode to use to determine the width.
        /// </summary>
        public FitMode horizontalFit { get { return m_HorizontalFit; } set { if (SetPropertyUtility.SetStruct(ref m_HorizontalFit, value)) SetDirty(); } }

        /// <summary>
        /// The fit mode to use to determine the height.
        /// </summary>
        public FitMode verticalFit { get { return m_VerticalFit; } set { if (SetPropertyUtility.SetStruct(ref m_VerticalFit, value)) SetDirty(); } }



        [System.NonSerialized] private RectTransform m_Rect;
        private RectTransform rectTransform
        {
            get
            {
                if (m_Rect == null)
                    m_Rect = GetComponent<RectTransform>();
                return m_Rect;
            }
        }

        private DrivenRectTransformTracker m_Tracker;

        protected NestedContentSizeFitter()
        { }

        protected override void OnEnable()
        {
            base.OnEnable();
            SetDirty();

            m_ContentSize = m_ReferenceTransform.rect.size;
        }

        protected override void OnDisable()
        {
            m_Tracker.Clear();
            LayoutRebuilder.MarkLayoutForRebuild(rectTransform);
            base.OnDisable();
        }

        private void LateUpdate()
        {
            if (m_ReferenceTransform == null)
                return;

            Vector2 newSize = m_ReferenceTransform.rect.size;
            bool isDirty = false;
            if (horizontalFit != FitMode.Unconstrained)
            {
                if (m_ContentSize.x != newSize.x)
                    isDirty = true;
                m_ContentSize.x = newSize.x;
            }
            if (verticalFit != FitMode.Unconstrained)
            {
                if (m_ContentSize.y != newSize.y)
                    isDirty = true;
                m_ContentSize.y = newSize.y;
            }
            if (isDirty)
                this.SetDirty();
        }

        protected override void OnRectTransformDimensionsChange()
        {
            SetDirty();
        }

        private void HandleSelfFittingAlongAxis(int axis)
        {
            FitMode fitting = (axis == 0 ? horizontalFit : verticalFit);
            if (fitting == FitMode.Unconstrained)
            {
                // Keep a reference to the tracked transform, but don't control its properties:
                m_Tracker.Add(this, rectTransform, DrivenTransformProperties.None);
                return;
            }

            m_Tracker.Add(this, rectTransform, (axis == 0 ? DrivenTransformProperties.SizeDeltaX : DrivenTransformProperties.SizeDeltaY));

            if (m_ReferenceTransform != null)
            {
                // Set size to min or preferred size
                if (fitting == FitMode.MinSize)
                    rectTransform.SetSizeWithCurrentAnchors((RectTransform.Axis)axis, LayoutUtility.GetMinSize(m_ReferenceTransform, axis));
                else if (fitting == FitMode.PreferredSize)
                    rectTransform.SetSizeWithCurrentAnchors((RectTransform.Axis)axis, LayoutUtility.GetPreferredSize(m_ReferenceTransform, axis));
                else
                {
                    float sizeLimit = (axis == 0 ? m_HorizontalLimit : m_VerticalLimit);
                    float allowance = (axis == 0 ? m_HorizontalAllowance : m_VerticalAllowance);
                    rectTransform.SetSizeWithCurrentAnchors((RectTransform.Axis)axis, Mathf.Min(LayoutUtility.GetPreferredSize(m_ReferenceTransform, axis), sizeLimit) + allowance);
                }
            }
        }

        /// <summary>
        /// Calculate and apply the horizontal component of the size to the RectTransform
        /// </summary>
        public virtual void SetLayoutHorizontal()
        {
            m_Tracker.Clear();
            HandleSelfFittingAlongAxis(0);
        }

        /// <summary>
        /// Calculate and apply the vertical component of the size to the RectTransform
        /// </summary>
        public virtual void SetLayoutVertical()
        {
            HandleSelfFittingAlongAxis(1);
        }

        public void SetDirty()
        {
            if (!IsActive())
                return;

            LayoutRebuilder.MarkLayoutForRebuild(rectTransform);
        }

#if UNITY_EDITOR
        protected override void OnValidate()
        {
            SetDirty();
        }

#endif
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(NestedContentSizeFitter), true)]
    [CanEditMultipleObjects]
    /// <summary>
    /// Custom Editor for the NestedContentSizeFitter Component.
    /// Extend this class to write a custom editor for a component derived from NestedContentSizeFitter.
    /// </summary>
    public class NestedContentSizeFitterEditor : SelfControllerEditor
    {
        SerializedProperty m_ReferenceTransform;
        SerializedProperty m_HorizontalFit;
        SerializedProperty m_HorizontalLimit;
        SerializedProperty m_HorizontalAllowance;
        SerializedProperty m_VerticalFit;
        SerializedProperty m_VerticalLimit;
        SerializedProperty m_VerticalAllowance;

        protected virtual void OnEnable()
        {
            m_ReferenceTransform = serializedObject.FindProperty("m_ReferenceTransform");
            m_HorizontalFit = serializedObject.FindProperty("m_HorizontalFit");
            m_HorizontalLimit = serializedObject.FindProperty("m_HorizontalLimit");
            m_HorizontalAllowance = serializedObject.FindProperty("m_HorizontalAllowance");
            m_VerticalFit = serializedObject.FindProperty("m_VerticalFit");
            m_VerticalLimit = serializedObject.FindProperty("m_VerticalLimit");
            m_VerticalAllowance = serializedObject.FindProperty("m_VerticalAllowance");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            EditorGUILayout.PropertyField(m_ReferenceTransform, true);

            if (m_ReferenceTransform.objectReferenceValue == null)
                EditorGUILayout.HelpBox($"A {nameof(RectTransform)} is required for {nameof(NestedContentSizeFitter.FitMode.PreferredSize)} or {nameof(NestedContentSizeFitter.FitMode.LimitedPreferredSize)}", MessageType.Warning);
            EditorGUILayout.PropertyField(m_HorizontalFit, true);
            if (m_HorizontalFit.enumValueIndex == 3)
            {
                EditorGUILayout.PropertyField(m_HorizontalLimit, true);
                EditorGUILayout.PropertyField(m_HorizontalAllowance, true);
            }
            EditorGUILayout.PropertyField(m_VerticalFit, true);
            if (m_VerticalFit.enumValueIndex == 3)
            {
                EditorGUILayout.PropertyField(m_VerticalLimit, true);
                EditorGUILayout.PropertyField(m_VerticalAllowance, true);
            }
            serializedObject.ApplyModifiedProperties();

            base.OnInspectorGUI();
        }
    }
#endif

    public static class SetPropertyUtility
    {
        public static bool SetColor(ref Color currentValue, Color newValue)
        {
            if (currentValue.r == newValue.r && currentValue.g == newValue.g && currentValue.b == newValue.b && currentValue.a == newValue.a)
                return false;

            currentValue = newValue;
            return true;
        }

        public static bool SetStruct<T>(ref T currentValue, T newValue) where T : struct
        {
            if (EqualityComparer<T>.Default.Equals(currentValue, newValue))
                return false;

            currentValue = newValue;
            return true;
        }

        public static bool SetClass<T>(ref T currentValue, T newValue) where T : class
        {
            if ((currentValue == null && newValue == null) || (currentValue != null && currentValue.Equals(newValue)))
                return false;

            currentValue = newValue;
            return true;
        }
    }

}