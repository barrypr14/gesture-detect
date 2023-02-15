# gesture-detect

## System Structure
#### flexible sensor
The flex sensor will be about 25kΩ under normal conditions, and the resistance value will be increased after bending. The value from flexible sensor will be changed when bending. Without bending, the value is about 250 ~ 270, while the flex sensor is bent, the output value will drop greatly to around 130 ~ 150. After several tests, I observed that the numerical change of the resistance is linear. Therefore, I deduced a linear formula to be my transform approach to imitate the real-word fingers angles . 


<center class= "half">
  <img src="https://user-images.githubusercontent.com/63724884/218966603-5519da0d-9dc5-428d-9d8b-39d4bb16ea1a.png"><img src="https://user-images.githubusercontent.com/63724884/218967539-14ed7d3c-5ee8-4556-a2c1-6cba7491cc59.png">
  <image 1> Read the signal from Arduino <image 2> the output value of the flex sensor
<center>
