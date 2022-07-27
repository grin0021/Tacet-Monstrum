using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneLoaderTrigger : MonoBehaviour
{
    public SceneLoader m_sceneLoader;

    private void OnTriggerEnter(Collider other)
    {
        // Get the tag list off the colliding object
        TagList m_objTagList = other.gameObject.GetComponent<TagList>();
        // if it doesn't have a tag list then don't bother with the rest
        if (m_objTagList == null)
        {
            return;
        }

        // if it has a tag list then check if it has the player tag
        if (m_objTagList.HasTag("Player") && m_sceneLoader != null)
        {
            // if it all is well so far then we can tell the level loader script
            // to load the scene this loader is responsible for
            m_sceneLoader.m_triggerCount++;
            m_sceneLoader.UpdateScene();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // Get the tag list off the colliding object
        TagList m_objTagList = other.gameObject.GetComponent<TagList>();
        // if it doesn't have a tag list then don't bother with the rest
        if (m_objTagList == null)
        {
            return;
        }

        // if it has a tag list then check if it has the player tag
        if (m_objTagList.HasTag("Player") && m_sceneLoader != null)
        {
            // if it all is well so far then we can tell the level managment script
            // to unload the scene this loader is responsible for
            m_sceneLoader.m_triggerCount--;
            m_sceneLoader.UpdateScene();
        }
    }
}
