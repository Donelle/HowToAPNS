//
//  PDRegistrar.h
//  Pusher
//
//  Created by Donelle Sanders Jr on 1/3/13.
//  Copyright (c) 2013 The Potter's Den, Inc. All rights reserved.
//

#import <Foundation/Foundation.h>


@protocol PDRegistrarDelegate <NSObject>


- (void)registrarFailedWith:(NSError *)error;
- (void)registrarDidRegister;

@end


@interface PDRegistrar : NSObject

@property (weak, nonatomic) id<PDRegistrarDelegate> delegate;
- (void)registerWith:(NSString *)host;

@end
