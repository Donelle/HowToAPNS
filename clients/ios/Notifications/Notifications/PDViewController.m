//
//  PDViewController.m
//  Notifications
//
//  Created by Donelle Sanders Jr on 1/9/13.
//  Copyright (c) 2013 The Potter's Den, Inc. All rights reserved.
//
#import <QuartzCore/QuartzCore.h>
#import "PDViewController.h"
#import "PDRepository.h"
#import "PDRegistrar.h"

#pragma mark - PDTableViewCell Implementation

@interface PDTableViewCell ()

@property (nonatomic, retain) PDNotificationModel * model;

- (void)cellWasSwiped:(UISwipeGestureRecognizer *)recognizer;

@end


@implementation PDTableViewCell
@synthesize model = _model;
@synthesize textLabel;


- (void)setModel:(PDNotificationModel *)model
{
    _model = model;
    [self setNeedsDisplay];
}


-(void)awakeFromNib
{
    [super awakeFromNib];
    
    UISwipeGestureRecognizer * swipeRecognizer = [[UISwipeGestureRecognizer alloc] initWithTarget:self action:@selector(cellWasSwiped:)];
	[swipeRecognizer setDirection:(UISwipeGestureRecognizerDirectionLeft | UISwipeGestureRecognizerDirectionRight)];
	
    [self.contentView addGestureRecognizer:swipeRecognizer];
}

-(void)drawRect:(CGRect)rect
{
    [super drawRect:rect];
    
    UIBezierPath* rectanglePath = [UIBezierPath bezierPathWithRoundedRect:self.contentView.bounds cornerRadius:4.0f];
    [[UIColor whiteColor] setFill];
    [rectanglePath fill];
    [[UIColor lightGrayColor] setStroke];
    rectanglePath.lineWidth = 2;
    [rectanglePath stroke];
    
}

- (void)layoutSubviews
{
    [super layoutSubviews];
    
    CGSize textSize = [_model.message sizeWithFont:[UIFont systemFontOfSize:15] constrainedToSize:CGSizeMake(293, 400)];
    CGRect bounds = self.textLabel.bounds;
    
    [self.textLabel setHidden:NO];
    [self.textLabel setText:_model.message];
    [self.textLabel setBounds:CGRectMake(0, 0, bounds.size.width, textSize.height)];
    
    CGSize widthHeight = self.textLabel.bounds.size;
    [self.textLabel setFrame:CGRectMake(9, 12, widthHeight.width, widthHeight.height)];
    
    CGRect rcCell = self.contentView.frame;
    rcCell.size = CGSizeMake(rcCell.size.width, self.textLabel.frame.origin.y + widthHeight.height + 10);
    [self.contentView setFrame:rcCell];
}

#pragma mark - Instance Methods


-(void)cellWasSwiped:(UISwipeGestureRecognizer *)recognizer
{
    [[PDRepository sharedInstance] deleteNotification:_model];
}

@end



#pragma mark - PDViewController Implementation

@interface PDViewController () <NSFetchedResultsControllerDelegate,UIAlertViewDelegate,PDRegistrarDelegate> {
    UIView * _loadingView;
}

@property (nonatomic, retain) NSFetchedResultsController * notificationController;
@property (nonatomic, retain) PDRegistrar * registrar;
@property (nonatomic, readonly) UIView * loadingView;

-(void)showLoadingView;
-(void)hideLoadingView;

@end


@implementation PDViewController

@synthesize notificationController = _notificationController;
@synthesize notificationView = _notificationView;
@synthesize registrar = _registrar;


- (void)viewDidLoad
{
    [super viewDidLoad];
    
    
    self.notificationController = [[PDRepository sharedInstance] createRepositoryController:self];
    self.registrar = [[PDRegistrar alloc] init];
    self.registrar.delegate = self;
    
    NSError * errors = nil;
    if(![_notificationController performFetch:&errors]) {
        UIAlertView * alert = [[UIAlertView alloc] initWithTitle:@"Pusher"
                                                         message:@"An error occured loading your data please restart this app"
                                                        delegate:nil
                                               cancelButtonTitle:@"OK"
                                               otherButtonTitles:nil];
        [alert show];
    }
    
}

- (void)didReceiveMemoryWarning
{
    [super didReceiveMemoryWarning];
    self.notificationController = nil;
    self.registrar = nil;
}



#pragma mark - UITableView Data Source Methods

- (NSInteger)tableView:(UITableView *)tableView numberOfRowsInSection:(NSInteger)section
{
    id<NSFetchedResultsSectionInfo> info = [[_notificationController sections] objectAtIndex:section];
    return [info numberOfObjects];
}

- (CGFloat)tableView:(UITableView *)tableView heightForRowAtIndexPath:(NSIndexPath *)indexPath
{
    CGFloat height = 9.0f; /* The nine value is the y axis point for both controls on the cell */
    CGFloat padding = 10.0f;
    
    PDNotificationModel * model = [_notificationController objectAtIndexPath:indexPath];
    CGSize textSize = [model.message sizeWithFont:[UIFont systemFontOfSize:15] constrainedToSize:CGSizeMake(293, 400)];
    height += textSize.height + padding;
    
    return height;
}


#pragma mark - UITableView Delegate Methods

- (UITableViewCell *)tableView:(UITableView *)tableView cellForRowAtIndexPath:(NSIndexPath *)indexPath
{
    static NSString *CellIdentifier = @"CellIdentifier";
    
    PDTableViewCell * cell = [tableView dequeueReusableCellWithIdentifier:CellIdentifier];
    if (!cell)
        cell = [[PDTableViewCell alloc] init];
    
    cell.model = [_notificationController objectAtIndexPath:indexPath];
    return cell;
}



#pragma mark - NSFetchedResultController Delegate

- (void)controller:(NSFetchedResultsController *)controller
   didChangeObject:(id)anObject
       atIndexPath:(NSIndexPath *)indexPath
     forChangeType:(NSFetchedResultsChangeType)type
      newIndexPath:(NSIndexPath *)newIndexPath
{
    
    switch(type) {
        case NSFetchedResultsChangeInsert:
            [_notificationView insertRowsAtIndexPaths:@[newIndexPath]
                                     withRowAnimation:UITableViewRowAnimationAutomatic];
            break;
            
        case NSFetchedResultsChangeDelete:
            [_notificationView deleteRowsAtIndexPaths:@[indexPath]
                                     withRowAnimation:UITableViewRowAnimationAutomatic];
            break;
    }
}


#pragma mark - UIAlertViewDelegate Methods

- (void)alertView:(UIAlertView *)alertView clickedButtonAtIndex:(NSInteger)buttonIndex
{
    if (buttonIndex == 1) {
        NSString * host = [alertView textFieldAtIndex:0].text;
        NSArray * components = [host componentsSeparatedByString:@":"];
        if (components.count  == 1)
            host = [NSString stringWithFormat:@"%@:12346", host];
        
        [self showLoadingView];
        [_registrar registerWith:host];
    }
    
}


#pragma mark - PDRegistrarDelegate Methods

- (void)registrarDidRegister
{
    [self hideLoadingView];
}


- (void)registrarFailedWith:(NSError *)error
{
    [self hideLoadingView];
    UIAlertView * alert = [[UIAlertView alloc] initWithTitle:@"Server Error"
                                                     message:@"An error occured trying to connect to the server please try again."
                                                    delegate:nil
                                           cancelButtonTitle:@"OK"
                                           otherButtonTitles:nil];
    [alert show];
}


#pragma mark - UI Events

- (IBAction)didClickSetup:(id)sender
{
    UIAlertView * serverSetup = [[UIAlertView alloc] initWithTitle:@"Connect to Host"
                                                           message:@"Enter the server information to register"
                                                          delegate:self
                                                 cancelButtonTitle:@"Cancel"
                                                 otherButtonTitles:@"Register", nil];
    
    serverSetup.alertViewStyle = UIAlertViewStylePlainTextInput;
    [serverSetup textFieldAtIndex:0].placeholder = @"hostname or hostname:port";
    [serverSetup show];
}


#pragma mark - Instance Methods

-(UIView *)loadingView
{
    if (_loadingView != nil)
        return _loadingView;
    
    CGRect frame = self.view.frame;
    int yPos = (frame.size.height / 2) - 23;
    UIView * innerContainer = [[UIView alloc] initWithFrame:CGRectMake(90, yPos, 145, 55)];
    innerContainer.backgroundColor = [UIColor colorWithRed:30.0/255.0 green:30.0/255.0 blue:30.0/255.0 alpha:1.0];
    innerContainer.layer.cornerRadius = 5;
    
    UIActivityIndicatorView * indicator = [[UIActivityIndicatorView alloc] initWithActivityIndicatorStyle:UIActivityIndicatorViewStyleWhite];
    [indicator setFrame:CGRectMake(9, 19, 20, 20)];
    [indicator startAnimating];
    [innerContainer addSubview:indicator];
    
    UILabel * label = [[UILabel alloc] initWithFrame:CGRectMake(37, 19, 96, 21)];
    label.text = @"Please wait..";
    label.textColor = [UIColor whiteColor];
    label.backgroundColor = [UIColor clearColor];
    [innerContainer addSubview:label];
    
    _loadingView = [[UIView alloc] initWithFrame:CGRectMake(0, 0, frame.size.width, frame.size.height)];
    _loadingView.backgroundColor = [UIColor colorWithRed:0.0 green:0.0 blue:0.0 alpha:0.4300];
    [_loadingView addSubview:innerContainer];
    [self.view addSubview:_loadingView];
    
    return _loadingView;
}


-(void)showLoadingView
{
    [self.loadingView setAlpha:0.0];
    [self.loadingView setHidden:NO];
    
    [UIView transitionWithView:self.loadingView
                      duration:1.0
                       options:UIViewAnimationCurveEaseInOut
                    animations:^() { [self.loadingView setAlpha:1.0]; } completion:nil];
}

-(void)hideLoadingView
{
    [UIView transitionWithView:self.loadingView
                      duration:1.0
                       options:UIViewAnimationCurveEaseInOut
                    animations:^() {
                        [self.loadingView setAlpha:0.0];
                    }
                    completion:^(BOOL b){
                        [self.loadingView setHidden:NO];
                    }];
}


- (IBAction)didPressSetup:(id)sender
{
    UIAlertView * serverSetup = [[UIAlertView alloc] initWithTitle:@"Connect to Host"
                                                           message:@"Enter the server information to register"
                                                          delegate:self
                                                 cancelButtonTitle:@"Cancel"
                                                 otherButtonTitles:@"Register", nil];
    
    serverSetup.alertViewStyle = UIAlertViewStylePlainTextInput;
    [serverSetup textFieldAtIndex:0].placeholder = @"hostname or hostname:port";
    [serverSetup show];
}

@end
