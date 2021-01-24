using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Text;
using System.IO.Ports;
using System.Threading;
using System.Linq.Expressions;
using JetBrains.Annotations;
using System.Runtime.Remoting.Messaging;
using UnityEngine.PlayerLoop;
using UnityEngine.UIElements;
using System.Linq;
using Boo.Lang.Environments;
using UnityEditor.UI;
using System.Net;

public class move : MonoBehaviour
{
    const string Port_Name1 = "COM10";
    const string Port_Name2 = "COM9";
    //pp Thread thread;
    //public bool isRun = true;

    public SerialPort sp1;//藍芽 六軸
    public SerialPort sp2;//彎曲感測器

    Thread mythread;
    Thread sthread;
    public bool isRun = true;
    string[] fingerarray;
    string[] sarray;
    public LinkedList<string> commandLinkedList1 = new LinkedList<string> ();
    public LinkedList<string> commandLinkedList2 = new LinkedList<string> ();

    public Transform elbow;
    public Transform palm;

    //thumb
    public Transform finger1knuckles1;
    public Transform finger1joint1;
    public Transform finger1knuckles2;
    public Transform finger1joint2;

    float[] finger1_value;


    public List<float> finger_1 = new List<float>();

    public float finger_1degree;//中指第一節的角度
    public float finger_1lastdegree = 0; //中指第一節此秒需要轉的角度
    public float finger_1rotatedegree;//中指第一節此秒需要轉的角度

    //point
    public Transform finger2knuckles1;
    public Transform finger2joint1;
    public Transform finger2knuckles2;
    public Transform finger2joint2;
    public Transform finger2kncukles3;
    public Transform finger2joint3;

    float[] finger2_value;


    public float finger2_1lastdegree = 0; //中指第一節此秒需要轉的角度
    public float finger2_1rotatedegree;//中指第一節此秒需要轉的角度
    public float finger2_2lastdegree = 0; //中指第二節此秒需要轉的角度
    public float finger2_2rotatedegree;//中指第二節此秒需要轉的角度

    //middle
    public Transform finger3knuckles1;
    public Transform finger3joint1;
    public Transform finger3knuckles2;
    public Transform finger3joint2;
    public Transform finger3kncukles3;
    public Transform finger3joint3;

    float[] finger3_value; 


    public float finger3_1lastdegree = 0; //中指第一節此秒需要轉的角度
    public float finger3_1rotatedegree;//中指第一節此秒需要轉的角度
    public float finger3_2lastdegree = 0; //中指第二節此秒需要轉的角度
    public float finger3_2rotatedegree;//中指第二節此秒需要轉的角度

    //ring
    public Transform finger4knuckles1;
    public Transform finger4joint1;
    public Transform finger4knuckles2;
    public Transform finger4joint2;
    public Transform finger4kncukles3;
    public Transform finger4joint3;

    float [] finger4_value;


    public float finger4_1lastdegree = 0; //中指第一節此秒需要轉的角度
    public float finger4_1rotatedegree;//中指第一節此秒需要轉的角度
    public float finger4_2lastdegree = 0; //中指第二節此秒需要轉的角度
    public float finger4_2rotatedegree;//中指第二節此秒需要轉的角度

    //little
    public Transform finger5knuckles1;
    public Transform finger5joint1;
    public Transform finger5knuckles2;
    public Transform finger5joint2;
    public Transform finger5kncukles3;
    public Transform finger5joint3;

    float[] finger5_value;

    public float finger5_1lastdegree = 0; //中指第一節此秒需要轉的角度
    public float finger5_1rotatedegree;//中指第一節此秒需要轉的角度
    public float finger5_2lastdegree = 0; //中指第二節此秒需要轉的角度
    public float finger5_2rotatedegree;//中指第二節此秒需要轉的角度

    //mpu6050
    public double RollAngle = 0; //對X軸旋轉
    public double PitchAngle = 0; //Y
    //public double YawAngle; //Z

    //上秒的角度位置
    public double LastRollAngle;   
    public double LastPitchAngle;    
    //public double LastYawAngle;
    
    //旋轉角度大小
    public double Rollturn = 0;
    public double Pitchturn = 0;
    //public double Yawturn = 0;

    public string receivedData1 = "";
    public string receivedData2 = "";


// Start is called before the first frame update
    void Start()
    {
        Debug.Log("start");
        palm.parent = elbow;

        //thumb
        finger1joint1.parent = palm;
        finger1knuckles1.parent = finger1joint1;
        finger1joint2.parent = finger1knuckles1;
        finger1knuckles2.parent = finger1joint2;

        //point
        finger2joint1.parent = palm;
        finger2knuckles1.parent = finger2joint1;
        finger2joint2.parent = finger2knuckles1;
        finger2knuckles2.parent = finger2joint2;
        finger2joint3.parent = finger2knuckles2;
        finger2kncukles3.parent = finger2joint3;

        //middle
        finger3joint1.parent = palm;
        finger3knuckles1.parent = finger3joint1;
        finger3joint2.parent = finger3knuckles1;
        finger3knuckles2.parent = finger3joint2;
        finger3joint3.parent = finger3knuckles2;
        finger3kncukles3.parent = finger3joint3;

        //ring
        finger4joint1.parent = palm;
        finger4knuckles1.parent = finger4joint1;
        finger4joint2.parent = finger4knuckles1;
        finger4knuckles2.parent = finger4joint2;
        finger4joint3.parent = finger4knuckles2;
        finger4kncukles3.parent = finger4joint3;

        //little
        finger5joint1.parent = palm;
        finger5knuckles1.parent = finger5joint1;
        finger5joint2.parent = finger5knuckles1;
        finger5knuckles2.parent = finger5joint2;
        finger5joint3.parent = finger5knuckles2;
        finger5kncukles3.parent = finger5joint3;


        sp1 = new SerialPort( Port_Name1, 9600);
        sp2 = new SerialPort(Port_Name2, 9600);
        sp1.Open();
        sp1.ReadTimeout = 1000;
        

        //暫 校正六軸位置
        //Initialized();
        //彎曲電阻的arduino
        sp2.Open();
        sp2.ReadTimeout = 1000;

        mythread = new Thread(getReceivedData);
        mythread.IsBackground = true;
        mythread.Start();

        Debug.Log("BT is collection");
        LastRollAngle = Roll_init;
        LastPitchAngle = Pitch_init;
        // LastYawAngle = Yaw_init;
        
        finger1_value = new float[6];
        finger2_value = new float[6];
        finger3_value = new float[6];
        finger4_value = new float[6];
        finger5_value = new float[6];

    }

    // Update is called once per frame
    void Update()
    {
        if (sp1.IsOpen && sp2.IsOpen)
        {
            try
            {
                //獲取arduino1數值資料(四指前端+大拇指)
                string value1 = getCommand1();
                string value2 = getCommand2();
                //Debug.Log("value2 = " + value2);
                 if (value2 != "")
                 {
                   
                    //Debug.Log("value2 time = " + Time.time);
                    sarray = value2.Split('|');
                    //Debug.Log("inside");
                    //////////////////////////////////////////////
                    for (int i = 6; i <= 9; i++)
                    {
                        if (float.Parse(sarray[i]) > 270)
                            sarray[i] = 270.ToString();
                        else if (float.Parse(sarray[i]) < 130)
                            sarray[i] = 130.ToString();
                    }

                    //point  獲取第一指節資料
                    // finger2_1.Add(float.Parse(sarray[6]));
                    finger2_value[2] = float.Parse(sarray[6]);

                    if (float.Parse(sarray[6]) > finger2_value[0])
                        finger2_value[0] = float.Parse(sarray[6]);
                    else if (float.Parse(sarray[6]) < finger2_value[1]) 
                        finger2_value[1] = float.Parse(sarray[6]);

                    finger2_1rotatedegree = getfingerAngle(finger2_value[0], finger2_value[1], float.Parse(sarray[6]), 90, finger2_1lastdegree);
                    finger2knuckles1.rotation = Quaternion.Euler(90 + finger2_1rotatedegree , 90, 0);

                    //////////////////////////////////////////////
                    ///middle  獲取第一指節資料
                    //finger3_1.Add(float.Parse(sarray[7]));
                    finger3_value[2] = float.Parse(sarray[7]);

                    if (float.Parse(sarray[7]) > finger3_value[0])
                        finger3_value[0] = float.Parse(sarray[7]);
                    else if (float.Parse(sarray[7]) < finger3_value[1])
                        finger3_value[1] = float.Parse(sarray[7]);

                    finger3_1rotatedegree = getfingerAngle(finger3_value[0], finger3_value[1], float.Parse(sarray[7]), 90, finger3_1lastdegree);
                    finger3knuckles1.rotation = Quaternion.Euler(90 + finger3_1rotatedegree, 90, 0);

                    //////////////////////////////////////////////////////////
                    ///ring  獲取第一指節資料
                    //finger4_1.Add(float.Parse(sarray[8]));
                    finger4_value[2] = float.Parse(sarray[8]);

                    if (float.Parse(sarray[8]) > finger4_value[0])
                        finger4_value[0] = float.Parse(sarray[8]);
                    else if (float.Parse(sarray[8]) < finger4_value[1])
                        finger4_value[1] = float.Parse(sarray[8]);

                    finger4_1rotatedegree = getfingerAngle(finger4_value[0], finger4_value[1], float.Parse(sarray[8]), 90, finger4_1lastdegree);
                    finger4knuckles1.rotation = Quaternion.Euler(90 + finger4_1rotatedegree, 90, 0);

                    /////////////////////////////////////////////////////////
                    ///little  獲取第一指節資料
                    //finger5_1.Add(float.Parse(sarray[9]));
                    finger5_value[2] = float.Parse(sarray[9]);

                    if (float.Parse(sarray[9]) > finger5_value[0])
                        finger5_value[0] = float.Parse(sarray[9]);
                    else if (float.Parse(sarray[9]) < finger5_value[1])
                        finger5_value[1] = float.Parse(sarray[9]);

                    finger5_1rotatedegree = getfingerAngle(finger5_value[0], finger5_value[1], float.Parse(sarray[9]), 60, finger5_1lastdegree);
                    finger5knuckles1.rotation = Quaternion.Euler(90 + finger5_1rotatedegree, 90, 0);


                    //////////////////////////////////////////////////
                    ///六軸 roll/pitch
                    RollAngle = getAngle(float.Parse(sarray[1]) , 1);
                    Rollturn = (RollAngle - LastRollAngle);
                    PitchAngle = getAngle(float.Parse(sarray[2]), 2);
                    Pitchturn = PitchAngle - LastPitchAngle;
                    //  YawAngle = float.Parse(sarray[3]);
                    // Yawturn = YawAngle - LastYawAngle;

                    //旋轉角度
                    Quaternion CurrentAngle = Quaternion.Euler( 180 - (float)PitchAngle , 90 , -(float)RollAngle );//我在測試rotation到底是什麼?
                    palm.rotation = CurrentAngle;

                    float xmove = float.Parse(sarray[3]);
                    float ymove = float.Parse(sarray[4]);


                    //elbow.position = elbow.position + new Vector3 (xmove , ymove , 0 )* Time.deltaTime*Time.deltaTime;
                    //保存上一刻的角度位置
                    LastRollAngle = RollAngle;
                    LastPitchAngle = PitchAngle;
                    //LastYawAngle = YawAngle;
                
                }
                if (value1 != "")
                {
                    
                   // Debug.Log("value1 time = " + Time.time);
                    fingerarray = value1.Split('|');

                    for (int i = 1; i <= 5; i++)
                    {
                        if (float.Parse(fingerarray[i]) > 270)
                            fingerarray[i] = 270.ToString();
                        else if (float.Parse(fingerarray[i]) < 130)
                            fingerarray[i] = 130.ToString();
                    }

                    /////////////////////////////////
                    ///thumb  獲取第一指節資料

                    //finger_1.Add(float.Parse(fingerarray[5]));
                    finger1_value[5] = float.Parse(fingerarray[5]);

                    if (float.Parse(fingerarray[5]) > finger1_value[3])
                        finger1_value[3] = float.Parse(fingerarray[5]);
                    else if (float.Parse(fingerarray[5]) < finger1_value[4])
                        finger1_value[4] = float.Parse(fingerarray[5]);

                    //計算彎曲角度
                    finger_1rotatedegree = getfingerAngle(finger1_value[3], finger1_value[4], float.Parse(fingerarray[5]), 180, finger_1lastdegree);

                    finger1knuckles1.rotation = Quaternion.Euler(90 + finger_1rotatedegree , 90 , 0);

                    /////////////////////////////////////////////////////
                    //point 獲取第二指節資料
                    //finger2_2.Add(float.Parse(fingerarray[4]));
                    finger2_value[5] = float.Parse(fingerarray[4]);

                    //if (float.Parse(fingerarray[4]) > finger2_value[3])
                    // finger2_value[3] = float.Parse(fingerarray[4]);
                    // else if (float.Parse(fingerarray[4]) < finger2_value[4])
                    //   finger2_value[4] = float.Parse(fingerarray[4]);
                    changeMaxMin(finger2_value , float.Parse(fingerarray[4]), 2);
                    
                    finger2_2rotatedegree = getfingerAngle(finger2_value[3],finger2_value[4], float.Parse(fingerarray[4]), 180, finger2_2lastdegree);
                    finger2knuckles2.rotation = Quaternion.Euler(90 + finger2_2rotatedegree + finger2_1rotatedegree ,90, 0);
         
                    ////////////////////////////////////////////////////
                    //middle 獲取第二指節資料
                    //finger3_2.Add(float.Parse(fingerarray[3]));
                    finger3_value[5] = float.Parse(fingerarray[3]);

                    if (float.Parse(fingerarray[3]) > finger3_value[3])
                        finger3_value[3] = float.Parse(fingerarray[3]);
                    else if (float.Parse(fingerarray[3]) < finger1_value[4])
                        finger3_value[4] = float.Parse(fingerarray[3]);


                    finger3_2rotatedegree = getfingerAngle(finger3_value[3], finger3_value[4], float.Parse(fingerarray[3]), 180, finger3_2lastdegree);
                    finger3knuckles2.rotation = Quaternion.Euler(90 + finger3_2rotatedegree + finger3_1rotatedegree, 90, 0);

                    //////////////////////////////////////////////////
                    //ring 獲取第二指節資料
                    //finger4_2.Add(float.Parse(fingerarray[2]));
                    finger4_value[5] = float.Parse(fingerarray[2]);

                    if (float.Parse(fingerarray[2]) > finger4_value[3])
                        finger4_value[3] = float.Parse(fingerarray[2]);
                    else if (float.Parse(fingerarray[2]) < finger4_value[4])
                        finger4_value[4] = float.Parse(fingerarray[2]);

                    finger4_2rotatedegree = getfingerAngle(finger4_value[3], finger4_value[4], float.Parse(fingerarray[2]), 180, finger4_2lastdegree);
                    finger4knuckles2.rotation = Quaternion.Euler(90 + finger4_2rotatedegree + finger4_1rotatedegree, 90, 0);

                    ////////////////////////////////////////////////
                    //little 獲取第二指節資料
                    //finger5_2.Add(float.Parse(fingerarray[1]));
                    finger5_value[5] = float.Parse(fingerarray[1]);

                    if (float.Parse(fingerarray[1]) > finger5_value[3])
                        finger5_value[3] = float.Parse(fingerarray[1]);
                    else if (float.Parse(fingerarray[1]) < finger5_value[4])
                        finger5_value[4] = float.Parse(fingerarray[1]);

                    finger5_2rotatedegree = getfingerAngle(finger5_value[3], finger5_value[4], float.Parse(fingerarray[1]), 180, finger5_2lastdegree);
                    finger5knuckles2.rotation = Quaternion.Euler(  90 + finger5_2rotatedegree , 90, 0);
                }

            }
            catch (TimeoutException){}
        }
    }

    public void changeMaxMin(float [] array , float data , int i)
    {
        if( i == 2)
        {
            array[5] = data;
            if (array[3] < array[5])
                array[3] = data;
            else if (array[4] > array[5])
                array[4] = data;
        }
            
    }
    public double Roll_init;
    public double Pitch_init;
    public double Yaw_init;

    public float getfingerAngle(Single fmax , Single fmin , float degree , float cc , float lastdegree)
    { 
        float fingerdegree = (fmax - degree) * (fmax - fmin) / 180;
        if (fingerdegree > 90)
            return 90;
        return fingerdegree;
    }

    public double getAngle(float data , int i )
    {
        double temp = 0;
        if (i == 2 && data > 0)
        {
            temp = 0.0567 * Math.Pow(data, 2) - 0.2702 * data + 1.8768;
            if (temp > 90)
                return 90;
            return temp;
        }
        else if (i == 2 && data < 0)
        {
            temp = -(0.0567 * Math.Pow(data, 2) - 0.2702 * data + 1.8768);
            if (temp < -90)
                return - 90;
            return temp; 
        }

        else if (i == 1 && data > 0)
        {
            temp = 0.0482 * Math.Pow(data, 2) + 0.1737 * data + 2.0384;
            if (temp > 90)
                return 90;
            return temp;
        }
        else if (i == 1 && data < 0)
        {
            temp = -(0.0482 * Math.Pow(data, 2) + 0.1737 * data + 2.0384);
            if (temp < -90)
                return -90;
            return temp;
        }
        else
            return 0;
    }

    public String getCommand1()
    {
        string s1 = "";

        s1 = "";
        if (commandLinkedList1.Count > 0)
        {
            s1 = commandLinkedList1.First.Value;
            commandLinkedList1.RemoveFirst();
            return s1;
        }
        return "";
    }

    public String getCommand2()
    {
        string s2 = "";

        s2 = "";
        if (commandLinkedList2.Count > 0)
        {
            s2 = commandLinkedList2.First.Value;  
            //Debug.Log("getcommand2 = " + s2);
            commandLinkedList2.RemoveFirst();
            return s2;
        }
        return "";
    }

    public void getReceivedData()
    {
        while (isRun)
        {
            try
            {
                string s = "";
                int len1 = 0;
                int len2 = 0;

                receivedData1 += sp1.ReadLine(); // com10 獲取arduino1數值資料(四指前端+大拇指)
                receivedData2 += sp2.ReadLine();// com9 獲取arduino2數值資料(四指根端+六軸)
               
                len1 = receivedData1.Length;
                len2 = receivedData2.Length;


                for (int i = 0; i < len1; i++)
                {
                    // 組合指令.
                    if (receivedData1[i] != '@')
                    {
                        s += receivedData1[i];      
                    }
                    else
                    {
                        // 加入指令.
                        commandLinkedList1.AddLast(s);
                        s = "";
                        
                    }
                }
                
                for (int i = 0; i < len2; i++)
                {
                    // 組合指令.
                    if (receivedData2[i] != '@')
                    {
                        s += receivedData2[i];
                    }
                    else
                    {
                        // 加入指令.
                        commandLinkedList2.AddLast(s);
                        s = "";
                    }
                }
                //Debug.Log(receivedData2);
                
                receivedData1 = s;
                receivedData2 = s;
            }
            catch(TimeoutException) { }                 
        }
    }

    void OnApplicationQuit()
    {
        isRun = false;
        mythread = null;
        if (sp1 != null && sp2 != null && sp1.IsOpen && sp2.IsOpen)
        {
            sp1.Close();
            sp2.Close();
        }
    }
}



