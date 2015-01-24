from OpenGL.GL import *
from OpenGL.GLUT import *
from OpenGL.GLU import *
from math import tan, pi
 
VIEW = 0
         
def KeyPress(*args):
    global VIEW
    if   args[0] == '\033': sys.exit()
    elif args[0] >= '0' and args[0] <= '9' : VIEW = ord(args[0]) - ord('0')
 
def DrawScene():
 
    glClear(GL_COLOR_BUFFER_BIT | GL_DEPTH_BUFFER_BIT)
    glViewport(0, 0, 500, 500)
    glLoadIdentity()

    h = 100

    if VIEW == 0 : 
        glOrtho(-2*h, 2*h, -2*h, 2*h, -2*h, 2*h)
    else : 
        alpha = 15 * VIEW
        focusZ = - (h + 2 * h / tan(pi * alpha / 360))
        
        gluPerspective(alpha, 1, 0, 2*h)
        glTranslatef(0,0,focusZ)

    glBegin(GL_QUADS)


    glColor3f(0,1,0)
    glVertex3f( h, h,-h)
    glVertex3f(-h, h,-h)
    glVertex3f(-h, h, h)
    glVertex3f( h, h, h) 

    glColor3f(1,0,0)
    glVertex3f( h,-h, h)
    glVertex3f(-h,-h, h)
    glVertex3f(-h,-h,-h)
    glVertex3f( h,-h,-h) 

    glColor3f(1,1,0)
    glVertex3f( h,-h,-h)
    glVertex3f(-h,-h,-h)
    glVertex3f(-h, h,-h)
    glVertex3f( h, h,-h)

    glColor3f(0,0,1)
    glVertex3f(-h, h, h) 
    glVertex3f(-h, h,-h)
    glVertex3f(-h,-h,-h) 
    glVertex3f(-h,-h, h) 

    glColor3f(1,0,1)
    glVertex3f( h, h,-h) 
    glVertex3f( h, h, h)
    glVertex3f( h,-h, h)
    glVertex3f( h,-h,-h)

    glEnd()
    glutSwapBuffers()	 


 
def main():
 
    glutInit(sys.argv)
    glutInitDisplayMode(GLUT_RGBA | GLUT_DOUBLE | GLUT_DEPTH)
    glutInitWindowSize(500,500)

    glutCreateWindow('Cube: press 0 - ortho, press 1..9 - perspective with different angles')

    glutDisplayFunc(DrawScene)
    glutIdleFunc(DrawScene)
    glutKeyboardFunc(KeyPress)
   
    glutMainLoop()
 
main() 