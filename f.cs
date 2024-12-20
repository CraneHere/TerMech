#version 330 core
layout(location = 0) in vec3 aPos;
layout(location = 1) in vec3 aColor;

out vec3 fragColor;

uniform mat4 model;
uniform mat4 view;
uniform mat4 projection;

void main()
{
    fragColor = aColor;
    gl_Position = projection * view * model * vec4(aPos, 1.0);
}

#version 330 core
out vec4 FragColor;

in vec3 fragColor;

void main()
{
    FragColor = vec4(fragColor, 1.0);
}
