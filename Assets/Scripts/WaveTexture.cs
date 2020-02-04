using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveTexture : MonoBehaviour {
    public int waveWidth=128;
    public int waveHeight=128;

    //数据交换数组
    public float[,] waveA;
    public float[,] waveB;

    public int radius;

    //记录数据
    public Texture2D tex_uv;
    // Start is called before the first frame update
    void Start() {
        radius = 10;
        waveA = new float[waveWidth, waveHeight];
        waveB = new float[waveWidth, waveHeight];
        tex_uv = new Texture2D(waveWidth, waveWidth);
        GetComponent<Renderer>().material.SetTexture("_WaveTex",tex_uv);
        //PutDrop(64,64);
    }

    // Update is called once per frame
    void Update() {
        if (Input.GetMouseButton(0)) {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray,out hit)) {
                Vector3 pos = hit.point;
                pos = transform.worldToLocalMatrix.MultiplyPoint(pos);
                int w = (int)((pos.x + 0.5)*waveWidth);
                int  h = (int)((pos.y + 0.5) * waveHeight);
                PutDrop(w,h,radius);
            }
        }
        ComputeWave();
    }

    void ComputeWave() {
        for (int w = 1; w < waveWidth - 1; w++) {
            for (int h = 1; h < waveHeight - 1; h++) {
                //八个方位
                waveB[w, h] = (waveA[w - 1, h] +
                              waveA[w + 1, h] +
                              waveA[w, h - 1] +
                              waveA[w, h + 1] +
                              waveA[w - 1, h - 1] +
                              waveA[w + 1, h - 1] +
                              waveA[w - 1, h + 1] +
                              waveA[w + 1, h + 1]) / 4 - waveB[w, h];
                float value = waveB[w, h];
                if (value > 1) {
                    waveB[w, h] = 1;
                }

                if (value < -1) {
                    waveB[w, h] = -1;
                }

                float offset_u = (waveB[w - 1, h] - waveB[w + 1, h]) / 2;
                float offset_v = (waveB[w, h - 1] - waveB[w, h + 1]) / 2;
                float r = offset_u / 2 + 0.5f;
                float g = offset_v / 2 + 0.5f;
                tex_uv.SetPixel(w,h,new Color(r,g,0));
                waveB[w, h] -= waveB[w, h] * 0.0025f;//衰减
            }
        }
        tex_uv.Apply();
        float[,] temp = waveA;
        waveA = waveB;
        waveB = temp;
    }

    void PutDrop(int x,int y, int radius) {
        double dis;
        for (int i = -radius; i <=radius; i++) {
            for (int j = -radius; j <=radius; j++) {
                if (x+i>0&&x+i<waveWidth-1&&y+j>=0&&y+j<waveHeight-1) {
                    dis = Mathf.Sqrt(i * i + j * j);
                    if (dis<radius) {
                        waveA[x + i, y + j] = Mathf.Cos((float) dis * Mathf.PI / radius);
                    }
                }
            }
        }
    }
}
