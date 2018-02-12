#region Veision Info.
/**
*	@file BroadcastRecieverExtension.cs
*	@brief
*	Provides a BroadcastReceiver class equipped functions to register and unregister to Application.Context,
*	and execute the specified delegate or events when a broadcasted Intent object is received.
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

using Android.App;
using Android.Content;

namespace Chronoir_net.Chronica.WatchfaceExtension {

	/// <summary>
	///		Provides a <see cref="BroadcastReceiver"/> class equipped functions to register and unregister to <see cref="Application.Context"/>.
	/// </summary>
	public abstract class RegistrationSwitchableBroadcastReceiver : BroadcastReceiver {

		/// <summary>
		///		Represents the MIME type for identifying <see cref="Intent"/> information.
		/// </summary>
		private IntentFilter intentFilter;

		/// <summary>
		///		Indicates whether <see cref="BroadcastReceiver"/> object is registered in <see cref="Application.Context"/>.
		/// </summary>
		private bool isRegistered = false;
		/// <summary>
		///		Gets and sets the value indicating whether <see cref="BroadcastReceiver"/> object is registered in <see cref="Application.Context"/>.
		/// </summary>
		/// <value>true : Registered / false : Unregisered</value>
		public bool IsRegistered {
			get => isRegistered;
			set {
				if( value != isRegistered ) {
					if( value ) {
						Application.Context.RegisterReceiver( this, intentFilter );
					}
					else {
						Application.Context.UnregisterReceiver( this );
					}
					isRegistered = value;
				}
			}
		}

		/// <summary>
		///		Creates a new instance of <see cref="RegistrationSwitchableBroadcastReceiver"/> class from the specified <see cref="Intent"/> MIME type.
		/// </summary>
		/// <param name="filter">MIME type for identifying <see cref="Intent"/> information</param>
		public RegistrationSwitchableBroadcastReceiver( string filter ) => intentFilter = new IntentFilter( filter );

		/// <summary>
		///		Register the <see cref="BroadcastReceiver"/> to <see cref="Application.Context"/>.
		/// </summary>
		public void RegisterToContext() => IsRegistered = true;

		/// <summary>
		///		Unregister the <see cref="BroadcastReceiver"/> from <see cref="Application.Context"/>.
		/// </summary>
		public void UnregisterFromContext() => IsRegistered = false;
	}

	/// <summary>
	///		Provides a <see cref="BroadcastReceiver"/> class that execute the specified delegate when a broadcasted <see cref="Intent"/> object is received.
	/// </summary>
	public class ActionExecutableBroadcastReceiver : RegistrationSwitchableBroadcastReceiver {

		/// <summary>
		///		Represents the delegate executed when a broadcasted <see cref="Intent"/> is received.
		/// </summary>
		private Action<Intent> broadcastedIntentRecieved;

		/// <summary>
		///		Creates a new instance of <see cref="ActionExecutableBroadcastReceiver"/> class from the specified delegate and <see cref="Intent"/> MIME type.
		/// </summary>
		/// <param name="action">Delegate to execute when receiving <see cref="Intent"/></param>
		/// <param name="filter">MIME type for identifying <see cref="Intent"/> information</param>
		public ActionExecutableBroadcastReceiver( Action<Intent> action, string filter ) : base( filter ) => broadcastedIntentRecieved = action;

		/// <summary>
		///		Invoked when receives a broadcast <see cref="Intent"/>.
		/// </summary>
		/// <param name="context"><see cref="Context"/> object registering receiver</param>
		/// <param name="intent">Broadcasted <see cref="Intent"/> object</param>
		public override void OnReceive( Context context, Intent intent ) => broadcastedIntentRecieved?.Invoke( intent );
	}

	/// <summary>
	///		Provides a <see cref="BroadcastReceiver"/> class that excecute the specified events when a broadcasted <see cref="Intent"/> object is received.
	/// </summary>
	public class EventExecutableBroadcastReceiver : RegistrationSwitchableBroadcastReceiver {

		/// <summary>
		///		Represents the event handler executed when a broadcasted <see cref="Intent"/> object is received.
		/// </summary>
		public event EventHandler<Intent> BroadcastedIntentRecieved;

		/// <summary>
		///		Creates a new instance of <see cref="EventExecutableBroadcastReceiver"/> class from the specified <see cref="Intent"/> MIME type.
		/// </summary>
		/// <param name="filter">MIME type for identifying <see cref="Intent"/> information</param>
		public EventExecutableBroadcastReceiver( string filter ) : base( filter ) { }

		/// <summary>
		///		Invoked when receives a broadcasted <see cref="Intent"/>.
		/// </summary>
		/// <param name="context"><see cref="Context"/> object registering receiver (Passed to the delegate method's "sender".)</param>
		/// <param name="intent">Broadcasted <see cref="Intent"/> object (Passed to the delegate method's "e".)</param>
		public override void OnReceive( Context context, Intent intent ) => BroadcastedIntentRecieved?.Invoke( context, intent );
	}
}