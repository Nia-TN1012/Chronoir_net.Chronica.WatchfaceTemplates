#region Veision Info.
/**
*	@file AnalogHandStroke.cs
*	@brief Represents the basis of the analog meter stroke function stored a Paint object, a XY coordinate of a hand tip and hand length.
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
*	@@nia_tn1012（ https://twitter.com/nia_tn1012/ ）
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
	///		Represents the basis of the analog meter stroke function stored a <see cref="Android.Graphics.Paint"/> object, a XY coordinate of a hand tip and hand length.
	/// </summary>
	public abstract class AnalogHandStroke {
		/// <summary>
		///		Gets the <see cref="Android.Graphics.Paint"/> object of the hand.
		/// </summary>
		public Paint Paint { get; private set; }

		/// <summary>
		///		Gets the X coordinate of the hand tip.
		/// </summary>
		public float X { get; protected set; } = 0.0f;
		/// <summary>
		///		Gets the Y coordinate of the hand tip.
		/// </summary>
		public float Y { get; protected set; } = 0.0f;

		/// <summary>
		///		Gets the length of the hand.
		/// </summary>
		public float Length { get; set; } = 0.0f;

		/// <summary>
		///		Creates a new instance of <see cref="AnalogHandStroke"/> class from the specified <see cref="Android.Graphics.Paint"/> object and hand length.
		/// </summary>
		/// <param name="paint"><see cref="Android.Graphics.Paint"/> object of the hand</param>
		/// <param name="length">Hand lendth</param>
		public AnalogHandStroke( Paint paint, float length = 0.0f ) {
			Paint = paint ?? new Paint();
			Length = length;
		}
	}

	/// <summary>
	///		Provides analog meter stroke function for second hand.
	/// </summary>
	public class SecondAnalogHandStroke : AnalogHandStroke {

		/// <summary>
		///		Creates a new instance of <see cref="SecondAnalogHandStroke"/> class from the specified <see cref="Paint"/> object and hand length.
		/// </summary>
		/// <param name="paint"><see cref="Paint"/> object of the hand</param>
		/// <param name="length">Hand lendth</param>
		public SecondAnalogHandStroke( Paint paint, float length = 0.0f ) : base( paint, length ) { }

		/// <summary>
		///		Calculates the XY coordinates of the second hand tip for the specified seconds.
		/// </summary>
		/// <param name="second">Second ( 0～59 )</param>
		public void SetTime( int second ) {
			// Because 2π = 360 degrees is divided into 60, the angle per second is 6 degrees.
			// θ_sec = second / 30 * π
			float handRotation = second / 30f * ( float )Math.PI;
			// Converts from polar coordinates to XY coordinates at the second hand tip coordinates.
			X = ( float )Math.Sin( handRotation ) * Length;
			Y = ( float )-Math.Cos( handRotation ) * Length;
		}

		/// <summary>
		///		Calculates the XY coordinates of the second hand tip for the seconds of the specified <see cref="Time"/> object.
		/// </summary>
		/// <param name="time"><see cref="Time"/> object storing the time</param>
#if __ANDROID_22__
		[Obsolete( "This method is obsoleted in this android platform." )]
#endif
		public void SetTime( Time time ) => SetTime( time.Second );

		/// <summary>
		///		Calculates the XY coordinates of the second hand tip for the seconds of the specified <see cref="Java.Util.Calendar"/> object.
		/// </summary>
		/// <param name="time"><see cref="Java.Util.Calendar"/> object storing the time</param>
		public void SetTime( Java.Util.Calendar time ) => SetTime( time.Get( Java.Util.CalendarField.Second ) );

		/// <summary>
		///		Calculates the XY coordinates of the second hand tip for the seconds of the specified <see cref="DateTime"/> object.
		/// </summary>
		/// <param name="time"><see cref="DateTime"/> object storing the time</param>
		public void SetTime( DateTime time ) => SetTime( time.Second );
	}

	/// <summary>
	///		Provides analog meter stroke function for minute hand.
	/// </summary>
	public class MinuteAnalogHandStroke : AnalogHandStroke {

		/// <summary>
		///		Creates a new instance of <see cref="MinuteAnalogHandStroke"/> class from the specified <see cref="Paint"/> object and hand length.
		/// </summary>
		/// <param name="paint"><see cref="Paint"/> object of the hand</param>
		/// <param name="length">Hand lendth</param>
		public MinuteAnalogHandStroke( Paint paint, float length = 0.0f ) : base( paint, length ) { }

		/// <summary>
		///		Calculates the XY coordinates of the minute hand tip for the specified minutes.
		/// </summary>
		/// <param name="minute">Minute ( 0～59 )</param>
		public void SetTime( int minute ) {
			// As with the second hand, the angle per minute is 6 degrees.
			float handRotation = minute / 30f * ( float )Math.PI;
			X = ( float )Math.Sin( handRotation ) * Length;
			Y = ( float )-Math.Cos( handRotation ) * Length;
		}

		/// <summary>
		///		Calculates the XY coordinates of the minute hand tip for the minutes of the specified <see cref="Time"/> object.
		/// </summary>
		/// <param name="time"><see cref="Time"/> object storing the time</param>
#if __ANDROID_22__
		[Obsolete( "This method is obsoleted in this android platform." )]
#endif
		public void SetTime( Time time ) => SetTime( time.Minute );

		/// <summary>
		///		Calculates the XY coordinates of the minute hand tip for the minutes of the specified <see cref="Java.Util.Calendar"/> object.
		/// </summary>
		/// <param name="time"><see cref="Java.Util.Calendar"/> object storing the time</param>
		public void SetTime( Java.Util.Calendar time ) => SetTime( time.Get( Java.Util.CalendarField.Minute ) );

		/// <summary>
		///		Calculates the XY coordinates of the minute hand tip for the minutes of the specified <see cref="DateTime"/> object.
		/// </summary>
		/// <param name="time"><see cref="DateTime"/> object storing the time</param>
		public void SetTime( DateTime time ) => SetTime( time.Minute );
	}

	/// <summary>
	///		Provides analog meter stroke function for hour hand.
	/// </summary>
	public class HourAnalogHandStroke : AnalogHandStroke {

		/// <summary>
		///		Creates a new instance of <see cref="HourAnalogHandStroke"/> class from the specified <see cref="Paint"/> object and hand length.
		/// </summary>
		/// <param name="paint"><see cref="Paint"/> object of the hand</param>
		/// <param name="length">Hand lendth</param>
		public HourAnalogHandStroke( Paint paint, float length = 0.0f ) : base( paint, length ) { }

		/// <summary>
		///		Calculates the XY coordinates of the hour hand tip for the specified hours and minutes.
		/// </summary>
		/// <param name="hour">Hour ( 0～23 )</param>
		/// <param name="minute">Minute ( 0～59 )</param>
		public void SetTime( int hour, int minute ) {
			float handRotation = ( ( hour + ( minute / 60f ) ) / 6f ) * ( float )Math.PI;
			X = ( float )Math.Sin( handRotation ) * Length;
			Y = ( float )-Math.Cos( handRotation ) * Length;
		}

		/// <summary>
		///		Calculates the XY coordinates of the hour hand tip for the hours and minutes of the specified <see cref="Time"/> object.
		/// </summary>
		/// <param name="time"><see cref="Time"/> object storing the time</param>
#if __ANDROID_22__
		[Obsolete( "This method is obsoleted in this android platform." )]
#endif
		public void SetTime( Time time ) => SetTime( time.Hour, time.Minute );

		/// <summary>
		///		Calculates the XY coordinates of the hour hand tip for the hours and minutes of the specified <see cref="Java.Util.Calendar"/> object.
		/// </summary>
		/// <param name="time"><see cref="Java.Util.Calendar"/> object storing the time</param>
		public void SetTime( Java.Util.Calendar time ) => SetTime( time.Get( Java.Util.CalendarField.Hour ), time.Get( Java.Util.CalendarField.Minute ) );

		/// <summary>
		///		Calculates the XY coordinates of the hour hand tip for the hours and minutes of the specified <see cref="DateTime"/> object.
		/// </summary>
		/// <param name="time"><see cref="DateTime"/> object storing the time</param>
		public void SetTime( DateTime time ) => SetTime( time.Hour, time.Minute );
	}
}