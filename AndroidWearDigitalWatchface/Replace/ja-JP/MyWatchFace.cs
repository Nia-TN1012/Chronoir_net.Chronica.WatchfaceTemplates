using System;

#region Java��import�\����C#��using�f�B���N�e�B�u�Ƃ̊֌W
/*
	��{�I�ɁA
	�EJava��import�\���̃p�b�P�[�W��
	�EC#��using�f�B���N�e�B�u�̖��O���
	�����݂ɑΉ����Ă��܂��B
	�i�A���A�p�b�P�[�W���͑S�ď������ŁA���O��Ԃ̓p�X�J���P�[�X�i���̂̏ꍇ�͂��ׂđ啶���j�ł��j

	�� Android.Content
	�EJava
		import android.content.BroadcastReceiver;
		import android.content.Context;
		import android.content.Intent;
		import android.content.IntentFilter;
		import android.content.res.Resources;
	�EC#
		using Android.Content;

	�� Android.Graphics
	�EJava
		import android.graphics.Canvas;
		import android.graphics.Color;
		import android.graphics.Paint;
		import android.graphics.Rect;
	�EC#
		using Android.Graphics;

	�� Android.Graphics.Drawable
	�EJava
		import android.graphics.drawable.BitmapDrawable;
	�EC#
		using Android.Graphics.Drawable;
	
	�� Android.OS
	�EJava
		import android.os.Bundle;
		import android.os.Handler;
		import android.os.Message;
	�EC#
		using Android.OS;
	
	�� Android.Support.Wearable.Watchface
	�EJava
		import android.support.wearable.watchface.CanvasWatchFaceService;
		import android.support.wearable.watchface.WatchFaceStyle;
	�EC#
		using Android.Support.Wearable.Watchface;

	�� Android.Support.V4.Content
	�EJava
		import android.support.v4.content.ContextCompat;
	�EC#
		using Android.Support.V4.Content;
	
	�� Android.Text.Format
	�EJava
		import android.text.format.Time;
	�EC#
		using Android.Text.Format;

	�� Android.View
	�EJava
		import android.view.SurfaceHolder;
	�EC#
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

// Service�����AMetadata�����Ŏg�p���܂��B
using Android.App;
// WallpaperService�N���X�Ŏg�p���܂��B
using Android.Service.Wallpaper;
using Android.Util;

using Chronoir_net.Chronica.WatchfaceExtension;

namespace $safeprojectname$ {

	/// <summary>
	/// 	�f�W�^�����v�̃E�H�b�`�t�F�C�X�T�[�r�X��񋟂��܂��B
	/// </summary>
	/// <remarks>
	///		<see cref="CanvasWatchFaceService"/>�N���X���p�����Ď������܂��B
	/// </remarks>
	// C#�̑����ŁA�E�H�b�`�t�F�C�X�T�[�r�X�v�f�̓��e���\�����܂��B
	// Label�v���p�e�B�ɂ́A�E�H�b�`�t�F�C�X�̑I���ɕ\�����閼�O��ݒ肵�܂��B
	// ���FName�v���p�e�B�ɂ��� : 
	//		Xamarin�ł́A�u.�v����n�܂� Android��Shortcut �����p�ł��܂���B
	//�@		�Ȃ��A�ȗ������ꍇ�AName�ɂ́umd[GUID].�N���X���v�ƂȂ�܂��B
	[Service( Label = "@string/my_watch_name", Permission = "android.permission.BIND_WALLPAPER" )]
	// �ǎ��Ƀo�C���h����p�[�~�b�V������錾���܂��B
	[MetaData( "android.service.wallpaper", Resource = "@xml/watch_face" )]
	// �v���r���[�ɕ\������摜���w�肵�܂��B
	// preview���l�p�`�p�Apreview_circular���ی`�p�ł��B
	[MetaData( "com.google.android.wearable.watchface.preview", Resource = "@drawable/preview" )]
	[MetaData( "com.google.android.wearable.watchface.preview_circular", Resource = "@drawable/preview_circular" )]
	// ����Watch Face�Ŏg�p����IntentFilter��錾���܂��B
	[IntentFilter( new[] { "android.service.wallpaper.WallpaperService" }, Categories = new[] { "com.google.android.wearable.watchface.category.WATCH_FACE" } )]
	public class MyWatchFaceService : CanvasWatchFaceService {

		/// <summary>
		///		���O�o�͗p�̃^�O��\���܂��B
		/// </summary>
		private const string logTag = nameof( MyWatchFaceService );

		/// <summary>
		/// 	�C���^���N�e�B�u���[�h�ɂ�����X�V�Ԋu�i�~���b�P�ʁj��\���܂��B
		/// </summary>
		/// <remarks>
		///		�A���r�G���g���[�h�̎��́A���̃t�B�[���h�̐ݒ�ɂ�����炸�A1���Ԋu�ōX�V����܂��B
		/// </remarks>
		#region �� : �X�V�Ԋu��1�b�ɐݒ肷��ꍇ
		/*
			// Java.Util.Concurrent.TimeUnit���g�p����ꍇ
			Java.Util.Concurrent.TimeUnit.Seconds.ToMillis( 1 )

			// System.TimeSpan���g�p����ꍇ�i �߂�l��double�^�Ȃ̂ŁAlong�^�ɃL���X�g���܂� �j
			( long )System.TimeSpan.FromSeconds( 1 ).TotalMilliseconds
		*/
		#endregion
		private static readonly long InteractiveUpdateRateMilliseconds = Java.Util.Concurrent.TimeUnit.Seconds.ToMillis( 1 );

		/// <summary>
		/// 	�C���^���N�e�B�u���[�h�ɂāA����I�Ɏ������X�V���邽�߂́A�n���h���[�p�̃��b�Z�[�W��ID��\���܂��B
		/// </summary>
		private const int MessageUpdateTime = 0;


		/// <summary>
		///		<see cref="CanvasWatchFaceService.Engine"/>�I�u�W�F�N�g���쐬����鎞�Ɏ��s���܂��B
		/// </summary>
		/// <returns><see cref="CanvasWatchFaceService.Engine"/>�N���X���p�������I�u�W�F�N�g</returns>
		public override WallpaperService.Engine OnCreateEngine() {
			// CanvasWatchFaceService.Engine�N���X���p������MyWatchFaceEngine�̃R���X�g���N�^�[�ɁA
			// MyWatchFace�I�u�W�F�N�g�̎Q�Ƃ��w�肵�܂��B
			return new MyWatchFaceEngine( this );
		}

		/// <summary>
		///		�A�i���O���v�̃E�H�b�`�t�F�C�X��<see cref="CanvasWatchFaceService.Engine"/>�@�\��񋟂��܂��B
		/// </summary>
		/// <remarks>
		///		<see cref="CanvasWatchFaceService.Engine"/>�N���X���p�����Ď������܂��B��<see cref="CanvasWatchFaceService"/>�͏ȗ��\�ł��B
		/// </remarks>
		private class MyWatchFaceEngine : Engine {

			/// <summary>
			///		<see cref="CanvasWatchFaceService"/>�I�u�W�F�N�g�̎Q�Ƃ�\���܂��B
			/// </summary>
			private CanvasWatchFaceService owner;

			#region �^�C�}�[�n

			/// <summary>
			///		�������X�V�������̏�����\���܂��B
			/// </summary>
			/// <remarks>
			///		Android Studio�i Java�n �j���ł����AEngineHandler�̖����������܂��B
			/// </remarks>
			private readonly Handler updateTimeHandler;

			#region ���ݎ����̎擾�ɁA�ǂ̃��C�u�������g�p����΂悢�̂ł����H
			/*
			 *		1. Android.Text.Format.Time�N���X
			 *		�@�@Android��API�ŗp�ӂ���Ă�����t�E�����̃N���X�ł��B
			 *		�@�@Time�I�u�W�F�N�g.SetToNow���\�b�h�ŁA���݂̃^�C���]�[���ɑ΂��錻�ݎ����ɃZ�b�g���邱�Ƃ��ł��܂��B
			 *		�@�@Time�I�u�W�F�N�g.Clear���\�b�h�ŁA�w�肵���^�C���]�[����ID�ɑ΂���^�C���]�[����ݒ肵�܂��B
			 *		�@�@��2032�N�܂ł��������Ȃ���肪���邽�߁AAndroid API Level 22�ȍ~�ł͋��`���ƂȂ��Ă��܂��B
			 *		�@�@
			 *		2. Java.Util.Calendar�N���X
			 *		�@�@Java�ŗp�ӂ���Ă�����t�E�����̃N���X�ł��B
			 *		�@�@Calendar.GetInstance���\�b�h�ŁA���݂̃^�C���]�[���ɑ΂��錻�ݎ����ɃZ�b�g���邱�Ƃ��ł��܂��B
			 *		�@�@Calendar�I�u�W�F�N�g.TimeZone�v���p�e�B�ŁA�^�C���]�[����ݒ肷�邱�Ƃ��ł��܂��B
			 *		
			 *		3. System.DateTime�\����
			 *		�@�@.NET Framework�ŗp�ӂ���Ă�����t�E�����̃N���X�ł��B
			 *		�@�@DateTime.Now�ŁA���݂̃^�C���]�[���ɑ΂��錻�ݎ������擾���邱�Ƃ��ł��܂��B
			 *		�@�@���^�C���]�[���́AAndroid Wear�f�o�C�X�ƃy�A�����O���Ă���X�}�[�g�t�H���̃^�C���]�[���ƂȂ�܂��B
			 */
			#endregion

			/// <summary>
			///		�������i�[����I�u�W�F�N�g��\���܂��B
			/// </summary>
			// Time ( Android )
			//private Time nowTime;
			// Calendar ( Java )
			private Java.Util.Calendar nowTime;
			// DateTime ( C# )
			//private DateTime nowTime;

			#endregion

			#region �O���t�B�b�N�X�n

			/// <summary>
			///		�w�i�p�̃y�C���g�I�u�W�F�N�g��\���܂��B
			/// </summary>
			private Paint backgroundPaint;

			/// <summary>
			///		�����\���̃e�L�X�g�p�̃I�u�W�F�N�g��\���܂��B
			/// </summary>
			private DigitalTextStyle digitalTimeText;

			#endregion

			#region ���[�h�n

			/// <summary>
			///		�A���r�G���g���[�h�ł��邩�ǂ�����\���܂��B
			/// </summary>
			private bool isAmbient;

			/// <summary>
			///		�f�o�C�X��LowBit�A���r�G���g���[�h��K�v�Ƃ��Ă��邩�ǂ�����\���܂��B
			/// </summary>
			/// <remarks>
			///		<para>�f�o�C�X��LowBit�A���r�G���g���[�h���g�p����ꍇ�A�A���r�G���g���[�h�̎��́A�ȉ���2�_�̍H�v���K�v�ɂȂ�܂��B</para>
			///		<para>�E�g�p�ł���F��8�F�i �u���b�N�A�z���C�g�A�u���[�A���b�h�A�}�[���^�A�O���[���A�V�A���A�C�G���[ �j�݂̂ƂȂ�܂��B</para>
			///		<para>�E�A���`�G�C���A�X�������ƂȂ�܂��B</para>
			/// </remarks>
			private bool isRequiredLowBitAmbient;

			/// <summary>
			///		�f�o�C�X��Burn-in-protection�i�Ă��t���h�~�j��K�v�Ƃ��Ă��邩�ǂ�����\���܂��B
			/// </summary>
			/// <remarks>
			///		<para>
			///			�f�B�X�v���C���L�@EL�ȂǁABurn-in-protection���K�v�ȏꍇ�A�A���r�G���g���[�h�̎��́A�ȉ���2�_�̍H�v���K�v�ɂȂ�܂��B
			///		</para>
			///		<para>�E�摜�͂Ȃ�ׂ��֊s�݂̂ɂ��܂��B</para>
			///		<para>�E�f�B�X�v���C�̒[���琔�s�N�Z���ɂ͕`�悵�Ȃ��悤�ɂ��܂��B</para>
			/// </remarks>
			private bool isReqiredBurnInProtection;

			/// <summary>
			///		�~���[�g��Ԃł��邩�ǂ�����\���܂��B
			/// </summary>
			private bool isMute;

			#endregion

			#region ���V�[�o�[

			/// <summary>
			///		�^�C���]�[����ύX�������ɒʒm���󂯎�郌�V�[�o�[��\���܂��B
			/// </summary>
			private ActionExecutableBroadcastReceiver timeZoneReceiver;

			#endregion

			/// <summary>
			///		<see cref="MyWatchFaceEngine"/>�N���X�̐V�����C���X�^���X�𐶐����܂��B
			/// </summary>
			/// <param name="owner"><see cref="CanvasWatchFaceService"/>�N���X���p�������I�u�W�F�N�g�̎Q��</param>
			public MyWatchFaceEngine( CanvasWatchFaceService owner ) : base( owner ) {
				// CanvasWatchFaceService�N���X���p�������I�u�W�F�N�g�̎Q�Ƃ��Z�b�g���܂��B
				this.owner = owner;
				// �������X�V�������̏������\�����܂��B
				updateTimeHandler = new Handler(
					message => {
#if DEBUG
						if( Log.IsLoggable( logTag, LogPriority.Info ) ) {
							Log.Info( logTag, $"Updating timer: Message = {message.What}" );
						}
#endif

						// What�v���p�e�B�Ń��b�Z�[�W�𔻕ʂ��܂��B
						switch( message.What ) {
							case MessageUpdateTime:
								// TODO : �����̍X�V�̃��b�Z�[�W�̎��̏��������܂��B
								// �E�H�b�`�t�F�C�X���ĕ`�悵�܂��B
								Invalidate();
								// �^�C�}�[�𓮍삳���邩�ǂ����𔻕ʂ��܂��B
								if( ShouldTimerBeRunning ) {
									/*
										Java�ł́ASystem.currentTimeMillis���\�b�h�Ő��E���莞�i�~���b�j���擾���܂��B
										���C#�ł́ADateTime.UtcNow.Ticks�v���p�e�B�Ő��E���莞�i100�i�m�b�j�擾���A
										TimeSpan.TicksPerMillisecond�t�B�[���h�Ŋ����āA�~���b�̒l�����߂܂��B
									*/
									long timeMillseconds = DateTime.UtcNow.Ticks / TimeSpan.TicksPerMillisecond;
									// delayMs = �X�V�Ԋu - ( ���ݎ����i�~���b�j % �X�V�Ԋu) -> �X�V�Ԋu�Ƃ̍�
									long delayMilliseconds = InteractiveUpdateRateMilliseconds - ( timeMillseconds % InteractiveUpdateRateMilliseconds );
									// UpdateTimeHandler�Ƀ��b�Z�[�W���Z�b�g���܂��B
									// SendEmptyMessageDelayed���\�b�h�͎w�肵�����Ԍ�Ƀ��b�Z�[�W�𔭍s���܂��B
									updateTimeHandler.SendEmptyMessageDelayed( MessageUpdateTime, delayMilliseconds );
								}
								break;
						}
					}
				);

				// TimeZoneReceiver�̃C���X�^���X�𐶐����܂��B
				timeZoneReceiver = new ActionExecutableBroadcastReceiver(
					intent => {
						// TODO : �u���[�h�L���X�g���ꂽ Intent.ActionTimezoneChanged ��Intent�I�u�W�F�N�g���󂯎�������Ɏ��s���鏈�������܂��B
						// Intent����^�C���]�[��ID���擾���āATime�I�u�W�F�N�g�̃^�C���]�[���ɐݒ肵�A���ݎ������擾���܂��B
						// intent.GetStringExtra( "time-zone" )�̖߂�l�̓^�C���]�[����ID�ł��B
						// Time ( Android )
						//nowTime.Clear( intent.GetStringExtra( "time-zone" ) );
						//nowTime.SetToNow();
						// Calendar ( Java )
						nowTime = Java.Util.Calendar.GetInstance( Java.Util.TimeZone.GetTimeZone( intent.GetStringExtra( "time-zone" ) ) );
						// DateTime ( C# )
						//nowTime = DateTime.Now;
					},
					// Intent�t�B���^�[�ɁuActionTimezoneChanged�v���w�肵�܂��B
					Intent.ActionTimezoneChanged
				);
			}

			/// <summary>
			///		<see cref="MyWatchFaceEngine"/>�̃C���X�^���X���������ꂽ���Ɏ��s���܂��B
			/// </summary>
			/// <param name="holder">�f�B�X�v���C�\�ʂ�\���I�u�W�F�N�g</param>
			public override void OnCreate( ISurfaceHolder holder ) {
				// TODO : �����ł͎�ɁA�ȉ��̏������s���܂��B
				// �E���\�[�X����摜�̓ǂݍ���
				// �EPaint�Ȃǂ̃O���t�B�b�N�X�I�u�W�F�N�g�𐶐�
				// �E�������i�[����I�u�W�F�N�g�̍쐬
				// �E�V�X�e����UI�i�C���W�P�[�^�[��OK Google�̕\���Ȃǁj�̐ݒ�

				// �V�X�e����UI�̔z�u���@��ݒ肵�܂��B
				SetWatchFaceStyle(
					new WatchFaceStyle.Builder( owner )
						#region �E�H�b�`�t�F�C�X�̃X�^�C���̐ݒ�

						// ���[�U�[����̃^�b�v�C�x���g��L���ɂ��邩�ǂ����ݒ肵�܂��B
						//   true  : �L��
						//   false : �����i�f�t�H���g�j
						//.SetAcceptsTapEvents( true )

						// �ʒm���������̒ʒm�J�[�h�̍�����ݒ肵�܂��B
						//   WatchFaceStyle.PeekModeShort    : �ʒm�J�[�h���E�B���h�E�̉����ɏ������\�����܂��B�i�f�t�H���g�j
						//   WatchFaceStyle.PeekModeVariable : �ʒm�J�[�h���E�B���h�E�̑S�ʂɕ\�����܂��B
						.SetCardPeekMode( WatchFaceStyle.PeekModeShort )

						// �ʒm�J�[�h�̔w�i�̕\�����@��ݒ肵�܂��B
						//   WatchFaceStyle.BackgroundVisibilityInterruptive : �d�b�̒��M�Ȃǈꕔ�̒ʒm�̂݁A�w�i��p�����܂��B�i�f�t�H���g�j
						//   WatchFaceStyle.BackgroundVisibilityPersistent   : �ʒm�J�[�h�̎�ނɂ�����炸�A���̔w�i����ɕ\�����܂��B
						.SetBackgroundVisibility( WatchFaceStyle.BackgroundVisibilityInterruptive )

						// �A���r�G���g���[�h���ɒʒm�J�[�h��\�����邩�ǂ�����ݒ肵�܂��B
						//   WatchFaceStyle.AmbientPeekModeVisible : �ʒm�J�[�h��\�����܂��B�i�f�t�H���g�j
						//   WatchFaceStyle.AmbientPeekModeHidden  : �ʒm�J�[�h��\�����܂���B
						//.SetAmbientPeekMode( WatchFaceStyle.AmbientPeekModeHidden )

						// �V�X�e��UI�̃f�W�^�����v��\�����邷�邩�ǂ�����ݒ肵�܂��B�i�g�p���Ă����Ƃ��āA�f�t�H���g�ŗp�ӂ���Ă���u�V���v���v������܂��B�j
						//   true  : �\��
						//   false : ��\���i�f�t�H���g�j
						.SetShowSystemUiTime( false )

						// �X�e�[�^�X�A�C�R���Ȃǂɔw�i��t���邩�ǂ�����ݒ肵�܂��B
						//   �f�t�H���g                               : �X�e�[�^�X�A�C�R���Ȃǂɔw�i��\�����܂���B
						//   WatchFaceStyle.ProtectStatusBar        : �X�e�[�^�X�A�C�R���ɔw�i��\�����܂��B
						//   WatchFaceStyle.ProtectHotwordIndicator : �uOK Google�v�ɔw�i��\�����܂��B
						//   WatchFaceStyle.ProtectWholeScreen      :�@�E�H�b�`�t�F�C�X�̔w�i�������Â߂ɂ��܂��B
						// ���p�����[�^�[�͘_���a�őg�ݍ��킹�邱�Ƃ��ł��܂��B
						//.SetViewProtectionMode( WatchFaceStyle.ProtectStatusBar | WatchFaceStyle.ProtectHotwordIndicator )

						// �ʒm�J�[�h�𓧖��ɂ��邩�ǂ�����ݒ肵�܂��B
						//   WatchFaceStyle.PeekOpacityModeOpaque      : �s�����i�f�t�H���g�j
						//   WatchFaceStyle.PeekOpacityModeTranslucent : ����
						//.SetPeekOpacityMode( WatchFaceStyle.PeekOpacityModeTranslucent )

						// �X�e�[�^�X�A�C�R����uOK Google�v�̈ʒu��ݒ肵�܂��B
						//   GravityFlags.Top | GravityFlags.Left   : ����i�p�`�̃f�t�H���g�j
						//   GravityFlags.Top | GravityFlags.Center : �㕔�̒����i�ی`�̃f�t�H���g�j
						// �� : GravityFlags�͗񋓑̂Ȃ̂ŁAint�^�ɃL���X�g���܂��B
						//.SetStatusBarGravity( ( int )( GravityFlags.Top | GravityFlags.Center ) )
						//.SetHotwordIndicatorGravity( ( int )( GravityFlags.Top | GravityFlags.Center ) )

						#endregion
						// �ݒ肵���X�^�C�������r���h���܂��B���̃��\�b�h�͍Ō�ɌĂяo���܂��B
						.Build()
				);
				// �x�[�X�N���X��OnCreate���\�b�h�����s���܂��B
				base.OnCreate( holder );

				#region �ŐV��Android SDK�ɂ�����AAndroid.Content.Res.Resources.GetColor���\�b�h�ɂ���
				/*
					Android.Content.Res.Resources.GetColor���\�b�h�́AAndroid SDK Level 23�ȍ~�Ŕ񐄏��iDeprecated�j�ƂȂ��Ă��܂��B
					����̕��@�Ƃ��āAAndroid.Support.V4.Content.ContextCompat.GetColor���\�b�h���g�p���܂��B
					
					[CanvasWatchFaceService�I�u�W�F�N�g].Resources.GetColor( Resource.Color.[���\�[�X��] );
					��
					ContextCompat.GetColor( [CanvasWatchFaceService�I�u�W�F�N�g], Resource.Color.[���\�[�X��] );
					��CanvasWatchFaceService�N���X��Context�N���X���p�����Ă��܂��B

					�Ȃ��AContextCompat.GetColor�̖߂�l��Color�^�łȂ��AARGB�l���i�[����int�^�ƂȂ�܂��B
					Chronoir_net.Chronica.WatchfaceExtension.WatchfaceUtility.ConvertARGBToColor( int )���\�b�h�ŁAColor�^�ɕϊ����邱�Ƃ��ł��܂��B
				*/
				#endregion

				// �w�i�p�̃O���t�B�b�N�X�I�u�W�F�N�g�𐶐����܂��B
				backgroundPaint = new Paint();
				// ���\�[�X����w�i�F��ǂݍ��݂܂��B
				backgroundPaint.Color = WatchfaceUtility.ConvertARGBToColor( ContextCompat.GetColor( owner, Resource.Color.background ) );

				// �����\���p�̃y�C���g�I�u�W�F�N�g�𐶐����܂��B
				var digitalTimeTextPaint = new Paint();
				digitalTimeTextPaint.Color = WatchfaceUtility.ConvertARGBToColor( ContextCompat.GetColor( owner, Resource.Color.foreground ) );
				// �����̃X�^�C����ݒ肵�܂��B
				digitalTimeTextPaint.SetTypeface( Typeface.Default );
				// �A���`�G�C���A�X��L���ɂ��܂��B
				digitalTimeTextPaint.AntiAlias = true;
				// Y���W�̈ʒu���擾���܂��B
				var yOffset = owner.Resources.GetDimension( Resource.Dimension.digital_y_offset );
				// �����\���p�I�u�W�F�N�g�����������܂��B
				digitalTimeText = new DigitalTextStyle( paint: digitalTimeTextPaint, yOffset: yOffset );

				// �������i�[����I�u�W�F�N�g�𐶐����܂��B
				// Time ( Android )
				//nowTime = new Time();
				// Calendar ( Java )
				nowTime = Java.Util.Calendar.GetInstance( Java.Util.TimeZone.Default );
				// DateTime ( C# )
				// DateTime�\���̂͒l�^�Ȃ̂ŁA�I�u�W�F�N�g�̐����͕͂s�v�ł��B
			}

			/// <summary>
			///		���̃E�H�b�`�t�F�C�X�T�[�r�X���j������鎞�Ɏ��s���܂��B
			/// </summary>
			/// <remarks>
			///		�Ⴆ�΁A���̃E�H�b�`�t�F�C�X����ʂ̃E�H�b�`�t�F�C�X�ɐ؂�ւ������ɌĂяo����܂��B
			/// </remarks>
			public override void OnDestroy() {
				// UpdateTimeHandler�ɃZ�b�g����Ă��郁�b�Z�[�W���폜���܂��B
				updateTimeHandler.RemoveMessages( MessageUpdateTime );
				// �x�[�X�N���X��OnDestroy���\�b�h�����s���܂��B
				base.OnDestroy();
			}

			/// <summary>
			///		<see cref="WindowInsets"/>��K�p���鎞�Ɏ��s���܂��B
			/// </summary>
			/// <param name="insets">�K�p�����<see cref="WindowInsets"/>�I�u�W�F�N�g</param>
			/// <remarks>Android Wear���ی`���ǂ������A���̃��\�b�h���Ŕ��ʂ��邱�Ƃ��ł��܂��B</remarks>
			public override void OnApplyWindowInsets( WindowInsets insets ) {
				base.OnApplyWindowInsets( insets );

#if DEBUG
				if( Log.IsLoggable( logTag, LogPriority.Info ) ) {
					Log.Info( logTag, $"{nameof( OnApplyWindowInsets )}: Round = {insets.IsRound}" );
				}
#endif

				// TODO: �E�B���h�E�̌`��ɂ���Đݒ肷�鏈�������܂��B
				// Android Wear���ی`���ǂ����𔻕ʂ��܂��B
				bool isRound = insets.IsRound;
				var xOffset = owner.Resources.GetDimension( isRound ? Resource.Dimension.digital_x_offset_round : Resource.Dimension.digital_x_offset );
				var textSize = owner.Resources.GetDimension( isRound ? Resource.Dimension.digital_text_size_round : Resource.Dimension.digital_text_size );
				digitalTimeText.XOffset = xOffset;
				digitalTimeText.Paint.TextSize = textSize;
			}

			/// <summary>
			///		�E�H�b�`�t�F�C�X�̃v���p�e�B���ύX���ꂽ���Ɏ��s���܂��B
			/// </summary>
			/// <param name="properties">�v���p�e�B�l���i�[�����o���h���I�u�W�F�N�g</param>
			public override void OnPropertiesChanged( Bundle properties ) {
				// �x�[�X�N���X��OnPropertiesChanged���\�b�h�����s���܂��B
				base.OnPropertiesChanged( properties );
				// LowBit�A���r�G���g���[�h���g�p���邩�ǂ����̒l���擾���܂��B
				isRequiredLowBitAmbient = properties.GetBoolean( PropertyLowBitAmbient, false );
				// Burn-in-protection���[�h���g�p���邩�ǂ����̒l���擾���܂��B
				isReqiredBurnInProtection = properties.GetBoolean( PropertyBurnInProtection, false );

#if DEBUG
				if( Log.IsLoggable( logTag, LogPriority.Info ) ) {
					Log.Info( logTag, $"{nameof( OnPropertiesChanged )}: Low-bit ambient = {isRequiredLowBitAmbient}" );
					Log.Info( logTag, $"{nameof( OnPropertiesChanged )}: Burn-in-protection = {isReqiredBurnInProtection}" );
				}
#endif
			}

			/// <summary>
			///		���Ԃ��X�V�������Ɏ��s���܂��B
			/// </summary>
			/// <remarks>
			///		��ʂ̕\���E��\���⃂�[�h�Ɋւ�炸�A1�����ƂɌĂяo����܂��B
			/// </remarks>
			public override void OnTimeTick() {
				// �x�[�X�N���X��OnTimeTick���\�b�h�����s���܂��B
				base.OnTimeTick();

#if DEBUG
				if( Log.IsLoggable( logTag, LogPriority.Info ) ) {
					Log.Info( logTag, $"OnTimeTick" );
				}
#endif

				// �E�H�b�`�t�F�C�X���ĕ`�悵�܂��B
				Invalidate();
			}

			/// <summary>
			///		�A���r�G���g���[�h���ύX���ꂽ���Ɏ��s����܂��B
			/// </summary>
			/// <param name="inAmbientMode">�A���r�G���g���[�h�ł��邩�ǂ����������l</param>
			public override void OnAmbientModeChanged( bool inAmbientMode ) {
				// �x�[�X�N���X��OnAmbientModeChanged���\�b�h�����s���܂��B
				base.OnAmbientModeChanged( inAmbientMode );

				// �A���r�G���g���[�h���ύX���ꂽ���ǂ����𔻕ʂ��܂��B
				if( isAmbient != inAmbientMode ) {
#if DEBUG
					if( Log.IsLoggable( logTag, LogPriority.Info ) ) {
						Log.Info( logTag, $"{nameof( OnAmbientModeChanged )}: Ambient-mode = {isAmbient} -> {inAmbientMode}" );
					}
#endif

					// ���݂̃A���r�G���g���[�h���Z�b�g���܂��B
					isAmbient = inAmbientMode;
					// �f�o�C�X��LowBit�A���r�G���g���[�h���T�|�[�g���Ă��邩�ǂ����𔻕ʂ��܂��B
					if( isRequiredLowBitAmbient ) {
						bool antiAlias = !inAmbientMode;

						// TODO : LowBit�A���r�G���g���[�h���T�|�[�g����Ă��鎞�̏��������܂��B
						// �A���r�G���g���[�h�̎��́A�e�L�X�g��Paint�I�u�W�F�N�g�̃A���`�G�C���A�X�𖳌��ɂ��A
						// �����łȂ���ΗL���ɂ��܂��B
						digitalTimeText.Paint.AntiAlias = antiAlias;

						// �E�H�b�`�t�F�C�X���ĕ`�悵�܂��B
						Invalidate();
					}
					// �^�C�}�[���X�V���܂��B
					UpdateTimer();
				}
			}

			/// <summary>
			///		Interruption�t�B���^�[���ύX���ꂽ���Ɏ��s���܂��B
			/// </summary>
			/// <param name="interruptionFilter">Interruption�t�B���^�[</param>
			public override void OnInterruptionFilterChanged( int interruptionFilter ) {
				// �x�[�X�N���X��OnInterruptionFilterChanged���\�b�h�����s���܂��B
				base.OnInterruptionFilterChanged( interruptionFilter );
				// Interruption�t�B���^�[���ύX���ꂽ���ǂ������ʂ��܂��B
				bool inMuteMode = ( interruptionFilter == InterruptionFilterNone );

				// �~���[�g���[�h���ύX���ꂽ���ǂ������ʂ��܂��B
				if( isMute != inMuteMode ) {
#if DEBUG
					if( Log.IsLoggable( logTag, LogPriority.Info ) ) {
						Log.Info( logTag, $"{nameof( OnInterruptionFilterChanged )}: Mute-mode = {isMute} -> {inMuteMode}" );
					}
#endif

					isMute = inMuteMode;
					// TODO : �ʒm��Ԃ�OFF�̎��̏��������܂��B
					// �E�H�b�`�t�F�C�X���ĕ`�悵�܂��B
					Invalidate();
				}
			}

			/// <summary>
			///		���[�U�[���E�H�b�`�t�F�C�X���^�b�v�������Ɏ��s����܂��B
			/// </summary>
			/// <param name="tapType">�^�b�v�̎��</param>
			/// <param name="xValue">�^�b�v��X�ʒu</param>
			/// <param name="yValue">�^�b�v��Y�ʒu</param>
			/// <param name="eventTime">��ʂ��^�b�`���Ă��鎞�ԁH</param>
			/// <remarks>
			///		Android Wear 1.3�ȏ�ɑΉ����Ă��܂��B
			///		���̃��\�b�h���Ăяo������ɂ́A<see cref="WatchFaceStyle.Builder"/>�̐����ɂ����āASetAcceptsTapEvents( true )���Ăяo���K�v������܂��B
			///		�C���^���N�e�B�u���[�h�̂ݗL���ł��B
			///	</remarks>
			public override void OnTapCommand( int tapType, int xValue, int yValue, long eventTime ) {
#if DEBUG
				if( Log.IsLoggable( logTag, LogPriority.Info ) ) {
					Log.Info( logTag, $"{nameof( OnTapCommand )}: Type = {tapType}, ( x, y ) = ( {xValue}, {yValue} ), Event time = {eventTime}" );
				}
#endif

				//var resources = owner.Resources;

				// �^�b�v�̎�ނ𔻕ʂ��܂��B
				switch( tapType ) {
					case TapTypeTouch:
						// TODO : ���[�U�[����ʂ��^�b�`�������̏��������܂��B
						break;
					case TapTypeTouchCancel:
						// TODO : ���[�U�[����ʂ��^�b�`�����܂܁A�w�𓮂��������̏��������܂��B
						break;
					case TapTypeTap:
						// TODO : ���[�U�[���^�b�v�������̏��������܂��B
						break;
				}
			}

			/// <summary>
			///		�E�H�b�`�t�F�C�X�̕`�掞�Ɏ��s����܂��B
			/// </summary>
			/// <param name="canvas">�E�H�b�`�t�F�C�X�ɕ`�悷�邽�߂̃L�����o�X�I�u�W�F�N�g</param>
			/// <param name="bounds">��ʂ̃T�C�Y���i�[����I�u�W�F�N�g</param>
			public override void OnDraw( Canvas canvas, Rect bounds ) {
				// TODO : ���ݎ������擾���A�E�H�b�`�t�F�C�X��`�悷�鏈�������܂��B
				// ���ݎ����ɃZ�b�g���܂��B
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

				// �w�i��`�悵�܂��B
				// �A���r�G���g���[�h�ł��邩�ǂ������ʂ��܂��B
				if( IsInAmbientMode ) {
					// �A���r�G���g���[�h�̎��́A���F�œh��Ԃ��܂��B
					canvas.DrawColor( Color.Black );
				}
				else {
					// �����łȂ����́A�w�i�摜��`�悵�܂��B
					canvas.DrawRect( 0, 0, canvas.Width, canvas.Height, backgroundPaint );
				}

				canvas.DrawText(
					WatchfaceUtility.ConvertToDateTime( nowTime ).ToString( isAmbient ? "HH:mm" : "HH:mm:ss" ),
					digitalTimeText.XOffset, digitalTimeText.YOffset, digitalTimeText.Paint
				);
			}

			/// <summary>
			///		�E�H�b�`�t�F�C�X�̕\���E��\�����؂�ւ�������Ɏ��s���܂��B
			/// </summary>
			/// <param name="visible">�E�H�b�`�t�F�C�X�̕\���E��\��</param>
			public override void OnVisibilityChanged( bool visible ) {
				// �x�[�X�N���X��OnVisibilityChanged���\�b�h�����s���܂��B
				base.OnVisibilityChanged( visible );

#if DEBUG
				if( Log.IsLoggable( logTag, LogPriority.Info ) ) {
					Log.Info( logTag, $"{nameof( OnVisibilityChanged )}: Visible = {visible}" );
				}
#endif

				// �E�H�b�`�t�F�C�X�̕\���E��\���𔻕ʂ��܂��B
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
					// �^�C���]�[���p�̃��V�[�o�[��o�^���܂��B
					timeZoneReceiver.IsRegistered = true;
					// �E�H�b�`�t�F�C�X����\���̎��Ƀ^�C���]�[�����ω������ꍇ�̂��߂ɁA�^�C���]�[�����X�V���܂��B
					// Time ( Android )
					//nowTime.Clear( Java.Util.TimeZone.Default.ID );
					//nowTime.SetToNow();
					// Calendar ( Java )
					nowTime = Java.Util.Calendar.GetInstance( Java.Util.TimeZone.Default );
					// DateTime ( C# )
					//nowTime = DateTime.Now;
				}
				else {
					// �^�C���]�[���p�̃��V�[�o�[��o�^�������܂��B
					timeZoneReceiver.IsRegistered = false;
				}
				// �^�C�}�[�̓�����X�V���܂��B
				UpdateTimer();
			}

			/// <summary>
			///		�^�C�}�[�̓�����X�V���܂��B
			/// </summary>
			private void UpdateTimer() {
#if DEBUG
				if( Log.IsLoggable( logTag, LogPriority.Info ) ) {
					Log.Info( logTag, $"{nameof( UpdateTimer )}" );
				}
#endif

				// UpdateTimeHandler����MsgUpdateTime���b�Z�[�W����菜���܂��B
				updateTimeHandler.RemoveMessages( MessageUpdateTime );
				// �^�C�}�[�𓮍삳���邩�ǂ����𔻕ʂ��܂��B
				if( ShouldTimerBeRunning ) {
					// UpdateTimeHandler��MsgUpdateTime���b�Z�[�W���Z�b�g���܂��B
					updateTimeHandler.SendEmptyMessage( MessageUpdateTime );
				}
			}

			/// <summary>
			///		�^�C�}�[�𓮍삳���邩�ǂ�����\���l���擾���܂��B
			/// </summary>
			private bool ShouldTimerBeRunning =>
				IsVisible && !IsInAmbientMode;
		}
	}
}