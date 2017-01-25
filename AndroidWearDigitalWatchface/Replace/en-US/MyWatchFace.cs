using System;

#region Relationship between Java import syntax and C# using directive
/*
	Basically, the following 2 correspond to each other.
	* Package name of Java's import syntax
	* Namespace of using directive in C#
	Note: The package name is all lowercase letters, the namespace is Pascal case (all capital letters in abbreviation)

	* Android.Content
		- Java
			import android.content.BroadcastReceiver;
			import android.content.Context;
			import android.content.Intent;
			import android.content.IntentFilter;
			import android.content.res.Resources;
		- C#
			using Android.Content;

	* Android.Graphics
		- Java
			import android.graphics.Canvas;
			import android.graphics.Color;
			import android.graphics.Paint;
			import android.graphics.Rect;
		- C#
			using Android.Graphics;

	* Android.Graphics.Drawable
		- Java
			import android.graphics.drawable.BitmapDrawable;
		- C#
			using Android.Graphics.Drawable;
	
	* Android.OS
		- Java
			import android.os.Bundle;
			import android.os.Handler;
			import android.os.Message;
		- C#
			using Android.OS;
	
	* Android.Support.Wearable.Watchface
		- Java
			import android.support.wearable.watchface.CanvasWatchFaceService;
			import android.support.wearable.watchface.WatchFaceStyle;
		- C#
			using Android.Support.Wearable.Watchface;

	* Android.Support.V4.Content
		- Java
			import android.support.v4.content.ContextCompat;
		- C#
			using Android.Support.V4.Content;
	
	* Android.Text.Format
		- Java
			import android.text.format.Time;
		- C#
			using Android.Text.Format;

	* Android.View
		- Java
			import android.view.SurfaceHolder;
		- C#
			using Android.Views;
*/
#endregion
using Android.Content;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Support.V4.Content;
using Android.Support.Wearable.Watchface;
using Android.Text.Format;
using Android.Views;

// Used with Service and Metadata attributes.
using Android.App;
// Used in the WallpaperService class.
using Android.Service.Wallpaper;
#if DEBUG
// Used for log output (Only available for debug build).
using Android.Util;
#endif

using Chronoir_net.Chronica.WatchfaceExtension;

namespace AndroidWearDigitalWatchface {

	/// <summary>
	/// 	Provides watch face service of digital watch.
	/// </summary>
	/// <remarks>
	///		Inherits the <see cref="CanvasWatchFaceService"/> class and implement it.
	/// </remarks>
	// In the attribute of C#, configures the contents of the watch face service element.
	// In the Label property, sets the name to be displayed when selecting the watch face.
	// Note: For the Name property;
	//		* Xamarin cannot use Android's Shortcut starting with ".".
	//		* If omitted, the Name is set to "md[GUID].Class name".
	[Service( Label = "@string/my_watch_name", Permission = "android.permission.BIND_WALLPAPER" )]
	// Declares the permission "android.service.wallpaper".
	[MetaData( "android.service.wallpaper", Resource = "@xml/watch_face" )]
	// Specifies the images to be displayed as a preview.
	// Note: "~.preview" is used for square form, "~.preview_circular" is used for round form.
	[MetaData( "com.google.android.wearable.watchface.preview", Resource = "@drawable/preview" )]
	[MetaData( "com.google.android.wearable.watchface.preview_circular", Resource = "@drawable/preview_circular" )]
	// Declares an IntentFilter to use with this Watchface.
	[IntentFilter( new[] { "android.service.wallpaper.WallpaperService" }, Categories = new[] { "com.google.android.wearable.watchface.category.WATCH_FACE" } )]
	public class MyWatchFaceService : CanvasWatchFaceService {

#if DEBUG
		/// <summary>
		///		Represents a tag for log output.
		/// </summary>
		private const string logTag = nameof( MyWatchFaceService );
#endif

		/// <summary>
		/// 	Represents the refresh interval(in milliseconds) in interactive mode.
		/// </summary>
		/// <remarks>
		///		In ambient mode, updated every minute, regardless of the setting of this field.
		/// </remarks>
		#region Example: Setting the update interval to 1 second
		/*
			// Using Java.Util.Concurrent.TimeUnit
			Java.Util.Concurrent.TimeUnit.Seconds.ToMillis( 1 )

			// Using System.TimeSpan (the return value is double type, cast it to long type)
			( long )System.TimeSpan.FromSeconds( 1 ).TotalMilliseconds
		*/
		#endregion
		private static readonly long InteractiveUpdateRateMilliseconds = Java.Util.Concurrent.TimeUnit.Seconds.ToMillis( 1 );

		/// <summary>
		/// 	Represents the message ID of the handler for periodically updating the time in interactive mode.
		/// </summary>
		private const int MessageUpdateTime = 0;


		/// <summary>
		///		Invoked when a <see cref="CanvasWatchFaceService.Engine"/> object is created.
		/// </summary>
		/// <returns>Object inheriting <see cref="CanvasWatchFaceService.Engine"/> class</returns>
		public override WallpaperService.Engine OnCreateEngine() {
			// Specifies thw reference to the MyWatchFace object in the constructor of MyWatchFaceEngine
			// that inherits the CanvasWatchFaceService.Engine class.
			return new MyWatchFaceEngine( this );
		}

		/// <summary>
		///		Provides a <see cref="CanvasWatchFaceService.Engine"/> function of digital watch face.
		/// </summary>
		/// <remarks>
		///		Inherits the <see cref="CanvasWatchFaceService.Engine"/> class and implement it. (Note: <see cref="CanvasWatchFaceService"/> can be omitted.)
		/// </remarks>
		private class MyWatchFaceEngine : Engine {

			/// <summary>
			///		Represents the reference to a <see cref="CanvasWatchFaceService"/> object.
			/// </summary>
			private CanvasWatchFaceService owner;

			#region Time

			/// <summary>
			///		Represents the handler that performs processing when time is updated.
			/// </summary>
			/// <remarks>
			///		It has the role of EngineHandler on Android Studio( Java) side.
			/// </remarks>
			private readonly Handler updateTimeHandler;

			#region Which library should I use to get the current time?
			/*
				1. Android.Text.Format.Time class
				　　  Provided by Android API.
					 In the [Time object].SetToNow method, set it to the current time for the current time zone.
				     In the [Time object].Clear method, set the time zone for the ID of the specified time zone.
				   Note: it is deprecated in Android API Level 22 and later, because it has problems that can only be handled until 2032.
				　　
				2. Java.Util.Calendar class
				　　  Provided by Java.
				     In the Calendar.GetInstance method, gets the current time for the specified time zone.
					 In the [Calendar object].TimeZone property, sets the time zone.
				
				3. System.DateTime structure
				　　  Provided by .NET Framework.
					 In the DateTime.Now property, gets the current time for the current time zone.
					 Note: The time zone is the same as the smartphone's it, paired with the Android Wear device.
			*/
			#endregion

			/// <summary>
			///		Represents the object that stores time.
			/// </summary>
			// Time ( Android )
			//private Time nowTime;
			// Calendar ( Java )
			private Java.Util.Calendar nowTime;
			// DateTime ( C# )
			//private DateTime nowTime;

			#endregion

			#region Graphics

			/// <summary>
			///		Represents the paint object for the background.
			/// </summary>
			private Paint backgroundPaint;

			/// <summary>
			///		Represents an object for time display text.
			/// </summary>
			private DigitalTextStyle digitalTimeText;

			#endregion

			#region Mode

			/// <summary>
			///		Indicates whether the watch face is in ambient mode.
			/// </summary>
			private bool isAmbient;

			/// <summary>
			///		Indicates whether the Android Wear device requires Low-bit ambient mode.
			/// </summary>
			/// <remarks>
			///		<para>When the Android Wear device needs Low-bit ambient mode, in the ambient mode, the following 2 measures are necessary.</para>
			///		<para>* Limit usable colors to only the following 8 colors (black, white, blue, red, magenta, green, cyan, yellow).</para>
			///		<para>* Disable anti-aliasing.</para>
			/// </remarks>
			private bool isRequiredLowBitAmbient;

			/// <summary>
			///		Indicates whether the Android Wear device requires Burn-in-protection.
			/// </summary>
			/// <remarks>
			///		<para>When the device (equipped with organic EL display etc.) needs Burn-in-protection, in the ambient mode, the following two measures are necessary.</para>
			///		<para>* Draws illustrations and images with only outlines as possible.</para>
			///		<para>* Not draw in a few pixels from the edge of the display as possible.</para>
			/// </remarks>
			private bool isReqiredBurnInProtection;

			/// <summary>
			///		Indicates whether the Android Wear device is muted.
			/// </summary>
			private bool isMute;

			#endregion

			#region BroadcastRecievers

			/// <summary>
			///		Represents the receiver that receives notifications when changing the time zone.
			/// </summary>
			private ActionExecutableBroadcastReceiver timeZoneReceiver;

			#endregion

			/// <summary>
			///		Creates a new instance of <see cref="MyWatchFaceEngine"/> class.
			/// </summary>
			/// <param name="owner">Reference to a <see cref="CanvasWatchFaceService"/> object</param>
			public MyWatchFaceEngine( CanvasWatchFaceService owner ) : base( owner ) {
				// Sets a reference to an object that inherits CanvasWatchFaceService class.
				this.owner = owner;
				// Configures processing when updating time.
				updateTimeHandler = new Handler(
					message => {
#if DEBUG
						if( Log.IsLoggable( logTag, LogPriority.Info ) ) {
							Log.Info( logTag, $"Updating timer: Message = {message.What}" );
						}
#endif

						// Determines the message ID with the What property.
						switch( message.What ) {
							case MessageUpdateTime:
								// TODO: Writes here, the processing at the time of the time update message.
								// Redraws the watch face.
								Invalidate();
								// Determinea whether to activate the timer.
								if( ShouldTimerBeRunning ) {
									/*
										In Java, gets Universal Time Coordinated (milli-seconds) by System.currentTimeMillis method.
										Meanwhile, in C#, gets Universal Time Coordinated (100 nano-seconds) by DateTime.UtcNow.Ticks property,
										and divides by TimeSpan.TicksPerMillisecond field to calculate the value in milliseconds.
									*/
									long timeMillseconds = DateTime.UtcNow.Ticks / TimeSpan.TicksPerMillisecond;
									// delayMilliseconds = [Update interval] - ([Current time( milliseconds )] % [Update interval] ) -> Difference from update interval
									long delayMilliseconds = InteractiveUpdateRateMilliseconds - ( timeMillseconds % InteractiveUpdateRateMilliseconds );
									// Sets the message in UpdateTimeHandler.
									updateTimeHandler.SendEmptyMessageDelayed( MessageUpdateTime, delayMilliseconds );
								}
								break;
						}
					}
				);

				// Creates an instance of ActionExecutableBroadcastReceiver.
				timeZoneReceiver = new ActionExecutableBroadcastReceiver(
					intent => {
						// TODO: Writes here, process to be executed when receiving Intent object of broadcasted with Intent.ActionTimezoneChanged.
						// Gets the time zone ID from the Intent, set it as the time zone of the Time object, and get the current time.
						// Time ( Android )
						//nowTime.Clear( intent.GetStringExtra( "time-zone" ) );
						//nowTime.SetToNow();
						// Calendar ( Java )
						nowTime = Java.Util.Calendar.GetInstance( Java.Util.TimeZone.GetTimeZone( intent.GetStringExtra( "time-zone" ) ) );
						// DateTime ( C# )
						//nowTime = DateTime.Now;
					},
					// Specifies "android.intent.action.TIMEZONE_CHANGED" for the Intent filter.
					Intent.ActionTimezoneChanged
				);
			}

			/// <summary>
			///		Invoked when a <see cref="MyWatchFaceEngine"/> object is created.
			/// </summary>
			/// <param name="holder">Object representing display surface</param>
			public override void OnCreate( ISurfaceHolder holder ) {
				// TODO: Here, mainly perform the following processing.
				// * Loads images from resources
				// * Generates graphics objects such as Paint etc.
				// * Generates an object that stores time
				// * Sets the system UI (status icon, notification card and "Ok Google" etc.)

				// Sets the display method of the system UI.
				SetWatchFaceStyle(
					new WatchFaceStyle.Builder( owner )
						#region Setting watch face style

						// Sets whether to enable tap event from user.
						//   true  : Enabled
						//   false : Disabled（default）
						//.SetAcceptsTapEvents( true )

						// Sets the height of the notification card when receiving the notification.
						//   WatchFaceStyle.PeekModeShort    : Displays the notification card small at the bottom of the window.（default）
						//   WatchFaceStyle.PeekModeVariable : Displays the notification card on the front of the window.
						.SetCardPeekMode( WatchFaceStyle.PeekModeShort )

						// Sets the display method of the notification card background.
						//   WatchFaceStyle.BackgroundVisibilityInterruptive : Displays the notification card's background, only when some important notification such as incoming calls.（default）
						//   WatchFaceStyle.BackgroundVisibilityPersistent   : Displays the notification card's background regardless of the notification type.
						.SetBackgroundVisibility( WatchFaceStyle.BackgroundVisibilityInterruptive )

						// Sets whether or not to display notification cards in ambient mode.
						//   WatchFaceStyle.AmbientPeekModeVisible : Visible（default）
						//   WatchFaceStyle.AmbientPeekModeHidden  : Hidden
						//.SetAmbientPeekMode( WatchFaceStyle.AmbientPeekModeHidden )

						// Sets whether to display the digital clock of the system UI. (As an example of use, there is "Simple" which is provided by default.)
						//   true  : Visible
						//   false : Hidden（default）
						.SetShowSystemUiTime( false )

						// Sets whether to add a background to the status icon etc.
						//   Default                                : Nothing
						//   WatchFaceStyle.ProtectStatusBar        : Displays the background on the status icon.
						//   WatchFaceStyle.ProtectHotwordIndicator : Displays the background on the "OK Google".
						//   WatchFaceStyle.ProtectWholeScreen      : Makes the background of the watch face a little darker.
						// Note: Parameters can be combined by logical OR.
						//.SetViewProtectionMode( WatchFaceStyle.ProtectStatusBar | WatchFaceStyle.ProtectHotwordIndicator )

						// Sets whether the notification card is transparent.
						//   WatchFaceStyle.PeekOpacityModeOpaque      : Opacity（default）
						//   WatchFaceStyle.PeekOpacityModeTranslucent : Transparent
						//.SetPeekOpacityMode( WatchFaceStyle.PeekOpacityModeTranslucent )

						// Sets the status icon and "OK Google" position.
						//   GravityFlags.Top | GravityFlags.Left   : Left-Top（Square form's default）
						//   GravityFlags.Top | GravityFlags.Center : Center-Top（Round form's default）
						// Note: Parameters can be combined by logical OR.
						// Note: Casts GravityFlags's value to int type, because it is an enumeration.
						//.SetStatusBarGravity( ( int )( GravityFlags.Top | GravityFlags.Center ) )
						//.SetHotwordIndicatorGravity( ( int )( GravityFlags.Top | GravityFlags.Center ) )

						#endregion
						// Builds the set style information. This method is called last.
						.Build()
				);

				base.OnCreate( holder );

				#region About method that getting color values from resources in the latest Android SDK
				/*
					Android.Content.Res.Resources.GetColor method is deprecated as Android SDK Level 23 or later.
					As an alternative, use the Android.Support.V4.Content.ContextCompat.GetColor method.
					
					[CanvasWatchFaceService object].Resources.GetColor( Resource.Color.[Resource name] );
					↓
					ContextCompat.GetColor( [CanvasWatchFaceService object], Resource.Color.[Resource name] );
					
					Note: CanvasWatchFaceService class inherits Context class.

					However, the return value of ContextCompat.GetColor method, because it is not a color type
					but an int type storing an ARGB value, cannot be directly set to [Paint object].Color.
					-> Using the Chronoir_net.Chronoface.Utility.WatchfaceUtility.ConvertARGBToColor( int ) method,
					   can convert the integer value storing the ARGB value to Color type.
				*/
				#endregion

				// Creates a graphics object for the background.
				backgroundPaint = new Paint();
				// Reads background color from resource.
				backgroundPaint.Color = WatchfaceUtility.ConvertARGBToColor( ContextCompat.GetColor( owner, Resource.Color.background ) );

				// Creates a paint object for time display.
				var digitalTimeTextPaint = new Paint();
				digitalTimeTextPaint.Color = WatchfaceUtility.ConvertARGBToColor( ContextCompat.GetColor( owner, Resource.Color.foreground ) );
				// Sets the text style.
				digitalTimeTextPaint.SetTypeface( Typeface.Default );
				// Eneble anti-aliasing.
				digitalTimeTextPaint.AntiAlias = true;
				// Gets the position of Y coordinate.
				var yOffset = owner.Resources.GetDimension( Resource.Dimension.digital_y_offset );
				// Creates the time display object.
				digitalTimeText = new DigitalTextStyle( paint: digitalTimeTextPaint, yOffset: yOffset );

				// Creates an object that stores time.
				// Time ( Android )
				//nowTime = new Time();
				// Calendar ( Java )
				nowTime = Java.Util.Calendar.GetInstance( Java.Util.TimeZone.Default );
				// DateTime ( C# )
				// This object need not create, because DateTime structure is value type.
			}

			/// <summary>
			///		Invoked when this watch face service is destroyed.
			/// </summary>
			/// <remarks>
			///		For example, invoked when switching from this watch face to another.
			/// </remarks>
			public override void OnDestroy() {
				// Deletes the message ID set in UpdateTimeHandler.
				updateTimeHandler.RemoveMessages( MessageUpdateTime );

				base.OnDestroy();
			}

			/// <summary>
			///		Invoked when applying <see cref="WindowInsets"/>.
			/// </summary>
			/// <param name="insets">Applicable <see cref="WindowInsets"/> object</param>
			/// <remarks>It is possible to determine whether Android Wear's display is round.</remarks>
			public override void OnApplyWindowInsets( WindowInsets insets ) {
				base.OnApplyWindowInsets( insets );

#if DEBUG
				if( Log.IsLoggable( logTag, LogPriority.Info ) ) {
					Log.Info( logTag, $"{nameof( OnApplyWindowInsets )}: Round = {insets.IsRound}" );
				}
#endif

				// TODO: Writes here, the processing to set according to the shape of the window.
				bool isRound = insets.IsRound;
				var xOffset = owner.Resources.GetDimension( isRound ? Resource.Dimension.digital_x_offset_round : Resource.Dimension.digital_x_offset );
				var textSize = owner.Resources.GetDimension( isRound ? Resource.Dimension.digital_text_size_round : Resource.Dimension.digital_text_size );
				digitalTimeText.XOffset = xOffset;
				digitalTimeText.Paint.TextSize = textSize;
			}

			/// <summary>
			///		Invoked when the watch face property is changed.
			/// </summary>
			/// <param name="properties">Bundle object containing property values</param>
			public override void OnPropertiesChanged( Bundle properties ) {
				base.OnPropertiesChanged( properties );

				// Gets the value whether Android Wear device requires Low-bit ambient mode.
				isRequiredLowBitAmbient = properties.GetBoolean( PropertyLowBitAmbient, false );
				// Gets the value whether Android Wear device requires Burn-in-protection.
				isReqiredBurnInProtection = properties.GetBoolean( PropertyBurnInProtection, false );

#if DEBUG
				if( Log.IsLoggable( logTag, LogPriority.Info ) ) {
					Log.Info( logTag, $"{nameof( OnPropertiesChanged )}: Low-bit ambient = {isRequiredLowBitAmbient}" );
					Log.Info( logTag, $"{nameof( OnPropertiesChanged )}: Burn-in-protection = {isReqiredBurnInProtection}" );
				}
#endif
			}

			/// <summary>
			///		Invoked when the time is updated.
			/// </summary>
			/// <remarks>
			///		Invoked every minute regardless of mode.
			/// </remarks>
			public override void OnTimeTick() {
				base.OnTimeTick();

#if DEBUG
				if( Log.IsLoggable( logTag, LogPriority.Info ) ) {
					Log.Info( logTag, $"OnTimeTick" );
				}
#endif

				// Redraws the watch face.
				Invalidate();
			}

			/// <summary>
			///		Invoked when the ambient mode is changed.
			/// </summary>
			/// <param name="inAmbientMode">Value indicating whether it is an ambient mode</param>
			public override void OnAmbientModeChanged( bool inAmbientMode ) {
				base.OnAmbientModeChanged( inAmbientMode );

				// Determines whether ambient mode has changed.
				if( isAmbient != inAmbientMode ) {
#if DEBUG
					if( Log.IsLoggable( logTag, LogPriority.Info ) ) {
						Log.Info( logTag, $"{nameof( OnAmbientModeChanged )}: Ambient-mode = {isAmbient} -> {inAmbientMode}" );
					}
#endif

					// Sets the current ambient mode.
					isAmbient = inAmbientMode;
					// Determines whether Android Wear device requires Low-bit ambient mode.
					if( isRequiredLowBitAmbient ) {
						bool antiAlias = !inAmbientMode;

						// TODO: Writes here, processing when Android Wear device requires Low-bit ambient mode.
						digitalTimeText.Paint.AntiAlias = antiAlias;
					}

					Invalidate();
					UpdateTimer();
				}
			}

			/// <summary>
			///		Invoked when the Interruption filter is changed.
			/// </summary>
			/// <param name="interruptionFilter">Interruption filter</param>
			public override void OnInterruptionFilterChanged( int interruptionFilter ) {
				base.OnInterruptionFilterChanged( interruptionFilter );

				// Calculates the value whether in mute mode.
				bool inMuteMode = ( interruptionFilter == InterruptionFilterNone );

				// Determines whether mute mode has changed.
				if( isMute != inMuteMode ) {
#if DEBUG
					if( Log.IsLoggable( logTag, LogPriority.Info ) ) {
						Log.Info( logTag, $"{nameof( OnInterruptionFilterChanged )}: Mute-mode = {isMute} -> {inMuteMode}" );
					}
#endif

					isMute = inMuteMode;
					// TODO: Writes here, process to be executed when changing the mute mode.

					Invalidate();
				}
			}

			/// <summary>
			///		Invoked when the user taps the watch face.
			/// </summary>
			/// <param name="tapType">Tap type</param>
			/// <param name="xValue">X position of tap</param>
			/// <param name="yValue">Y position of tap</param>
			/// <param name="eventTime">Time of the event (milliseconds)</param>
			/// <remarks>
			///		This is compatible with Android Wear 1.3 or higher.
			///		In order for this method to be called, in <see cref="WatchFaceStyle.Builder"/> creation, call method and specify true for argument.
			///		This is only available in interactive mode.
			///	</remarks>
			public override void OnTapCommand( int tapType, int xValue, int yValue, long eventTime ) {
#if DEBUG
				if( Log.IsLoggable( logTag, LogPriority.Info ) ) {
					Log.Info( logTag, $"{nameof( OnTapCommand )}: Type = {tapType}, ( x, y ) = ( {xValue}, {yValue} ), Event time = {eventTime}" );
				}
#endif

				//var resources = owner.Resources;

				#region Order of occurrence of tap events
				/*
					* Tap
						TapTypeTouch -> TapTypeTap
					* Long tap, Hold, Swipe, Flick, Pinch-in / out
						TapTypeTouch -> TapTypeTouchCancel
				*/
				#endregion

				// Determines the tap type.
				switch( tapType ) {
					case TapTypeTouch:
						// TODO: Writes here, processing when the user touches the screen.
						break;
					case TapTypeTouchCancel:
						// TODO: Writes here, processing when the user long presses the screen or moves the finger after the user touches the screen.
						break;
					case TapTypeTap:
						// TODO: Writes here, processing when you release your finger from the screen after the user touches the screen.
						break;
				}
			}

			/// <summary>
			///		Invoke when drawing a watch face.
			/// </summary>
			/// <param name="canvas"><see cref="Canvas"/> object for drawing on the watch face</param>
			/// <param name="bounds">Object storing the screen size</param>
			public override void OnDraw( Canvas canvas, Rect bounds ) {
				// TODO: Writes here, process of acquiring the current time and drawing the watch face.
				// Gets current time.
				// Time ( Android )
				//nowTime.SetToNow();
				// Calendar ( Java )
				nowTime = Java.Util.Calendar.GetInstance( nowTime.TimeZone );
				// DateTime ( C# )
				//nowTime = DateTime.Now;

#if DEBUG
				if( Log.IsLoggable( logTag, LogPriority.Info ) ) {
					Log.Info( logTag, $"{nameof( OnDraw )}: Now time = {WatchfaceUtility.ConvertToDateTime( nowTime ):yyyy/MM/dd HH:mm:ss K}" );
				}
#endif

				// Draws the background.
				// Determines whether in ambient mode.
				if( IsInAmbientMode ) {
					// In ambient mode, fills with black color.
					canvas.DrawColor( Color.Black );
				}
				else {
					// Otherwise, draws the background graphics.
					canvas.DrawRect( 0, 0, canvas.Width, canvas.Height, backgroundPaint );
				}

				// Draws the text of the current time.
				canvas.DrawText(
					WatchfaceUtility.ConvertToDateTime( nowTime ).ToString( isAmbient ? "HH:mm" : "HH:mm:ss" ),
					digitalTimeText.XOffset, digitalTimeText.YOffset, digitalTimeText.Paint
				);
			}

			/// <summary>
			///		Invoked when the watch face is switched between display / non-display.
			/// </summary>
			/// <param name="visible">Display / non-display of watch face</param>
			public override void OnVisibilityChanged( bool visible ) {
				base.OnVisibilityChanged( visible );

#if DEBUG
				if( Log.IsLoggable( logTag, LogPriority.Info ) ) {
					Log.Info( logTag, $"{nameof( OnVisibilityChanged )}: Visible = {visible}" );
				}
#endif

				// Determines whether the watch face is displayed or not.
				if( visible ) {
					if( timeZoneReceiver == null ) {
						timeZoneReceiver = new ActionExecutableBroadcastReceiver(
							intent => {
								// Time ( Android )
								//nowTime.Clear( intent.GetStringExtra( "time-zone" ) );
								//nowTime.SetToNow();
								// Calendar ( Java )
								nowTime = Java.Util.Calendar.GetInstance( Java.Util.TimeZone.GetTimeZone( intent.GetStringExtra( "time-zone" ) ) );
								// DateTime ( C# )
								//nowTime = DateTime.Now;
							},
							Intent.ActionTimezoneChanged
						);
					}
					// Register the BroadcastReciever for the time zone.
					timeZoneReceiver.IsRegistered = true;
					// Updates time in case the time zone changes when the watch face is hidden.
					// Time ( Android )
					//nowTime.Clear( Java.Util.TimeZone.Default.ID );
					//nowTime.SetToNow();
					// Calendar ( Java )
					nowTime = Java.Util.Calendar.GetInstance( Java.Util.TimeZone.Default );
					// DateTime ( C# )
					//nowTime = DateTime.Now;
				}
				else {
					// Unregister the BroadcastReciever for the time zone.
					timeZoneReceiver.IsRegistered = false;
				}

				UpdateTimer();
			}

			/// <summary>
			///		Update timer.
			/// </summary>
			private void UpdateTimer() {
#if DEBUG
				if( Log.IsLoggable( logTag, LogPriority.Info ) ) {
					Log.Info( logTag, $"{nameof( UpdateTimer )}" );
				}
#endif

				// Remove message ID from UpdateTimeHandler.
				updateTimeHandler.RemoveMessages( MessageUpdateTime );
				// Determines whether to activate the timer.
				if( ShouldTimerBeRunning ) {
					// Sends message ID to UpdateTimeHandler.
					updateTimeHandler.SendEmptyMessage( MessageUpdateTime );
				}
			}

			/// <summary>
			///		Gets a value indicating whether to activate the timer.
			/// </summary>
			private bool ShouldTimerBeRunning =>
				IsVisible && !IsInAmbientMode;
		}
	}
}