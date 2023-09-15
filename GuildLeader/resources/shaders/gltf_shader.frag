#version 330 core

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
};

in vec3 viewPos;

in vec3 fragPos;
in vec3 fragNormal;
in vec4 objColor;
in Material material_frag;

in vec2 texCoord;
in float texAlpha;

in Light light;

uniform sampler2D texture0;

out vec4 fragColor;

void main()
{
	// AMBIENT
	vec3 ambientPow = material_frag.AmbientFactor;
	vec3 ambient = light.AmbientFactor * ambientPow;
	
	// DIFFUSE
	vec3 diffusePow = material_frag.DiffuseFactor;
	vec3 norm = normalize(fragNormal);
	vec3 lightDir = normalize(light.Position - fragPos);
	float diff = max(dot(norm, lightDir), 0f);
	vec3 diffuse = light.DiffuseFactor * (diff * diffusePow);
	
	// SPECULAR
	vec3 specularPow = material_frag.SpecularFactor;
	float shinyPow = material_frag.ShinyFactor;
	vec3 viewDir = normalize(viewPos - fragPos);
	vec3 reflectDir = reflect(-lightDir, norm);
	float spec = pow(max(dot(viewDir, reflectDir), 0f), shinyPow);
	vec3 specular = light.SpecularFactor * (spec * specularPow);
	
	// RESULT
	vec4 result = vec4(ambient + diffuse + specular, 1f);
	vec4 texColor = texture(texture0, texCoord);
	if (texColor == vec4(0, 0, 0, 1))
	{
		fragColor = result * objColor;
	}
	else
	{
		fragColor = result * objColor * texColor;
	}
}