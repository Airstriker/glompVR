
using System;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace glomp {
        
    public abstract class SceneNode {
        
        ////////////////
        // PROPERTIES //
        ////////////////
        internal float scale;
        internal Vector3 position;
        internal Vector3 originOffset;
        internal float rotY;
        internal bool alphaBlended = false;
    
        public float Scale {
            get { return scale; }
            set { scale = value; }
        }
        
        public Vector3 Position {
            get { return position; }
            set { position = value; }
        }
        
        public Vector3 OriginOffset {
            get { return originOffset; }
            set { originOffset = value; }
        }
        
        public float RotY {
            get { return rotY; }
            set { rotY = value; }
        }
        
        public bool AlphaBlended {
            get { return alphaBlended; }
            set { alphaBlended = value; }
        }
        
        //////////////////
        // CONSTRUCTORS //
        //////////////////
        public SceneNode() {
            scale = 1f;
            position = Vector3.Zero;
            originOffset = Vector3.Zero;
            rotY = 0f;
        }
        
        
        ////////////////////
        // PUBLIC METHODS //
        ////////////////////
        public void Put(Vector3 newPosition, float yRot) {
            position = newPosition;
            rotY = yRot;
        }
        
		public void MoveIntoPosition(bool rotate, ref Matrix4 modelMatrixToBeAffected) {
			Matrix4 translationMatrix = Matrix4.CreateTranslation (position);
			modelMatrixToBeAffected = translationMatrix * modelMatrixToBeAffected;
			if (rotate) {
				Matrix4 rotationYMatrix = Matrix4.CreateRotationY (Util.DegreesToRadians(rotY));
				modelMatrixToBeAffected = rotationYMatrix * modelMatrixToBeAffected;
			}
        }
        
		public void ApplyRotation(ref Matrix4 modelMatrixToBeAffected) {
			Matrix4 rotationYMatrix = Matrix4.CreateRotationY (Util.DegreesToRadians(rotY));
			modelMatrixToBeAffected = rotationYMatrix * modelMatrixToBeAffected;
        }
        
        public abstract void Render();       
    }
}
