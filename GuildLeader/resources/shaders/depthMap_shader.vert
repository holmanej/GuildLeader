#version 330 core

// vertex
layout (location = 0) in vec3 vPosition;

uniform mat4 obj_translate;
uniform mat4 obj_scale;
uniform mat4 obj_rotate;

uniform mat4 projection;
uniform mat4 light_direction;

void main()
{
	vec4 obj = vec4(vPosition, 1f) * obj_scale * obj_rotate * obj_translate;
	mat4 light_matrix = projection * light_direction;	
	gl_Position = light_matrix * obj;
}