//using System;
//using System.Collections;
//using UnityEngine;
//using UnityEngine.InputSystem;

//public class ScrapCollecting : MonoBehaviour
//{
//    //TODO: Implement scrap collecting logic here for the player
//    private Coroutine collectCoroutine;


//    // Start is called once before the first execution of Update after the MonoBehaviour is created
//    void Start()
//    {
        
//    }

//    // Update is called once per frame
//    void Update()
//    {
//        if(Keyboard.current.eKey.wasPressedThisFrame)
//        {
//            // Implement scrap collecting logic here
//            Debug.Log("Collecting scrap...");
//            if(collectCoroutine == null)
//            {
//                collectCoroutine = StartCoroutine(ScrapCollectingCoroutine());
//            }
//            //Every time E is pressed and hold for 3 seconds, collect scrap

//        }

//        if(Keyboard.current.eKey.wasReleasedThisFrame)
//        {
//            if(collectCoroutine != null)
//            {
//                StopCoroutine(collectCoroutine);
//                collectCoroutine = null;
//                Debug.Log("Stopped collecting scrap.");
//            }
//        }
//    }

//    IEnumerator ScrapCollectingCoroutine()
//    {
//        yield return new WaitForSeconds(3f);

//        while(true)
//        {
//            // Collect scrap logic here

//            Debug.Log("Scrap collected!");
//            yield return new WaitForSeconds(3f); // Collect scrap every 3 seconds
//        }
//    }
//}
