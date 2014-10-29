from OpenGL.GL import *
from OpenGL.GLUT import *
from OpenGL.GLU import *
 
VIEW = 0
         
def KeyPress(*args):
    global VIEW
    if   args[0] == '\033': sys.exit()
    elif args[0] >= '0' and args[0] <= '9' : VIEW = ord(args[0]) - ord('0')
 
def DrawScene():
 
    glClear(GL_COLOR_BUFFER_BIT | GL_DEPTH_BUFFER_BIT)
    glViewport(0, 0, 500, 500)
    glLoadIdentity()

    glMatrixMode(GL_PROJECTION)
    if VIEW == 0 : glOrtho(-1.1, 1.1, -1.1, 1.1, -5, 10)
    else : gluPerspective(VIEW + 25, 1, 0, 10)
    

    glTranslatef(0,0,-4)

    glBegin(GL_QUADS)

    glColor3f(0,1,0)
    glVertex3f( 1, 1,-1)
    glVertex3f(-1, 1,-1)
    glVertex3f(-1, 1, 1)
    glVertex3f( 1, 1, 1) 

    glColor3f(1,0,0)
    glVertex3f( 1,-1, 1)
    glVertex3f(-1,-1, 1)
    glVertex3f(-1,-1,-1)
    glVertex3f( 1,-1,-1) 

    glColor3f(1,1,0)
    glVertex3f( 1,-1,-1)
    glVertex3f(-1,-1,-1)
    glVertex3f(-1, 1,-1)
    glVertex3f( 1, 1,-1)

    glColor3f(0,0,1)
    glVertex3f(-1, 1, 1) 
    glVertex3f(-1, 1,-1)
    glVertex3f(-1,-1,-1) 
    glVertex3f(-1,-1, 1) 

    glColor3f(1,0,1)
    glVertex3f( 1, 1,-1) 
    glVertex3f( 1, 1, 1)
    glVertex3f( 1,-1, 1)
    glVertex3f( 1,-1,-1)

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