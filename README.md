# HowToAPNS Tutorial
--
A simple HowTo project demonstrating sending notifications using Apples' Push Notification Service and a Microsoft Windows WCF endpoint as a gateway. This tutorial source code is written in `C#` and `Objective-C`.


## System Requirements

To install the platform on your computer the following requirements have to be met:

Microsoft Windows 7+

* Microsoft Visual Studio 2010+ or MSBuild
* Microsoft .NET Framework 4
* WIFI - __Firewall port 12346 open__

Mac OS X 10.7+

* XCode 4.5+
* iOS Developer Account

Devices

* iPhone/iPod running iOS 6
* WIFI 


## Directory Map

* clients/ - (root directory for windows and ios clients)
	
	* windows/ 
		* PushNotification/ (sender code)
	* ios/
		* Notifications/ (reciever code)		
* server/ - (root directory for windows service) 
	
	* NotificationService/ - (WCF Service code)
	* NotificationService.Host/ - (Console Application code)
	* References/ - (Assembly dependencies)
* Bin/ - (automatically created executing the **msbuild.bat** script)
* msbuild.bat - client and service compilation script
* LICENCES.md - (self explanatory)
* README.md - ( this :-) )


## Compilation Instructions


#### Microsoft Windows

On `Microsoft Windows` systems you can open the solutions using Visual Studio and build from within or build from the commandline using the `msbuild.bat` file **_(recommended)_** .



Executing the MSBuild from commandline will produce a `Bin` directory with the client and server programs in the root directory.

* PushNotification.exe - The client application used to prepare data to send to the iOS device
* NotificationService.Host.exe - The server application used to facility communication between the windows client and the iOS device

#### Mac OS X

On `Mac OS X` systems you can open the project using XCode and attaching your iOS device to the computer in order to install the client app on the device. __You must run application on the device in order to work properly__


## Demo

This demonstration uses a Microsoft Windows client to push content to an iOS client using Apple's Push Notification Server (APNS). The communcation is unidirectional between the Windows client and the iOS client as described:


		  Sender			 		   Gateway            								     Receiver
	|-----------------|           |--------------|            |--------------|            |--------------|
											
	| Windows Client  |=========> |  WCF Service |==========> |     APNS     |==========> |  iOS Device  | 
	
	|-----------------|			  |--------------|            |--------------|			  |--------------|


The Windows client provides the ability to send arbitrary text to an iOS device indirectly through a WCF endpoint which forwards the content to Apple's push service. Before the iOS device can receive content it must register with the WCF endpoint's RESTful interface so the device can be discovered. 

The following describes the steps you must follow in order to perform the demonstration. 

##### App ID Setup

1. Login into your account on [iOS Dev Center](https://developer.apple.com/) and navigate to the iOS Provisioning Portal.
2. Next, click the **App ID** link which is located on the left side bar. Aftewards, click the button **New App ID**.
3. On the New App ID screen, create a description, select your Bundle Seed ID, and for your Bundle Identifier create a unique id because you will be needing it again later. 
4. Now locate your new App ID and click the **Configure** link on the right.
5. Next, you should have arrived to a new page with configuration options for your application. Locate the section **Enable for Apple Push Notification service** and select the checkbox next to it to enable Push Notification feature.
6. Next, You should now see two options available **Development Push SSL Certificate** (for Development) and **Production Push SSL Certificate** (for Production) with a **Configure** button next to each. Click the configure button for the development environment and then the  **Apple Push Notification service SSL Certificate Assistant** window should appear.
7. After you've followed all the prompts to the end you should have been given the option to download a file named **aps_development.cer** _(remember the location where this file is located)_.

##### Provisioning Setup

1. While logged in the iOS Provisioning Portal, click the **Provisioning** option which is located on the left side bar. 
2. Next, under the Development tab, click the **New Profile** button. Fill out the Profile Name, select a Certificate, select **App ID** your created earlier, and choose the device you want to use. Finally click the **Submit** button.
3. Next, you should see the name of the Provisioning Profile listed under the Development tab. _**Note:** If the status of your profile is **Pending** refresh the page repeatedly until the status changes to **Active**_
4. Next, on your desktop open up XCode and navigate to **Organizer** _(Shift + Command + 2)_
5. From the left side bar click **Provisioning Profiles**, and on the lower right handside click the **Refresh** button. _This should download and install your newly created provisioning profile_



##### App Key Setup

1. Back to your desktop, open up **KeyChain Access** application from **LaunchPad** _(it should be located in the Utilities group)_
2. After the app is open, in the Category section select **My Certificates** and locate your iPhone developer certificate. It should be similar to this: **iPhone Developer: Donelle Sanders Jr (C3W5RD3T67)**. Next to it should be a gray triangle, select it and it should unfold with a sub entry noted by a **key** icon. 
3. Right click the sub entry and select the **Export** menu item. When prompted, save the new file in the same location as the certificate and provisioning profile that you previously downloaded. _**Note:** make sure the file extension ends with **.p12**_
4. Now, open a **Terminal** session and change directory to where you downloaded your **aps_development.cer** and your exported developer key and enter the following commands in **order**:

    1. openssl x509 -in aps_development.cer -inform DER -out aps_development.pem -outform PEM
    2. openssl pkcs12 -nocerts -out exported_key.pem -in your_exported_key.p12
    3. cat aps_developer.pem exported_key.pem > HowToNotifications.pem
    4. openssl pkcs12 -export -in HowToNotifications.pem -out HowToNotifications.p12
    5. cp HowToNotifications.p12 /to/your/windows/filesystem/push/notification/service/

    To test to see if everything worked correctly enter the following commands: 

    openssl s_client -connect gateway.sandbox.push.apple.com:2195 -cert HowToNotifications.pem -key exported_key.pem




#### Service Setup

1. On your Windows system, navigate to the **Bin** folder and edit the **NotificationService.Host.exe.config**. Locate the **\<apnsService\>** tag and set the **Certificate** attribute to the path where you copied your **HowToNotifications.p12** and set the **Password** attribute you chose for it. 
2.Next, start the **NotificationService.Host.exe** console application. After launching successfully, the console should display the current **IP address** and **port** the service is listening on. There are two ways you can execute this service and both require you to **"Run as Administrator"**
	* **Option 1**- Open NotificationService Visual Studio the solution and click **Debug / (F5)**
	* **Option 2**- Execute NotificationService.Host.exe on the commandline from the `Bin` directory on the root.

#### Client Setup

[TODO]

# [WORK IN PROGRESS]








