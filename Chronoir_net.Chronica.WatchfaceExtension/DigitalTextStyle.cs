#region Veision Info.
/**
*	@file DigitalTextStyle.cs
*	@brief Represents the text style stored the Paint object of the text and the top left XY coordinates.
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

using Android.Graphics;

namespace Chronoir_net.Chronica.WatchfaceExtension {

	/// <summary>
	///		Represents the text style stored the <see cref="Android.Graphics.Paint"/> object of the text and the top left XY coordinates.
	/// </summary>
	public class DigitalTextStyle {
		/// <summary>
		///		Gets the text <see cref="Android.Graphics.Paint"/> object.
		/// </summary>
		public Paint Paint { get; private set; }

		/// <summary>
		///		Gets and sets the left top X coordinate.
		/// </summary>
		public float XOffset { get; set; } = 0.0f;
		/// <summary>
		///		Gets and sets the left top Y coordinate.
		/// </summary>
		public float YOffset { get; set; } = 0.0f;

		/// <summary>
		///		Creates a new instance of the <see cref="DigitalTextStyle"/> class from the specified <see cref="Android.Graphics.Paint"/> object and the left top XY coordinates.
		/// </summary>
		/// <param name="paint">Text <see cref="Android.Graphics.Paint"/> object</param>
		/// <param name="xOffset">Left top X coordinate</param>
		/// <param name="yOffset">Left top Y coordinate</param>
		public DigitalTextStyle( Paint paint, float xOffset = 0.0f, float yOffset = 0.0f ) {
			Paint = paint ?? new Paint();
			XOffset = xOffset;
			YOffset = yOffset;
		}
	}
}