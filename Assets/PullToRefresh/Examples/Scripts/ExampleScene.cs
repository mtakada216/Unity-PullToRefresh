using System.Collections;
using PullToRefresh;
using UnityEngine;

public class ExampleScene : MonoBehaviour
{
    [SerializeField] private UIRefreshControl m_UIRefreshControl;

    private void Start()
    {
        // Register callback
        // This registration is possible even from Inspector.
        m_UIRefreshControl.OnRefresh.AddListener(RefreshItems);
    }

    private void RefreshItems()
    {
        StartCoroutine(FetchDataDemo());
    }

    private IEnumerator FetchDataDemo()
    {
        // Instead of data acquisition.
        yield return new WaitForSeconds(1.5f);

        // Call EndRefreshing() when refresh is over.
        m_UIRefreshControl.EndRefreshing();
    }

    // Register the callback you want to call to OnRefresh when refresh starts.
    public void OnRefreshCallback()
    {
        Debug.Log("OnRefresh called.");
    }
}
