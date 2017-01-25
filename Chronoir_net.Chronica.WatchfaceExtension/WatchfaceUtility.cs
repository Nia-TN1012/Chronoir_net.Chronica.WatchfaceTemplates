#region Veision Info.
/**
*	@file WatchFaceUtility.cs
*	@brief Provides useful functions for Watch Face application development of Android Wear.
*
*	@par Version
*	1.0.2.0
*	@par Author
*	Nia Tomonaka
*	@par Copyright
*	Copyright (C) 2016-2017 Chronoir.net
*	@par Released Day
*	2016/12/04
*	@par Last modified Day
*	2017/01/25
*	@par Licence
*	MIT Licence
*	@par Contact
*	@@nia_tn1012Åi https://twitter.com/nia_tn1012/ Åj
*	@par Homepage
*	- http://chronoir.net/ (Homepage)
*	- https://github.com/Nia-TN1012/Chronoir_net.Chronica.WatchfaceTemplates/ (GitHub)
*	- https://www.nuget.org/packages/Chronoir_net.Chronica.WatchfaceExtension/ (NuGet Gallery)
*/
#endregion

using System;

using Android.Graphics;
using Android.Text.Format;

namespace Chronoir_net.Chronica.WatchfaceExtension {

	/// <summary>
	///		Provides useful functions for Watch Face application development of Android Wear.
	/// </summary>
	public static class WatchfaceUtility {

		/// <summary>
		///		Converts the integer value containing the ARGB value to <see cref="Color"/> type.
		/// </summary>
		/// <param name="argb">ARGB value</param>
		/// <returns><See cref = "Color" /> type object equivalent to ARGB value</returns>
		public static Color ConvertARGBToColor( int argb ) =>
			Color.Argb( ( argb >> 24 ) & 0xFF, ( argb >> 16 ) & 0xFF, ( argb >> 8 ) & 0xFF, argb & 0xFF );

		/// <summary>
		///		Converts the <see cref="Time"/> type object to <see cref="DateTime"/> type.
		/// </summary>
		/// <param name="time"><see cref="Time"/> type object</param>
		/// <returns><see cref="DateTime"/> type object equivalent to argment</returns>
#if __ANDROID_22__
		[Obsolete( "This method is obsoleted in this android platform." )]
#endif
		public static DateTime ConvertToDateTime( Time time ) =>
			new DateTime( time.Year, time.Month, time.MonthDay, time.Hour, time.Minute, time.Second, DateTimeKind.Local );

		/// <summary>
		///		Converts the <see cref="Java.Lang.Character"/> type object to <see cref="DateTime"/> type.
		/// </summary>
		/// <param name="time"><see cref="Java.Util.Calendar"/> type object</param>
		/// <returns><see cref="DateTime"/> type object equivalent to argment</returns>
		public static DateTime ConvertToDateTime( Java.Util.Calendar time ) =>
			new DateTime(
				// Note : The range of Calender.Month is 0 (January) to 11 (December).
				time.Get( Java.Util.CalendarField.Year ), time.Get( Java.Util.CalendarField.Month ) + 1, time.Get( Java.Util.CalendarField.Date ),
				time.Get( Java.Util.CalendarField.HourOfDay ), time.Get( Java.Util.CalendarField.Minute ), time.Get( Java.Util.CalendarField.Second ),
				DateTimeKind.Local
			);

		/// <summary>
		///		Returns the <see cref="DateTime"/> type object specified as an argument as it is.
		/// </summary>
		/// <param name="time"><see cref="DateTime"/> type object</param>
		/// <returns>Specified argument itself</returns>
		/// <remarks>
		///		This is defined to ensure compatibility with <see cref="ConvertToDateTime(Time)"/> method and <see cref="ConvertToDateTime(Java.Util.Calendar)"/> method.
		/// </remarks>
		public static DateTime ConvertToDateTime( DateTime time ) => time;
	}
}