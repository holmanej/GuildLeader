#version 330 core

// vertex
layout (location = 0) in vec3 vPosition;
layout (location = 1) in vec3 vNormal;
layout (location = 2) in vec4 vColor;
layout (location = 3) in vec2 tCoord;

uniform mat4 obj_translate;
uniform mat4 obj_scale;
uniform mat4 obj_rotate;
uniform float tex_alpha;

uniform mat4 view_translate;
uniform mat4 view_rotate;
uniform mat4 projection;

uniform vec3 player_position;

out vec3 fragPos;
out vec3 fragNormal;

out vec2 texCoord;
out float texAlpha;

out vec4 objColor;
out vec3 lightColor;
out vec3 lightPos;
out vec3 viewPos;

void main()
{
	vec4 obj = vec4(vPosition, 1f) * obj_scale * obj_rotate * obj_translate;
	gl_Position = obj * view_translate * view_rotate * projection;
	
	fragPos = vPosition;
	fragNormal = vNormal;
	
	texCoord = tCoord;
	texAlpha = tex_alpha;
	
	objColor = vColor;
	lightColor = vec3(1f, 1f, 1f);
	lightPos = vec3(-50f, 10f, -50f);
	viewPos = player_position;
}