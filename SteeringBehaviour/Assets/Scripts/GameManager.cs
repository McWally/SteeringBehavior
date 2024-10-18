using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    // Variables para el control de los peces
    [SerializeField] Transform bigFishSpawn;
    [SerializeField] Transform smallFishSpawn;
    [SerializeField] GameObject bigFish;
    [SerializeField] GameObject smallFish;
    [SerializeField] TextMeshProUGUI smallFishCounter;
    [SerializeField] TextMeshProUGUI bigFishCounter;
    int bigFishesCount = 0;
    int smallFishesCount = 0;

    // Instancia un pez (big o small) y actualiza los contadores
    public void InstanceFish(bool isBig){
        GameObject newFish;
        if(isBig){
            newFish = Instantiate(bigFish, bigFishSpawn.position, bigFishSpawn.rotation);
            newFish.transform.SetParent(bigFishSpawn);
            bigFishesCount++;
        }
        else{
            newFish = Instantiate(smallFish, smallFishSpawn.position, smallFishSpawn.rotation);
            newFish.transform.SetParent(smallFishSpawn);
            smallFishesCount++;
        }
        
        smallFishCounter.text = smallFishesCount.ToString();
        bigFishCounter.text = bigFishesCount.ToString();
    }

    // Elimina un pez (big o small) y actualiza los contadores
    public void RemoveFish(bool isBig){
        GameObject toDelete = null;
        if(isBig && bigFishSpawn.childCount > 0){
            toDelete = bigFishSpawn.GetChild(0).gameObject;
            bigFishesCount--;
        }
        else if (!isBig && smallFishSpawn.childCount > 0){
            toDelete = smallFishSpawn.GetChild(0).gameObject;
            smallFishesCount--;
        }
        Destroy(toDelete);
        smallFishCounter.text = smallFishesCount.ToString();
        bigFishCounter.text = bigFishesCount.ToString();
    }
}
