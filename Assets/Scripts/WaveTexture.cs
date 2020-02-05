using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class WaveTexture : MonoBehaviour {
    private int waveWidth = 512;
    private int waveHeight = 512;

    //数据交换数组
    public float[,] waveA;
    public float[,] waveB;
    //鼠标的半径
    public int radius;
    //产生波纹后的衰减值
    public float Decrement;

    //记录数据
    public Texture2D tex_uv;

    private bool isRun = true;

    private int sleepTime;

    private Color[] colorBuffer;//颜色缓冲区
    // Start is called before the first frame update
    void Start() {
        Decrement = 0.025f;
        radius = 10;
        waveA = new float[waveWidth, waveHeight];
        waveB = new float[waveWidth, waveHeight];
        colorBuffer=new Color[waveWidth*waveHeight];
        tex_uv = new Texture2D(waveWidth, waveWidth);
        GetComponent<Renderer>().material.SetTexture("_WaveTex", tex_uv);
        PutDrop(64,64,radius);
        Thread th = new Thread(new ThreadStart(ComputeWave));
        th.Start();
    }

    // Update is called once per frame
    void Update() {
        sleepTime = (int)Time.deltaTime * 1000;
        tex_uv.SetPixels(colorBuffer);
        tex_uv.Apply();
        if (Input.GetMouseButton(0)) {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit)) {
                Vector3 pos = transform.worldToLocalMatrix.MultiplyPoint(hit.point);//将世界坐标转化到模型空间坐标系
                int w = (int)((pos.x + 0.5) * waveWidth);
                int h = (int)((pos.y + 0.5) * waveHeight);
                PutDrop(w, h, radius);
            }
        }
        //ComputeWave();
    }

    void ComputeWave() {
        while (isRun) {
            for (int w = 1; w < waveWidth - 1; w++)
                for (int h = 1; h < waveHeight - 1; h++) {
                    //八个方位
                    waveB[w, h] = (waveA[w - 1, h] + waveA[w + 1, h] + waveA[w, h - 1] + waveA[w, h + 1] + waveA[w - 1, h - 1] + waveA[w + 1, h - 1] + waveA[w - 1, h + 1] + waveA[w + 1, h + 1]) / 4 - waveB[w, h];
                    waveB[w, h] = Mathf.Clamp(waveB[w, h], -1, 1);//将值限定在-1到1之间
                    float offset_u = (waveB[w - 1, h] - waveB[w + 1, h]) / 2;
                    float offset_v = (waveB[w, h - 1] - waveB[w, h + 1]) / 2;
                    float r = offset_u / 2 + 0.5f;
                    float g = offset_v / 2 + 0.5f;
                    //tex_uv.SetPixel(w, h, new Color(r, g, 0));
                    colorBuffer[w+waveWidth*h]=new Color(r,g,0);
                    waveB[w, h] -= waveB[w, h] * Decrement;//衰减
                }
            //tex_uv.Apply();
            float[,] temp = waveA;
            waveA = waveB;
            waveB = temp;
            Thread.Sleep(sleepTime);
        }

    }

    void PutDrop(int x, int y, int radius) {
        double dis;
        for (int i = -radius; i <= radius; i++)
            for (int j = -radius; j <= radius; j++)
                if (x + i > 0 && x + i < waveWidth - 1 && y + j >= 0 && y + j < waveHeight - 1) {
                    dis = Mathf.Sqrt(i * i + j * j);
                    if (dis < radius)
                        waveA[x + i, y + j] = Mathf.Cos((float)dis * Mathf.PI / radius);
                }
    }

    void OnDestroy() {
        isRun = false;
    }
}
