using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FadeOutThenDestroy : MonoBehaviour {

    private Image img;
    private Text txt;

    private float alphaPercent = 3.0f;

	// Use this for initialization
	void Start () {
        img = this.GetComponent<Image>();
        txt = this.GetComponentInChildren<Text>();
    }
	
	// Update is called once per frame
	void Update () {

        if (alphaPercent > 0)
            alphaPercent -= Time.deltaTime;
        else
            alphaPercent = 0;
        
        img.color = new Color(img.color.r, img.color.g, img.color.b, 
            alphaPercent < 1 ? img.color.a * alphaPercent : 1
            );
        txt.color = new Color(txt.color.r, txt.color.g, txt.color.b,
            alphaPercent < 1 ? txt.color.a * alphaPercent : 1
            );

        if (img.color.a == 0 && txt.color.a == 0)
        {
            Destroy(this.gameObject);
        }

	}
}
