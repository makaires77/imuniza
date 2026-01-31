using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeechPanel : MonoBehaviour
{
    [SerializeField] private GameObject panel = null;

    private bool enableClosePanel = false;
	private int timingSpawnPanel = 10;
	
    private IEnumerator Start()
    {
        yield return new WaitForSeconds(timingSpawnPanel);
		
		int curSec = 0;
		while(curSec < timingSpawnPanel)
		{
			if(!GameManager.Instance.Pause) 
				curSec++;
			
			yield return new WaitForSeconds(1);
		}

        GameManager.Instance.Pause = true;
        panel.SetActive(true);
        AudioManager.ReduceVolume();

        yield return new WaitForSeconds(1);
        enableClosePanel = true;
    }

    public void ClosePanel()
    {
        if (enableClosePanel)
        {
            GameManager.Instance.Pause = false;
            panel.SetActive(false);
            AudioManager.RestoreVolume();
        }
    }
}
