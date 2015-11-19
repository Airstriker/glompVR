
using System;
using OpenTK.Graphics.OpenGL;
using OpenTK;

namespace glomp {


    public class Camera {

        private Vector3 position;
        private float pitch;
        private float yaw;
        private Vector3 lastMove;
        
		// XYZ position of the camera
		private Vector3 eye = new Vector3(0.0f, 0.0f, 0.0f);

		private Vector3 target = new Vector3 (0.0f, 0.0f, 1.0f); //(camEye + camDirection) 

		// up vector : perpendicular to both direction and right
		private Vector3 up = new Vector3 (0.0f, 1.0f, 0.0f);

		// horizontal angle (in radians): 0
		float horizontalAngle = 0.0f;

		// vertical angle (in radians) : 0, look at the horizon
		float verticalAngle = 0.0f;

		//the lower value of FOV, the higher zoom and lower texture quality. 
		float fieldOfView = (float)Math.PI / 8; //In radians

		//Shaders related stuff
		/////////////////////////////////////////////////////////////////////////////////////////////////////////
		private Matrix4 cameraModelMatrix = Matrix4.Identity;
		/////////////////////////////////////////////////////////////////////////////////////////////////////////


		public Matrix4 CameraModelMatrix {
			get { return cameraModelMatrix; }
		}

        public Vector3 Position {
            get { return position; }
        }

		public float Yaw {
			get { return yaw; }
		}

		public float Pitch {
			get { return pitch; }
		}
        
        public Vector3 LastMove {
            get { return lastMove; }
		}

		public float FieldOfView {
			get { return fieldOfView; }
			set { fieldOfView = value; }
		}
        
		public Vector3 Eye {
			get { return eye; }
			set { eye = value; }
		}

		public Vector3 Target {
			get { return target; }
			set { target = value; }
		}

		public Vector3 Up {
			get { return up; }
			set { up = value; }
		}

        public Camera() {
            position = Vector3.Zero;
            pitch = 0.0f;
            yaw = 0.0f;
        }
        
        
        public void Put(Vector3 newPosition) {
            position = newPosition;
          
        }
        public void Put(Vector3 newPosition, float newPitch, float newYaw) {
            position = newPosition;
            pitch = newPitch;
            yaw = newYaw;  
        }
        
        public void Move(Vector3 moveVector) {
            position += moveVector;
            lastMove = moveVector;
        }
        
        public void adjustYaw(float amount) {
            yaw += amount;
        }
        
        public void adjustPitch(float amount) {
            pitch += amount;
        }
        
        public void Transform() {
            // set up transforms which are inverse of our position
            // ie bring the world to meet the camera rather than move the camera
            Vector3 translate = -position;
            float rotY = 360.0f - yaw;
            float rotX = 360.0f - pitch;

			Matrix4 rotationXMatrix = Matrix4.CreateRotationX (Util.DegreesToRadians(rotX));
			Matrix4 rotationYMatrix = Matrix4.CreateRotationY (Util.DegreesToRadians(rotY));
			Matrix4 translationMatrix = Matrix4.CreateTranslation (translate);

            //OpenTK uses row vectors (unconventional but OpenGL way??), that's why the multiplication order is as follows
            //OpenTK matrices are transposed
            cameraModelMatrix = translationMatrix * rotationYMatrix * rotationXMatrix;
        }

		public void updateCameraParams(Mouse inputDevice, float frameDelta, MainWindow mainWindow) { //frameDelta needed for fps dependent behavior

			if (inputDevice != null /* && inputDevice.GetType() == Mouse*/) {

				int xMouseDelta = 0;
				int yMouseDelta = 0;
				int zMouseDelta = 0;
				inputDevice.getMousePositionChange (out xMouseDelta, out yMouseDelta, out zMouseDelta);

				horizontalAngle -= inputDevice.MouseSpeed * frameDelta * (float)(xMouseDelta);
				verticalAngle -= inputDevice.MouseSpeed * frameDelta * (float)(yMouseDelta);

				//Protection against going upside down
				if (Util.RadiansToDegrees(verticalAngle) < -90) {
					verticalAngle = Util.DegreesToRadians(-90);
				} else if (Util.RadiansToDegrees(verticalAngle) > 90) {
					verticalAngle = Util.DegreesToRadians(90);
				}

				// Direction : Spherical coordinates to Cartesian coordinates conversion
				Vector3 direction = new Vector3 (
					(float)Math.Cos (verticalAngle) * (float)Math.Sin (horizontalAngle),
					(float)Math.Sin (verticalAngle),
					(float)Math.Cos (verticalAngle) * (float)Math.Cos (horizontalAngle)
				);

				// Right vector
				Vector3 camRight = new Vector3 (
					                  (float)Math.Sin (horizontalAngle - 3.14f / 2.0f),
					                  0,
					                  (float)Math.Cos (horizontalAngle - 3.14f / 2.0f)
				                  );

				// Up vector : perpendicular to both direction and right
				up = Vector3.Cross (camRight, direction);

				target = eye + direction;

				// updating fieldOfView (zoom)
				if (zMouseDelta != 0) {
					float angleInDegrees = (Util.RadiansToDegrees(this.fieldOfView) - (200.0f * zMouseDelta * frameDelta));
					if (angleInDegrees > 75)
						angleInDegrees = 75;
					if (angleInDegrees < 5)
						angleInDegrees = 5;
					this.fieldOfView = Util.DegreesToRadians(angleInDegrees);
					mainWindow.InitOrUpdateProjectionMatrix ();
				}
			}
		}
        
    }
}
