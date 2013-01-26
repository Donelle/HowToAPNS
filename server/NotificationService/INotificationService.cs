#region Copyright Information
//
// INotificationService.cs
// NotificationService
// 
// Copyright (c) 2012 Donelle Sanders Jr <donellesanders@thepottersden.com>
// Copyright (c) 2012 The Potter's Den, Inc. All rights reserved.
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
	using System;
	using System.Collections.Generic;
	using System.ServiceModel;
	using System.ServiceModel.Web;
	using System.Runtime.Serialization;
	using System.Net.Sockets;
	using System.Net;
	using System.Threading.Tasks;
	using System.Text;


	[ServiceContract (Namespace = "http://thepottersden.com/")]
	public interface INotificationService 
	{
		[OperationContract]
		[WebInvoke (Method = "POST", UriTemplate = "/register", BodyStyle = WebMessageBodyStyle.WrappedRequest, RequestFormat = WebMessageFormat.Json)]
		void Register (String recipient);

		[OperationContract (IsOneWay = true), WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.Wrapped)]
		void Push (String message, IEnumerable<String> recipients);

		[OperationContract]
		IEnumerable<KeyValuePair<String, String>> GetRecipients ();
	}


	[ServiceBehavior (IncludeExceptionDetailInFaults = true, InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Multiple)]
	public sealed class NotificationService : INotificationService {
		#region Fields
		Dictionary<String, String> _recipients = new Dictionary<String, String> ();
		#endregion

		#region IPusherService Members

		void INotificationService.Register (String recipient)
		{
			recipient.Guard ("recipient parameter was null");
			Boolean bFound = false;

			_recipients.ForEach (item => {
				if (item.Value.Equals (recipient)) 
					return bFound = true;

				return false;
			});

			if (!bFound) _recipients.Add (String.Concat("Recipient_", _recipients.Count), recipient);
		}

		void INotificationService.Push (String message, IEnumerable<String> recipients)
		{
			message.Guard ("message parameter was null");
			recipients.Guard ("recipient parameter was null");

			ApnsService.GetChannel ().PostNotification (message, recipients);
		}

		IEnumerable<KeyValuePair<String, String>> INotificationService.GetRecipients ()
		{
			return _recipients;
		}

		#endregion

	}
}
