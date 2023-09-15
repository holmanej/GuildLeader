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
in float blinn_frag;

uniform sampler2D texture0;

out vec4 fragColor;

void main()
{
	vec3 ambientPow = material_frag.AmbientFactor;
	vec3 diffusePow = material_frag.DiffuseFactor;
	vec3 specularPow = material_frag.SpecularFactor;
	float shinyPow = material_frag.ShinyFactor;
	
	vec3 viewDir = normalize(viewPos - fragPos);
	vec3 lightDir = normalize(light.Position - fragPos);
	vec3 normalDir = normalize(fragNormal);
	vec3 halfwayDir = normalize(lightDir + viewDir);
	vec3 reflectDir = reflect(-lightDir, normalDir);
	
	// AMBIENT
	vec3 ambient = light.AmbientFactor * ambientPow;
	
	// DIFFUSE
	float diff = max(dot(normalDir, lightDir), 0f);
	vec3 diffuse = light.DiffuseFactor * (diff * diffusePow);
	
	// SPECULAR
	float spec = 0f;
	if (blinn_frag > 0)
	{
		spec = pow(max(dot(normalDir, halfwayDir), 0f), shinyPow * 4);
	}
	else
	{	
		spec = pow(max(dot(viewDir, reflectDir), 0f), shinyPow);
	}
	vec3 specular = light.SpecularFactor * (spec * specularPow);
	
	// RESULT
	vec4 result = vec4(ambient + diffuse + specular, 1f);
	vec4 texColor = texture(texture0, texCoord);
	
	// if no texture use only object color
	if (texColor == vec4(0, 0, 0, 1))
	{
		fragColor = result * objColor;
	}
	else
	{
		fragColor = result * objColor * texColor;
	}
}