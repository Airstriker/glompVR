#version 330

uniform mat4 Projection; //the Projection matrix uniform
uniform mat4 View; //the View matrix uniform
uniform mat4 Model; //the Model matrix uniform
layout(location = 0) in vec4 vposition; // position of the vertex in model space
layout(location = 1) in vec2 vtexcoord;
out vec2 ftexcoord;

void main() {
	ftexcoord = vtexcoord;

   //transforming the incoming vertex position
   gl_Position = Projection * View * Model * vposition;
}