#region Copyright Information
//
// INotificationService.cs
// NotificationService
// 
// Copyright (c) 2012 Donelle Sanders Jr <donellesanders@thepottersden.com>
// Copyright (c) 2012 The Potter's Den, Inc. All rights reserved.
// Copyright (c) PushSharp https://github.com/Redth/PushSharp 
//	
//  This program is free software: you can redistribute it and/or modify it
//  under the terms of the GNU Affero General Public License version 3,
//  as published by the Free Software Foundation.
//	
//  This program is distributed in the hope that it will be useful, but
//  WITHOUT ANY WARRANTY; without even the implied warranties of
//  MERCHANTABILITY, SATISFACTORY QUALITY, or FITNESS FOR A PARTICULAR
//  PURPOSE.  See the GNU Affero General Public License for more details.
//
//  You should have received a copy of the GNU Affero General Public License
//  along with this program.  If not, see <http://www.gnu.org/licenses/>.
#endregion

namespace NotificationService 
{
	#region Namespaces
	using System;
	using System.Collections.Generic;
	using System.Collections.Concurrent;
	using System.Linq;
	using System.IO;
	using System.Net.Sockets;
	using System.Net.Security;
	using System.Security.Authentication;
	using System.Security.Cryptography.X509Certificates;
	using System.Threading;
	using System.Threading.Tasks;
	using System.Net;
	using System.Text;
	using System.Web.Script.Serialization;
	#endregion

	internal class ApnsService {
		#region Fields
		readonly String apnsHostName = "gateway.sandbox.push.apple.com";
		static readonly Object syncRoot = new Object ();
		static ApnsService _apnsService = new ApnsService ();
		TcpClient _connection;
		X509Certificate _certificate;
		SslStream _sslStream;
		#endregion

		#region Construction

	        ApnsService ()
		{
			_connection = new TcpClient ();
		}

		~ApnsService ()
		{
			if (_connection.Connected)
				_connection.Close ();

			if (!_sslStream.IsNullOrDefault ()) {
				_sslStream.Close ();
				_sslStream.Dispose ();
			}
		}

		#endregion

		#region Properties

		Boolean IsConnected {
			get {
				return _connection.Connected;
			}
		}

		#endregion

		#region Public Methods

		internal void PostNotification(String notification, IEnumerable<String> recipients)
		{
			try {
				if (!this.IsConnected)
					GetChannel ();
#if DEBUG
				NSLogger.Log (NSLogLevel.Info, String.Format ("Sending notification: {0}...", notification));
#endif
				recipients.ToBytes (notification).ForEach (
					recipient => _sslStream.Write (recipient, 0, recipient.Length)
				);

#if DEBUG
				NSLogger.Log (NSLogLevel.Info, "Notification Sent!");
#endif
			} catch (Exception ex) {
				NSLogger.LogException (ex);
			}
		}

		
		#endregion

		#region Plumbing

		/// <summary>
		/// Sets up a connection to APNS and initializes the thread for sending notifications
		/// </summary>
		void _Connect ()
		{
			var configuration = ApnsServiceConfiguration.GetConfiguration ();
			_certificate = new X509Certificate2 (File.ReadAllBytes (configuration.Certificate), configuration.Password,
				X509KeyStorageFlags.MachineKeySet | X509KeyStorageFlags.PersistKeySet | X509KeyStorageFlags.Exportable);
			try {
				if (!_connection.IsNullOrDefault ())
					_connection.Close ();

#if DEBUG
				NSLogger.Log (NSLogLevel.Info, "Connecting to APNS...");
#endif
				_connection = new TcpClient (apnsHostName, 2195);

				if (!_sslStream.IsNullOrDefault ())
					_sslStream.Close ();

				_sslStream = new SslStream (_connection.GetStream (), false,
						new RemoteCertificateValidationCallback ((sender, cert, chain, sslPolicyErrors) => { return true; }),
						new LocalCertificateSelectionCallback ((sender, targetHost, localCerts, remoteCert, acceptableIssuers) => {
							return _certificate;
						}));

				var certificates = new X509CertificateCollection { _certificate };
				_sslStream.AuthenticateAsClient (apnsHostName, certificates, SslProtocols.Ssl3, false);

				if (!_sslStream.IsMutuallyAuthenticated)
					throw new ApplicationException ("SSL Stream Failed to Authenticate", null);

				if (!_sslStream.CanWrite)
					throw new ApplicationException ("SSL Stream is not Writable", null);

#if DEBUG
				NSLogger.Log (NSLogLevel.Info, "Connected!");
#endif

			} catch (Exception) {
				if (_connection.Connected) {
					_connection.Close ();
				}

				if (!_sslStream.IsNullOrDefault ()) {
					_sslStream.Close ();
					_sslStream.Dispose ();
				}
				throw;
			}
		}


		#endregion

		#region Static Methods

		public static ApnsService GetChannel ()
		{
			lock (syncRoot) {
				if (!_apnsService.IsConnected) {
					_apnsService._Connect ();
				}
			}

			return _apnsService;
		}

		#endregion
	}


	internal static class ApnsServiceExtension {
		#region Helper Methods
		static int nextIdentifier = 1;
		static Object nextIdentifierLock = new Object ();
		static DateTime UNIX_EPOCH = new DateTime (1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

		static int GetNextIdentifier ()
		{
			lock (nextIdentifierLock) {
				if (nextIdentifier >= int.MaxValue - 10)
					nextIdentifier = 1;

				return nextIdentifier++;
			}
		}

		#endregion

		#region Extension Methods

		internal static String ToJson (this String notification)
		{
			var json = new StringBuilder ();
			new JavaScriptSerializer ().Serialize (new {
				aps = new {
					alert = new { body = notification },
					badge = 1
				}
			}, json);

			return json.ToString ();
		}

		internal static List<Byte[]> ToBytes (this IEnumerable<String> deviceTokens, String notification)
		{
			const int DEVICE_TOKEN_BINARY_SIZE = 32;
			const int MAX_PAYLOAD_SIZE = 256;

			Byte[] identifierBytes = BitConverter.GetBytes (IPAddress.HostToNetworkOrder (GetNextIdentifier ()));
			//
			// APNS will not store-and-forward a notification with no expiry, so set it one month in the future
			// if the client does not provide it.
			//
			DateTime expireDate = DateTime.UtcNow.AddMonths (1).ToUniversalTime ();
			Int32 expiryTimeStamp = (Int32)(expireDate - UNIX_EPOCH).TotalSeconds;
			Byte[] expiry = BitConverter.GetBytes (IPAddress.HostToNetworkOrder (expiryTimeStamp));
			//
			// Build out the notification
			//
			Byte[] payload = Encoding.UTF8.GetBytes (notification.ToJson());
			if (payload.Length > MAX_PAYLOAD_SIZE) {
				int newSize = notification.Length - (payload.Length - MAX_PAYLOAD_SIZE);
				if (newSize > 0) {
					notification = notification.Substring (0, newSize);
					payload = Encoding.UTF8.GetBytes (notification.ToJson ());
				} else {
					do {
						notification = notification.Remove (notification.Length - 1);
						payload = Encoding.UTF8.GetBytes (notification.ToJson ());
					} while (payload.Length > MAX_PAYLOAD_SIZE && !string.IsNullOrEmpty (notification));
				}

				if (payload.Length > MAX_PAYLOAD_SIZE)
					throw new InvalidOperationException ("Payload exceeds maximum size");
			}

			Byte[] payloadSize = BitConverter.GetBytes (IPAddress.HostToNetworkOrder (Convert.ToInt16 (payload.Length)));
			//
			// Loop through and create notification payloads for each recipient we are sending 
			// a notification to
			//
			List<Byte[]> recipients = new List<Byte[]> (deviceTokens.Count ());
			deviceTokens.ForEach (deviceToken => {

				Byte[] token = new Byte[DEVICE_TOKEN_BINARY_SIZE];
				for (int i = 0; i < deviceToken.Length; i++)
					token[i] = byte.Parse (deviceToken.Substring (i * 2, 2), System.Globalization.NumberStyles.HexNumber);

				if (deviceToken.Length != DEVICE_TOKEN_BINARY_SIZE)
					throw new InvalidOperationException ("Invalid device token");

				Byte[] tokenSize = BitConverter.GetBytes (IPAddress.HostToNetworkOrder (Convert.ToInt16 (token.Length)));
				recipients.Add (
					new List<Byte[]> {
						{ new Byte[] { 0x01 } }, // Enhanced notification format command
						{ identifierBytes },
						{ expiry },
						{ tokenSize },
						{ token },
						{ payloadSize },
						{ payload }
					}.Collapse ()
				);
			});

			return recipients;
		}

		#endregion
	}
}
