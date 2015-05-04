#version 330

uniform mat4 Projection; //the Projection matrix uniform
uniform mat4 View; //the View matrix uniform
uniform mat4 Model; //the Model matrix uniform
uniform mat3 m_3x3_inv_transp; //the transposed inverse of Model matrix
layout(location = 0) in vec4 vposition; // position of the vertex in model space
layout(location = 1) in vec2 vtexcoord;
layout(location = 2) in vec3 vnormal; //surface normal vector in object coordinates (model space)
out vec4 vWorldSpacePos;  // position of the vertex (and fragment) in world space
//out vec4 vEyeSpacePos;  // position of the vertex (and fragment) in eye space
out vec3 varyingNormalDirection;  // surface normal vector in world space
out vec2 ftexcoord;

void main() {
   ftexcoord = vtexcoord;

   vWorldSpacePos = Model * vposition;
   //vEyeSpacePos = View * Model * vposition;

   // If the surface is being represented locally by a tangent vector, this feature requires that a transformed normal vector is orthogonal to a transformed direction vector if the original normal vector is orthogonal to the original direction vector.
   // Mathematically spoken, a normal vector n is orthogonal to a direction vector v if their dot product is 0. It turns out that if v is transformed by a 3×3 matrix A, the normal vector has to be transformed by the transposed inverse of A: (A^(-1))^T.
   varyingNormalDirection = normalize(m_3x3_inv_transp * vnormal); //Note that the vertex shader writes a normalized vector to varyingNormalDirection in order to make sure that all directions are weighted equally in the interpolation. The fragment shader normalizes it again because the interpolated directions are no longer normalized.
 
   //transforming the incoming vertex position
   gl_Position = Projection * View * Model * vposition;
}