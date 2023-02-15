# gesture-detect
This is my course ES3473 Principle of Robotics final project in National Cheng Kung Unviersity, Taiwan.
## System Structure
#### flexible sensor
The flex sensor will be about 25kΩ under normal conditions, and the resistance value will be increased after bending. The value from flexible sensor will be changed when bending. Without bending, the value is about 250 ~ 270, while the flex sensor is bent, the output value will drop greatly to around 130 ~ 150. After several tests, I observed that the numerical change of the resistance is linear. Therefore, I deduced a linear formula to be my transform approach to imitate the real-word fingers angles . 

#### MPU6050 
MPU6050 is composed of x,y,z, three-axis accelerometer and angular velocity meter, which can get ACC_X、ACC_Y、ACC_Z、GYR_X、GYR_Y、GYR_Z. When simulating the angle of an object, we can define three mutually perpendicular angles of Roll, Pitch, and Yaw. If we integrate the angular velocity meter, we can use the sensor to measure these three angles.   
MPU6050 correction
Fix the MPU6050 on the horizontal plane, increase the angle of this horizontal plane at intervals of 10 degrees, and then input the actual angle value and the measured angle value into excel to simulate the quadratic curve, so that a more accurate angle can be obtained .

#### Kalman filter
In real life, noise will affect the accuracy of the sensor, so we use the Kalman filter to filter the MPU6050, and we refer to the built-in Kalman filter of the arduino library.
some unsolved problem : computing time cause the delay.
#### Unity model
My simulation object model.  
![image](https://user-images.githubusercontent.com/63724884/218973160-b98e7b59-00f7-4b05-9713-4290fe89e0b7.png)

Using the movement process of fingers and wrists, the bending sensor and MPU605 record the movement angles and transmit them to the computer to simulate the data transmission/reception of fingers and wrists. We string the data collected by Arduino into strings, and then It is transmitted to the computer, the following code collects the data, puts the collected data into the linkedlist first, and removes the data after using it to reduce memory waste.  

```C#
  public void getReceviedData(){
    while(isRun){
      try{
        string s = "":
        recievedData += sp.ReadLine();

        int len = recievedData;

        addtolinklist(recievedData1,len);

        recievedData = "";
      }
    }catch(TimeoutException){}
  }
  
  public String getCommand(){
    string s = "";
    if(commandLinkedList.count > 0){
      s = commandLinkedList.First.Value;
      commandLinkedList.RemoveFirst();
      return s;
    }
    return "";
  }
```
In the process of object rotation, generally speaking, the concept of Euler Angle is used to rotate, but this will cause the problem of universal lock, and the concept of quaternion is used to avoid this problem. There is a convenient function in Unity called `Quaternion.Euler(new Vector)`, can easily convert Euler Angle into quaternion form.

During the Unity simulation process, we set up a multi-threading method to avoid the problem that the simulation is not real-time when our computer reads the data, and because we have two Arduino Unos, we open one more thread to synchronize.
```C#
  mythread = new Thread(getReceivedData);
  mythread.isBackground = true;
  mythread.Start();
```
When reading the bending sensor, because each bending sensor will have different values when standing still, it is assumed that when the tester wears gloves, the palm of the hand will be placed on the table, and the maximum value of each joint When the rotation angle is 90 degrees, when the device is started, the collected data is stored in the list continuously, and the maximum value is searched, which will make the search speed slower and delay the simulation, so the method is changed to a computer It will always judge the maximum and minimum values of the bending sensor and store them in the array, reducing the time to search the entire list again.
