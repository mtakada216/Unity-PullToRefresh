using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

/*
    MIT License

    Copyright (c) 2018 kiepng

    Permission is hereby granted, free of charge, to any person obtaining a copy
    of this software and associated documentation files (the "Software"), to deal
    in the Software without restriction, including without limitation the rights
    to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
    copies of the Software, and to permit persons to whom the Software is
    furnished to do so, subject to the following conditions:

    The above copyright notice and this permission notice shall be included in all
    copies or substantial portions of the Software.

    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
    IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
    FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
    AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
    LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
    OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
    SOFTWARE.
*/
namespace PullToRefresh
{
    public class UIRefreshControl : MonoBehaviour
    {
        [Serializable] public class RefreshControlEvent : UnityEvent { }

        [SerializeField] private ScrollRect m_ScrollRect;
        [SerializeField] private float m_PullDistanceRequiredRefresh = 150f;
        [SerializeField] private Animator m_LoadingAnimator;
        [SerializeField] RefreshControlEvent m_OnRefresh = new RefreshControlEvent();


        private float m_InitialPosition;
        private float m_Progress;
        private bool m_IsPulled;
        private bool m_IsRefreshing;
        private Vector2 m_PositionStop;
        private IScrollable m_ScrollView;

        /// <summary>
        /// Progress until refreshing begins. (0-1)
        /// </summary>
        public float Progress
        {
            get { return m_Progress; }
        }

        /// <summary>
        /// Refreshing?
        /// </summary>
        public bool IsRefreshing
        {
            get { return m_IsRefreshing; }
        }

        /// <summary>
        /// Callback executed when refresh started.
        /// </summary>
        public RefreshControlEvent OnRefresh
        {
            get { return m_OnRefresh; }
            set { m_OnRefresh = value; }
        }

        /// <summary>
        /// Call When Refresh is End.
        /// </summary>
        public void EndRefreshing()
        {
            m_ScrollRect.vertical = true;
            m_IsPulled = false;
            m_IsRefreshing = false;
            m_LoadingAnimator.SetBool(_activityIndicatorStartLoadingName, false);
        }

        const string _activityIndicatorStartLoadingName = "Loading";

        private void Start()
        {
            m_InitialPosition = GetContentAnchoredPosition();
            m_PositionStop = new Vector2(m_ScrollRect.content.anchoredPosition.x, m_InitialPosition - m_PullDistanceRequiredRefresh);
            m_ScrollView = m_ScrollRect.GetComponent<IScrollable>();
            m_ScrollRect.onValueChanged.AddListener(OnScroll);
        }

        private void OnScroll(Vector2 normalizedPosition)
        {
            var distance = m_InitialPosition - GetContentAnchoredPosition();

            if (distance < 0f)
            {
                return;
            }

            OnPull(distance);
        }

        private void OnPull(float distance)
        {
            if (m_IsRefreshing && Math.Abs(distance) < 1f)
            {
                m_IsRefreshing = false;
            }

            if (m_IsPulled && m_ScrollView.Dragging)
            {
                return;
            }

            m_Progress = distance / m_PullDistanceRequiredRefresh;

            if (m_Progress < 1f)
            {
                return;
            }

            // Start animation when you reach the required distance while dragging.
            if (m_ScrollView.Dragging)
            {
                m_IsPulled = true;
                m_LoadingAnimator.SetBool(_activityIndicatorStartLoadingName, true);
            }

            if (m_IsPulled && !m_ScrollView.Dragging)
            {
                m_ScrollRect.vertical = false;
                m_ScrollRect.content.anchoredPosition = m_PositionStop;
                m_IsRefreshing = true;
                m_OnRefresh.Invoke();
                m_IsPulled = false;
            }

            m_Progress = 0f;
        }

        private float GetContentAnchoredPosition()
        {
            return m_ScrollRect.content.anchoredPosition.y;
        }
    }
}
