using UnityEngine;

public class GameManager : MonoBehaviour {
    public static GameManager Instance;

    void Awake() {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void CheckWinCondition() {
        TubeController[] allTubes = FindObjectsOfType<TubeController>();
        foreach (TubeController tube in allTubes) {
            var nuts = tube.GetCurrentNuts();
            if (nuts.Count == 0) continue; 
            if (nuts.Count != 4) return; 
            string tagCheck = nuts[0].tag;
            bool allSame = nuts.TrueForAll(nut => nut.tag == tagCheck);
            if (!allSame) return; 
        }
        Debug.Log("Win");
    }
}
