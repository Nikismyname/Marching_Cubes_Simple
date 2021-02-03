# Marching_Cubes_Simple

This is a Unity implementation of marching cubes, but only the code necessary to get the basics down. This means naive implementation of the meshing but with chunking, so that the deformation of terrain can work even on big fields. 
From here there are three major optimizations that can be done but will not be part of this repo:
1) Do not create all vertices on every cube. 
2) Dots multithreading meshing,
3) Meshing on the GPU

## Controls
Right mouse button hold to rotate.
Click on mesh to deform. 
Shift click on mesh to deform in the other direction.

Version of Unity used: 2019.4.18. More recent ones should work too.

![MC](https://github.com/Nikismyname/Marching_Cubes_Simple/blob/main/Resources/MC.png)
