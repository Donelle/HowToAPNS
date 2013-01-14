#region Copyright Information
//
// Extensions.cs
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
	using System.Collections;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;
	using System.Runtime.Serialization.Formatters.Binary;
	using System.Diagnostics;
	using System.Reflection;

	internal static class PSExtensions {

		internal static void Guard<T> (this T obj, string message)
		{
			if (obj == null)
				throw new ArgumentNullException (message);

		}

		internal static Boolean IsNullOrDefault<T> (this T obj) where T : class
		{
			return obj == null || obj == default (T);
		}

		internal static void ForEach<T> (this IEnumerable<T> source, Action<T> action)
		{
			foreach (var item in source) {
				action (item);
			}
		}

		internal static void ForEach<T> (this IEnumerable<T> source, Func<T, Boolean> action)
		{
			foreach (var item in source) {
				if (action (item))
					break;
			}
		}

		internal static T[] Collapse<T> (this IEnumerable<T[]> source)
		{
			Int32 bufferSize = 0;
			source.ForEach (element => bufferSize += element.Length);

			Int32 position = 0;
			T[] buffer = new T[bufferSize];
			source.ForEach (element => {
				Buffer.BlockCopy (element, 0, buffer, position, element.Length);
				position += element.Length;
			});

			return buffer;
		}

		internal static String DebugException (this Exception ex)
		{
			StringBuilder trace = new StringBuilder ("--- Beginning of Error Stack ---");
			trace.AppendLine (Environment.NewLine);

			Exception exception = ex;
			while (exception != null) {
				StackTrace stack = new StackTrace (exception, true);
				StackFrame frame = stack.GetFrame (stack.FrameCount - 1);

				List<String> paramList = new List<String> ();
				paramList.Add (String.Concat ("Summary: ", exception.Message ?? String.Empty));
				paramList.Add (String.Concat ("Source: ", exception.Source ?? String.Empty));

				if (frame != null) {
					MethodInfo method = (MethodInfo)frame.GetMethod ();
#if DEBUG
					paramList.Add (String.Concat ("Object: ", method.DeclaringType.FullName, ".", method.Name));
					paramList.Add (String.Concat ("Location: ", frame.GetFileName ()));
					paramList.Add (String.Concat ("Line: ", frame.GetFileLineNumber ()));
#else

					paramList.Add (String.Concat ("Object: ", method.DeclaringType.FullName, ".", method.Name));
#endif
				} else {
					paramList.Add ("Location: Uknown there was no stack frame");
				}

				paramList.Add (String.Concat ("Stack Trace: ", Environment.NewLine, ex.StackTrace));
				String line = String.Join (Environment.NewLine, paramList.ToArray ());
				trace.AppendLine (String.Concat (line, Environment.NewLine));
				exception = exception.InnerException;
			}

			trace.AppendLine ("-- End of Error Stack ---");
			return trace.ToString ();
		}
	}
}
