#version 330
precision highp float;
uniform mat4 v_inv; //the inverse of View matrix uniform
uniform sampler2D tex; // texture uniform
uniform float spotLightDirectionAngle; //changing with every frame
in vec2 ftexcoord;
in vec4 vWorldSpacePos;  // position of the vertex (and fragment) in world space
//in vec4 vEyeSpacePos;  // position of the vertex (and fragment) in world space
in vec3 varyingNormalDirection;  // surface normal vector in world space
layout(location = 0) out vec4 FragColor;

//Implementation of per-pixel lighting (Phong Shading): normal vectors and positions are interpolated for each fragment and the lighting is computed in the fragment shader.

//Default values for GL_LIGHT0:
//GL_AMBIENT	(0, 0, 0, 1)
//GL_DIFFUSE	(1, 1, 1, 1)
//GL_SPECULAR	(1, 1, 1, 1)
//GL_POSITION	(0, 0, 1, 0)
//GL_SPOT_DIRECTION	(0, 0, −1)
//GL_SPOT_EXPONENT	0
//GL_SPOT_CUTOFF	180
//GL_CONSTANT_ATTENUATION	1
//GL_LINEAR_ATTENUATION	0
//GL_QUADRATIC_ATTENUATION	0

//Default values for GL_LIGHT1 - GL_LIGHT7:
//GL_AMBIENT	(0, 0, 0, 1)
//GL_DIFFUSE	(0, 0, 0, 1)
//GL_SPECULAR	(0, 0, 0, 1)
//GL_POSITION	(0, 0, 1, 0)
//GL_SPOT_DIRECTION	(0, 0, −1)
//GL_SPOT_EXPONENT	0
//GL_SPOT_CUTOFF	180
//GL_CONSTANT_ATTENUATION	1
//GL_LINEAR_ATTENUATION	0
//GL_QUADRATIC_ATTENUATION	0

struct lightSource
{
    vec4 position; //The first three elements of the array are the light position
    vec4 diffuse; //The first three floats set the color / intensity of the positional/spot light
    vec4 specular;
    float constantAttenuation, linearAttenuation, quadraticAttenuation;
    float spotCutoff, spotExponent; //spotCutoff = maximum spread angle of a light source; spotExponent = specifies the intensity distribution of the light. Range 0-128. Higher value means more focused light source, regardless of the spot cut off angle.
    vec3 spotDirection; //Three elements of the array are the spot light direction in homogeneous object coordinates
};

const int numberOfLights = 4;
const vec4 scene_ambient = vec4(0.1, 0.1, 0.1, 1.0);

  
struct material
{
    vec4 ambient;
    vec4 diffuse;
    vec4 specular;
    float shininess;
};

material frontMaterial = material(
    vec4(1.2, 1.2, 1.2, 1.0),
    vec4(1.0, 1.0, 1.0, 1.0),
    vec4(0.1, 0.1, 0.1, 1.0),
    50.0
);


uniform struct fogParameters 
{ 
   vec4 vFogColor; // Fog color
   float fStart; // This is only for linear fog
   float fEnd; // This is only for linear fog
   float fDensity; // For exp and exp2 equation
   int iEquation; // 0 = linear, 1 = exp, 2 = exp2
}; 

fogParameters fogParams = fogParameters(
   vec4(0.1f, 0.1f, 0.2f, 1.0f),
   -30.0f,
   -50.0f,
   0.07f, 
   1
);

float getFogFactor(fogParameters params, float fFogCoord) 
{ 
   float fResult = 0.0; 
   if(params.iEquation == 0)
   {
      if (params.fEnd < 0 && params.fStart < 0)
      {
      	 fResult = (params.fEnd+fFogCoord)/(params.fEnd-params.fStart);
      	 if (fResult < 0.0) 
      	 	fResult = 1.0;
      	 else if (fResult > 1.0)
      	 	fResult = 0.0;
      	 return fResult;
      }
      else
      	fResult = (params.fEnd-fFogCoord)/(params.fEnd-params.fStart);
   }
   else if(params.iEquation == 1) 
      fResult = exp(-params.fDensity*fFogCoord); 
   else if(params.iEquation == 2) 
      fResult = exp(-pow(params.fDensity*fFogCoord, 2.0)); 
       
   fResult = 1.0-clamp(fResult, 0.0, 1.0); 
    
   return fResult; 
}


void main() {

	lightSource lights[numberOfLights];

	lightSource light0 = lightSource(
  		vec4(0.0,  0.0,  1.0, 1.0),
  		vec4(20.0,  20.0,  20.0, 1.0),
  		vec4(1.0,  1.0,  1.0, 1.0),
  		0.1, 0.1, 0.1,
  		50.0, 20.0,
  		vec3(cos(spotLightDirectionAngle), 0.0,  1.0) //The spot lights are moving in constant speed
	);

	lightSource light1 = lightSource(
    	vec4(0.0, 0.0,  1.0, 1.0),
    	vec4(20.0,  20.0,  20.0, 1.0),
    	vec4(1.0,  1.0,  1.0, 1.0),
    	0.1, 0.1, 0.1,
    	50.0, 20.0,
    	vec3(-1.0, cos(spotLightDirectionAngle),  0.0) //The spot lights are moving in constant speed
	);

	lightSource light2 = lightSource(
  		vec4(0.0,  0.0,  1.0, 1.0),
  		vec4(20.0,  20.0,  20.0, 1.0),
  		vec4(1.0,  1.0,  1.0, 1.0),
  		0.1, 0.1, 0.1,
  		50.0, 20.0,
  		vec3(cos(spotLightDirectionAngle), 0.0,  -1.0) //The spot lights are moving in constant speed
	);

	lightSource light3 = lightSource(
    	vec4(0.0, 0.0,  1.0, 1.0),
    	vec4(20.0,  20.0,  20.0, 1.0),
    	vec4(1.0,  1.0,  1.0, 1.0),
    	0.1, 0.1, 0.1,
    	50.0, 20.0,
    	vec3(1.0, cos(spotLightDirectionAngle),  0.0) //The spot lights are moving in constant speed
	);



    lights[0] = light0;
  	lights[1] = light1;
  	lights[2] = light2;
  	lights[3] = light3;

 
  	vec3 normalDirection = normalize(varyingNormalDirection);
  	vec3 viewDirection = normalize(vec3(v_inv * vec4(0.0, 0.0, 0.0, 1.0) - vWorldSpacePos));
  	vec3 lightDirection;
  	float attenuation;
 
  	// initialize total lighting with ambient lighting
  	vec3 totalLighting = vec3(scene_ambient) * vec3(frontMaterial.ambient);
 
  	for (int index = 0; index < numberOfLights; index++) // for all light sources
    {
        if (0.0 == lights[index].position.w) // directional light?
	    {
	        attenuation = 1.0; // no attenuation
	  	 	lightDirection = normalize(vec3(lights[index].position));
	    } 
        else // point light or spotlight (or other kind of light) 
	  	{
	     	vec3 positionToLightSource = vec3(lights[index].position - vWorldSpacePos);
	  	 	float distance = length(positionToLightSource);
	  	 	lightDirection = normalize(positionToLightSource);
	  	 	attenuation = 1.0 / (lights[index].constantAttenuation
			       + lights[index].linearAttenuation * distance
			       + lights[index].quadraticAttenuation * distance * distance);
 
	  	 	if (lights[index].spotCutoff <= 90.0) // spotlight?
	  	 	{
	        	float clampedCosine = max(0.0, dot(-lightDirection, normalize(lights[index].spotDirection)));
	        	if (clampedCosine < cos(radians(lights[index].spotCutoff))) // outside of spotlight cone?
		    	{
		  			attenuation = 0.0;
				}
	        	else
				{
		  			attenuation = attenuation * pow(clampedCosine, lights[index].spotExponent);   
				}
	  		}
		}
 
      	vec3 diffuseReflection = attenuation * vec3(lights[index].diffuse) * vec3(frontMaterial.diffuse) * max(0.0, dot(normalDirection, lightDirection));
 
      	vec3 specularReflection;
      	if (dot(normalDirection, lightDirection) < 0.0) // light source on the wrong side?
	  	{
	  	 	specularReflection = vec3(0.0, 0.0, 0.0); // no specular reflection
	  	}
      	else // light source on the right side
	  	{
	  	 	specularReflection = attenuation * vec3(lights[index].specular) * vec3(frontMaterial.specular) * pow(max(0.0, dot(reflect(-lightDirection, normalDirection), viewDirection)), frontMaterial.shininess);
	  	}
 
      	totalLighting = totalLighting + diffuseReflection + specularReflection;
   	}
 
   	//FragColor = vec4( ambient + diffuse * lightColor, 1.0 );
   	//FragColor = vec4(totalLighting, 1.0);
   	FragColor = texture(tex, ftexcoord) * vec4(totalLighting, 1.0);

   	// Add fog
   	//float fogCoord = abs(vEyeSpacePos.z/vEyeSpacePos.w);
  	//FragColor = mix(FragColor, fogParams.vFogColor, getFogFactor(fogParams, fogCoord));
}