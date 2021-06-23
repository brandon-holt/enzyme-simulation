clear; clc; close all;

% initialize parameters
ny1 = 0;
ny2 = 1000;
ne = 200;
Kon = 3;
Koff = 1;
Kcat = 3;
valency = 40; % sensors per nanoparticle
particle_diameter = 30; % nm
time_step = 0.1; % s, resolution
t_max = 400; % s, total time simulated

% run the simulation
[t, sy1, py1, sy2, iy2, py2] = simulate_activity(ny1, ny2, ne, Kon, Koff, Kcat, valency, particle_diameter, time_step, t_max);

% plot the results
plot(t, py2);