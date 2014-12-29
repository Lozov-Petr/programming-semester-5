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

    h = 1

    # glMatrixMode(GL_PROJECTION)
    if VIEW == 0 : glOrtho(-h, h, -h, h, 0, 2*h)
    else : gluPerspective(45 + 15 * VIEW, 1, 0, 2*h)
    

    glTranslatef(0,0,-h)

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

    # glColor3f(1,1,1)
    # glVertex3f( 0,-h, h)
    # glVertex3f(-h,-h, h)
    # glVertex3f(-h, 0, h)
    # glVertex3f( 0, 0, h)

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