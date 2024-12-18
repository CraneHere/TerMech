// g++ lr2.cpp -lGL -lGLEW -lGLU -lsfml-window -lsfml-system -lsfml-graphics -lglut
#include <SFML/Graphics.hpp>
#include <SFML/OpenGL.hpp>
#include <GL/glu.h>
#include <GL/gl.h>
#include <GL/glut.h>
#include <glm/glm.hpp>
#include <glm/gtc/matrix_transform.hpp>
#include <glm/gtc/type_ptr.hpp>
#include <cmath>

void drawCube(float size) {
    float halfSize = size / 2.0f;

    float cube_shift_x = 3;

    glBegin(GL_QUADS);
    // Front face
    glColor3f(0.2f, 0.0f, 0.1f);
    glVertex3f(cube_shift_x + -halfSize, -halfSize,  halfSize);
    glVertex3f(cube_shift_x + halfSize, -halfSize,  halfSize);
    glVertex3f(cube_shift_x + halfSize,  halfSize,  halfSize);
    glVertex3f(cube_shift_x + -halfSize,  halfSize,  halfSize);
    // Back face
    glColor3f(0.3f, 0.0f, 0.0f);
    glVertex3f(cube_shift_x + -halfSize, -halfSize, -halfSize);
    glVertex3f(cube_shift_x + -halfSize,  halfSize, -halfSize);
    glVertex3f(cube_shift_x + halfSize,  halfSize, -halfSize);
    glVertex3f(cube_shift_x + halfSize, -halfSize, -halfSize);
    // Top face
    glColor3f(0.5f, 0.0f, 0.0f);
    glVertex3f(cube_shift_x +-halfSize,  halfSize, -halfSize);
    glVertex3f(cube_shift_x +-halfSize,  halfSize,  halfSize);
    glVertex3f(cube_shift_x + halfSize,  halfSize,  halfSize);
    glVertex3f(cube_shift_x + halfSize,  halfSize, -halfSize);
    // Bottom face
    glColor3f(0.7f, 0.0f, 0.0f);
    glVertex3f(cube_shift_x + -halfSize, -halfSize, -halfSize);
    glVertex3f(cube_shift_x + halfSize, -halfSize, -halfSize);
    glVertex3f(cube_shift_x + halfSize, -halfSize,  halfSize);
    glVertex3f(cube_shift_x + -halfSize, -halfSize,  halfSize);
    // Right face
    glColor3f(0.7f, 0.3f, 0.3f);
    glVertex3f(cube_shift_x + halfSize, -halfSize, -halfSize);
    glVertex3f(cube_shift_x + halfSize,  halfSize, -halfSize);
    glVertex3f(cube_shift_x + halfSize,  halfSize,  halfSize);
    glVertex3f(cube_shift_x + halfSize, -halfSize,  halfSize);
    // Left face
    glColor3f(0.7f, 0.5f, 0.5f);
    glVertex3f(cube_shift_x + -halfSize, -halfSize, -halfSize);
    glVertex3f(cube_shift_x + -halfSize, -halfSize,  halfSize);
    glVertex3f(cube_shift_x + -halfSize,  halfSize,  halfSize);
    glVertex3f(cube_shift_x + -halfSize,  halfSize, -halfSize);
    glEnd();
}

void drawPyramid(float size, float height) {
    float halfSize = size / 2.0f;

    float pyramid_shift_x = 2;

    glBegin(GL_TRIANGLES);
    // Front face
    glColor3f(0.0f, 1.0f, 0.0f);
    glVertex3f(pyramid_shift_x + 0.0f, height, 0.0f);
    glVertex3f(pyramid_shift_x + -halfSize, 0.0f, halfSize);
    glVertex3f(pyramid_shift_x + halfSize, 0.0f, halfSize);
    // Right face
    glColor3f(0.0f, 0.7f, 0.0f);
    glVertex3f(pyramid_shift_x + 0.0f, height, 0.0f);
    glVertex3f(pyramid_shift_x + halfSize, 0.0f, halfSize);
    glVertex3f(pyramid_shift_x + halfSize, 0.0f, -halfSize);
    // Back face
    glColor3f(0.0f, 0.5f, 0.0f);
    glVertex3f(pyramid_shift_x + 0.0f, height, 0.0f);
    glVertex3f(pyramid_shift_x + halfSize, 0.0f, -halfSize);
    glVertex3f(pyramid_shift_x + -halfSize, 0.0f, -halfSize);
    // Left face
    glColor3f(0.0f, 0.3f, 0.0f);
    glVertex3f(pyramid_shift_x + 0.0f, height, 0.0f);
    glVertex3f(pyramid_shift_x + -halfSize, 0.0f, -halfSize);
    glVertex3f(pyramid_shift_x + -halfSize, 0.0f, halfSize);
    glEnd();

    glBegin(GL_QUADS);
    // Base
    glColor3f(1.0f, 1.0f, 0.0f);
    glVertex3f(pyramid_shift_x + -halfSize, 0.0f, halfSize);
    glVertex3f(pyramid_shift_x + halfSize, 0.0f, halfSize);
    glVertex3f(pyramid_shift_x + halfSize, 0.0f, -halfSize);
    glVertex3f(pyramid_shift_x + -halfSize, 0.0f, -halfSize);
    glEnd();
}

void drawCylinder(float radius, float height, int slices) {
    float angleStep = 2 * M_PI / slices;

    float cylinder_shift_x = 2;

    glBegin(GL_QUAD_STRIP);
    for (int i = 0; i <= slices; ++i) {
        float angle = i * angleStep;
        float x = radius * cos(angle);
        float z = radius * sin(angle);
        glColor3f(0.0f, 0.0f, 0.5f);
        glVertex3f(cylinder_shift_x + x, 0.0f, z);
        glColor3f(0.5f, 0.0f, 0.0f);
        glVertex3f(cylinder_shift_x + x, height, z);
    }
    glEnd();

    glBegin(GL_TRIANGLE_FAN);
    glVertex3f(cylinder_shift_x + 0.0f, 0.0f, 0.0f);
    for (int i = 0; i <= slices; ++i) {
        float angle = i * angleStep;
        float x = radius * cos(angle);
        float z = radius * sin(angle);
        glColor3f(0.0f, 0.0f, 1.0f);
        glVertex3f(cylinder_shift_x + x,0.0f, z);
    }
    glEnd();

    glBegin(GL_TRIANGLE_FAN);
    glVertex3f(cylinder_shift_x + 0.0f, height, 0.0f);
    for (int i = 0; i <= slices; ++i) {
        float angle = i * angleStep;
        float x = radius * cos(angle);
        float z = radius * sin(angle);
        glColor3f(0.0f, 0.0f, 1.0f);
        glVertex3f(cylinder_shift_x + x, height, z);
    }
    glEnd();
}

void drawLightSource(const glm::vec3& position) {
    glPushMatrix();
    glTranslatef(position.x, position.y, position.z);
    glutSolidSphere(0.1, 10, 10); // Отрисовка источника света как сфера
    glPopMatrix();
}

int main(int argc, char* argv[]) {
    
    sf::ContextSettings settings;
    settings.depthBits = 24;
    settings.stencilBits = 8;
    settings.antialiasingLevel = 4;
    settings.majorVersion = 3;
    settings.minorVersion = 0;

    glutInit(&argc,argv);

    sf::RenderWindow window(sf::VideoMode(800, 600), "3D Scene with Cube, Pyramid, and Cylinder", sf::Style::Default, settings);
    window.setFramerateLimit(60);

    glm::vec3 lightPositions[] = {
        {1.0f, 1.0f, 1.0f},
        {-1.0f, 1.0f, -1.0f}
    };

    glEnable(GL_DEPTH_TEST);

    float anglex = 70.0f;
    float angley = 5.0f;

    while (window.isOpen()) {
        sf::Event event;
        while (window.pollEvent(event)) {

            if (event.type == sf::Event::Closed)
                window.close();
        }

        // Обработка клавиатуры для перемещения угла обзора
        if (sf::Keyboard::isKeyPressed(sf::Keyboard::Right)) anglex += 1.f;
        if (sf::Keyboard::isKeyPressed(sf::Keyboard::Left)) anglex -= 1.f;
        if (sf::Keyboard::isKeyPressed(sf::Keyboard::Up)) angley += 0.5f;
        if (sf::Keyboard::isKeyPressed(sf::Keyboard::Down)) angley -= 0.5f;

        // Обработка клавиатуры для перемещения источников света
        if (sf::Keyboard::isKeyPressed(sf::Keyboard::W)) lightPositions[0].y += 0.01f;
        if (sf::Keyboard::isKeyPressed(sf::Keyboard::S)) lightPositions[0].y -= 0.01f;
        if (sf::Keyboard::isKeyPressed(sf::Keyboard::A)) lightPositions[0].x -= 0.01f;
        if (sf::Keyboard::isKeyPressed(sf::Keyboard::D)) lightPositions[0].x += 0.01f;
        if (sf::Keyboard::isKeyPressed(sf::Keyboard::Q)) lightPositions[0].z -= 0.01f;
        if (sf::Keyboard::isKeyPressed(sf::Keyboard::E)) lightPositions[0].z += 0.01f;

        if (sf::Keyboard::isKeyPressed(sf::Keyboard::I)) lightPositions[1].y += 0.01f;
        if (sf::Keyboard::isKeyPressed(sf::Keyboard::K)) lightPositions[1].y -= 0.01f;
        if (sf::Keyboard::isKeyPressed(sf::Keyboard::J)) lightPositions[1].x -= 0.01f;
        if (sf::Keyboard::isKeyPressed(sf::Keyboard::L)) lightPositions[1].x += 0.01f;
        if (sf::Keyboard::isKeyPressed(sf::Keyboard::U)) lightPositions[1].z -= 0.01f;
        if (sf::Keyboard::isKeyPressed(sf::Keyboard::O)) lightPositions[1].z += 0.01f;

        

        glClear(GL_COLOR_BUFFER_BIT | GL_DEPTH_BUFFER_BIT);
        glMatrixMode(GL_PROJECTION);
        glLoadIdentity();
        gluPerspective(45.0f, window.getSize().x / window.getSize().y, 1.0f, 500.0f);

        glMatrixMode(GL_MODELVIEW);
        glLoadIdentity();
        //gluLookAt(0.0f, 0.0f, 8.0f, 0.0f, 0.0f, 0.0f, 0.0f, 1.0f, 0.0f);
        gluLookAt(0.0f, angley, 8.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.1f, 0.0f);

        glRotatef(anglex, 0.0f, 1.0f, 0.0f);

        // Draw Cube
        glPushMatrix();
        glTranslatef(-2.0f, 0.0f, 0.0f);
        //glColor3f(1.0f, 0.0f, 0.0f);
        drawCube(1.0f);
        glPopMatrix();

        // Draw Pyramid
        glPushMatrix();
        glTranslatef(2.0f, 0.0f, 0.0f);
        //glColor3f(0.0f, 1.0f, 0.0f);
        drawPyramid(1.0f, 1.5f);
        glPopMatrix();

        // Draw Cylinder
        glPushMatrix();
        glTranslatef(0.0f, 0.0f, -2.0f);
        //glColor3f(0.0f, 0.0f, 1.0f);
        drawCylinder(0.5f, 1.5f, 32);
        glPopMatrix();

        // Draw light sources
        glColor3f(1.0f, 1.0f, 1.0f);
        drawLightSource(lightPositions[0]);
        drawLightSource(lightPositions[1]);

        window.display();
    }

    return 0;
}
