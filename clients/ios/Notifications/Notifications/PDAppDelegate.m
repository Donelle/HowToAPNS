//
//  PDAppDelegate.m
//  Notifications
//
//  Created by Donelle Sanders Jr on 1/9/13.
//  Copyright (c) 2013 The Potter's Den, Inc. All rights reserved.
//

#import "PDAppDelegate.h"
#import "PDRepository.h"


@implementation PDAppDelegate

- (BOOL)application:(UIApplication *)application didFinishLaunchingWithOptions:(NSDictionary *)launchOptions
{
    if (launchOptions != nil) {
        NSDictionary * remoteNotification = [launchOptions valueForKey:UIApplicationLaunchOptionsRemoteNotificationKey];
        if (remoteNotification) {
            
            id alert = [[remoteNotification valueForKey:@"aps"] valueForKey:@"alert"];
            [[PDRepository sharedInstance] insertNotification:[alert valueForKey:@"body"]];
            
            application.applicationIconBadgeNumber = application.applicationIconBadgeNumber - 1;
        }
    }
    
    UIRemoteNotificationType notifyTypes = (UIRemoteNotificationTypeBadge | UIRemoteNotificationTypeSound | UIRemoteNotificationTypeAlert);
    [[UIApplication sharedApplication] registerForRemoteNotificationTypes: notifyTypes];
    
    return YES;
}

-(void)application:(UIApplication *)application didRegisterForRemoteNotificationsWithDeviceToken:(NSData *)deviceToken
{
    NSString* newToken = [deviceToken description];
	newToken = [newToken stringByTrimmingCharactersInSet:[NSCharacterSet characterSetWithCharactersInString:@"<>"]];
	newToken = [newToken stringByReplacingOccurrencesOfString:@" " withString:@""];
    
    [[NSUserDefaults standardUserDefaults] setValue:newToken forKey:@"deviceToken"];
}

-(void)application:(UIApplication *)application didFailToRegisterForRemoteNotificationsWithError:(NSError *)error
{

#ifdef DEBUG
        NSLog(@"Failed to get token, error: %@", error);
#endif
    
    UIAlertView * alert = [[UIAlertView alloc] initWithTitle:@"Apple Push Notifications"
                                                      message:@"An error occured registering for notification this app need to be restarted"
                                                     delegate:nil
                                            cancelButtonTitle:@"OK"
                                            otherButtonTitles:nil];
    [alert show];
}


-(void)application:(UIApplication *)application didReceiveRemoteNotification:(NSDictionary *)userInfo
{
    id alert = [[userInfo valueForKey:@"aps"] valueForKey:@"alert"];
    [[PDRepository sharedInstance] insertNotification:[alert valueForKey:@"body"]];
    
    if (application.applicationState == UIApplicationStateInactive) {
        application.applicationIconBadgeNumber = application.applicationIconBadgeNumber - 1;
    }
}


@end
