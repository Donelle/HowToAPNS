#region Copyright Information
//
// ApnsServiceConfiguration.cs
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
	using System.Configuration;

	public class ApnsServiceConfiguration : ConfigurationSection  {
		#region Fields
		/// <summary>
		/// The name of the data configuration section.
		/// </summary>
	        const String SectionName = "apnsService";
		#endregion

		#region Construction

		public ApnsServiceConfiguration () : base ()
		{ }

		#endregion

		#region Properties

		[ConfigurationProperty ("Certificate", IsRequired = true)]
		public string Certificate
		{
			get
			{
				String val = this["Certificate"] as String;
				return val ?? String.Empty;
			}
			set { this["Certificate"] = value; }
		}

		[ConfigurationProperty ("CertificatePassword", IsRequired = true)]
		public string Password
		{
			get
			{
				String val = this["CertificatePassword"] as String;
				return val ?? String.Empty;
			}
			set { this["CertificatePassword"] = value; }
		}


		#endregion

		#region Methods

		internal static ApnsServiceConfiguration GetConfiguration ()
		{
			return ConfigurationManager.GetSection (SectionName) as ApnsServiceConfiguration;
		}

		#endregion
	}
}

