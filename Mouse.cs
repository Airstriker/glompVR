using System;
using OpenTK.Input;

namespace glomp
{
	public class Mouse
	{
		private float mouseSpeed = 0.5f;
		OpenTK.Input.MouseState currentMouseState, previousMouseState;

		public Mouse ()
		{
			previousMouseState = OpenTK.Input.Mouse.GetState();
		}

		public float MouseSpeed {
			get { return mouseSpeed;}
			set { mouseSpeed = value;}
		}

		public void getMousePositionChange(out int xMouseDelta, out int yMouseDelta, out int zMouseDelta)
		{
			// Get mouse position change
			currentMouseState = OpenTK.Input.Mouse.GetState();
			if (currentMouseState != previousMouseState) {
				// Mouse state has changed
				if (currentMouseState [OpenTK.Input.MouseButton.Right]) { //Move only when right mouse button pressed
					xMouseDelta = currentMouseState.X - previousMouseState.X;
					yMouseDelta = currentMouseState.Y - previousMouseState.Y;
					zMouseDelta = currentMouseState.Wheel - previousMouseState.Wheel;
				} else {
					xMouseDelta = 0;
					yMouseDelta = 0;
					zMouseDelta = 0;
				}
			} else {
				xMouseDelta = 0;
				yMouseDelta = 0;
				zMouseDelta = 0;
			}
			previousMouseState = currentMouseState;

			//int xpos = System.Windows.Forms.Cursor.Position.X;
			//int ypos = System.Windows.Forms.Cursor.Position.X;

			//System.Windows.Forms.Cursor.Position = new Point (glwidget1.Allocation.Width / 2, glwidget1.Allocation.Height / 2);
		}
	}
}

