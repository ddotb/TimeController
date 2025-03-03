using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeController : MonoBehaviour
{
    private float m_CurrentDesiredTimescale = 0.0f;
    
    private HashSet<Component> m_PauseTokenSet = new HashSet<Component>();
    private Dictionary<Component, float> m_TimescaleRequestDictionary = new Dictionary<Component, float>();

    private IEnumerator m_TimescaleEnumerator;

    public bool IsPaused => m_PauseTokenSet.Count > 0;

    public float DesiredTimescale => m_CurrentDesiredTimescale;
    
    private void Update()
    {
        float desiredTimescale = 1.0f;
        if (m_PauseTokenSet.Count > 0)
        {
            desiredTimescale = 0.0f;
        }
        else if (m_TimescaleRequestDictionary.Count > 0)
        {
            //Start at default timescale
            float lowestRequest = 1.0f;
            
            //Current behaviour is go with slowest current request
            m_TimescaleEnumerator = m_TimescaleRequestDictionary.Values.GetEnumerator();
            while (m_TimescaleEnumerator.MoveNext())
            {
                float current = (float)m_TimescaleEnumerator.Current;

                if (current < lowestRequest)
                {
                    lowestRequest = current;
                }
            }

            desiredTimescale = lowestRequest;
        }
        else
        {
            desiredTimescale = 1.0f;
        }

        m_CurrentDesiredTimescale = desiredTimescale;
        
        Time.timeScale = m_CurrentDesiredTimescale;
    }

    public void RegisterPauseToken(Component component) //TODO Pull out to TimeSystem
    {
        m_PauseTokenSet.Add(component);
    }

    public void UnregisterPauseToken(Component component)
    {
        m_PauseTokenSet.Remove(component);
    }
    
    public void RegisterTimescaleRequest(Component component, float timescale)
    {
        m_TimescaleRequestDictionary.Add(component, timescale);
    }

    public void UpdateTimescaleRequest(Component component, float timescale)
    {
        m_TimescaleRequestDictionary[component] = timescale;
    }
    
    public void UnregisterTimescaleRequest(Component component)
    {
        m_TimescaleRequestDictionary.Remove(component);
    }
}
