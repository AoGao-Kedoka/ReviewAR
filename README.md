# ReviewAR
<img src="https://user-images.githubusercontent.com/58142398/213578452-836ad925-ed98-403d-aee4-1631a290f019.png" width=40% height=40%>

An AR Android App overlaying business reviewing informations in the AR scene. Build with latest Geospatial API from ARFoundation Extension.

# Screenshots
<img src="./demo.gif">

# Download
Please write us for downloads cause we have limited quota.  
Discord: **Kedoka#2059** or **ksDOT#3200**  
Email: ao.gao *at* tum.de

# How to build!
1. Open the project in Unity
2. Enable `ARCore API` in Google Cloud Platform and setup Keystore Manager and `SHA-1` fingerprint (Detailed: https://developers.google.com/ar/develop/unity-arf/geospatial/enable-android)
3. Enable `Google Maps Platform API` and save the credentials in a config file in `Assets/Resources/Keys.txt`. The format of the content should be like:
```
MapApiKey=xxxxxxxx
```
4. Sign the app, build and run the app
# Enable and disable Debug
If you want to enable Debug of this project, the main debug option is located at DebugManager under `DebugController`  
  
**Enabled**  
  
![image](https://user-images.githubusercontent.com/58142398/212759276-9bbf1fa2-7bbc-45a6-b483-428b3603dffe.png)
  
    
**Disabled**  
  
![image](https://user-images.githubusercontent.com/58142398/212759305-82cd58a7-76d3-4e29-a185-ae10145fdd6b.png)

# Attributions
- Logo generated with DALLE 2
- unity-tweens (https://github.com/jeffreylanters/unity-tweens)
- HierarchyDecorator (https://github.com/WooshiiDev/HierarchyDecorator)
- InGameDebugConsole (https://github.com/yasirkula/UnityIngameDebugConsole)
- Markup-Attributes (https://github.com/gasgiant/Markup-Attributes#conditionals)
