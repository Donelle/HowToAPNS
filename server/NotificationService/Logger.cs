#region Copyright Information
//
// Logger.cs
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

namespace NotificationService {
	using System;
	using log4net;
	using log4net.Config;

	[Flags]
	public enum NSLogLevel {
		Debug = 2,
		Info = 4,
		Warn = 8,
		Error = 16,
		Fatal = 32
	};

	public static class NSLogger {
		#region Fields
		static readonly ILog logger;
		#endregion

		#region Construction

		static NSLogger ()
		{
			XmlConfigurator.Configure ();
			logger = LogManager.GetLogger (typeof (NSLogger));
		}

		#endregion

		#region Methods

		public static void LogException (Exception ex)
		{
			Log (NSLogLevel.Fatal, ex.DebugException ());
		}

		public static void Log (NSLogLevel level, String message)
		{
			if (level == (NSLogLevel.Debug & level))
				logger.Debug (message);

			if (level == (NSLogLevel.Error & level))
				logger.Error (message);

			if (level == (NSLogLevel.Fatal & level))
				logger.Fatal (message);

			if (level == (NSLogLevel.Info & level))
				logger.Info (message);

			if (level == (NSLogLevel.Warn & level))
				logger.Warn (message);

		}

		#endregion
	}
}
