//
//  PDNotificationModel.h
//  Notifications
//
//  Created by Donelle Sanders Jr on 1/9/13.
//  Copyright (c) 2013 The Potter's Den, Inc. All rights reserved.
//

#import <Foundation/Foundation.h>
#import <CoreData/CoreData.h>


@interface PDNotificationModel : NSManagedObject

@property (nonatomic, retain) NSString * message;
@property (nonatomic) NSTimeInterval createDate;

@end
