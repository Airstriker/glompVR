#version 330
precision highp float;
uniform sampler2D tex; // texture uniform
uniform vec4 color; //label color
in vec2 ftexcoord;
layout(location = 0) out vec4 FragColor;

void main() {
   	FragColor = texture(tex, ftexcoord) * color;
}