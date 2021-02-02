# Marching_Cubes_Simple

This is Unity implementation of marching cubes, but only the code necessary to get the basics down. This means naive implementation of the meshing but with chunking, so that the defermation of terrain can work even on big fields. 
From here there are three major optimizations that can be done but will not be part of this repo:
1) Do not create all vertecies on every cube. 
2) Dots muthitreaded meshing,
3) Mesing on the GPU

## Controls
Right mouse button hold to rotate.
Click on mesh to deform. 
Shift click on mesh to deform in the other direction.
