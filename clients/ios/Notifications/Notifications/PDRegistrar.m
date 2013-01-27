//
//  PDRegistrar.m
//  Pusher
//
//  Created by Donelle Sanders Jr on 1/3/13.
//  Copyright (c) 2013 The Potter's Den, Inc. All rights reserved.
//

#import "PDRegistrar.h"

@interface PDRegistrar () <NSURLConnectionDelegate> {
    int _statusCode;
}

@property (retain, nonatomic) NSMutableData * recievedData;
@property (retain, nonatomic) NSMutableDictionary * responseHeaders;

- (NSURLRequest *)prepareRequest:(NSString *)host body:(NSDictionary *)payload;

@end


@implementation PDRegistrar

@synthesize recievedData = _recievedData;
@synthesize responseHeaders = _responseHeaders;
@synthesize delegate = _delegate;


#pragma mark - Instance Methods

-(void)registerWith:(NSString *)host
{
    NSString * deviceToken =  [[NSUserDefaults standardUserDefaults] valueForKey:@"deviceToken"];
    NSMutableDictionary * registration = [[NSMutableDictionary alloc] initWithCapacity:1];
    [registration setValue:deviceToken forKey:@"recipient"];
    
    NSURLRequest * request = [self prepareRequest:host body:registration];
    NSURLConnection * connection = [NSURLConnection connectionWithRequest:request delegate:self];
    if (connection != nil) {
        self.recievedData = [NSMutableData data];
        self.responseHeaders = [NSMutableDictionary dictionary];
    } else {
        NSError * error = [NSError errorWithDomain:@"Registrar"
                                              code:0
                                          userInfo:[NSDictionary dictionaryWithObject:@"Failed to connect to the internet" forKey:@"NSURLErrorNotConnectedToInternet"]];
        [_delegate registrarFailedWith:error];
    }

}

#pragma mark - Private Methods

- (NSURLRequest *)prepareRequest:(NSString *)host body:(NSDictionary *)payload
{
   
    NSString * url = [NSString stringWithFormat:@"http://%@/Services/register", host];
    NSMutableURLRequest *request = [NSMutableURLRequest requestWithURL:[NSURL URLWithString:url] cachePolicy:NSURLRequestReloadIgnoringCacheData timeoutInterval:15];
    
    NSError * error = nil;
#if DEBUG
    NSData * body = [NSJSONSerialization dataWithJSONObject:payload options:NSJSONWritingPrettyPrinted error:&error];
    if (error)
        NSLog(@"Error serializing dictionary: %@\n", error);
    else
        NSLog(@"Sending: %@\n", [NSString stringWithUTF8String:[body bytes]]);
#else
    NSData * body = [NSJSONSerialization dataWithJSONObject:payload options:0 error:&error];
#endif
    
    [request setHTTPMethod:@"POST"];
    [request setValue:@"application/json" forHTTPHeaderField:@"Content-Type"];
    [request setValue:[NSString stringWithFormat:@"%d", [body length]] forHTTPHeaderField:@"Content-Length"];
    [request setHTTPBody:body];
    
    return request;
}



#pragma mark - NSURLConnectionDelegate Methods

- (void)connection:(NSURLConnection *)connection didReceiveResponse:(NSURLResponse *)response;
{
    NSHTTPURLResponse * httpResponse = (NSHTTPURLResponse *)response;
    _statusCode = [httpResponse statusCode];
    [self.responseHeaders setDictionary:httpResponse.allHeaderFields];
    [self.recievedData setLength:0];
}

- (void)connection:(NSURLConnection *)connection didReceiveData:(NSData *)data
{
    [self.recievedData appendData:data];
}

- (void)connectionDidFinishLoading:(NSURLConnection *)connection;
{
    NSError * error = nil;
    
    switch (_statusCode)
    {
        case 500:
        case 404:
        case 400:
        {
#ifdef DEBUG
            NSLog (@"request failed: %@", [NSString stringWithUTF8String:(const char *)[self.recievedData bytes]]);
#endif //DEBUG
            NSString * message = [NSString stringWithFormat:@"Server failed with status code %d", _statusCode];
            error = [NSError errorWithDomain:@"Registrar"
                                        code:_statusCode
                                    userInfo:[NSDictionary dictionaryWithObject:message forKey:NSLocalizedDescriptionKey]];
            [_delegate registrarFailedWith:error];
            break;
        }
            
        default:
            [_delegate registrarDidRegister];
            break;
    }
}

- (void)connection:(NSURLConnection *)connection didFailWithError:(NSError *)error
{
#ifdef DEBUG
    NSLog(@"Connection failed! Error - %@ %@", [error localizedDescription], [[error userInfo] objectForKey:NSURLErrorFailingURLStringErrorKey]);
#endif // DEBUG
    
    [_delegate registrarFailedWith:error];
}


@end
