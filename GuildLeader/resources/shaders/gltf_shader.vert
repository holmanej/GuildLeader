#version 330 core

// vertex
layout (location = 0) in vec3 vPosition;
layout (location = 1) in vec3 vNormal;
layout (location = 2) in vec4 vColor;
layout (location = 3) in vec2 tCoord;

struct Material
{
	vec3 AmbientFactor;
	vec3 DiffuseFactor;
	vec3 SpecularFactor;
	
	float ShinyFactor;
};

struct Light
{
	vec3 Position;
	
	vec3 AmbientFactor;
	vec3 DiffuseFactor;
	vec3 SpecularFactor;
	//mat4 Projection;
};

struct Transform
{
	mat4 Translation;
	mat4 Scale;
	mat4 Rotation;
};

uniform Transform modelT;
uniform Transform viewT;
uniform mat4 view_projection;
uniform Material modelMat;

uniform float blinn;
uniform float tex_alpha;

out vec3 fragPos;
out vec3 fragNormal;
out vec4 objColor;
out Material material_frag;

out vec2 texCoord;
out float texAlpha;

out vec3 viewPos;
out Light light;
out float blinn_frag;

void main()
{
	vec4 obj = vec4(vPosition, 1f);
	mat4 model = modelT.Scale * modelT.Rotation * modelT.Translation;
	mat4 view = viewT.Translation * viewT.Rotation * view_projection;
	gl_Position = obj * model * view;
	
	viewPos = vec3(viewT.Translation[0][1], viewT.Translation[1][2], viewT.Translation[2][3]);
	
	fragPos = vPosition;
	fragNormal = vNormal;
	objColor = vColor;
	material_frag = modelMat;
	
	texCoord = tCoord;
	texAlpha = tex_alpha;	
	
	light.Position = vec3(5f, 20f, 5f);	
	light.AmbientFactor = vec3(1f, 1f, 1f) * 0.1f;
	light.DiffuseFactor = vec3(1f, 1f, 1f);
	light.SpecularFactor = vec3(1f, 1f, 1f);
	blinn_frag = blinn;
}