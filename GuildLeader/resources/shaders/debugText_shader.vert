#version 330 core

// vertex
layout (location = 0) in vec3 vPosition;
layout (location = 1) in vec3 vNormal;
layout (location = 2) in vec4 vColor;
layout (location = 3) in vec2 tCoord;

uniform mat4 obj_translate;
uniform mat4 obj_scale;
uniform float tex_alpha;

out vec2 texCoord;
out float texAlpha;

void main()
{
	gl_Position = vec4(vPosition, 1) * obj_scale * obj_translate;
	
	texCoord = tCoord;	
	texAlpha = tex_alpha;
}