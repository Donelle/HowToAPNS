//
//  PDRepository.m
//  Notifications
//
//  Created by Donelle Sanders Jr on 1/9/13.
//  Copyright (c) 2013 The Potter's Den, Inc. All rights reserved.
//

#import "PDRepository.h"


@interface PDRepository ()  {
@private
    NSPersistentStoreCoordinator * _persistentStoreCoordinator;
    NSManagedObjectModel * _managedObjectModel;
}

@property (readonly, nonatomic) NSManagedObjectContext * managedObjectContext;
- (void)initiateContext;

@end


@implementation PDRepository
@synthesize managedObjectContext = _managedObjectContext;

- (id)init
{
    self = [super init];
    if (self != nil) {
        [self initiateContext];
    }
    
    return self;
}


- (void)initiateContext
{
    NSURL *modelURL = [[NSBundle mainBundle] URLForResource:@"Notifications" withExtension:@"momd"];
    _managedObjectModel = [[NSManagedObjectModel alloc] initWithContentsOfURL:modelURL];
    
    NSURL *appDirectory = [[[NSFileManager defaultManager] URLsForDirectory:NSDocumentDirectory inDomains:NSUserDomainMask] lastObject];
    NSURL *storeURL = [appDirectory URLByAppendingPathComponent:@"Notifications.sqlite"];
    
    NSError *error = nil;
    _persistentStoreCoordinator = [[NSPersistentStoreCoordinator alloc] initWithManagedObjectModel:_managedObjectModel];
    if (![_persistentStoreCoordinator addPersistentStoreWithType:NSSQLiteStoreType configuration:nil URL:storeURL options:nil error:&error])
    {
        NSLog(@"An unexpected error attempting to loading database! Error: %@, %@", error, [error userInfo]);
#ifdef DEBUG
        abort();
#endif
    }
    
    _managedObjectContext = [[NSManagedObjectContext alloc] init];
    [_managedObjectContext setPersistentStoreCoordinator:_persistentStoreCoordinator];
}


+ (PDRepository *)sharedInstance
{
    static PDRepository * shared = nil;
    if (shared == nil)
        shared = [[PDRepository alloc] init];
    
    return shared;
}


- (BOOL)saveChanges
{
    NSError * errors = nil;
    BOOL bSuccessful = [_managedObjectContext save:&errors];
#ifdef DEBUG
    if (!bSuccessful) {
        NSLog(@"Saving changes to disk failed with: %@", errors);
    }
#endif
    
    return bSuccessful;
}


-(void)insertNotification:(NSString *)notification
{
    PDNotificationModel * entity = [NSEntityDescription insertNewObjectForEntityForName:@"Notification" inManagedObjectContext:_managedObjectContext];
    [entity setMessage:notification];
    [entity setCreateDate:[NSDate timeIntervalSinceReferenceDate]];
    [self saveChanges];
}

-(void)deleteNotification:(PDNotificationModel *)notification
{
    [_managedObjectContext deleteObject:notification];
}


- (NSFetchedResultsController *)createRepositoryController:(id<NSFetchedResultsControllerDelegate>)delegate
{
    NSSortDescriptor * sortByName = [[NSSortDescriptor alloc] initWithKey:@"createDate" ascending:NO];
    
    NSFetchRequest * request = [[NSFetchRequest alloc] initWithEntityName:@"Notification"];
    [request setFetchBatchSize:20];
    [request setSortDescriptors:@[sortByName]];
    
    NSString * cacheName = @"FetchNotificationCache";
#ifdef DEBUG
    cacheName = nil;
#endif
    NSFetchedResultsController * controller = [[NSFetchedResultsController alloc] initWithFetchRequest:request
                                                                                  managedObjectContext:_managedObjectContext
                                                                                    sectionNameKeyPath:nil
                                                                                             cacheName:cacheName];
    
    NSError * errors = nil;
    if ([controller performFetch:&errors]) {
        [controller setDelegate:delegate];
    }else {
        controller = nil;
#ifdef DEBUG
        NSLog(@"Errors occured fetching messages: %@", errors);
#endif // DEBUG
    }
    
    return controller;
}



@end
