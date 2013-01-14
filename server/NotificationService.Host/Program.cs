#region Copyright Information
//
// Program.cs
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

namespace NotificationService.Host {
	using System;
	using System.Collections.Generic;
	using System.Net;
	using System.ServiceModel;
	using System.Linq;
	using System.Net.Sockets;

	class Program {
		static void Main (string[] args)
		{
			ServiceHost host = new ServiceHost (typeof (NotificationService));

			try {
				host.Open ();
				var port = host.Description.Endpoints[0].Address.Uri.Port;
				var address = Dns.GetHostAddresses (Dns.GetHostName ())
						 .Where (ip => ip.AddressFamily == AddressFamily.InterNetwork)
						 .First ();

				Console.WriteLine ("The service is ready and listening on: {0}:{1}\n", address, port);
				Console.WriteLine ("Press <ENTER> to terminate service");
				Console.ReadLine ();
			} catch (System.TimeoutException ex) {
				Console.WriteLine (ex.Message);
				Console.ReadLine ();
			} catch (CommunicationException ex) {
				Console.WriteLine (ex.Message);
				Console.ReadLine ();
			} finally {
				host.Close ();
			}
		}
	}
}
