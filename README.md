# HowToAPNS Tutorial
--
A simple HowTo project demonstrating sending notifications using Apples' Push Notification Service using a Microsoft Windows WCF endpoint as a gateway. This tutorial source code is written in `C#` and `Objective-C`.


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
		* NotificationSender/ (sender code)
	* ios/
		* Notification/ (reciever code)		
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

* NotificationSender.exe - The client application used to prepare data to send to the iOS device
* NotificationService.Host.exe - The server application used to facility communication between the windows client and the iOS device

#### Mac OS X

On `Mac OS X` systems you can open the project using XCode and attaching your iOS device to the computer in order to install the client app on the device. __You must run application on the device in order to work properly__


## Demo

This demonstration uses a Microsoft Windows client to push content to an iOS client over WIFI. The communcation is unidirectional between the Windows client and the iOS client as described:


		  Sender			 		   Gateway            								     Receiver
	|-----------------|           |--------------|            |--------------|            |--------------|
											
	| Windows Client  |=========> |  WCF Service |==========> |     APNS     |==========> |  iOS Device  | 
	
	|-----------------|			  |--------------|            |--------------|			  |--------------|


The Windows client provides the ability to send arbitrary text and images to an iOS device indirectly through a WCF endpoint. Before the iOS device can receive content it must register with the WCF endpoint's RESTful interface so the device can be discovered. 

The following describes the steps you must follow in order to perform the demonstration. 

#### Service Setup

1. On your Windows system, start the **PusherService.Host.exe** console application. After launching successfully, the console should display the current **IP address** and **port** the service is listening on. There are two ways you can execute this service and both require you to **"Run as Administrator"**
	* **Option 1**- Open PusherService Visual Studio the solution and click **Debug / (F5)**
	* **Option 2**- Execute PusherService.Host.exe on the commandline from the `Bin` directory on the root.

#### Client Setup


# [WORK IN PROGRESS]








