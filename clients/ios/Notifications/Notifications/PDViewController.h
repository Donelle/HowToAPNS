//
//  PDViewController.h
//  Notifications
//
//  Created by Donelle Sanders Jr on 1/9/13.
//  Copyright (c) 2013 The Potter's Den, Inc. All rights reserved.
//

#import <UIKit/UIKit.h>


@interface PDTableViewCell : UITableViewCell

@property (weak, nonatomic) IBOutlet UILabel *textLabel;

@end

@interface PDViewController : UIViewController<UITableViewDelegate,UITableViewDataSource>

@property (weak, nonatomic) IBOutlet UITableView *notificationView;

@end
