from cv2 import *
import numpy as np

img = imread("mandril.bmp", IMREAD_GRAYSCALE)

afterFFT = np.fft.fft2(img)

shiftFFT = np.fft.fftshift(afterFFT)

rows, cols = img.shape
crow, ccol = rows / 2, cols / 2

shiftFFT[crow - 30: crow + 30, ccol - 30: ccol + 30] = 0

shiftBack = np.fft.ifftshift(shiftFFT)
imgBack = np.fft.ifft2(shiftBack)
imgBack = np.abs(imgBack)

result = imgBack.astype(np.uint8)
laplace = Laplacian(img, IPL_DEPTH_32F, 3)

imshow('Fourier', result)
imshow("Laplacian", laplace)

imwrite("fourier.bmp", result)
imwrite("laplace.bmp", laplace)

waitKey(0)
